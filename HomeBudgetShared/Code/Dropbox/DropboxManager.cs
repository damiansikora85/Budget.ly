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

        public event EventHandler<BudgetData> OnDownloadFinished;
        public event EventHandler OnDownloadError;

        public DropboxCloudStorage()
        {
            _dropboxClient = null;
        }

        public async Task<DateTime> GetCloudFileModifiedTime()
        {
            if (DropboxClient == null)
                return DateTime.MinValue;

            try
            {
                var metadata = await DropboxClient.Files.GetMetadataAsync(DROPBOX_DATA_FILE_PATH) as FileMetadata;
                return metadata.ServerModified;

            }
            catch (ApiException<GetMetadataError>)
            {
                return DateTime.MinValue;
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        public async Task<BudgetData> DownloadData()
        {
            if (DropboxClient == null)
                return null;

            var budgetData = new BudgetData();
            var cloudFileModifiedDate = await GetCloudFileModifiedTime();
            try
            {
                LogsManager.Instance.WriteLine("DownloadData - Dropbox loading");
                //var metadata = await DropboxClient.Files.GetMetadataAsync(DROPBOX_DATA_FILE_PATH) as FileMetadata;
               
                using (var response = await DropboxClient.Files.DownloadAsync(DROPBOX_DATA_FILE_PATH))
                {
                    var bytes = await response.GetContentAsByteArrayAsync();
                    var stream = new MemoryStream(bytes, 0, bytes.Length);
                    budgetData = Serializer.Deserialize<BudgetData>(stream);


                    /*var protoReader = new ProtoReader(stream, null, new SerializationContext { });

                    while (protoReader.ReadFieldHeader() > 0)
                    {
                        switch (protoReader.WireType)
                        {
                            case WireType.None:
                                break;
                            case WireType.Variant:
                                protoReader.SkipField();
                                break;
                            case WireType.Fixed64:
                                protoReader.SkipField();
                                break;
                            case WireType.String:
                                var s = protoReader.ReadString();
                                break;
                            case WireType.StartGroup:
                                protoReader.SkipField();
                                break;
                            case WireType.EndGroup:
                                protoReader.SkipField();
                                break;
                            case WireType.Fixed32:
                                protoReader.SkipField();
                                break;
                            case WireType.SignedVariant:
                                protoReader.SkipField();
                                break;
                        }
                    }*/

                    //OnDownloadFinished?.Invoke(this, budgetData);
                    //OnDownloadFinished?.Invoke(null);
                }
                return budgetData;
            }
            catch (ApiException<GetMetadataError>)
            {
                LogsManager.Instance.WriteLine("Dropbox file not found");
                //file not found
                //OnDownloadFinished?.Invoke(this, null);
                return null;
            }
            catch(Exception e)
            {
                LogsManager.Instance.WriteLine("Dropbox other exception: "+e.Message);
                if(e.InnerException != null)
                {
                    LogsManager.Instance.WriteLine("Dropbox other exception: " + e.InnerException.Message);
                }
                //OnDownloadError?.Invoke();
                //OnDownloadFinished?.Invoke(this, null);
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
                var msg = exc.Message;
            }

            return cloudFileModifedDate;
        }

        public async Task<bool> HasStoredData()
        {
            if (DropboxClient == null)
                return false;

            try
            {
                LogsManager.Instance.WriteLine("HasStoredData - Dropbox loading");
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
