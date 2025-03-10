using Firebase.Crashlytics;
using HomeBudgetShared.Code.Interfaces;

namespace HomeBudgetStandard.Interfaces.Impl;

public class XamarinCrashReporter : ICrashReporter
{
    public void Report(Exception exc)
    {
        FirebaseCrashlytics.Instance.RecordException(Java.Lang.Throwable.FromException(exc));
    }
}
