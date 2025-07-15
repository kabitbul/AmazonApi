

using AmazonAPI;
using System.Net.Mail;
using System.Net;
using System.Runtime.Intrinsics.X86;

namespace AmazonApi
{   
    class ConsoleSubscriber
    {
        static HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
          CancellationToken cancellationToken = CancellationToken.None;
          int min;
          int hour;
          string createdAfter ;
          string createdBefore;
          string invDate;
          Console.WriteLine("Start at " + UtilityMethods.IsraelDateTime());
//string aaa = GetOrdersClass.getLastDate();
//*************KT START***************KT START********KT START*******KT START*********KT START***//
       //  SD.accessToken = GetAccessTokenClass.getAccessToken();
       //   Console.WriteLine("token - first time : " + SD.accessToken);
         createdAfter =  "2025-04-23T00:00:00Z";//DateTime.UtcNow.AddHours(-7).AddMinutes(-6).ToString("yyyy-MM-ddTHH:mm:ssZ");
      createdBefore = DateTime.UtcNow.AddMinutes(-15).ToString("yyyy-MM-ddTHH:mm:ssZ");
      //   GetOrdersClass.GetOrders(SD.USMarketplace,createdAfter,createdBefore);
invDate = UtilityMethods.PDTDateTime(DateTime.UtcNow).AddDays(-10).
                             AddMinutes(-2).ToString("yyyy-MM-ddTHH:mm:ssZ");
 //SD.accessToken = "";
                  //token:
                //  SD.accessToken = GetAccessTokenClass.getAccessToken();
//                  if(SD.accessToken == null || SD.accessToken == "")
//                   {
//                       Console.WriteLine("token returned null");
//                       UtilityMethods.WriteToTextLog("token returned null", "ERR");
// UtilityMethods.SendErrMail("token returned null");
//                   }
//                  else
//                  {
//                      // inventory: "FuelHose_3ft_1/4inch_clear" "CarCoasters_FBA"
               //    GetListingItemClass.GetItemBySellerSKU(SD.accessToken,"FuelHose_3ft_1/4inch_clear",SD.USMarketplace);
                     // GetInventoryClass.GetInventory(SD.accessToken,SD.USMarketplace,invDate);
                    // GetCatalogItemClass.GetCatalogItemBySellerSKU(SD.accessToken,"CarCoasters_FBA",SD.USMarketplace);
                   //  GetInventoryClass.GetAWDInventory(SD.accessToken);
//                  
//                  }
//*************KT END***************KT END********KT END*******KT END*********KT END***//
//************************************************************************************///
//************************************************************************************//
//return;

           while (true)
             {
              await Task.Delay(58000, cancellationToken);
              min = UtilityMethods.IsraelDateTime().TimeOfDay.Minutes;
              hour = UtilityMethods.IsraelDateTime().TimeOfDay.Hours;
            

             createdAfter =  DateTime.UtcNow.AddHours(-3).AddMinutes(-6).ToString("yyyy-MM-ddTHH:mm:ssZ");
             createdBefore = DateTime.UtcNow.AddMinutes(-15).ToString("yyyy-MM-ddTHH:mm:ssZ");
             invDate = UtilityMethods.PDTDateTime(DateTime.UtcNow).AddDays(-10).
                             AddMinutes(-2).ToString("yyyy-MM-ddTHH:mm:ssZ");
            
            
              if(min == 0)
              {
                Console.WriteLine("======Start Orders Update for KT======");
                UtilityMethods.WriteToTextLog("======Start Orders Update for KT======", "INFO");
                SD.accessToken = "";
                //token:
                SD.accessToken = GetAccessTokenClass.getAccessToken();
                if(SD.accessToken == null || SD.accessToken == "")
                {
                  Console.WriteLine("token returned null");
                  UtilityMethods.WriteToTextLog("token returned null", "ERR");
                  UtilityMethods.SendErrMail("token returned null");
                 }
                else
                 { 
                   //orders:
                Console.WriteLine("----Start Orders For US---");
                UtilityMethods.WriteToTextLog("----Start Orders For US---", "INFO");
                   GetOrdersClass.GetOrders(SD.USMarketplace,createdAfter,createdBefore);//US
                Console.WriteLine("----Start Orders For CA---");
                UtilityMethods.WriteToTextLog("----Start Orders For CA---", "INFO");
                   GetOrdersClass.GetOrders(SD.CAMarketplace,createdAfter,createdBefore);//CA
                 }
                Console.WriteLine("======End Orders Update for KT======");
                UtilityMethods.WriteToTextLog("======End Orders Update for KT======", "INFO");
               // Console.WriteLine("======Start Orders Update for KESEM======");
              //  UtilityMethods.WriteToTextLog("======Start Orders Update for KESEM======", "INFO");
                
               }
             
              if (hour == 20 && min == 30)
               { 
                  SD.accessToken = "";
                  //token:
                  SD.accessToken = GetAccessTokenClass.getAccessToken();
                  if(SD.accessToken == null || SD.accessToken == "")
                   {
                       Console.WriteLine("token returned null");
                       UtilityMethods.WriteToTextLog("token returned null", "ERR");
 UtilityMethods.SendErrMail("token returned null");
                   }
                  else
                  {
                      // inventory: 
                      UtilityMethods.WriteToTextLog("starting inventory","INF");
                      Console.WriteLine("starting inventory");
                      GetInventoryClass.GetInventory(SD.accessToken,SD.USMarketplace,invDate);
                      GetInventoryClass.GetInventory(SD.accessToken,SD.CAMarketplace,invDate);
                      UtilityMethods.WriteToTextLog("ending inventory","INF");
                      Console.WriteLine("ending inventory");
                      // AWD inventory: 
                      UtilityMethods.WriteToTextLog("starting AWD inventory","INF");
                      Console.WriteLine("starting AWD inventory");
                      GetInventoryClass.GetAWDInventory(SD.accessToken);
                      UtilityMethods.WriteToTextLog("ending AWD inventory","INF");
                      Console.WriteLine("ending AWD inventory");
                  }
             }
              if(hour == 2 && min == 20)
              {
                Console.WriteLine("======Start Orders make sure all orders are added from last 2 days======");
                UtilityMethods.WriteToTextLog("======Start Orders make sure all orders are added from last 2 days======", "INFO");
                SD.accessToken = "";
                //token:
                SD.accessToken = GetAccessTokenClass.getAccessToken();
                if(SD.accessToken == null || SD.accessToken == "")
                {
                  Console.WriteLine("token returned null");
                  UtilityMethods.WriteToTextLog("token returned null", "ERR");
                  UtilityMethods.SendErrMail("token returned null");
                 }
                else
                 {
                   createdAfter =  DateTime.UtcNow.AddHours(-48).AddMinutes(-6).ToString("yyyy-MM-ddTHH:mm:ssZ");
                   createdBefore = DateTime.UtcNow.AddMinutes(-15).ToString("yyyy-MM-ddTHH:mm:ssZ");
                   //orders:
                Console.WriteLine("----Start Orders For US---");
                UtilityMethods.WriteToTextLog("----Start Orders For US---", "INFO");
                   GetOrdersClass.GetOrders(SD.USMarketplace,createdAfter,createdBefore);//US
                Console.WriteLine("----Start Orders For CA---");
                UtilityMethods.WriteToTextLog("----Start Orders For CA---", "INFO");
                   GetOrdersClass.GetOrders(SD.CAMarketplace,createdAfter,createdBefore);//CA
                 }
                Console.WriteLine("======End Orders Update for KT======");
                UtilityMethods.WriteToTextLog("======End Orders Update for KT======", "INFO");
                Console.WriteLine("======Start Orders Update for KESEM======");
                UtilityMethods.WriteToTextLog("======Start Orders Update for KESEM======", "INFO");
                
               }
//--------------------------------------------------------
             }//end while loop

          }
     }
}
