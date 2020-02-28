using Android.Content;
using HomeBudget.Components;
using HomeBudget.Droid.Renderers;
using System;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(CustomListView), typeof(CustomListViewRenderer))]
namespace HomeBudget.Droid.Renderers
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        private CustomListView _myListView;
        private int _firstElementHeight = -1;

        public CustomListViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == _myListView)
            {
                _myListView = null;
            }
            if (e.NewElement is CustomListView)
            {
                _myListView = Element as CustomListView;
                _myListView.OnScrollToElement += ScrollToElementAt;
                Control.ScrollChange += Control_ScrollChange;
            }
        }

        private void Control_ScrollChange(object sender, ScrollChangeEventArgs e)
        {
            float visiblePercentage = 1;
            var firstElement = Control.GetChildAt(0);
            if (_firstElementHeight <= 0 && firstElement != null)
            {
                _firstElementHeight = firstElement.Height;
            }
            var topY = _firstElementHeight;
            if (Control.FirstVisiblePosition == 1 || Control.FirstVisiblePosition == 0)
            {
                if (firstElement != null)
                {
                    topY = Math.Abs(firstElement.Top);
                    visiblePercentage = topY/ (float)_firstElementHeight;
                }
            }
            else
            {
                topY = _firstElementHeight;
            }
            var s = Control.ScrollY;
            _myListView.FirstElementVisibiltyPerc = visiblePercentage;
            _myListView.ScrollPosition = topY;
            _myListView.WasScrolled();
        }

        public void ScrollToElementAt(int elementAt)
        {
            Control.SetSelection(elementAt);
        }
    }
}