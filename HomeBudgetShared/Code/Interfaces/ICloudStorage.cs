using HomeBudget.Code;
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
        Task<DateTime> UploadData(BudgetData data);
        Task<BudgetData> DownloadData();
        Task<bool> HasStoredData();
        Task<DateTime> GetCloudFileModifiedTime();
        Task<DateTime> UploadBudgetTemplate(BudgetDescription budgetTemplate);
        Task<BudgetDescription> DownloadBudgetTemplate();

        event EventHandler<BudgetData> OnDownloadFinished;
        event EventHandler OnDownloadError;
    }
}
