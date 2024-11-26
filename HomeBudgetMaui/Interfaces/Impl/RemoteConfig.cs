using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudget.Standard
{
    public partial class RemoteConfig : IRemoteConfig
    {
        private static RemoteConfig _instance;
        public static RemoteConfig Instance
        {
            get
            {
                _instance ??= new RemoteConfig();
                return _instance;
            }
        }
           
        public partial string GetActiveInappName();

        public partial void Init();

        public partial bool IsFeatureEnabled(string v);

        public partial bool IsPromoActive();
        private RemoteConfig()
        {
        }
    }
}
