using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgeStandard.Interfaces
{
    public interface IActiveAware
    {
        bool IsActive { get; set; }
        Task Activate();
        event EventHandler IsActiveChanged;
    }
}
