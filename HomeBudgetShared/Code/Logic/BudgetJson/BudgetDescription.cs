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

        public string ToJson()
        {
            return string.Empty;
        }
    }
}
