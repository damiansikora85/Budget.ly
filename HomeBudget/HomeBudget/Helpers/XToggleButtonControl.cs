using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace HomeBudget
{
    public class XToggleButtonControl : ContentView
    {
        public static readonly BindableProperty CommandProperty =
        BindableProperty.Create<XToggleButtonControl, ICommand>(p => p.Command, null);

        public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create<XToggleButtonControl, object>(p => p.CommandParameter, null);

        public static readonly BindableProperty CheckedProperty =
        BindableProperty.Create<XToggleButtonControl, bool>(p => p.Checked, false);

        public static readonly BindableProperty AnimateProperty =
        BindableProperty.Create<XToggleButtonControl, bool>(p => p.Animate, false);

        public static readonly BindableProperty CheckedImageProperty =
        BindableProperty.Create<XToggleButtonControl, ImageSource>(p => p.CheckedImage, null);

        public static readonly BindableProperty UnCheckedImageProperty =
        BindableProperty.Create<XToggleButtonControl, ImageSource>(p => p.UnCheckedImage, null);

        private ICommand _toggleCommand;
        private Image _toggleImage;

        public XToggleButtonControl()
        {
            Initialize();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public bool Checked
        {
            get { return (bool)GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(CheckedProperty, value); }
        }

        public ImageSource CheckedImage
        {
            get { return (ImageSource)GetValue(CheckedImageProperty); }
            set { SetValue(CheckedImageProperty, value); }
        }

        public ImageSource UnCheckedImage
        {
            get { return (ImageSource)GetValue(UnCheckedImageProperty); }
            set { SetValue(UnCheckedImageProperty, value); }
        }

        public ICommand ToogleCommand
        {
            get
            {
                return _toggleCommand
                ?? (_toggleCommand = new Command(
                async () =>
                {
                    if (_toggleImage.Source == UnCheckedImage)
                    {
                        _toggleImage.Source = CheckedImage;
                        Checked = true;
                    }
                    else
                    {
                        _toggleImage.Source = UnCheckedImage;
                        Checked = false;
                    }

                    if (Animate)
                    {
                        await this.ScaleTo(0.8, 250, Easing.Linear);
                        await this.ScaleTo(1, 250, Easing.Linear);
                    }
                    Command?.Execute(CommandParameter);
                }
                ));
            }
        }

        private void Initialize()
        {
            _toggleImage = new Image();

            Animate = true;
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = ToogleCommand
            });
            Debug.WriteLine("Init with Checked: { Checked}");
            _toggleImage.Source = Checked ? CheckedImage : UnCheckedImage;
            Content = _toggleImage;
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            _toggleImage.Source = Checked ? CheckedImage : UnCheckedImage;
            Content = _toggleImage;
        }
    }
}
