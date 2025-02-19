using Android.Content;
using Android.Graphics.Drawables;
using Android.Widget;
using HomeBudget.Components;

//[assembly: ExportRenderer(typeof(GradientView), typeof(HomeBudget.Droid.Renderers.GradientViewRenderer))]
//namespace HomeBudget.Droid.Renderers
//{
//    public class GradientViewRenderer : ViewRenderer<GradientView, Android.Views.View>
//    {
//        private Color _startColor;
//        private Color _endColor;
//        LinearLayout layout;
//        private Context _context;

//        public GradientViewRenderer(Context ctx) : base(ctx)
//        {
//            _context = ctx;
//        }

//        public void CreateGradient()
//        {
//            //Need to convert the colors to Android Color objects
//            var androidColors = new int[] { _startColor.ToAndroid(), _endColor.ToAndroid() };

//            var gradient = new GradientDrawable(GradientDrawable.Orientation.LeftRight, androidColors);

//            layout.SetBackground(gradient);
//        }

//        protected override void OnElementChanged(ElementChangedEventArgs<GradientView> e)
//        {
//            base.OnElementChanged(e);

//            if (Control == null)
//            {
//                layout = new LinearLayout(_context);
//                layout.SetBackgroundColor(Color.White.ToAndroid());

//                _startColor = e.NewElement.StartColor;
//                _endColor = e.NewElement.EndColor;

//                CreateLayout();
//            }

//            if (e.OldElement != null)
//            {
//                // Unsubscribe from event handlers and cleanup any resources
//            }

//            if (e.NewElement != null)
//            {
//                // Configure the control and subscribe to event handlers
//                _startColor = e.NewElement.StartColor;
//                _endColor = e.NewElement.EndColor;

//                CreateLayout();
//            }
//        }

//        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            base.OnElementPropertyChanged(sender, e);

//            if (e.PropertyName == GradientView.StartColorProperty.PropertyName)
//            {
//                this._startColor = Element.StartColor;
//                CreateLayout();
//            }
//            else if (e.PropertyName == GradientView.EndColorProperty.PropertyName)
//            {
//                this._endColor = Element.EndColor;
//                CreateLayout();
//            }
//        }

//        private void CreateLayout()
//        {
//            CreateGradient();
//            SetNativeControl(layout);
//        }
//    }
//}