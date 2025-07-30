using RestSharp;
using System.Text.Json;

namespace AmazonAPI
{
    public class AAmzGetAccessTokenClass
    {
      public static string getAccessToken(int storeId)
      {
         string strResp = "";
         var options = new RestClientOptions("https://api.amazon.com")
           {
             MaxTimeout = -1,
            };
        var client = new RestClient(options);
        var request = new RestRequest("/auth/o2/token", Method.Post);
       // request.AddHeader("Host", "secure-wms.com");
        //request.AddHeader("Connection", "keep-alive");
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
   
       var body = $@"{{
            ""grant_type"": ""refresh_token"",
             ""refresh_token"": ""{DataByStoreClass.getRefreshToken(storeId)}"",
             ""client_id"": ""{DataByStoreClass.getClientId(storeId)}"",
             ""client_secret"": ""{DataByStoreClass.getClientSecret(storeId)}""
             }}";
   

       //Console.WriteLine(System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(authInfo)));
       //Console.WriteLine(body);
         request.AddStringBody(body, DataFormat.Json);
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
