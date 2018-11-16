using Plugin.InAppBilling.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgeStandard.Interfaces
{
    public interface IPurchaseService
    {
        Task MakePurchase(string productName);
        Task<InAppBillingProduct> GetProductInfo(string produsctName);
        Task<bool> IsProductAlreadyBought(string productName);
    }
}
