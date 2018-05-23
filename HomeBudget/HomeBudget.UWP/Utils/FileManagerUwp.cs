using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using System.IO;
using System.Runtime.Serialization;

namespace HomeBudget.UWP.Utils
{
    public class FileManagerUwp : IFileManager
    {
        public async Task<BudgetData> Load()
        {
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            if (await storageFolder.TryGetItemAsync("budget.dat") != null)
            {
                var sampleFile = await storageFolder.GetFileAsync("budget.dat");
                var stream = await sampleFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var data = Serializer.Deserialize<BudgetData>(stream.AsStream());
                
                return data;
            }
            else return null;
        }

        public async Task Save(BudgetData saveData)
        {
            var storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;  
            var sampleFile = await storageFolder.CreateFileAsync("budget.dat", Windows.Storage.CreationCollisionOption.ReplaceExisting);

            using (Stream writeStream = await sampleFile.OpenStreamForWriteAsync())
            {
                Serializer.Serialize(writeStream, saveData);
                await writeStream.FlushAsync();
                writeStream.Dispose();
            }
        }
    }
}
