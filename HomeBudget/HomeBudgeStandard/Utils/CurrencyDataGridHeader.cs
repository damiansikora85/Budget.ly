using Syncfusion.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace HomeBudget.Utils
{
    public class CurrencyDataGridHeader : ISummaryAggregate
    {
        public CurrencyDataGridHeader()
        {

        }
        public string Currency { get; set; }

        Action<IEnumerable, string, PropertyDescriptor> ISummaryAggregate.CalculateAggregateFunc()
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
            var sum = values.Select(selector).Sum(elem => elem.Value);
            var cultureInfoPL = new CultureInfo("pl-PL");
            return string.Format(cultureInfoPL, "{0:c}", sum);
        }
    }
}
