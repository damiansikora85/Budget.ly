using HomeBudget.Code;
using HomeBudget.Code.Logic;
using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace HomeBudget.UnitTests
{
    [TestFixture]
    public class ProtobufTest
    {
        [Test]
        public void PlannedSubcatTest()
        {
            var subcat = PlannedSubcat.Create("test", 5);
            subcat.Value = 123.99;

            using (var file = File.Create("subcatTest.bin"))
            {
                Serializer.Serialize(file, subcat);
            }

            using (var file = File.OpenRead("subcatTest.bin"))
            {
                var deserializedSubcat = Serializer.Deserialize<PlannedSubcat>(file);
                Assert.IsTrue(AreEqual(subcat, deserializedSubcat));
            }
        }

        [Test]
        public void RealSubcatTest()
        {
            var subcat = RealSubcat.Create("test", 5);
            subcat.Values[0].Value = 1.0;
            subcat.Values[5].Value = 12.3;
            subcat.Values[10].Value = 1;
            subcat.Values[15].Value = 1;
            subcat.Values[30].Value = 1;

            using (var file = File.Create("subcatTest.bin"))
            {
                Serializer.Serialize(file, subcat);
            }

            using (var file = File.OpenRead("subcatTest.bin"))
            {
                var deserializedSubcat = Serializer.Deserialize<RealSubcat>(file);
                Assert.AreEqual(subcat.Value, deserializedSubcat.Value);
            }
        }

        [Test]
        public void PlannedCategoryTest()
        {
            var assembly = typeof(MainBudget).GetTypeInfo().Assembly;

            var names = assembly.GetManifestResourceNames();
            var stream = assembly.GetManifestResourceStream("HomeBudgeStandard.template.json");
            var jsonString = "";
            using (var reader = new StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
            }
            var budgetDescription = JsonConvert.DeserializeObject<BudgetDescription>(jsonString);
            var category = BudgetPlannedCategory.Create(budgetDescription.Categories[0]);

            using (var file = File.Create("categoryTest.bin"))
            {
                Serializer.Serialize(file, category);
            }

            using (var file = File.OpenRead("categoryTest.bin"))
            {
                var deserializedCategory= Serializer.Deserialize<BudgetPlannedCategory>(file);
                Assert.AreEqual(category.TotalValues, deserializedCategory.TotalValues);
            }
        }

        [Test]
        public void RealCategoryTest()
        {
            var assembly = typeof(MainBudget).GetTypeInfo().Assembly;

            var names = assembly.GetManifestResourceNames();
            var stream = assembly.GetManifestResourceStream("HomeBudgeStandard.template.json");
            var jsonString = "";
            using (var reader = new StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
            }
            var budgetDescription = JsonConvert.DeserializeObject<BudgetDescription>(jsonString);
            var category = BudgetRealCategory.Create(budgetDescription.Categories[budgetDescription.Categories.Count-1]);

            using (var file = File.Create("realCategoryTest.bin"))
            {
                Serializer.Serialize(file, category);
            }

            using (var file = File.OpenRead("realCategoryTest.bin"))
            {
                var deserializedCategory = Serializer.Deserialize<BudgetRealCategory>(file);
                Assert.AreEqual(category.TotalValues, deserializedCategory.TotalValues);
            }
        }

        [Test]
        public void BudgetMonthTest()
        {
            var assembly = typeof(MainBudget).GetTypeInfo().Assembly;

            var names = assembly.GetManifestResourceNames();
            var stream = assembly.GetManifestResourceStream("HomeBudgeStandard.template.json");
            var jsonString = "";
            using (var reader = new StreamReader(stream))
            {
                jsonString = reader.ReadToEnd();
            }
            var budgetDescription = JsonConvert.DeserializeObject<BudgetDescription>(jsonString);

            var monthData = BudgetMonth.Create(budgetDescription.Categories, DateTime.Now);

            using (var file = File.Create("monthTest.bin"))
            {
                Serializer.Serialize(file, monthData);
            }

            using (var file = File.OpenRead("monthTest.bin"))
            {
                var deserializedMonth = Serializer.Deserialize<BudgetMonth>(file);
                Assert.AreEqual(monthData.GetTotalExpenseReal(), deserializedMonth.GetTotalExpenseReal());
            }
        }

        private bool AreEqual(BaseBudgetSubcat first, BaseBudgetSubcat second)
        {
            return first.Name == second.Name
                && first.Id == second.Id
                && first.Value == second.Value;
        }
    }
}
