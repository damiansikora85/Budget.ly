using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudget.Standard
{
    public interface IRemoteConfig
    {
        void Init();
        string GetActiveInappName();
        bool IsPromoActive();
    }
}
