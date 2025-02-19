using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Code
{
	public class BudgetDescription
	{
		public string name { get; set; }

		[JsonProperty("categories")]
		public List<BudgetCategoryTemplate> Categories { get; set; }

        public void UpdateCategories(List<BudgetCategoryForEdit> updatedCategories)
        {

            foreach(var category in updatedCategories)
            {
                var foundCategory = Categories.FirstOrDefault(cat => cat.Id == category.Id);
                if(foundCategory != null)
                {
                    foundCategory.Update(category);
                }
            }
        }

        public List<BudgetCategoryForEdit> GetBudgetTemplateEdit()
        {
            var result = Categories.Select(category =>
            {
                int id = 0;
                var item = new BudgetCategoryForEdit { Name = category.Name, Id = category.Id, IconFile = category.IconFileName };
                var subcats = category.subcategories.Select(subcat => new BudgetSubcatEdit { Name = subcat, Id = id++ });
                foreach (var subcat in subcats)
                {
                    item.Add(subcat);
                }
                return item;
            }).ToList();

            return result;
        }
    }
}
