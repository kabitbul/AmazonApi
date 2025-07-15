using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.SqlClient;
using System.Data;
using RestSharp.Serializers.NewtonsoftJson;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using FikaAmazonAPI.AmazonSpApiSDK.Models.FbaSmallandLight;
using System.Net.Http.Headers;

namespace AmazonAPI
{
    public class GetListingItemClass
    {
      public static string GetItemBySellerSKU(string token,  string sellerSKU,string marketplaceId)
      {
     try{ 
       var options = new RestClientOptions("https://sellingpartnerapi-na.amazon.com")
           {
             MaxTimeout = -1,
            };
        RestClient client = new RestClient(options);
        string sellerId = "A3SR1OERCH239J";//"A3SR1OERCH239J"; -- "A3HXXGZGCG0T4O"
        var request = new RestRequest("listings/2021-08-01/items/"+sellerId+"/"+sellerSKU, Method.Get);
        request.AddQueryParameter("marketplaceIds",marketplaceId);
        request.AddQueryParameter("includeInactive", "true");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("x-amz-access-token",token);
      //  request.AddQueryParameter("sellerSku",sellerSKU);
        request.AddQueryParameter("nextToken",null);
         ////////////////////////////
         ////////////////////////////

         RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
        JObject jobj =  JObject.Parse(response.Content);
         if(response.StatusCode != System.Net.HttpStatusCode.OK)
          {
            UtilityMethods.WriteToTextLog(
                    "GetItemBySellerSKU returned status " + response.StatusCode + "with sellerSKU "+sellerSKU ,"ERR");
                  UtilityMethods.SendErrMail("GetItemBySellerSKU returned status " + response.StatusCode + "with sellerSKU "+sellerSKU);
                    Console.WriteLine
                    ("GetItemBySellerSKU returned status " + response.StatusCode + "with sellerSKU "+sellerSKU + "-" + UtilityMethods.IsraelDateTime());
                    return null;
          }
        JArray ordersArray = (JArray)jobj["summaries"];
foreach(JObject obj in ordersArray)
       {
             string asin = (string)obj["asin"];
             if (asin != null)
               return asin;
               return null; 
       }         

         return null;
      
}catch( Exception e){
          UtilityMethods.WriteToTextLog(
                    "EXCEPTION in GetItemBySellerSKU with sellerSKU " + sellerSKU,"ERR");
UtilityMethods.SendErrMail("EXCEPTION in GetItemBySellerSKU with sellerSKU " + sellerSKU + " " + e.Message);
                    Console.WriteLine
                    ("EXCEPTION in GetItemBySellerSKU with sellerSKU " +sellerSKU +" " + UtilityMethods.IsraelDateTime());
          UtilityMethods.WriteToTextLog(e.Message,"ERR");
                    return null;
            }
    }
}
}
