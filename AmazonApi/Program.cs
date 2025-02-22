

using AmazonAPI;

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
          string token = "";

           while (true)
             {
              await Task.Delay(58000, cancellationToken);
              min = UtilityMethods.IsraelDateTime().TimeOfDay.Minutes;
              hour = UtilityMethods.IsraelDateTime().TimeOfDay.Hours;
            //Console.WriteLine("Start at " + UtilityMethods.IsraelDateTime());
            

            string createdAfter =  DateTime.UtcNow.AddHours(-2).AddMinutes(-6).ToString("yyyy-MM-ddTHH:mm:ssZ");
            string createdBefore = DateTime.UtcNow.AddMinutes(-15).ToString("yyyy-MM-ddTHH:mm:ssZ");
            string invDate = UtilityMethods.PDTDateTime(DateTime.UtcNow).AddDays(-1).
                             AddMinutes(-2).ToString("yyyy-MM-ddTHH:mm:ssZ");
            
            
              if(min == 0)
              {
                token = "";
                //token:
                token = GetAccessTokenClass.getAccessToken();
                if(token == null || token == "")
                {
                  Console.WriteLine("token returned null");
                  UtilityMethods.WriteToTextLog("token returned null", "ERR");
                 }
                else
                 { 
                   //orders:
                   GetOrdersClass.GetOrders(token, SD.USMarketplace,createdAfter,createdBefore);//US
                   GetOrdersClass.GetOrders(token, SD.CAMarketplace,createdAfter,createdBefore);//CA
                 }
               }
             
              if (hour == 20 && min == 30)
               { 
                  token = "";
                  //token:
                  token = GetAccessTokenClass.getAccessToken();
                  if(token == null || token == "")
                   {
                       Console.WriteLine("token returned null");
                       UtilityMethods.WriteToTextLog("token returned null", "ERR");
                   }
                  else
                  {
                      // inventory: 
                      UtilityMethods.WriteToTextLog("starting inventory","INF");
                      Console.WriteLine("starting inventory");
                      GetInventoryClass.GetInventory(token,SD.USMarketplace,invDate);
                      GetInventoryClass.GetInventory(token,SD.CAMarketplace,invDate);
                      UtilityMethods.WriteToTextLog("ending inventory","INF");
                      Console.WriteLine("ending inventory");
                  }
               }
             }//end while loop

          }
     }
}
