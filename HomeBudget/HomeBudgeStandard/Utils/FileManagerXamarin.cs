using HomeBudget.Code.Logic;
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
        public Task DeleteFile(string filename)
        {
            return Task.Run(() =>
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                File.Delete(Path.Combine(documentsPath, filename));
            });
        }

        public async Task<BudgetData> Load()
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, "budget.dat");
            if(File.Exists(filePath))
            {
                BudgetData data;
                using (var file = File.OpenRead(filePath))
                {
                    data = Serializer.Deserialize<BudgetData>(file);
                }
                return data;
            }

            return null;
        }

        public Task<string> ReadFile(string filename)
        {
            return Task.Run(() =>
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, filename);

                return File.ReadAllText(filePath);
            });
        }

        public async Task Save(BudgetData saveData)
        {
            
        }

        public Task WriteLine(string filename, string message)
        {
            return Task.Run(() =>
            {
                var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                var filePath = Path.Combine(documentsPath, filename);

                File.WriteAllText(filePath, message);
            });
        }
    }
}
