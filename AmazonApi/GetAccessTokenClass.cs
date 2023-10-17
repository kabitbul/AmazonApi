using RestSharp;
using System.Text.Json;

namespace KTTexasAPI
{
    public class GetAccessTokenClass
    {
      public static string getAccessToken()
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
        //request.AddHeader("Authorization", "Basic " + 
        //System.Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(authInfo)));
        //request.AddHeader("Accept-Encoding", "gzip,deflate,sdch");//"*");
        //request.AddHeader("Accept-Language", "en-US,en;q=0.8");
       
         var body = $@"{{
            ""grant_type"": ""client_credentials"",
             ""client_id"": ""{SD.clientID}"",
             ""client_secret"": ""{SD.clientSecret}"",
             ""scope"": ""adx_reporting::appstore:marketer""
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
