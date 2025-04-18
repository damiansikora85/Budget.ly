﻿using ProtoBuf;
using System;

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
            if (subcats != null && subcats.Find(elem => elem.Id == subcatId) is RealSubcat subcat)
            {
                subcat.AddValue(value, date);
            }
        }

        internal void RemoveValue(double value, DateTime date, int subcatId)
        {
            if (subcats != null && subcats.Find(elem => elem.Id == subcatId) is RealSubcat subcat)
            {
                subcat.RemoveValue(value, date);
            }
        }
    }
}
