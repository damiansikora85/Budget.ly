using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public sealed class BudgetRealCategory : BaseBudgetCategory
    {
        public static BudgetRealCategory Create(BudgetCategoryTemplate categoryDesc)
        {
            var category = new BudgetRealCategory()
            {
                Name = categoryDesc.Name,
                Id = categoryDesc.Id,
                IsIncome = categoryDesc.IsIncome,
                IconName = categoryDesc.IconFileName
            };

            int index = 0;
            foreach (string subcatName in categoryDesc.subcategories)
            {
                var subcat = RealSubcat.Create(subcatName, index++);
                subcat.PropertyChanged += category.OnSubcatChanged;
                category.subcats.Add(subcat);
            }

            return category;
        }

        public override void Deserialize(BinaryData binaryData)
        {
            base.Deserialize(binaryData);
            var subcatNum = binaryData.GetInt();
            for (int i = 0; i < subcatNum; i++)
            {
                var subcat = new RealSubcat();
                subcat.PropertyChanged += OnSubcatChanged;
                subcat.Deserialize(binaryData);
                subcats.Add(subcat);
            }
        }

        public void AddValue(double value, DateTime date, int subcatId)
        {
            var subcat = subcats.Find(elem => elem.Id == subcatId) as RealSubcat;
            subcat.AddValue(value, date);
        }
    }
}
