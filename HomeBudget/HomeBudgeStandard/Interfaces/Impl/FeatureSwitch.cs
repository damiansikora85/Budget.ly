using HomeBudget.Code.Interfaces;
using HomeBudget.Standard;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace HomeBudgeStandard.Interfaces.Impl
{
    public class FeatureSwitch : IFeatureSwitch
    {
        private IRemoteConfig _remoteConfig;

        public FeatureSwitch()
        {
            _remoteConfig = DependencyService.Get<IRemoteConfig>();
        }

        public bool DeleteTransactionsEnabled => _remoteConfig != null && _remoteConfig.IsFeatureEnabled("feature_delete_transactions");

    }
}
