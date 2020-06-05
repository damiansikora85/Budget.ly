namespace HomeBudget.UnitTestsF

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open HomeBudget.Code
open HomeBudget.UseCases
open Moq
open HomeBudget.Code.Logic
open HomeBudgetShared.Code.Synchronize
open HomeBudgetShared.Code.Interfaces
open HomeBudget.Code.Interfaces

[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestBudgetInit () =
        MainBudget.Instance.Init(Mock<IFileManager>().Object, Mock<IBudgetSynchronizer>().Object, Mock<ICrashReporter>().Object, Mock<ISettings>().Object)
        Assert.IsNotNull(MainBudget.Instance);

    [<TestMethod>]
    member this.AddExpenseTest () =
        MainBudget.Instance.Init(Mock<IFileManager>().Object, Mock<IBudgetSynchronizer>().Object, Mock<ICrashReporter>().Object, Mock<ISettings>().Object)

        let currentMonth = MainBudget.Instance.GetCurrentMonthData()
        let category = currentMonth.BudgetReal.Categories.[0]
        let value = 123.99
        AddExpenseUseCase.AddExpense(value, DateTime.Now, category, 0, "test")
        Assert.AreEqual(category.GetSubcat(0).Value, value)
