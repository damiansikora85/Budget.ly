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
        public BudgetRealCategory() : base()
        {
        }

        public BudgetRealCategory(BaseBudgetCategory category) : base(category)
        {
            foreach (var subcat in category.subcats)
            {
                subcats.Add(new RealSubcat(subcat));
            }
        }

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

        public override void AddNewSubcat(BudgetSubcatEdit subcat)
        {
            var newSubcat = RealSubcat.Create(subcat.Name, subcat.Id);
            newSubcat.PropertyChanged += OnSubcatChanged;
            subcats.Add(newSubcat);
        }

        public void AddValue(double value, DateTime date, int subcatId)
        {
            var subcat = subcats.Find(elem => elem.Id == subcatId) as RealSubcat;
            subcat.AddValue(value, date);
        }
    }
}
