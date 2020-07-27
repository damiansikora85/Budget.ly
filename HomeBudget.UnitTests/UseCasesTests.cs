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
using NUnit.Framework;

namespace HomeBudget.UnitTests
{
    [TestFixture]
    public class UseCasesTests
    {
        [Test]
        public async Task MainBudgetInitTest()
        {
            await MainBudget.Instance.Init(new Mock<IFileManager>().Object, new Mock<IBudgetSynchronizer>().Object, new Mock<ICrashReporter>().Object, new Mock<ISettings>().Object, new Mock<IFeatureSwitch>().Object);
            Assert.IsTrue(true);
        }

        [Test]
        public async Task GetCurrentMonthTest()
        {
            await MainBudget.Instance.Init(new Mock<IFileManager>().Object, new Mock<IBudgetSynchronizer>().Object, new Mock<ICrashReporter>().Object, new Mock<ISettings>().Object, new Mock<IFeatureSwitch>().Object);
            var currentMonth = MainBudget.Instance.GetCurrentMonthData();
            Assert.IsNotNull(currentMonth);
        }

        [Test]
        public async Task AddIncomeTest()
        {
            await MainBudget.Instance.Init(new Mock<IFileManager>().Object, new Mock<IBudgetSynchronizer>().Object, new Mock<ICrashReporter>().Object, new Mock<ISettings>().Object, new Mock<IFeatureSwitch>().Object);
            var currentMonth = MainBudget.Instance.GetCurrentMonthData();
            var category = currentMonth.BudgetReal.GetIncomesCategories()[0];
            var income = 5000;
            BudgetUseCases.AddExpense(income, DateTime.Now, category, 0, "test");
            Assert.AreEqual(income, category.GetSubcat(0).Value);
            Assert.AreEqual(1, currentMonth.BudgetReal.Transactions.Count());
        }

        [Test]
        public async Task RemoveTransactionTest()
        {
            await MainBudget.Instance.Init(new Mock<IFileManager>().Object, new Mock<IBudgetSynchronizer>().Object, new Mock<ICrashReporter>().Object, new Mock<ISettings>().Object, new Mock<IFeatureSwitch>().Object);
            var currentMonth = MainBudget.Instance.GetCurrentMonthData();
            var category = currentMonth.BudgetReal.GetExpensesCategories()[0];
            var expense = 5000;

            BudgetUseCases.AddExpense(expense, DateTime.Now, category, 0, "test");
            Assert.AreEqual(1, currentMonth.BudgetReal.Transactions.Count());
            Assert.AreEqual(expense, category.TotalValues);

            var transaction = currentMonth.BudgetReal.Transactions.FirstOrDefault();
            BudgetUseCases.RemoveTransaction(transaction);
            Assert.AreEqual(0, currentMonth.BudgetReal.Transactions.Count());
            Assert.AreEqual(0, category.TotalValues);
        }
    }
}
