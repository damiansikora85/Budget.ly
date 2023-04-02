using System.Threading.Tasks;
using Plugin.InAppBilling;

namespace HomeBudgeStandard.Interfaces
{
    public interface IPurchaseService
    {
        Task<bool> MakePurchase(string productName);
        Task<InAppBillingProduct> GetProductInfo(string produsctName);
        Task<bool> IsProductAlreadyBought(string productName);
        Task ConsumeProduct(string name);
    }
}
