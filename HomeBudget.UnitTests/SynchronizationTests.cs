using HomeBudget.Code.Logic;
using HomeBudgetShared.Code.Interfaces;
using HomeBudgetShared.Code.Synchronize;
using NUnit.Framework;
using Moq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace HomeBudget.UnitTests
{
    [TestFixture]
    public class SynchronizationTests
    {
        [Test]
        public void StartStopSynchronizeTest()
        {
            var cloudStorageMock = new Mock<ICloudStorage>();
            var cloudStorage = cloudStorageMock.Object;
            var synchronizer = new BudgetSynchronizer(cloudStorage);
            synchronizer.Start();
            Thread.Sleep(100);
            Assert.IsTrue(synchronizer.IsRunning, "Synchronization not started");

            synchronizer.Stop();
            Thread.Sleep(5100);
            Assert.IsFalse(synchronizer.IsRunning, "Synchronization not stopped");
        }

        [Test]
        public void SynchronizeOkTest()
        {
            var cloudStorageMock = new Mock<ICloudStorage>();
            cloudStorageMock.Setup(cloud => cloud.UploadData(It.IsAny<BudgetData>()));
            var cloudStorage = cloudStorageMock.Object;
            var synchronizer = new BudgetSynchronizer(cloudStorage);
            synchronizer.Start();
            Thread.Sleep(100);
            synchronizer.ShouldSave = true;
            Thread.Sleep(5100);
            Assert.IsTrue(synchronizer.IsRunning, "Synchronization stop working");
        }

        [Test]
        public void SynchronizeFailTest()
        {
            var cloudStorageMock = new Mock<ICloudStorage>();
            cloudStorageMock.Setup(cloud => cloud.UploadData(It.IsAny<BudgetData>())).Throws(new System.Exception());
            var cloudStorage = cloudStorageMock.Object;
            var synchronizer = new BudgetSynchronizer(cloudStorage);
            synchronizer.Start();
            Thread.Sleep(100);
            synchronizer.ShouldSave = true;
            Thread.Sleep(5100);
            Assert.IsTrue(synchronizer.IsRunning, "Synchronization stop working after exception");
        }
    }
}
