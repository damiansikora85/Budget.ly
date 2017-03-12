using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.MobileServices;

namespace HomeBudget.Code
{
	public class AzureTest
	{

		public static MobileServiceClient MobileService = new MobileServiceClient("https://myawesomehomebudget.azurewebsites.net");

		public void Test()
		{
			//MobileService.
		}


	}
}
