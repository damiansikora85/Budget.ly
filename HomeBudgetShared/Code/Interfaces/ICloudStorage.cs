using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetShared.Code.Interfaces
{
    public interface ICloudStorage
    {
        // Init();
        Task UploadData(BudgetData data);
        Task DownloadData();

        Action<BudgetData> OnDownloadFinished { get; set; }
        Action OnDownloadError { get; set; }
    }
}
