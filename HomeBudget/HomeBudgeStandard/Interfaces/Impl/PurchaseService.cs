using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeBudgeStandard.Interfaces.Impl
{
    public class PurchaseService : IPurchaseService
    {
        public async Task<InAppBillingProduct> GetProductInfo(string productName)
        {
            try
            {
                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                {
                    //Couldn't connect to billing, could be offline, alert user
                    return null;
                }

                var products = await CrossInAppBilling.Current.GetProductInfoAsync(ItemType.InAppPurchase, new string[] { productName });
                if (products == null) return null;
                var productsList = products.ToList();
                if (productsList.Count == 0) return null;
                return productsList[0];
            }
            catch (Exception ex)
            {
                return null;
                //Something bad has occurred, alert user
            }
            finally
            {
                //Disconnect, it is okay if we never connected
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

        public async Task<bool> IsProductAlreadyBought(string productName)
        {
            try
            {
                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                {
                    //Couldn't connect to billing, could be offline, alert user
                    return false;
                }

                var purchases = await CrossInAppBilling.Current.GetPurchasesAsync(ItemType.InAppPurchase);
                if (purchases == null) return false;
                var purchasesList = purchases.ToList();
                return purchasesList.Count > 0;
            }
            catch (Exception ex)
            {
                //Something bad has occurred, alert user
                return false;
            }
            finally
            {
                //Disconnect, it is okay if we never connected
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }

        public async Task MakePurchase(string productName)
        {
            try
            {
                var connected = await CrossInAppBilling.Current.ConnectAsync();

                if (!connected)
                {
                    //Couldn't connect to billing, could be offline, alert user
                    return;
                }

                //try to purchase item
                var purchase = await CrossInAppBilling.Current.PurchaseAsync(productName, ItemType.InAppPurchase, "apppayload");
                if (purchase == null)
                {
                    //Not purchased, alert the user
                }
                else
                {
                    //Purchased, save this information
                    var id = purchase.Id;
                    var token = purchase.PurchaseToken;
                    var state = purchase.State;
                }
            }
            catch (Exception ex)
            {
                //Something bad has occurred, alert user
            }
            finally
            {
                //Disconnect, it is okay if we never connected
                await CrossInAppBilling.Current.DisconnectAsync();
            }
        }
    }
}
