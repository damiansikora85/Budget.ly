using System;
using System.Collections.Generic;
using System.Text;

namespace HomeBudgeStandard.Interfaces
{
    public interface INotificationService
    {
        void Initialize();
        void ScheduleNotification(string text, DayOfWeek[] days, TimeSpan dateTime);
        void ClearAllNotifications();
        DateTime GetNextNotificationDateTime();
    }
}
