using Newtonsoft.Json;
using PCLStorage;
using Syncfusion.SfChart.XForms;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        public int Value { get; set; }
        public int Id;
    }


    public class MainBudget
	{
        private const string SAVE_DIRECTORY_NAME = "save";
        private const string SAVE_FILE_NAME = "budget.data";
        public const int INCOME_CATEGORY_ID = 777;
		private List<BudgetMonth> months;

        public Action onBudgetLoaded = delegate { };

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

            DropboxManager.Instance.onDownloadFinished += SynchronizeData;
		}

        public void InitWithJson(string jsonString)
		{
            budgetDescription = JsonConvert.DeserializeObject<BudgetDescription>(jsonString);
            Load();
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
            byteList.AddRange(BitConverter.GetBytes(months.Count));
            foreach (BudgetMonth month in months)
            {
                byteList.AddRange(month.Serialize());
            }

            byte[] data = byteList.ToArray();
            char[] chars = new char[data.Length / sizeof(char)];
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
                int numMonths = binaryData.GetInt();
                for (int i = 0; i < numMonths; i++)
                {
                    BudgetMonth month = BudgetMonth.CreateFromBinaryData(binaryData);
                    months.Add(month);
                }
            }

            onBudgetLoaded();
        }

        private void SynchronizeData(byte[] data)
        {
            months.Clear();
            BinaryData binaryData = new BinaryData(data);
            int numMonths = binaryData.GetInt();
            for (int i = 0; i < numMonths; i++)
            {
                BudgetMonth month = BudgetMonth.CreateFromBinaryData(binaryData);
                months.Add(month);
            }

            Save(false);
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

        public async Task AddPlanedExpense(float value, int categoryID, int subcatID)
        {
            BudgetMonth month = GetCurrentMonthData();
            month.SetPlannedExpense(value, categoryID, subcatID);
            await Save();
        }

        public async Task SetPlanedIncome(float value, int incomeCategoryID)
        {
            BudgetMonth month = GetCurrentMonthData();
            month.SetPlannedIncome(value, incomeCategoryID);
            await Save();
        }

        private BudgetMonth GetMonth(DateTime date)
		{
			BudgetMonth month = months.Find(x => x.Month == date.Month && x.Year == date.Year);
			if (month == null)
			{
				month = BudgetMonth.Create(budgetDescription.Categories, budgetDescription.Incomes, date);
				months.Add(month);
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

        public ObservableCollection<GroupedCategory> GetPlanningData()
        {
            ObservableCollection<GroupedCategory> collection = new ObservableCollection<GroupedCategory>();

            GroupedCategory incomeGroup = new GroupedCategory()
            {
                Name = "Przychód",
                Id = INCOME_CATEGORY_ID
            };
            foreach (BudgetIncomeTemplate income in budgetDescription.Incomes)
            {
                incomeGroup.Add(new SimpleCategory()
                {
                    Name = income.Name,
                    Id = income.Id
                });
            }

            collection.Add(incomeGroup);

            foreach(BudgetCategoryTemplate category in budgetDescription.Categories)
            {
                GroupedCategory categoryCollection = new GroupedCategory()
                {
                    Name = category.Name,
                    Id = category.Id
                };

                int subcatId = 0;
                foreach(string subcat in category.subcategories)
                {
                    categoryCollection.Add(new SimpleCategory()
                    {
                        Name = subcat,
                        Value = 5,
                        Id = subcatId
                    });
                    subcatId++;
                }

                collection.Add(categoryCollection);
            }

            return collection;
        }
    }
}
