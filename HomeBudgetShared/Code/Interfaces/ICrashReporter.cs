using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetShared.Code.Interfaces
{
    public interface ICrashReporter
    {
        void Report(Exception exc);
    }
}
