﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudget.Code.Interfaces
{
    public interface ISettings
    {
        string CloudAccessToken { get; set; }
        string CloudRefreshToken { get; set; }
        bool FirstLaunch { get; set; }
        bool NeedSetupDefaultNotifications { get; set; }

    }
}
