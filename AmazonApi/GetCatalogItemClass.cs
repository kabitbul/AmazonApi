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
    public class GetCatalogItemClass
    {
      public static string GetCatalogItemByAsin(string token,  string asin,string marketplaceId)
      {
        string endpoint = "https://sellingpartnerapi-na.amazon.com"; // Change for your regio
   
        var client = new RestClient(endpoint);
        var request = new RestRequest($"/catalog/2022-04-01/items/{asin}?marketplaceIds={marketplaceId}", Method.Get);
        request.AddHeader("Authorization", $"Bearer {token}");
        request.AddHeader("x-amz-access-token", token);
        request.AddHeader("Accept", "application/json");

        RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
/////////
        return null;//OUTPUT EXAMPLE
  //       {"asin":"B07XJYHMFD",
  //"summaries":[{"marketplaceId":"ATVPDKIKX0DER",
  //              "adultProduct":false,
		//		"autographed":false,
		//		"brand":"KT Deals",
		//		"browseClassification":{"displayName":"Fuel Lines",
		//		                         "classificationId":"155366011"},
		//		"color":"Yellow 4PC",
		//		"itemClassification":"BASE_PRODUCT",
		//		"itemName":"4 Sizes Petrol Fuel Gas Line Pipe......",
		//		"manufacturer":"KT Deals",
		//		"memorabilia":false,
		//		"packageQuantity":1,
		//		"tradeInEligible":false,
		//		"websiteDisplayGroup":"home_improvement_display_on_website",
		//		"websiteDisplayGroupName":"Home Improvement"}]}
     }public static string GetCatalogItemBySellerSKU(string token,  string sellerSKU,string marketplaceId)
      {
       var options = new RestClientOptions("https://sellingpartnerapi-na.amazon.com")
           {
             MaxTimeout = -1,
            };
        RestClient client = new RestClient(options);
        var request = new RestRequest("catalog/2022-04-01/items/"+sellerSKU, Method.Get);
        request.AddQueryParameter("marketplaceIds",marketplaceId);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("x-amz-access-token",token);
      //  request.AddQueryParameter("sellerSku",sellerSKU);
        request.AddQueryParameter("nextToken",null);
         ////////////////////////////
         ////////////////////////////

         RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
        //if(response.StatusCode != System.Net.HttpStatusCode.OK)
         return null;
       }
    }
}
