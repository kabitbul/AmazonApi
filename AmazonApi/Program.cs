

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
           while (true)
            {
              await Task.Delay(59000, cancellationToken);
              min = UtilityMethods.IsraelDateTime().TimeOfDay.Minutes;
              hour = UtilityMethods.IsraelDateTime().TimeOfDay.Hours;
                    if (min != 0 || hour != 8)
                        continue;
            Console.WriteLine("Start at " + UtilityMethods.IsraelDateTime());
            

            string createdAfter =  DateTime.UtcNow.AddDays(-1).AddMinutes(-6).ToString("yyyy-MM-ddTHH:mm:ssZ");
            string createdBefore = DateTime.UtcNow.AddHours(-1).AddMinutes(-5).ToString("yyyy-MM-ddTHH:mm:ssZ");
            string invDate = UtilityMethods.PDTDateTime(DateTime.UtcNow).AddDays(-1).
                             AddMinutes(-2).ToString("yyyy-MM-ddTHH:mm:ssZ");
              //token:
             string token = GetAccessTokenClass.getAccessToken();

              //orders:
               GetOrdersClass.GetOrders(token, SD.USMarketplace,createdAfter,createdBefore);//US
              GetOrdersClass.GetOrders(token, SD.CAMarketplace,createdAfter,createdBefore);//CA
             

              // inventory: 
              GetInventoryClass.GetInventory(token,SD.USMarketplace,invDate);
              GetInventoryClass.GetInventory(token,SD.CAMarketplace,invDate);
              Console.WriteLine("End at " + UtilityMethods.IsraelDateTime());
             }//end while loop

          }
     }
}
