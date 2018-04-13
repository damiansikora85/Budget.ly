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
        public BudgetPlannedCategory() : base() { }

        public BudgetPlannedCategory(BudgetPlannedCategory category) : base(category)
        {
            foreach (BaseBudgetSubcat subcat in category.subcats)
            {
                PlannedSubcat plannedSubcat = new PlannedSubcat(subcat);
                plannedSubcat.PropertyChanged += OnSubcatChanged;
                subcats.Add(new PlannedSubcat(subcat));
            }
        }

        public static BudgetPlannedCategory Create(BudgetCategoryTemplate categoryDesc)
        {
            BudgetPlannedCategory category = new BudgetPlannedCategory()
            {
                Name = categoryDesc.Name,
                Id = categoryDesc.Id,
                IsIncome = categoryDesc.IsIncome,
                IconName = categoryDesc.IconFileName
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

        public override void Deserialize(BinaryData binaryData)
        {
            base.Deserialize(binaryData);
            int subcatNum = binaryData.GetInt();
            for (int i = 0; i < subcatNum; i++)
            {
                PlannedSubcat subcat = new PlannedSubcat();
                subcat.PropertyChanged += OnSubcatChanged;
                subcat.Deserialize(binaryData);
                subcats.Add(subcat);
            }
        }
    }
}
