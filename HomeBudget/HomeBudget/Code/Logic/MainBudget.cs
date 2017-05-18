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

    public class ExpenseSaveData
    { 
        public CategoryData Category;
        public CategoryData Subcategory;
        public DateTime Date;
    }


    public class MainBudget
	{
        private const string SAVE_DIRECTORY_NAME = "save";
        private const string SAVE_FILE_NAME = "budget.data";
		private List<BudgetMonth> months;
        public ExpenseSaveData CurrentExpenseSaveData;

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

        public async Task<bool> Load()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            ExistenceCheckResult result = await rootFolder.CheckExistsAsync(SAVE_DIRECTORY_NAME);
            
            if (result == ExistenceCheckResult.NotFound)
                return false;

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
                return true;
            }
            else return false;
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

        public async Task AddExpense(float value)
		{
			BudgetMonth month = GetMonth(CurrentExpenseSaveData.Date);
			month.AddExpense(value, CurrentExpenseSaveData);
            await Save();
		}

		private BudgetMonth GetMonth(DateTime date)
		{
			BudgetMonth month = months.Find(x => x.Month == date.Month && x.Year == date.Year);
			if (month == null)
			{
				month = BudgetMonth.Create(budgetDescription.Categories, date);
				months.Add(month);
			}

			return month;
		}

		public ObservableCollection<BudgetMonth.BudgetChartData> GetCurrentMonthData()
		{
			return GetMonth(DateTime.Now).GetData();
		}
	}
}
