using Syncfusion.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;
using HomeBudget.Pages.PC;
using System.Globalization;

namespace HomeBudget.Utils
{
    public class CurrencyDataGridHeader : ISummaryAggregate
    {
        public CurrencyDataGridHeader()
        {

        }
        public string Currency { get; set; }
        public Action<IEnumerable, string, PropertyInfo> CalculateAggregateFunc()
        {
            return (items, property, pd) =>
            {
                var enumerableItems = items as IEnumerable<BudgetViewModelData>;
                if (pd.Name == "Currency")
                {
                    this.Currency = enumerableItems.Currency(q => q.Subcat.Value);
                }
            };
        }
    }

    public static class LinqExtensions
    {
        public static string Currency<T>(this IEnumerable<T> values, Func<T, double?> selector)
        {
            double sum = values.Select(selector).Sum(elem => elem.Value);
            CultureInfo cultureInfoPL = new CultureInfo("pl-PL");
            return string.Format(cultureInfoPL, "{0:c}", sum);
        }
    }
}
