using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeBudget.Pages.Utils
{
    public class BudgetSideBar
    {
        private View categoriesView;
        private View subcatView;

        public void Init(View categories, View subcats)
        {
            categoriesView = categories;
            subcatView = subcats;
        }

        public void ShowCategories()
        {

        }

        public void ShowSubCategories(int categoryID)
        {

        }

        public void OnCategoryClick()
        {

        }

        public void OnSubcatClick()
        {

        }
    }
}
