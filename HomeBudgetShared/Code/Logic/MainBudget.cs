using HomeBudget.Code.Interfaces;
using HomeBudget.Code.Logic;
using HomeBudget.Helpers;
using HomeBudgetShared.Code.Interfaces;
using HomeBudgetShared.Code.Synchronize;
using HomeBudgetShared.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
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
        private const string TEMPLATE_FILENAME = "templateOrg.json";
        private const string SAVE_DIRECTORY_NAME = "save";

        private const string SAVE_FILE_NAME = "budget.data";
        public const int INCOME_CATEGORY_ID = 777;
        private const int VERSION = 1;
        private List<BudgetMonth> _months;
        private BudgetPlanned budgetPlanned;

        private object _updateLock = new object();
        private SemaphoreSlim _saveSemaphore = new SemaphoreSlim(1, 1);

        private IFileManager _fileManager;
        private IBudgetSynchronizer _budgetSynchronizer;
        private ICrashReporter _crashReporter;
        private ISettings _settings;
        private IFeatureSwitch _featureSwitch;
        public bool IsDataLoaded { get; private set; }

        public event Action onPlannedBudgetChanged;
        public event Action<bool> BudgetDataChanged = delegate { };

        public BudgetDescription BudgetDescription { get; private set; }

        public BudgetData ActualBudgetData { get; private set; }

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
            _months = new List<BudgetMonth>();
            budgetPlanned = new BudgetPlanned();
            IsDataLoaded = false;
        }

        public async Task Init(IFileManager fileManager, IBudgetSynchronizer budgetSynchronizer, ICrashReporter crashReporter, ISettings settings, IFeatureSwitch featureSwitch)
        {
            _crashReporter = crashReporter;
            _fileManager = fileManager;
            _featureSwitch = featureSwitch;

            if (budgetSynchronizer != null)
            {
                _budgetSynchronizer = budgetSynchronizer;
                _budgetSynchronizer.DataDownloaded += UpdateData;
            }
            _settings = settings;

            _months.Clear();

            try
            {
                if (!string.IsNullOrEmpty(_settings.CloudRefreshToken))
                {
                    var budgetTemplate = await _budgetSynchronizer.DownloadBudgetTemplate();
                    if (budgetTemplate != null)
                    {
                        BudgetDescription = budgetTemplate;
                        _fileManager.WriteCustomTemplate(BudgetDescription);
                    }
                }
                if (BudgetDescription == null && _fileManager.HasCustomTemplate())
                {
                    BudgetDescription = await _fileManager.ReadCustomTemplate();
                }
                if (BudgetDescription == null)
                {
                    var assembly = typeof(MainBudget).GetTypeInfo().Assembly;
                    //var name = assembly.GetName();
                    //var names = assembly.GetManifestResourceNames();
                    var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{TEMPLATE_FILENAME}");

                    var jsonString = "";
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        jsonString = reader.ReadToEnd();
                        BudgetDescription = JsonConvert.DeserializeObject<BudgetDescription>(jsonString);
                    }
                }

                //TODO
                //if BudgetDescription == null
                budgetPlanned.Setup(BudgetDescription.Categories);

                if (!string.IsNullOrEmpty(_settings.CloudRefreshToken))
                {
                    _budgetSynchronizer.Start();
                    LogsManager.Instance.WriteLine("Load data from cloud storage");
                    UpdateData(null, await _budgetSynchronizer.ForceLoad());
                }
                else
                {
                    LogsManager.Instance.WriteLine("Load data from local device");
                    await LoadAsync();
                }
            }
            catch (Exception exc)
            {
                var msg = exc.Message;
                LogsManager.Instance.WriteLine(exc.Message);
                LogsManager.Instance.WriteLine(exc.StackTrace);
                _crashReporter.Report(exc);
            }

            LogsManager.Instance.Init(fileManager);
        }

        public async Task OnCloudStorageConnected(bool overwriteLocal)
        {
            IsDataLoaded = !overwriteLocal;
            _budgetSynchronizer.ShouldSave = !overwriteLocal;

            if (overwriteLocal)
            {
                try
                {
                    var budgetTemplate = await _budgetSynchronizer.DownloadBudgetTemplate();
                    UpdateData(null, await _budgetSynchronizer.ForceLoad());
                    if (budgetTemplate != null)
                    {
                        BudgetDescription = budgetTemplate;
                        _fileManager.WriteCustomTemplate(BudgetDescription);
                    }
                    _budgetSynchronizer.Start();
                }
                catch (Exception exc)
                {
                    LogsManager.Instance.WriteLine(exc.Message);
                    _crashReporter.Report(exc);
                }
            }
        }

        public void UpdateBudgetCategories(List<BudgetCategoryForEdit> updatedCategories)
        {
            try
            {
                BudgetDescription.UpdateCategories(updatedCategories);
                budgetPlanned.Setup(BudgetDescription.Categories);
                _fileManager.WriteCustomTemplate(BudgetDescription);
                if (!string.IsNullOrEmpty(_settings.CloudRefreshToken))
                {
                    _budgetSynchronizer.UploadBudgetTemplate(BudgetDescription);
                }
                GetCurrentMonthData().UpdateBudgetCategories(updatedCategories);
                Task.Factory.StartNew(async () => await Save());
                BudgetDataChanged?.Invoke(false);
            }
            catch (Exception exc)
            {
                LogsManager.Instance.WriteLine($"Update template failed: {exc.Message}");
                _crashReporter.Report(exc);
            }
        }

        public async Task<bool> Save(bool upload = true)
        {
            await _saveSemaphore.WaitAsync();
            try
            {
                LogsManager.Instance.WriteLine("Save data");
                ActualBudgetData = new BudgetData
                {
                    Version = VERSION,
                    TimeStamp = DateTime.Now,
                    BudgetPlanned = new BudgetPlanned(budgetPlanned),
                    Months = _months.Select(item => item.Clone()).ToList(),
                    IsSynchronized = false
                };

                await _fileManager.Save(ActualBudgetData);

                if (upload)
                {
                    _budgetSynchronizer.ShouldSave = true;
                }
            }
            catch (Exception exc)
            {
                LogsManager.Instance.WriteLine("Save data error: " + exc.InnerException != null ? exc.InnerException.Message : string.Empty);
                LogsManager.Instance.WriteLine(exc.Message);
                LogsManager.Instance.WriteLine(exc.StackTrace);
                _crashReporter.Report(exc);
            }
            finally
            {
                _saveSemaphore.Release();
            }

            return true;
        }

        public async Task LoadAsync()
        {
            try
            {
                LogsManager.Instance.WriteLine("Load data");
                var data = await _fileManager.Load();
                if (data != null)
                {
                    if (data.BudgetPlanned != null)
                    {
                        budgetPlanned = data.BudgetPlanned;
                    }
                    if (data.Months != null)
                    {
                        _months = data.Months;
                        foreach (var month in _months)
                            month.Setup();
                    }
                }

                IsDataLoaded = true;
                //onBudgetLoaded?.Invoke();
                BudgetDataChanged?.Invoke(false);
            }
            catch (Exception exc)
            {
                LogsManager.Instance.WriteLine(exc.Message);
                LogsManager.Instance.WriteLine(exc.StackTrace);
                _crashReporter.Report(exc);
            }
        }

        private void OnPlannedBudgetChanged()
        {
            onPlannedBudgetChanged?.Invoke();
        }

        private void UpdateData(object sender, BudgetData data)
        {
            try
            {
                if (data == null)
                {
                    LogsManager.Instance.WriteLine("Cloud UpdateData == null");
                    Task.Run(() => LoadAsync());
                }
                else
                {
                    lock (_updateLock)
                    {
                        _months.Clear();
                        if (data.Months != null)
                        {
                            _months = data.Months;
                            foreach (var month in _months)
                                month.Setup();
                        }
                        if (data.BudgetPlanned != null)
                            budgetPlanned = data.BudgetPlanned;
                    }

                    IsDataLoaded = true;
                    BudgetDataChanged?.Invoke(true);
                    Task.Factory.StartNew(async () => await Save(false));
                }
            }
            catch (Exception exc)
            {
                LogsManager.Instance.WriteLine("SynchronizeData exception: " + exc.Message);
                LogsManager.Instance.WriteLine(exc.StackTrace);
                _crashReporter.Report(exc);
            }
        }

        private void SynchronizeError(object sender, EventArgs args)
        {
            LoadAsync();
        }

        public async Task UpdateMainPlannedBudget()
        {
            budgetPlanned = GetCurrentMonthData().BudgetPlanned;
            await Save().ConfigureAwait(false);
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
            var month = _months.Find(x => x.Month == date.Month && x.Year == date.Year);
            if (month == null)
            {
                month = BudgetMonth.Create(BudgetDescription.Categories, date);
                month.onBudgetPlannedChanged += OnPlannedBudgetChanged;
                month.UpdatePlannedBudget(budgetPlanned);
                month.Setup();
                _months.Add(month);
            }

            return month;
        }

        public bool HasMonthData(DateTime date) => _months.Find(x => x.Month == date.Month && x.Year == date.Year) != null;

        public BudgetMonth GetCurrentMonthData()
        {
            return GetMonth(DateTime.Now);
        }
    }
}
