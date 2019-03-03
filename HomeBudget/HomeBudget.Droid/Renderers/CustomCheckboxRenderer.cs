using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Widget;
using HomeBudget.Components;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomCheckbox), typeof(HomeBudget.Droid.Renderers.CustomCheckboxRenderer))]
namespace HomeBudget.Droid.Renderers
{
    public class CustomCheckboxRenderer : ViewRenderer<CustomCheckbox, CheckBox>, CompoundButton.IOnCheckedChangeListener
    {
        private const int DEFAULT_SIZE = 28;

        int[][] states = new int[][]
          {
            new int[] {-Android.Resource.Attribute.StateChecked}, // unchecked
            new int[] { Android.Resource.Attribute.StateChecked }  // pressed
          };

        int[] colors = new int[]
        {
            Color.Gray.ToAndroid(),
            Color.DodgerBlue.ToAndroid(),
        };

        

        public CustomCheckboxRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<CustomCheckbox> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var checkbox = new CheckBox(Android.App.Application.Context);
                    checkbox.SetOnCheckedChangeListener(this);
                    SetNativeControl(checkbox);
                }
                //var colors = Control.get
                Control.Checked = e.NewElement.IsChecked;
                Control.Text = e.NewElement.Text;
                //var tc = Control.TextColors;
                //var bc = Control.ButtonTintList;
                Control.SetTextColor(Color.Black.ToAndroid());

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    var myList = new ColorStateList(states, colors);
                    Control.ButtonTintList = myList;
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(Element.IsChecked))
            {
                Control.Checked = Element.IsChecked;
            }
            else if(e.PropertyName == nameof(Element.Text))
            {
                Control.Text = Element.Text;
            }
        }

        /*public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            var sizeConstraint = base.GetDesiredSize(widthConstraint, heightConstraint);

            if (sizeConstraint.Request.Width == 0)
            {
                var width = widthConstraint;
                if (widthConstraint <= 0)
                {
                    System.Diagnostics.Debug.WriteLine("Default values");
                    width = DEFAULT_SIZE;
                }
                else if (widthConstraint <= 0)
                {
                    width = DEFAULT_SIZE;
                }

                sizeConstraint = new SizeRequest(new Size(width, sizeConstraint.Request.Height),
                    new Size(width, sizeConstraint.Minimum.Height));
            }

            return sizeConstraint;
        }*/

        public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
        {
            ((IViewController)Element).SetValueFromRenderer(CustomCheckbox.IsCheckedProperty, isChecked);
            Element.CheckedCommand?.Execute(isChecked);
        }
    }
}