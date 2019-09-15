using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudgetShared.Code.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetShared.Code.Synchronize
{
    public interface IBudgetSynchronizer
    {
        bool IsRunning { get; }
        bool ShouldSave { get; set; }
        ICloudStorage CloudStorage { get; }

        event EventHandler<BudgetData> DataDownloaded;
        event EventHandler DownloadError;

        void Start();
        void Stop();

        Task<BudgetData> ForceLoad();
        Task UploadBudgetTemplate(BudgetDescription data);
        Task<BudgetDescription> DownloadBudgetTemplate();
    }
}
