using KTTexasAPI;

namespace AmazonApi
{   
    class ConsoleSubscriber
    {
        static HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
          string token = GetAccessTokenClass.getAccessToken();
         }
     }
}
