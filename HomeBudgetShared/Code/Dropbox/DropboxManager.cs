using Dropbox.Api;
using Dropbox.Api.Files;
using HomeBudget.Code.Logic;
using HomeBudgetShared.Code.Interfaces;
using HomeBudgetShared.Utils;
using Newtonsoft.Json;
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
        public static string DROPBOX_TEMPLATE_FILE_PATH = "/budget_template.json";

        public Action onLogedIn;
        public Action onUploadFinished;

        private bool _isSynchronized;
        private const int SYNCHRO_DELAY_MS = 5*1000;

        public event EventHandler<BudgetData> OnDownloadFinished;
        public event EventHandler OnDownloadError;

        private ICrashReporter _crashReporter;

        public DropboxCloudStorage(ICrashReporter crashReporter)
        {
            _dropboxClient = null;
            _crashReporter = crashReporter;
        }

        public async Task<DateTime> GetCloudFileModifiedTime()
        {
            if (DropboxClient == null)
                return DateTime.MinValue;

            try
            {
                LogsManager.Instance.WriteLine("Dropbox loading");
                var metadata = await DropboxClient.Files.GetMetadataAsync(DROPBOX_DATA_FILE_PATH) as FileMetadata;
                return metadata.ServerModified;

            }
            catch (ApiException<GetMetadataError> apiExc)
            {
                _crashReporter.Report(apiExc);
                return DateTime.MinValue;
            }
            catch (Exception exc)
            {
                _crashReporter.Report(exc);
                return DateTime.MinValue;
            }
        }

        public async Task<BudgetData> DownloadData()
        {
            if (DropboxClient == null)
                return null;

            try
            {
                LogsManager.Instance.WriteLine("Dropbox loading");

                BudgetData budgetData;
                using (var response = await DropboxClient.Files.DownloadAsync(DROPBOX_DATA_FILE_PATH))
                {
                    var bytes = await response.GetContentAsByteArrayAsync();
                    var stream = new MemoryStream(bytes, 0, bytes.Length);
                    budgetData = Serializer.Deserialize<BudgetData>(stream);
                }
                return budgetData;
            }
            catch (ApiException<GetMetadataError> apiExc)
            {
                _crashReporter.Report(apiExc);
                LogsManager.Instance.WriteLine("Dropbox file not found");
                //file not found
                return null;
            }
            catch(Exception exc)
            {
                _crashReporter.Report(exc);
                LogsManager.Instance.WriteLine("Dropbox other exception: "+exc.Message);
                if(exc.InnerException != null)
                {
                    LogsManager.Instance.WriteLine("Dropbox other exception: " + exc.InnerException.Message);
                }
                return null;
            }
        }

        public async Task<DateTime> UploadData(BudgetData budgetData)
        {
            if (DropboxClient == null)
                return DateTime.MinValue;

            DateTime cloudFileModifedDate = DateTime.MinValue;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, budgetData);
                    await stream.FlushAsync();
                    var bytes = stream.GetBuffer();
                    using (var memoryStream = new MemoryStream(bytes, 0, (int)stream.Length))
                    {
                        var metadata = await DropboxClient.Files.UploadAsync(DROPBOX_DATA_FILE_PATH, WriteMode.Overwrite.Instance, body: memoryStream);
                        cloudFileModifedDate = metadata.ServerModified;
                        onUploadFinished?.Invoke();
                    }
                }
            }
            catch(Exception exc)
            {
                _crashReporter.Report(exc);
            }

            return cloudFileModifedDate;
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
            catch (ApiException<GetMetadataError> apiExc)
            {
                _crashReporter.Report(apiExc);
                return false;
            }
            catch (Exception exc)
            {
                _crashReporter.Report(exc);
                return false;
            }
        }

        public async Task<DateTime> UploadBudgetTemplate(BudgetDescription budgetTemplate)
        {
            if (DropboxClient == null)
                return DateTime.MinValue;

            DateTime cloudFileModifedDate = DateTime.MinValue;
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    var templateJson = JsonConvert.SerializeObject(budgetTemplate);

                    using (var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(templateJson)))
                    {
                        var metadata = await DropboxClient.Files.UploadAsync(DROPBOX_TEMPLATE_FILE_PATH, WriteMode.Overwrite.Instance, body: memoryStream);
                        cloudFileModifedDate = metadata.ServerModified;
                    }
                }
            }
            catch (Exception exc)
            {
                _crashReporter.Report(exc);
            }

            return cloudFileModifedDate;
        }

        public async Task<BudgetDescription> DownloadBudgetTemplate()
        {
            if (DropboxClient == null)
                return null;

            try
            {
                LogsManager.Instance.WriteLine("Template loading");
                var metadata = await DropboxClient.Files.GetMetadataAsync(DROPBOX_TEMPLATE_FILE_PATH);
                if (metadata.IsFile)
                {
                    BudgetDescription budgetTemplate;
                    using (var response = await DropboxClient.Files.DownloadAsync(DROPBOX_TEMPLATE_FILE_PATH))
                    {
                        var templateJson = await response.GetContentAsStringAsync();
                        budgetTemplate = JsonConvert.DeserializeObject<BudgetDescription>(templateJson);
                    }
                    return budgetTemplate;
                }
                else
                {
                    return null;
                }
            }
            catch (ApiException<GetMetadataError> apiExc)
            {
                _crashReporter.Report(apiExc);
                LogsManager.Instance.WriteLine("Template file not found");
                //file not found
                return null;
            }
            catch (Exception exc)
            {
                _crashReporter.Report(exc);
                LogsManager.Instance.WriteLine("Dropbox other exception: " + exc.Message);
                if (exc.InnerException != null)
                {
                    LogsManager.Instance.WriteLine("Dropbox other exception: " + exc.InnerException.Message);
                }
                return null;
            }
        }
    }
}
