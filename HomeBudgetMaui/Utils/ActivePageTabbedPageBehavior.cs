﻿using HomeBudgetStandard.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui;

namespace HomeBudget.Utils
{
    public class ActivePageTabbedPageBehavior : Behavior<TabbedPage>
    {
        protected override void OnAttachedTo(TabbedPage tabbedPage)
        {
            base.OnAttachedTo(tabbedPage);
            tabbedPage.CurrentPageChanged += OnTabbedPageCurrentPageChanged;
        }

        protected override void OnDetachingFrom(TabbedPage tabbedPage)
        {
            base.OnDetachingFrom(tabbedPage);
            tabbedPage.CurrentPageChanged -= OnTabbedPageCurrentPageChanged;
        }

        private void OnTabbedPageCurrentPageChanged(object sender, EventArgs e)
        {
            var tabbedPage = (TabbedPage)sender;

            // Deactivate previously selected page
            /*var prevActiveAwarePage = tabbedPage.Children.OfType<IActiveAware>()
                .FirstOrDefault(c => c.IsActive && tabbedPage.CurrentPage != c);
            if (prevActiveAwarePage != null)
            {
                prevActiveAwarePage.IsActive = false;
            }*/

            // Activate selected page
            if (tabbedPage.CurrentPage is IActiveAware activeAwarePage && !activeAwarePage.IsActive)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    activeAwarePage.Activate();
                    activeAwarePage.IsActive = true;
                });
            }
        }
    }
}
