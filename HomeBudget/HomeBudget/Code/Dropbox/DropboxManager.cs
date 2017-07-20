using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dropbox.Api;
using Dropbox.Api.Files;
using Xamarin.Forms;
using System.IO;

namespace HomeBudget.Code
{
    public class DropboxManager
    {
        private static DropboxManager instance;
        public static DropboxManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new DropboxManager();

                return instance;
            }
        }

        private DropboxClient dropboxClient;
        private string accessToken;
        private const string DROPBOX_DATA_FILE_PATH = "/budget_data.dat";

        public Action onLogedIn;
        public Action<byte[]> onDownloadFinished;
        public Action onDownloadError;
        public Action onUploadFinished;

        private DropboxManager()
        {
            dropboxClient = null;
            if (Helpers.Settings.DropboxAccessToken != string.Empty)
            {
                accessToken = Helpers.Settings.DropboxAccessToken;
                dropboxClient = new DropboxClient(accessToken);
            }
        }

        public void Init()
        {
            dropboxClient = null;
            if (Helpers.Settings.DropboxAccessToken != string.Empty)
            {
                accessToken = Helpers.Settings.DropboxAccessToken;
                dropboxClient = new DropboxClient(accessToken);
            }
        }

        public async Task DownloadData()
        {
            if (dropboxClient == null)
                return;
            try
            {
                Metadata metadata = await dropboxClient.Files.GetMetadataAsync(DROPBOX_DATA_FILE_PATH);
                using (var response = await dropboxClient.Files.DownloadAsync(DROPBOX_DATA_FILE_PATH))
                {
                    byte[] data = await response.GetContentAsByteArrayAsync();
                    onDownloadFinished?.Invoke(data);
                }
            }
            catch
            {
                onDownloadError?.Invoke();
            }
        }

        public async Task UploadData(byte[] data)
        {
            if (dropboxClient == null)
                return;

            MemoryStream memoryStream = new MemoryStream(data);
            await dropboxClient.Files.UploadAsync(DROPBOX_DATA_FILE_PATH, WriteMode.Overwrite.Instance, false, null, false, memoryStream);
            onUploadFinished?.Invoke();
        }
    }
}
