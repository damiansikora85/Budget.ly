using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetShared.Utils
{
    public class LogsManager
    {
        private IFileManager _fileManager;
        private static LogsManager _instance;
        private const string FILENAME = "budget_logs.txt";

        public void Init(IFileManager fileManager)
        {
            _fileManager = fileManager;
            _fileManager.DeleteFile(FILENAME);
        }

        private LogsManager() { }

        public static LogsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LogsManager();
                return _instance;
            }
        }

        public void WriteLine(string message)
        {
            if (_fileManager == null) return;
            _fileManager.WriteLine(FILENAME, message);
        }

        public async Task<string> ReadAll()
        {
            if (_fileManager == null) return "";
            return await _fileManager.ReadFile(FILENAME);
        }
    }
}
