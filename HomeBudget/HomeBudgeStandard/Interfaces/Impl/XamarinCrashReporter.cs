using HomeBudgetShared.Code.Interfaces;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgeStandard.Interfaces.Impl
{
    public class XamarinCrashReporter : ICrashReporter
    {
        public void Report(Exception exc)
        {
            Crashes.TrackError(exc);
        }
    }
}
