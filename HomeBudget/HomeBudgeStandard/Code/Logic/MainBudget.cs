using HomeBudget.Code.Logic;
using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
    public struct CategoryData
    {
        public string Name;
        public int Id;
        public CategoryData(string name, int id)
        {
            Name = name;
            Id = id;
        }
    }

    public class GroupedCategory : ObservableCollection<SimpleCategory>
    {
        public string Name { get; set; }
        public double Total { get; set; }
        public int Id;
    }

    public class SimpleCategory
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public int Id;
    }

    public class MainBudget
	{
        private const string TEMPLATE_FILENAME = "HomeBudgeStandard.template.json";
        private const string SAVE_DIRECTORY_NAME = "save";
        private const string SAVE_FILE_NAME = "budget.data";
        public const int INCOME_CATEGORY_ID = 777;
        private const int VERSION = 1;
        private List<BudgetMonth> months;
        private BudgetPlanned budgetPlanned;
        private bool initialized;
        public bool IsInitialized
        {
            get { return initialized; }
        }

        public event Action onBudgetLoaded = delegate { };
        public event Action onPlannedBudgetChanged;

        private BudgetDescription budgetDescription;
		public BudgetDescription BudgetDescription
		{
			get { return budgetDescription; }
		}

		static MainBudget instance;
		public static MainBudget Instance
		{
			get
			{
				if (instance == null)
					instance = new MainBudget();

				return instance;
			}
		}

		private MainBudget()
		{
			months = new List<BudgetMonth>();
            budgetPlanned = new BudgetPlanned();
            initialized = false;

            DropboxManager.Instance.onDownloadFinished += SynchronizeData;
            DropboxManager.Instance.onDownloadError += SynchronizeError;
		}

        public void Init(Assembly assembly)
        {
            Stream stream = assembly.GetManifestResourceStream(TEMPLATE_FILENAME);
            string jsonString = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
                budgetDescription = JsonConvert.DeserializeObject<BudgetDescription>(jsonString);
                budgetPlanned.Setup(budgetDescription.Categories);
            }
        }

        public byte[] GetData()
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes(months.Count));
            foreach (BudgetMonth month in months)
            {
                byteList.AddRange(month.Serialize());
            }

            return byteList.ToArray();
        }

        public async Task<bool> Save(bool upload = true)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            ExistenceCheckResult result = await rootFolder.CheckExistsAsync(SAVE_DIRECTORY_NAME);
            if(result == ExistenceCheckResult.NotFound)
                await rootFolder.CreateFolderAsync(SAVE_DIRECTORY_NAME, CreationCollisionOption.OpenIfExists);

            IFolder folder = await rootFolder.GetFolderAsync(SAVE_DIRECTORY_NAME);

            IFile file = await folder.CreateFileAsync(SAVE_FILE_NAME, CreationCollisionOption.ReplaceExisting);

            List<byte> byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes(VERSION));
            byteList.AddRange(budgetPlanned.Serialize());
            byteList.AddRange(BitConverter.GetBytes(months.Count));
            foreach (BudgetMonth month in months)
            {
                byteList.AddRange(month.Serialize());
            }

            byte[] data = byteList.ToArray();
            char[] chars = new char[data.Length / sizeof(char)+1];
            Buffer.BlockCopy(data, 0, chars, 0, data.Length);

            await file.WriteAllTextAsync(new string(chars));

            if(upload)
                await DropboxManager.Instance.UploadData(data);

            return true;
        }

        public async Task Load()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            ExistenceCheckResult result = await rootFolder.CheckExistsAsync(SAVE_DIRECTORY_NAME);

            if (result == ExistenceCheckResult.NotFound)
            {
                initialized = true;
                onBudgetLoaded();
                return;
            }

            IFolder folder = await rootFolder.GetFolderAsync(SAVE_DIRECTORY_NAME);
            string path = folder.Path;
            IFile file = await folder.CreateFileAsync(SAVE_FILE_NAME, CreationCollisionOption.OpenIfExists);
            string dataString = await file.ReadAllTextAsync();
            if (dataString.Length > 0)
            {
                byte[] data = new byte[dataString.Length * sizeof(char)];
                Buffer.BlockCopy(dataString.ToCharArray(), 0, data, 0, dataString.Length*sizeof(char));
                BinaryData binaryData = new BinaryData(data);
                int version = binaryData.GetInt();
                budgetPlanned.Deserialize(binaryData);
                int numMonths = binaryData.GetInt();
                for (int i = 0; i < numMonths; i++)
                {
                    BudgetMonth month = BudgetMonth.CreateFromBinaryData(binaryData);
                    month.onBudgetPlannedChanged += OnPlannedBudgetChanged;
                    months.Add(month);
                }
            }

            initialized = true;
            onBudgetLoaded();
        }

        private void OnPlannedBudgetChanged()
        {
            if(onPlannedBudgetChanged != null)
                onPlannedBudgetChanged();
        }

        private void SynchronizeData(byte[] data)
        {
            months.Clear();
            BinaryData binaryData = new BinaryData(data);
            int numMonths = binaryData.GetInt();
            for (int i = 0; i < numMonths; i++)
            {
                BudgetMonth month = BudgetMonth.CreateFromBinaryData(binaryData);
                month.onBudgetPlannedChanged += OnPlannedBudgetChanged;
                months.Add(month);
            }

            onBudgetLoaded();
            Save(false);
        }

        private void SynchronizeError()
        {
            Load();
        }

        public async Task AddExpense(float value, DateTime date, int categoryID, int subcatID)
        {
            BudgetMonth month = GetMonth(date);
            month.AddExpense(value, date, categoryID, subcatID);
            await Save();
        }

        public async Task AddIncome(float value, DateTime date, int incomeCategoryId)
        {
            BudgetMonth month = GetMonth(date);
            month.AddIncome(value, date, incomeCategoryId);
            await Save();
        }

        public async Task UpdateMainPlannedBudget()
        {
            budgetPlanned = GetCurrentMonthData().BudgetPlanned;
            await Save();
        }

        public double GetTotalPlannedExpensesForCurrentMonth()
        {
            return GetCurrentMonthData().GetTotalExpensesPlanned();
        }

        public double GetTotalPlannedIncomeForCurrentMonth()
        {
            return GetCurrentMonthData().GetTotalIncomePlanned();
        }

        public BudgetMonth GetMonth(DateTime date)
		{
			BudgetMonth month = months.Find(x => x.Month == date.Month && x.Year == date.Year);
			if (month == null)
			{
				month = BudgetMonth.Create(budgetDescription.Categories, budgetDescription.Incomes, date);
                month.onBudgetPlannedChanged += OnPlannedBudgetChanged;
                month.UpdatePlannedBudget(budgetPlanned);
                months.Add(month);
                Save();
			}

			return month;
		}

		public ObservableCollection<BudgetMonth.BudgetChartData> GetCurrentMonthChartData()
		{
			return GetMonth(DateTime.Now).GetData();
		}

        public BudgetMonth GetCurrentMonthData()
        {
            return GetMonth(DateTime.Now);
        }
    }
}
