using System.Threading.Tasks;
using Plugin.InAppBilling;

namespace HomeBudgetStandard.Interfaces
{
    public interface IPurchaseService
    {
        Task<bool> MakePurchase(string productName);
        Task<InAppBillingProduct> GetProductInfo(string produsctName);
        Task<bool> IsProductAlreadyBought(string productName);
        Task ConsumeProduct(string name);
    }
}
