using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgeStandard.Code.Dropbox
{
    public interface IDropbox
    {
        void Init();
        Task DownloadData();
        Task UploadData(byte[] data);
    }
}
