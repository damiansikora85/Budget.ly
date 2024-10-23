using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgetStandard.Pages
{
    public class MainPageMenuItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public Type TargetType { get; set; }
        public Action OnClick { get; set; }
        public bool UseBrandsIcon { get; set; }
    }
}