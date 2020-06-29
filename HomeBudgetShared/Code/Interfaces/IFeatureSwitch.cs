using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudget.Code.Interfaces
{
    public interface IFeatureSwitch
    {
        bool DeleteTransactionsEnabled { get; }
    }
}
