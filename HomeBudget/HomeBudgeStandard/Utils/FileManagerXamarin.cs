using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudgetShared.Utils;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgeStandard.Utils
{
    public class FileManagerXamarin : IFileManager
    {
        private object lockFile = new object();
        private const string BudgetFilename = "budget.dat";
        private const string BudgetBackupFilename = "backup.dat";
        private const string BudgetTemplateFilename = "budgetTemplate.json";

        public async Task DeleteFile(string filename)
        {
            await Task.Run(() =>
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                File.Delete(Path.Combine(documentsPath, filename));
            });
        }

        public async Task<BudgetData> Load()
        {
            var budgetData = await LoadFromFile(BudgetFilename);
            if (budgetData == null)
            {
                budgetData = await LoadFromFile(BudgetBackupFilename);
            }


            return budgetData;
        }

        private async Task<BudgetData> LoadFromFile(string filename)
        {
            return await Task.Run(() =>
            {
                try
                {
                    lock (lockFile)
                    {
                        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        var filePath = Path.Combine(documentsPath, filename);
                        if (File.Exists(filePath))
                        {
                            BudgetData data;
                            using (var file = File.OpenRead(filePath))
                            {
                                data = Serializer.Deserialize<BudgetData>(file);
                            }
                            return data;
                        }
                    }
                }
                catch (Exception exc)
                {
                    Microsoft.AppCenter.Crashes.Crashes.TrackError(exc);
                    LogsManager.Instance.WriteLine(exc.Message);
                    LogsManager.Instance.WriteLine(exc.StackTrace);
                    DeleteFile(filename);
                }
                return null;
            });
        }

        private async Task SaveBackup()
        {
            await Task.Factory.StartNew(() =>
            {

            });
        }

        public Task Save(BudgetData saveData)
        {
            return SaveToFile(BudgetFilename, saveData);
        }

        private async Task SaveToFile(string filename, BudgetData saveData)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    lock (lockFile)
                    {
                        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        var filePath = Path.Combine(documentsPath, filename);

                        var fileMode = FileMode.Truncate;
                        if (!File.Exists(filePath))
                        {
                            fileMode = FileMode.CreateNew;
                        }

                        using (var file = new FileStream(filePath, fileMode))
                        {
                            Serializer.Serialize<BudgetData>(file, saveData);
                        }
                    }
                }
                catch (Exception exc)
                {
                    Microsoft.AppCenter.Crashes.Crashes.TrackError(exc);
                    LogsManager.Instance.WriteLine(exc.Message);
                    LogsManager.Instance.WriteLine(exc.StackTrace);
                }
            });
        }

        public Task<string> ReadFile(string filename)
        {
            return Task.Run(() =>
            {
                lock (lockFile)
                {
                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var filePath = Path.Combine(documentsPath, filename);

                    return File.ReadAllText(filePath);
                }
            });
        }

        public Task WriteLine(string filename, string message)
        {
            return Task.Run(() =>
            {
                lock (lockFile)
                {
                    var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    var filePath = Path.Combine(documentsPath, filename);

                    File.AppendAllLines(filePath, new List<string> { message });
                }
            });
        }

        public bool HasCustomTemplate()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, BudgetTemplateFilename);
            return File.Exists(filePath);
        }

        public Task<BudgetDescription> ReadCustomTemplate()
        {
            return Task.Factory.StartNew(() =>
            {
                BudgetDescription data = null;
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, BudgetTemplateFilename);
                if (File.Exists(filePath))
                {
                    var jsonString = File.ReadAllText(filePath);
                    data = JsonConvert.DeserializeObject<BudgetDescription>(jsonString);
                }

                return data;
            });
        }

        public void WriteCustomTemplate(BudgetDescription templateData)
        {
            try
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, BudgetTemplateFilename);
                var jsonString = JsonConvert.SerializeObject(templateData);
                File.WriteAllText(filePath, jsonString);
            }
            catch
            {
                throw;
            }
        }
    }
}
