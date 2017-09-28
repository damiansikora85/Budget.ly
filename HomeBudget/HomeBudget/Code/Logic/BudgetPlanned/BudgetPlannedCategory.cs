using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code.Logic
{
    public class BudgetPlannedCategory : BaseBudgetCategory
    {
        public static BudgetPlannedCategory Create(BudgetCategoryTemplate categoryDesc)
        {
            BudgetPlannedCategory category = new BudgetPlannedCategory()
            {
                Name = categoryDesc.Name,
                Id = categoryDesc.Id,
                IsIncome = categoryDesc.IsIncome
            };

            int index = 0;
            foreach (string subcatName in categoryDesc.subcategories)
            {
                PlannedSubcat subcat = PlannedSubcat.Create(subcatName, index++);
                subcat.PropertyChanged += category.OnSubcatChanged;
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
                PlannedSubcat subcat = new PlannedSubcat();
                subcat.Deserialize(binaryData);
                subcat.PropertyChanged += OnSubcatChanged;
                subcats.Add(subcat);
            }
        }
    }
}
