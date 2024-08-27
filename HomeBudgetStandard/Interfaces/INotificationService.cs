using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgetStandard.Interfaces
{
    public interface INotificationService
    {
        void Initialize();
        void ScheduleNotifications(string text, DayOfWeek[] days, TimeSpan dateTime);
        void ClearAllNotifications();
    }
}
