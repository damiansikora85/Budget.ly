using Android.Content;
using HomeBudget.Components;
using System;

//[assembly: Xamarin.Forms.ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]
//namespace HomeBudget.Droid.Renderers
//{
//    public class CustomListViewRenderer : ListViewRenderer
//    {
//        private CustomListView _myListView;
//        private int _firstElementHeight = -1;

//        public CustomListViewRenderer(Context context) : base(context)
//        {
//        }

//        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
//        {
//            base.OnElementChanged(e);

//            if (e.OldElement == _myListView)
//            {
//                _myListView = null;
//            }
//            if (e.NewElement is CustomListView customListView)
//            {
//                _myListView = customListView;
//                _myListView.OnScrollToElement += ScrollToElementAt;
//                _myListView.ScrollToTop = ScrollToTop;
//                Control.ScrollChange += Control_ScrollChange;
//            }
//        }

//        private void ScrollToTop()
//        {
//            Control.SetSelection(0);
//        }

//        private void Control_ScrollChange(object sender, ScrollChangeEventArgs e)
//        {
//            float visiblePercentage = 0;
//            var firstElement = Control.GetChildAt(0);

//            if (_firstElementHeight <= 0 && firstElement != null && firstElement.Height > 0)
//            {
//                _firstElementHeight = firstElement.Height;
//            }
//            var topY = _firstElementHeight;
//            if (Control.FirstVisiblePosition == 0)
//            {
//                if (firstElement != null)
//                {
//                    topY = Math.Abs(firstElement.Top);
//                    visiblePercentage = topY/ (float)_firstElementHeight;
//                }
//            }
//            else
//            {
//                topY = _firstElementHeight;
//                visiblePercentage = 1;
//            }
//            _myListView.FirstElementVisibiltyPerc = visiblePercentage;
//            _myListView.ScrollPosition = topY;
//            _myListView.WasScrolled();
//        }

//        public void ScrollToElementAt(int elementAt)
//        {
//            Control.SetSelection(elementAt);
//        }
//    }
//}