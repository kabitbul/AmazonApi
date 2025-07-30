using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.SqlClient;
using System.Data;
using Amazon.Runtime.Internal;
using System.Text.Json;
using System.Text;

namespace AmazonAPI
{
    public class AAmzGetOrdersClass
    {
      public static string GetOrders( string marketPlace,string createdAfter,
                                      string createdBefore, int storeId)
      {
         string storeName = DataByStoreClass.getStoreName(storeId);
         string nextToken = null;
         var options = new RestClientOptions("https://sellingpartnerapi-na.amazon.com")
           {
             MaxTimeout = -1,
            };
        var client = new RestClient(options);
        var request = new RestRequest("/orders/v0/orders", Method.Get);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("x-amz-access-token",SD.accessToken);
        request.AddQueryParameter("MarketplaceIds",marketPlace);
        request.AddQueryParameter("CreatedAfter",createdAfter);
        request.AddQueryParameter("CreatedBefore",createdBefore);
        request.AddQueryParameter("FulfillmentChannels","AFN");
        request. AddQueryParameter("NextToken",null); 
         RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
        if(response.StatusCode != System.Net.HttpStatusCode.OK)
           {
                UtilityMethods.WriteToTextLog(storeName +
                " GetOrders returned status " + response.StatusCode +"-","ERR");
            UtilityMethods.SendErrMail(storeName + " GetOrders returned status " + response.StatusCode);
                Console.WriteLine
                (storeName+" GetOrders e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                return null;
               }
        nextToken = (string)(JObject.Parse(response.Content))["payload"]["NextToken"];
         if(response.Content != null)
          {
              if (nextToken == null) // there are less then 100 and no paging
               {
                 bool result  = loopOrders(JObject.Parse(response.Content),storeId);
                }
               else // there is next token, more then 1 page
               {
                 bool result;
                 int countToNextToken=0;
                  while(nextToken != null)
                  {
                    countToNextToken++;
                    Console.WriteLine(storeName + " countToNextToken is " + countToNextToken);
                    if (countToNextToken == 12)
                     {
                       countToNextToken = 0;
                      }
                   CancellationToken cancellationToken = CancellationToken.None;
                     result  = loopOrders(JObject.Parse(response.Content), storeId);
                    request.AddOrUpdateParameter("NextToken", nextToken, ParameterType.QueryString);
                    response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
                    if(response.StatusCode != System.Net.HttpStatusCode.OK)
                     {
                      UtilityMethods.WriteToTextLog(storeName +
                      " GetOrders2 returned status " + response.StatusCode , "ERR");
                UtilityMethods.SendErrMail(storeName+" GetOrders2 returned status " + response.StatusCode);      
                Console.WriteLine
                      (storeName+" GetOrders2 e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                      Console.WriteLine(storeName + " " +response.Content);
                      UtilityMethods.WriteToTextLog(storeName+ " " +response.Content,"ERR");
                      ///////TEMP///////TEMP//////////TEMP/////////////////////////////////////////////
                      SD.accessToken = AAmzGetAccessTokenClass.getAccessToken(storeId);
                       Console.WriteLine(storeName +" token - first time : " + SD.accessToken);
                       UtilityMethods.WriteToTextLog(storeName + " token - first time : " + SD.accessToken,"ERR");
                      // createdAfter =  getLastDate();
       //  createdBefore = "2024-11-01T00:00:00Z";//DateTime.UtcNow.AddMinutes(-15).ToString("yyyy-MM-ddTHH:mm:ssZ");
         
                 //if(response.StatusCode.ToString() == "Forbidden" && 
                 //   DateTime.Parse(createdAfter)  < DateTime.Parse(createdBefore))
                 //     AAmzGetOrdersClass.GetOrders(SD.USMarketplace,createdAfter,createdBefore,storeId);
                      return null;
                     }
                     nextToken = (string)(JObject.Parse(response.Content))["payload"]["NextToken"];
                   }
                   //at the end of the loop nextToken is null so we are at the last page
                    result  = loopOrders(JObject.Parse(response.Content), storeId);

                }
            }
           return "";
}
//------------------------------------
//public static string getLastDate()
//        {
           
//            SqlConnection con = new SqlConnection(SD.connectionStr);
//            string sql = "select max(purchaseDate) from AAmzOrders a where MarketPlace = 'US' and PurchaseDate < '2024-11-01 00:00:00.0000000'";
//              try
//            {
//                con.Open();
                
//                SqlCommand cmd = new SqlCommand(sql, con);
              
//                 SqlDataReader reader = cmd.ExecuteReader();
                
//              while (reader.Read())
//                {
                 
//                 string res =  reader.GetDateTime(0).Date.ToString("yyyy-MM-ddTHH:mm:ssZ");
//                 con.Close();
//                 return res;
//                }
//              con.Close();
//              return null;
                
//            }
//            catch (Exception ex)
//            {
              
//                return null;
//            }
//        }
    private static readonly HttpClient _httpClient = new HttpClient();
    private const string TokenUrl = "https://api.amazon.com/auth/o2/token";

    //public static string RefreshAccessToken(string refreshToken)
    //{
    //    try
    //    {
    //        var requestData = new Dictionary<string, string>
    //        {
    //            { "grant_type", "refresh_token" },
    //            { "refresh_token", SD.refreshToken },
    //            { "client_id", SD.clientID },
    //            { "client_secret", SD.clientSecret }
    //        };

    //        var requestContent = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

    //        HttpResponseMessage response = _httpClient.PostAsync(TokenUrl, requestContent).GetAwaiter().GetResult();
    //        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            Console.WriteLine($"❌ Token refresh failed! HTTP {response.StatusCode}: {responseBody}");
    //            return null;
    //        }

    //        using var jsonDoc = JsonDocument.Parse(responseBody);
    //        string newAccessToken = jsonDoc.RootElement.GetProperty("access_token").GetString();

    //        Console.WriteLine("✅ Access token refreshed successfully!");
    //        return newAccessToken;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"❌ Exception in RefreshAccessToken: {ex.Message}");
    //        return null;
    //    }
    //}

//-------------------------------------

     public static bool loopOrders(JObject jobj, int storeId)
      {
      try{ 
           int i =0;
           string storeName = DataByStoreClass.getStoreName(storeId);
           JArray ordersArray = (JArray)jobj["payload"]["Orders"];
           foreach(JObject obj in ordersArray)
           {
             string amazonOrderId = (string)obj["AmazonOrderId"];
              bool existOrd = ExistOrder(amazonOrderId,storeId);
             if(existOrd)
               {
               Console.WriteLine(storeName + " "+amazonOrderId + "exsits in KT Date " + obj["PurchaseDate"]);
               UtilityMethods.WriteToTextLog(storeName + " "+amazonOrderId + "exsits in KT", "INF");
               }
             else{ 
               //Console.WriteLine(++i + "-"+amazonOrderId);
               DateTime purchaseDate = (DateTime)obj["PurchaseDate"];
               string marketPlace = "";
               if ((string)obj["MarketplaceId"] == "A2EUQ1WTGCTBG2")
                marketPlace = "CA";
                    else
               marketPlace = "US";
             bool isFBA =false;
               if ((string)obj["FulfillmentChannel"] == "AFN") //FBA only
                  {
                    GetOrderItem(amazonOrderId,marketPlace,purchaseDate,storeId);
                    }
            }
              }
             return true;
           }
          catch (Exception e){

          UtilityMethods.WriteToTextLog(DataByStoreClass.getStoreName(storeId) + " "+
                    "EXCEPTION in loopOrders-" ,"ERR");
                    Console.WriteLine
                    (DataByStoreClass.getStoreName(storeId) + " EXCEPTION in loopOrders-" + UtilityMethods.IsraelDateTime());
                 UtilityMethods.SendErrMail(DataByStoreClass.getStoreName(storeId) + " EXCEPTION in loopOrders " + e.Message);
                    return false;
            }
      }
public static bool ExistOrder(string amazonOrderId,int storeId)
{
   SqlConnection con = new SqlConnection(SD.connectionStr);
            string sql = "SELECT 1 FROM [dbo].AAmzOrders " +
"                         WHERE AmazonOrdId = @AMZORD AND StoreId = @STRID";
            try
            {
                con.Open();
                
                SqlCommand cmd = new SqlCommand(sql, con);
              cmd.Parameters.Add("@AMZORD", SqlDbType.VarChar, 20).Value = amazonOrderId;
              cmd.Parameters.Add("@STRID", SqlDbType.Int).Value = storeId;
                 SqlDataReader reader = cmd.ExecuteReader();
                
              while (reader.Read())
                {
                 con.Close();
                 return true;
                }
                con.Close();
                
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(DataByStoreClass.getStoreName(storeId) + " Exception in ExistInAsinToSku: "+ex.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
                return false;
            }
}
public static  string GetOrderItem( string orderId, string marketPlace,DateTime purchaseDate, int storeId)
      {
        string storeName = DataByStoreClass.getStoreName(storeId);
       try{ 
         CancellationToken cancellationToken = CancellationToken.None;
          Task.Delay(2000, cancellationToken).GetAwaiter().GetResult();
         var options = new RestClientOptions("https://sellingpartnerapi-na.amazon.com")
           {
             MaxTimeout = -1,
            };
        var client = new RestClient(options);
        var request = new RestRequest("/orders/v0/orders/"+orderId+"/orderItems", Method.Get);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("x-amz-access-token",SD.accessToken);
         RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
         if(response.StatusCode != System.Net.HttpStatusCode.OK)
                 {
                    UtilityMethods.WriteToTextLog(storeName + 
                    " GetOrderItem returned status " + response.StatusCode +"-","ERR");
      UtilityMethods.SendErrMail(storeName + " GetOrderItem returned status " + response.StatusCode);             
      Console.WriteLine
                    (storeName+" GetOrderItem e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                    Console.WriteLine(response.Content);
                    return null;
               }
          
         if(response.Content == null)
         {
           return "";
          }
          else
          {
            JObject jobj = JObject.Parse(response.Content);
            JArray ordersArray = (JArray)jobj["payload"]["OrderItems"];
             int qty =(int)ordersArray[0]["QuantityOrdered"];
            string asin = (string)ordersArray[0]["ASIN"];
            if(qty > 0)
               AddOrderToKT(orderId,marketPlace,purchaseDate,qty,asin,storeId); 
            return "";
          }
        }
        catch{
          UtilityMethods.WriteToTextLog(
                    storeName +" EXCEPTION in GetOrderItem-orderId " + orderId,"ERR" );
           UtilityMethods.SendErrMail(storeName+" EXCEPTION in GetOrderItem-orderId " + orderId);         
           Console.WriteLine
                    (storeName+" EXCEPTION in GetOrderItem-orderId " + orderId +" "+ UtilityMethods.IsraelDateTime());
                    return "";
            }
        }
public static void AddOrderToKT(string orderId, string marketPlace,DateTime purchaseDate,int qty,
                                string asin,int storeId)
{
   SqlConnection con = new SqlConnection(SD.connectionStr);
   string storeName = DataByStoreClass.getStoreName(storeId);
   try{          
       string sql ="";
         
  sql = "INSERT INTO [dbo].AAmzOrders " +
"                  (AmazonOrdId,MarketPlace,PurchaseDate,Qty,Asin,StoreId" +
"                  ) VALUES " +
"                  (" +
"                    @ORDID,@MP,@PDATE,@QTY,@ASIN,@STRID)";
         con.Open();
        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@ORDID", SqlDbType.VarChar, 100).Value = orderId;
        cmd.Parameters.Add("@MP", SqlDbType.VarChar, 5).Value = marketPlace;
        purchaseDate = UtilityMethods.PDTDateTime(purchaseDate);
        cmd.Parameters.Add("@PDATE", SqlDbType.DateTime, 100).Value = purchaseDate;
        cmd.Parameters.Add("@QTY", SqlDbType.Int, 100).Value = qty;
        cmd.Parameters.Add("@ASIN", SqlDbType.VarChar, 100).Value = asin;
        cmd.Parameters.Add("@STRID", SqlDbType.Int,100).Value = storeId;
        int effectedRows = cmd.ExecuteNonQuery();
        con.Close();
       // Console.WriteLine("Update of sku " + sku + " with quantity " + qty);
        //updateInventory(sku,qty,con);//UPDATE INVENTORY
       if(effectedRows != 1)
          {
             Console.WriteLine(storeName+" ERROR- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.WriteToTextLog(storeName+" ERROR- effected rows are " + effectedRows + " while adding order with asin " + asin,"ERR");
 UtilityMethods.SendErrMail(storeName+" ERROR- effected rows are " + effectedRows + " while adding order with asin " + asin);
          }
      else
       {
        Console.WriteLine(storeName+" Added Order "+orderId+" From MarketPlace " + marketPlace+" purchase date " +purchaseDate);
        UtilityMethods.WriteToTextLog(storeName+ "Added Order "+orderId+" From MarketPlace " + marketPlace+" purchase date " +purchaseDate,"INF");
       }
      }
   catch(Exception e)
            {
   UtilityMethods.WriteToTextLog(storeName+ " Exception on AddOrderToKT with asin " + asin+ "-" ,"ERR");
                Console.WriteLine(storeName+" Exception on AddOrderToKT with asin "  + asin+ "-" + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();
   UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
UtilityMethods.SendErrMail(storeName+" Exception on AddOrderToKT with asin " + asin );
}
}
}
}
