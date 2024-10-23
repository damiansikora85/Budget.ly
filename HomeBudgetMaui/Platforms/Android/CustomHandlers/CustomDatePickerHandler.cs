using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using HomeBudgetStandard.Components;
using Microsoft.Maui.Handlers;

namespace HomeBudgetMaui.Platforms.Android.CustomHandlers
{
    public partial class CustomDatePickerHandler : DatePickerHandler
    {
        protected override DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
        {
            //var view = Element;
            //var dialog = new DatePickerDialog(Context, (o, e) =>
            //{
            //    view.Date = e.Date;
            //    if (Element is CustomDatePicker datePicker)
            //    {
            //        datePicker.SelectedDateConfirmed?.Invoke();
            //    }
            //    ((IElementController)view).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
            //}, year, month, day);

            //return dialog;

            var dialog = new DatePickerDialog(Context!, (o, e) =>
            {
                if (VirtualView != null)
                {
                    VirtualView.Date = e.Date;
                    if (VirtualView is CustomDatePicker datePicker)
                    {
                        datePicker.SelectedDateConfirmed?.Invoke();
                    }
                }
            }, year, month, day);

            return dialog;
        }
    }
}
