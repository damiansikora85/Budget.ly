using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace HomeBudget.Code.Logic
{
    [ProtoContract]
    public class BudgetPlannedCategory : BaseBudgetCategory
    {
        public BudgetPlannedCategory() : base() { }

        public BudgetPlannedCategory(BudgetPlannedCategory category) : base(category)
        {
            foreach (var subcat in category.subcats)
            {
                var plannedSubcat = new PlannedSubcat(subcat);
                plannedSubcat.PropertyChanged += OnSubcatChanged;
                subcats.Add(new PlannedSubcat(subcat));
            }
        }

        public static BudgetPlannedCategory Create(BudgetCategoryTemplate categoryDesc)
        {
            var category = new BudgetPlannedCategory()
            {
                Name = categoryDesc.Name,
                Id = categoryDesc.Id,
                IsIncome = categoryDesc.IsIncome,
                IconName = categoryDesc.IconFileName
            };

            int index = 0;
            foreach (string subcatName in categoryDesc.subcategories)
            {
                var subcat = PlannedSubcat.Create(subcatName, index++);
                subcat.PropertyChanged += category.OnSubcatChanged;
                category.subcats.Add(subcat);
            }

            return category;
        }

        /*public override void Prepare()
        {
            foreach (var subcat in subcats)
            {
                subcat.Prepare();
                subcat.PropertyChanged += OnSubcatChanged;
            }
        }*/
    }
}
