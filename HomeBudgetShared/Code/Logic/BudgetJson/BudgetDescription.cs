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

        [JsonProperty("incomes")]
        public List<BudgetIncomeTemplate> Incomes { get; set; }

		[JsonProperty("categories")]
		public List<BudgetCategoryTemplate> Categories { get; set; }
	}
}
