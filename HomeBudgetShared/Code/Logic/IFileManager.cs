using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public interface IFileManager
    {
        Task Save(BudgetData saveData);
        Task<BudgetData> Load();

        Task WriteLine(string filename, string message);
        Task<string> ReadFile(string filename);
        Task DeleteFile(string filename);
    }
}
