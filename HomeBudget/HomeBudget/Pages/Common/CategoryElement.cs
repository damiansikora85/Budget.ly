using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HomeBudget.Pages
{
    class CategoryElement
    {
        private Button button;
        private Image icon;
        private Label label;
        private Image background;
        private int id;
        public int Id
        {
            get { return id; }
        }

        private string name;
        public string Name
        {
            get { return name; }
        }

        public Func<int, CategoryElement, Task> onClickFunc;

        public static CategoryElement CreateAndAddToGrid(int categoryID, string categoryName, string categoryIconFile, Grid grid)
        {
            CategoryElement categoryElement = new CategoryElement()
            {
                id = categoryID,
                name = categoryName,

                background = new Image()
                {
                    BackgroundColor = Color.Transparent
                },
                icon = new Image()
                {
                    Source = categoryIconFile,
                    VerticalOptions = LayoutOptions.End
                },
                label = new Label
                {
                    Text = categoryName,
                    FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                    VerticalOptions = LayoutOptions.Start,
                    HorizontalOptions = LayoutOptions.Center,
                },
                button = new Button()
                {
                    BackgroundColor = Color.Transparent,

                }
            };
            categoryElement.button.Clicked += categoryElement.OnClicked;

            Grid.SetColumnSpan(categoryElement.background, 3);
            Grid.SetRowSpan(categoryElement.background, 2);
            Grid.SetColumnSpan(categoryElement.label, 3);
            Grid.SetRow(categoryElement.label, 1);
            Grid.SetRow(categoryElement.icon, 0);
            Grid.SetColumn(categoryElement.icon, 1);
            Grid.SetColumnSpan(categoryElement.button, 3);
            Grid.SetRowSpan(categoryElement.button, 2);

            grid.Children.Add(categoryElement.background);
            grid.Children.Add(categoryElement.icon);
            grid.Children.Add(categoryElement.label);
            grid.Children.Add(categoryElement.button);

            return categoryElement;
        }

        private async void OnClicked(object sender, EventArgs e)
        {
            this.background.BackgroundColor = Color.FromHex("D2F3DF");
            //onClick(id, this);
            await onClickFunc(id, this);
        }

        public void Deselect()
        {
            this.background.BackgroundColor = Color.Transparent;
        }
    }
}
