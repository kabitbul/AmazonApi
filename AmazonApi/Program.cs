

using AmazonAPI;
using System.Net.Mail;
using System.Net;

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


            invDate = UtilityMethods.PDTDateTime(DateTime.UtcNow).AddDays(-10).
                                         AddMinutes(-2).ToString("yyyy-MM-ddTHH:mm:ssZ");
            //createdAfter =  DateTime.UtcNow.AddHours(-24).AddMinutes(-6).ToString("yyyy-MM-ddTHH:mm:ssZ");
            //createdBefore = DateTime.UtcNow.AddMinutes(-15).ToString("yyyy-MM-ddTHH:mm:ssZ");
            //Console.WriteLine("======Start Orders Update ======");
            //UtilityMethods.WriteToTextLog("======Start Orders Update======", "INFO");
            //foreach (int stid in SD.storesId)
            //{
            //    SD.accessToken = "";
            //    SD.accessToken = AAmzGetAccessTokenClass.getAccessToken(stid);
            //    if (SD.accessToken == null || SD.accessToken == "")
            //    {
            //        Console.WriteLine("token returned null for store " + DataByStoreClass.getStoreName(stid));
            //        UtilityMethods.WriteToTextLog("token returned null for store "+ DataByStoreClass.getStoreName(stid), "ERR");
            //        UtilityMethods.SendErrMail("token returned null for store "+ DataByStoreClass.getStoreName(stid));
            //    }
            //    else
            //    {
            //        foreach (string mp in SD.MarketplaceList)
            //        {
            //            //orders:
            //            Console.WriteLine("----Start Orders For Store "+DataByStoreClass.getStoreName(stid)+
            //              " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---");
            //            UtilityMethods.WriteToTextLog("----Start Orders For Store "+DataByStoreClass.getStoreName(stid)+
            //                    " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---", "INFO");

            //            AAmzGetOrdersClass.GetOrders(mp, createdAfter, createdBefore, stid);
            //            Console.WriteLine("======End Orders Update for Store "+DataByStoreClass.getStoreName(stid)+
            //            " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"======");
            //            UtilityMethods.WriteToTextLog("======End Orders Update for Store "+DataByStoreClass.getStoreName(stid)+
            //              " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"======", "INFO");
            //            //temp+++++ 
            //            //  GetOrdersClass.GetOrders(mp,createdAfter,createdBefore); 
            //        }
            //    }
            //}

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
                Console.WriteLine("======Start Orders Update ======");
                UtilityMethods.WriteToTextLog("======Start Orders Update======", "INFO");
                 foreach (int stid in SD.storesId)
                 {
                  SD.accessToken = "";
                  SD.accessToken = AAmzGetAccessTokenClass.getAccessToken(stid);
                  if(SD.accessToken == null || SD.accessToken == "")
                   {
                     Console.WriteLine("token returned null for store " + DataByStoreClass.getStoreName(stid));
                     UtilityMethods.WriteToTextLog("token returned null for store "+ DataByStoreClass.getStoreName(stid), "ERR");
                     UtilityMethods.SendErrMail("token returned null for store "+ DataByStoreClass.getStoreName(stid));
                 }
                 else{ 
                    foreach (string mp in SD.MarketplaceList)
                    {
                      //orders:
                      Console.WriteLine("----Start Orders For Store "+DataByStoreClass.getStoreName(stid)+
                        " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---");
                UtilityMethods.WriteToTextLog("----Start Orders For Store "+DataByStoreClass.getStoreName(stid)+
                        " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---", "INFO");
                     
                         AAmzGetOrdersClass.GetOrders(mp,createdAfter,createdBefore,stid);
                  Console.WriteLine("======End Orders Update for Store "+DataByStoreClass.getStoreName(stid)+
                  " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"======");
                UtilityMethods.WriteToTextLog("======End Orders Update for Store "+DataByStoreClass.getStoreName(stid)+
                  " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"======", "INFO");
                  //temp+++++ 
                //  GetOrdersClass.GetOrders(mp,createdAfter,createdBefore); 
                   }
                  } 
               }
              SD.accessToken = "";
             }
              if (hour == 20 && min == 30)
               { 
                Console.WriteLine("======Start Inventory======");
                UtilityMethods.WriteToTextLog("======Start Inventory======", "INFO");
                 foreach (int stid in SD.storesId)
                 {
                  SD.accessToken = "";
                  SD.accessToken = AAmzGetAccessTokenClass.getAccessToken(stid);
                  if(SD.accessToken == null || SD.accessToken == "")
                   {
                     Console.WriteLine("token returned null for store " + DataByStoreClass.getStoreName(stid));
                     UtilityMethods.WriteToTextLog("token returned null for store "+ DataByStoreClass.getStoreName(stid), "ERR");
                     UtilityMethods.SendErrMail("token returned null for store "+ DataByStoreClass.getStoreName(stid));
                   }
                 else{ 
                    foreach (string mp in SD.MarketplaceList)
                    {
                      //orders:
                      Console.WriteLine("----Start Inventory For Store "+DataByStoreClass.getStoreName(stid)+
                        " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---");
                UtilityMethods.WriteToTextLog("----Start Inventory For Store "+DataByStoreClass.getStoreName(stid)+
                        " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---", "INFO");
                         //----------------------------------------------------------------
                         AAmzGetInventoryClass.GetInventory(SD.accessToken,mp,invDate,stid,false);
                         Thread.Sleep(20000);
                         Console.WriteLine("----Start AWD Inventory For Store "+DataByStoreClass.getStoreName(stid)+
                        " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---");
                UtilityMethods.WriteToTextLog("----Start AWD Inventory For Store "+DataByStoreClass.getStoreName(stid)+
                        " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---", "INFO");
                           AAmzGetInventoryClass.GetInventory(SD.accessToken,mp,invDate,stid,true);
                           Thread.Sleep(20000);
                         AAmzGetInventoryClass.GetAWDInventory(SD.accessToken,stid);
                       Thread.Sleep(20000);
                         AAmzGetInventoryClass.DeleteTempSkuAsin(stid);
                         //---------------------------------------------------------------
                  Console.WriteLine("======End Inventory Update for Store "+DataByStoreClass.getStoreName(stid)+
                  " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"======");
                UtilityMethods.WriteToTextLog("======End Inventory Update for Store "+DataByStoreClass.getStoreName(stid)+
                  " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"======", "INFO");
                   //TEMP+++++++++++++++ 
                  // GetInventoryClass.GetInventory(SD.accessToken,mp,invDate);
                    }
                  }               
               }
               SD.accessToken = "";
                foreach (int stid in SD.storesId)
                 { 
                    foreach (string mp in SD.MarketplaceList)
                    { 
                      AAmzInventoryCostClass.populateDailyInventoryCostTable(stid,mp);
                    }
               }
             }
              if(hour == 2 && min == 20)
              {
                Console.WriteLine("======Start Orders make sure all orders are added from last 2 days======");
                UtilityMethods.WriteToTextLog("======Start Orders make sure all orders are added from last 2 days======", "INFO");
                 foreach (int stid in SD.storesId)
                 {
                  SD.accessToken = "";
                  SD.accessToken = AAmzGetAccessTokenClass.getAccessToken(stid);
                  if(SD.accessToken == null || SD.accessToken == "")
                   {
                     Console.WriteLine("token returned null for store " + DataByStoreClass.getStoreName(stid));
                     UtilityMethods.WriteToTextLog("token returned null for store "+ DataByStoreClass.getStoreName(stid), "ERR");
                     UtilityMethods.SendErrMail("token returned null for store "+ DataByStoreClass.getStoreName(stid));
                 }
                 else{ 
                    foreach (string mp in SD.MarketplaceList)
                    {
                     createdAfter =  DateTime.UtcNow.AddHours(-48).AddMinutes(-6).ToString("yyyy-MM-ddTHH:mm:ssZ");
                   createdBefore = DateTime.UtcNow.AddMinutes(-15).ToString("yyyy-MM-ddTHH:mm:ssZ");
                      //orders:
                      Console.WriteLine("----Start Orders For Store "+DataByStoreClass.getStoreName(stid)+
                        " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---");
                UtilityMethods.WriteToTextLog("----Start Orders For Store "+DataByStoreClass.getStoreName(stid)+
                        " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"---", "INFO");
                     
                         AAmzGetOrdersClass.GetOrders(mp,createdAfter,createdBefore,stid);
                  Console.WriteLine("======End Orders Update for Store "+DataByStoreClass.getStoreName(stid)+
                  " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"======");
                UtilityMethods.WriteToTextLog("======End Orders Update for Store "+DataByStoreClass.getStoreName(stid)+
                  " marketplace "+DataByStoreClass.getMarketplaceName(mp)+"======", "INFO");
                  //TEMP ++++++
                 // GetOrdersClass.GetOrders(mp,createdAfter,createdBefore);
                    }
                  } 
               }
                SD.accessToken = "";
               }
              if(hour == 16 && min == 10)
              {
                SendRunningStatusEmail();
              }
//--------------------------------------------------------
             }//end while loop

          }
  private static void SendRunningStatusEmail()
        {
            try
            {
                // Configure your SMTP client settings.
                // For example, using Gmail's SMTP server.
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("ktonlinemarketing1@gmail.com", "mqgcejocdvbsmxui"),
                    EnableSsl = true
                };

                // Create the email message.
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("ktonlinemarketing1@gmail.com"),
                    Subject = "🔔✅API Machine is running - KT✅🔔",
                    Body = "OK"
                };

                // Send the email to yourself.
                mailMessage.To.Add("ktonlinemarketing1@gmail.com");

                // Send the email.
                smtpClient.Send(mailMessage);
            }
            catch (Exception emailEx)
            {
                // If the email fails, write to the console (or log appropriately).
                Console.WriteLine("Failed to send exception email: " + emailEx.Message);
            }
        }
public static void populateDailyInventoryCostTable()
{
  
}
     }
}
