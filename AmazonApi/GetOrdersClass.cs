using RestSharp;
using System.Text.Json;

namespace KTTexasAPI
{
    public class GetOrdersClass
    {
      public static string GetOrders(string token)
      {
         string strResp = "";
         var options = new RestClientOptions("https://sellingpartnerapi-na.amazon.com")//"")//https://api.amazon.com")
           {
             MaxTimeout = -1,
            };
        var client = new RestClient(options);
        var request = new RestRequest("/orders/v0/orders", Method.Get);
       // request.AddHeader("Host", "secure-wms.com");
        //request.AddHeader("Connection", "keep-alive");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("x-amz-access-token",token);
        request.AddQueryParameter("MarketplaceIds","A2EUQ1WTGCTBG2");
        request.AddQueryParameter("CreatedAfter","2023-10-16T10:00:00Z");
         //request.AddHeader("Authorization", "Basic " + 
        //System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(authInfo)));
        //request.AddHeader("Accept-Encoding", "gzip,deflate,sdch");//"*");
        //request.AddHeader("Accept-Language", "en-US,en;q=0.8");
        //client_credentials
         var body = $@"{{
            ""MarketplaceIds"": ""A2EUQ1WTGCTBG2""
             }}";
       //Console.WriteLine(System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(authInfo)));
       //Console.WriteLine(body);
       //  request.AddStringBody(body, DataFormat.Json);
         RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
         //Console.WriteLine(response.Content);
         if(response.Content == null)
         {
           return "";
          }
          else
          {
             strResp = response.Content;
             AccessToken accTok = JsonSerializer.Deserialize<AccessToken>(strResp);
             return accTok.access_token;
          }

        }
    }
}
