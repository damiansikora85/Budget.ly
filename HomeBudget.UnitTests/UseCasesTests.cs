using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using HomeBudget.Code;
using HomeBudget.Code.Logic;
using HomeBudgetShared.Code.Synchronize;
using HomeBudgetShared.Code.Interfaces;
using HomeBudget.Code.Interfaces;
using HomeBudget.UseCases;

namespace HomeBudget.UnitTests
{
    [TestClass]
    public class UseCasesTests
    {
        [TestMethod]
        public async Task MainBudgetInitTest()
        {
            await MainBudget.Instance.Init(new Mock<IFileManager>().Object, new Mock<IBudgetSynchronizer>().Object, new Mock<ICrashReporter>().Object, new Mock<ISettings>().Object);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task GetCurrentMonthTest()
        {
            await MainBudget.Instance.Init(new Mock<IFileManager>().Object, new Mock<IBudgetSynchronizer>().Object, new Mock<ICrashReporter>().Object, new Mock<ISettings>().Object);
            var currentMonth = MainBudget.Instance.GetCurrentMonthData();
            Assert.IsNotNull(currentMonth);
        }

        [TestMethod]
        public async Task AddIncomeTest()
        {
            await MainBudget.Instance.Init(new Mock<IFileManager>().Object, new Mock<IBudgetSynchronizer>().Object, new Mock<ICrashReporter>().Object, new Mock<ISettings>().Object);
            var currentMonth = MainBudget.Instance.GetCurrentMonthData();
            var category = currentMonth.BudgetReal.GetIncomesCategories()[0];
            var income = 5000;
            AddExpenseUseCase.AddExpense(income, DateTime.Now, category, 0, "test");
            Assert.AreEqual(income, category.GetSubcat(0).Value);
        }
    }
}
