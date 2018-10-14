using Dropbox.Api;
using Dropbox.Api.Files;
using HomeBudget.Code.Logic;
using HomeBudgetShared.Code.Interfaces;
using HomeBudgetShared.Utils;
using ProtoBuf;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
    public class DropboxCloudStorage : ICloudStorage
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
        public static string DROPBOX_DATA_FILE_PATH = "/budget_data.dat";

        public Action onLogedIn;
        public Action onUploadFinished;

        private bool _isSynchronized;
        private const int SYNCHRO_DELAY_MS = 5*1000;
        private DateTime _synchroDate;

        public event EventHandler<BudgetData> OnDownloadFinished;
        public event EventHandler OnDownloadError;

        public DropboxCloudStorage()
        {
            _dropboxClient = null;
        }

        public async Task<BudgetData> DownloadData()
        {
            if (DropboxClient == null)
                return null;

            var budgetData = new BudgetData();
            try
            {
                LogsManager.Instance.WriteLine("Dropbox loading");
                var metadata = await DropboxClient.Files.GetMetadataAsync(DROPBOX_DATA_FILE_PATH) as FileMetadata;
                _synchroDate = metadata.ServerModified;
               
                using (var response = await DropboxClient.Files.DownloadAsync(DROPBOX_DATA_FILE_PATH))
                {
                    var bytes = await response.GetContentAsByteArrayAsync();
                    var stream = new MemoryStream(bytes, 0, bytes.Length);
                    budgetData = Serializer.Deserialize<BudgetData>(stream);
                    
                    //OnDownloadFinished?.Invoke(this, budgetData);
                    //OnDownloadFinished?.Invoke(null);
                }
                return budgetData;
            }      
            catch(ApiException<GetMetadataError>)
            {
                LogsManager.Instance.WriteLine("Dropbox file not found");
                //file not found
                //OnDownloadFinished?.Invoke(this, null);
                return null;
            }
            catch(Exception e)
            {
                LogsManager.Instance.WriteLine("Dropbox other exception: "+e.Message);
                //OnDownloadError?.Invoke();
                //OnDownloadFinished?.Invoke(this, null);
                return null;
            }
            /*finally
            {
                return budgetData;
            }*/
        }

        public async Task UploadData(BudgetData budgetData)
        {
            if (DropboxClient == null)
                return;

            try
            {
                var metadata = await DropboxClient.Files.GetMetadataAsync(DROPBOX_DATA_FILE_PATH) as FileMetadata;
                if (_synchroDate < metadata.ServerModified)
                {
                    await DownloadData();
                }
                else
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        Serializer.Serialize(stream, budgetData);
                        await stream.FlushAsync();
                        var l = stream.Length;
                        var bytes = stream.GetBuffer();

                        using (var memoryStream = new MemoryStream(bytes, 0, (int)stream.Length))
                        {
                            metadata = await DropboxClient.Files.UploadAsync(DROPBOX_DATA_FILE_PATH, WriteMode.Overwrite.Instance, body: memoryStream);
                            _synchroDate = metadata.ServerModified;
                            onUploadFinished?.Invoke();
                        }
                    }
                }
            }
            catch(Exception exc)
            {
                var msg = exc.Message;
            }
        }

        public async Task<bool> HasStoredData()
        {
            if (DropboxClient == null)
                return false;

            try
            {
                LogsManager.Instance.WriteLine("Dropbox loading");
                var metadata = await DropboxClient.Files.GetMetadataAsync(DROPBOX_DATA_FILE_PATH);
                
                return metadata.IsFile;
            }
            catch (ApiException<GetMetadataError>)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
