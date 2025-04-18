﻿using HomeBudgetStandard.Views.ViewModels;
using HomeBudget.Code.Logic;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui;

namespace HomeBudgetStandard.Utils
{
    public class TransactionViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EmptyCellDataTemplate { get; set; }
        public DataTemplate TransactionDataTemplate { get; set; }
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is TransactionsGroupViewModel group)
            {
                return group.IsEmpty ? EmptyCellDataTemplate : TransactionDataTemplate;
            }
            else
            {
                return EmptyCellDataTemplate;
            }
        }
    }
}
