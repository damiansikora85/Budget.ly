using Dropbox.Api;
using Dropbox.Api.Files;
using HomeBudgeStandard.Code.Dropbox;
using HomeBudget.Code.Logic;
using HomeBudgetShared.Code.Interfaces;
using ProtoBuf;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
    public class DropboxManager : ICloudStorage
    {
        private DropboxClient _dropboxClient;
        public DropboxClient DropboxClient
        {
            get
            {
                if (Helpers.Settings.DropboxAccessToken != string.Empty && _dropboxClient == null)
                {
                    var accessToken = Helpers.Settings.DropboxAccessToken;
                    _dropboxClient = new DropboxClient(accessToken);
                }
                return _dropboxClient;
            }
        }
        private const string DROPBOX_DATA_FILE_PATH = "/budget_data.dat";

        public Action onLogedIn;
        public Action onUploadFinished;

        private bool _isSynchronized;
        private const int SYNCHRO_DELAY_MS = 5*1000;

        public Action<BudgetData> OnDownloadFinished { get; set; }
        public Action OnDownloadError { get; set; }

        public DropboxManager()
        {
            _dropboxClient = null;
        }

        /*private async Task Synchronize(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if(_isSynchronized)
                    {
                        _isSynchronized = true;
                        await UploadData(MainBudget.Instance)
                    }
                    await Task.Delay(SYNCHRO_DELAY_MS, token);
                }
                catch
                {
                    await Task.Delay(5000, token);
                }
            }
        }*/

        public async Task DownloadData()
        {
            if (DropboxClient == null)
                return;
            try
            {
                var metadata = await DropboxClient.Files.GetMetadataAsync(DROPBOX_DATA_FILE_PATH);
                using (var response = await DropboxClient.Files.DownloadAsync(DROPBOX_DATA_FILE_PATH))
                {
                    var stream = await response.GetContentAsStreamAsync();
                    var budgetData = Serializer.Deserialize<BudgetData>(stream);
                    OnDownloadFinished?.Invoke(budgetData);
                    response.Dispose();
                }
            }      
            catch(ApiException<GetMetadataError>)
            {
                //file not found
                OnDownloadFinished?.Invoke(null);
            }
            catch(Exception e)
            {
                OnDownloadError?.Invoke();
            }
        }

        public async Task UploadData(BudgetData budgetData)
        {
            if (DropboxClient == null)
                return;

            using (MemoryStream stream = new MemoryStream())
            {
                Serializer.Serialize(stream, budgetData);
                await stream.FlushAsync();
                var bytes = stream.GetBuffer();

                using (var memoryStream = new MemoryStream(bytes))
                {
                    var metadata = await DropboxClient.Files.UploadAsync(DROPBOX_DATA_FILE_PATH, WriteMode.Overwrite.Instance, body: memoryStream);
                    onUploadFinished?.Invoke();
                }
            }
        }
    }
}
