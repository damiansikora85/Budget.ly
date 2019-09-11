using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudgetShared.Code.Interfaces;
using HomeBudgetShared.Code.Synchronize;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HomeBudgetShared.Code.Synchronize
{
    public class BudgetSynchronizer : IBudgetSynchronizer
    {
        public ICloudStorage _cloudStorage;
        private bool _isRunning;
        public bool IsRunning => _isRunning;

        public bool ShouldSave { get; set; }

        public ICloudStorage CloudStorage => _cloudStorage;

        private const int SYNC_DELAY = 5000; //5 sec
        private CancellationTokenSource _cancellationTokenSource;

        public event EventHandler<BudgetData> DataDownloaded;
        public event EventHandler DownloadError;

        private DateTime _dataSynchroTime;

        public BudgetSynchronizer(ICloudStorage cloudStorage)
        {
            _cloudStorage = cloudStorage;
            _cloudStorage.OnDownloadFinished += OnDataDownloaded;
            _cloudStorage.OnDownloadError += OnDownloadError;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            Synchronize(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        private void OnDataDownloaded(object sender, BudgetData data)
        {
            DataDownloaded.Invoke(this, data);
        }

        private void OnDownloadError(object sender, EventArgs args)
        {
            DownloadError.Invoke(this, args);
        }

        private Task Synchronize(CancellationToken cancelToken)
        {
            return Task.Run(async() =>
            {
                try
                {
                    _isRunning = true;
                    while (!cancelToken.IsCancellationRequested)
                    {
                        try
                        {
                            if (ShouldSave)
                            {
                                var data = MainBudget.Instance.ActualBudgetData;
                                _dataSynchroTime = await _cloudStorage.UploadData(data);
                                ShouldSave = false;
                            }
                            else
                            {
                                var cloudFileModifiedDate = await _cloudStorage.GetCloudFileModifiedTime();
                                
                                if(cloudFileModifiedDate > _dataSynchroTime)
                                {
                                    var downloadData = await _cloudStorage.DownloadData();
                                    _dataSynchroTime = cloudFileModifiedDate;
                                    DataDownloaded?.Invoke(this, downloadData);
                                }
                            }
                        }
                        catch(Exception exc)
                        {
                            var msg = exc.Message;
                        }
                        await Task.Delay(SYNC_DELAY);
                    }
                }
                catch (Exception exc)
                {
                    var msg = exc.Message;
                }
                finally
                {
                    _isRunning = false;
                }
            });
        }

        public async Task<BudgetData> ForceLoad()
        {
            _dataSynchroTime = await _cloudStorage.GetCloudFileModifiedTime();
            var downloadData = await _cloudStorage.DownloadData();
            return downloadData;
        }

        public async Task UploadBudgetTemplate(BudgetDescription data)
        {
            await _cloudStorage.UploadBudgetTemplate(data);
        }

        public async Task<BudgetDescription> DownloadBudgetTemplate()
        {
            return await _cloudStorage.DownloadBudgetTemplate();
        }
    }
}
