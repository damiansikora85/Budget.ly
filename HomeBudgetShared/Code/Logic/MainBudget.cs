using HomeBudget.Code.Logic;
using HomeBudgetShared.Code.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private const string TEMPLATE_FILENAME = "HomeBudget.UWP.template.json";
        private const string SAVE_DIRECTORY_NAME = "save";
        private const string SAVE_FILE_NAME = "budget.data";
        public const int INCOME_CATEGORY_ID = 777;
        private const int VERSION = 1;
        private List<BudgetMonth> months;
        private BudgetPlanned budgetPlanned;

        private IFileManager _fileManager;
        private ICloudStorage _cloudStorage;
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
		}

        public void CloudStorageConnected()
        {
            Task.Run(() => _cloudStorage.DownloadData());
        }

        public void Init(IFileManager fileManager, ICloudStorage cloudStorage)
        {
            _fileManager = fileManager;
            _cloudStorage = cloudStorage;
            _cloudStorage.OnDownloadFinished += SynchronizeData;
            _cloudStorage.OnDownloadError += SynchronizeError;

            var assembly = typeof(MainBudget).GetTypeInfo().Assembly;
            //var names = assembly.GetManifestResourceNames();
            var stream = assembly.GetManifestResourceStream(TEMPLATE_FILENAME);
            var jsonString = "";
            using (var reader = new System.IO.StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
                budgetDescription = JsonConvert.DeserializeObject<BudgetDescription>(jsonString);
                budgetPlanned.Setup(budgetDescription.Categories);
            }

            if (!string.IsNullOrEmpty(Helpers.Settings.DropboxAccessToken))
            {
                Task.Run(() => _cloudStorage.DownloadData());
            }
            else
            {
                Task.Run(() => LoadAsync());
            }
        }

        public byte[] GetData()
        {
            var byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes(months.Count));
            foreach (BudgetMonth month in months)
            {
                byteList.AddRange(month.Serialize());
            }

            return byteList.ToArray();
        }

        public async Task<bool> Save(bool upload = true)
        {
            try
            {
                var saveData = new BudgetData
                {
                    Version = VERSION,
                    TimeStamp = DateTime.Now,
                    BudgetPlanned = budgetPlanned,
                    Months = months
                };
                await _fileManager.Save(saveData);

                if(upload)
                    await _cloudStorage.UploadData(saveData);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
        }                                                                                                                                                                                                         

        public async Task LoadAsync()
        {
            var data = await _fileManager.Load();
            if (data != null)
            {
                budgetPlanned = data.BudgetPlanned;
                months = data.Months;
            }

            initialized = true;
            onBudgetLoaded?.Invoke();
        }

        private void OnPlannedBudgetChanged()
        {
            onPlannedBudgetChanged?.Invoke();
        }

        private void SynchronizeData(BudgetData data)
        {
            if (data == null)
            {
                Task.Run(() => LoadAsync());
            }
            else
            {
                months.Clear();
                if(data.Months != null)
                    months = data.Months;
                if(data.BudgetPlanned != null)
                    budgetPlanned = data.BudgetPlanned;

                /*var binaryData = new BinaryData(data);
                var numMonths = binaryData.GetInt();
                for (int i = 0; i < numMonths; i++)
                {
                    var month = BudgetMonth.CreateFromBinaryData(binaryData);
                    month.onBudgetPlannedChanged += OnPlannedBudgetChanged;
                    months.Add(month);
                */

                onBudgetLoaded?.Invoke();
                Task.Run(() => Save(false));
            }
        }

        private void SynchronizeError()
        {
            LoadAsync();
        }

        public async Task AddExpense(float value, DateTime date, int categoryID, int subcatID)
        {
            var month = GetMonth(date);
            month.AddExpense(value, date, categoryID, subcatID);
            Save();
        }

        public async Task AddIncome(float value, DateTime date, int incomeCategoryId)
        {
            var month = GetMonth(date);
            month.AddIncome(value, date, incomeCategoryId);
            Save();
        }

        public async Task UpdateMainPlannedBudget()
        {
            budgetPlanned = GetCurrentMonthData().BudgetPlanned;
            Save();
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
			var month = months.Find(x => x.Month == date.Month && x.Year == date.Year);
			if (month == null)
			{
				month = BudgetMonth.Create(budgetDescription.Categories, date);
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
