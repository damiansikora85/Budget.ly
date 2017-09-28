using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public class BudgetRealCategory : BaseBudgetCategory
    {
        public static BudgetRealCategory Create(BudgetCategoryTemplate categoryDesc)
        {
            BudgetRealCategory category = new BudgetRealCategory()
            {
                Name = categoryDesc.Name,
                Id = categoryDesc.Id,
                IsIncome = categoryDesc.IsIncome
            };

            int index = 0;
            foreach (string subcatName in categoryDesc.subcategories)
            {
                RealSubcat subcat = RealSubcat.Create(subcatName, index++);
                category.subcats.Add(subcat);
            }

            return category;
        }

        public override byte[] Serialize()
        {
            return base.Serialize();
        }

        public override void Deserialize(BinaryData binaryData)
        {
            base.Deserialize(binaryData);
            int subcatNum = binaryData.GetInt();
            for (int i = 0; i < subcatNum; i++)
            {
                RealSubcat subcat = new RealSubcat();
                subcat.Deserialize(binaryData);
                subcats.Add(subcat);
            }
        }

        public void AddValue(double value, DateTime date, int subcatId)
        {
            RealSubcat subcat = subcats.Find(elem => elem.Id == subcatId) as RealSubcat;
            subcat.AddValue(value, date);
        }
    }
}
