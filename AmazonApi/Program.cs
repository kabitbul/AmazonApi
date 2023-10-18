using KTTexasAPI;

namespace AmazonApi
{   
    class ConsoleSubscriber
    {
        static HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
          string token = GetAccessTokenClass.getAccessToken();
          //Atza|IwEBIBnYeVKkzB_8fwO4bGV7OyB2q5UV3nOyznPmbgyWantee-wFOEy0-Ji0MOdGnaMjgIqFUBY_ZGQgTriwv9k6Ru9F2JoinkvkaSaFJ9u6FfXriYRYekcwWxKOOn51KpglFfGTQPyGOxr8sBnFUcqxfFGVOvd1NO21J757HcCl47rwRUOpqnK3nixbnsh0rQ_RsY9p2Pl7yxAdrkMfdzNOclbWaH_TNOUutLaRaLNzafPGapXyh2UQjcbYyoUxygCBN4dcjHP3LN0-8HnQwk5OukB1gN6bDCHjAdM-3BNVAnVtXXdBzSVfZcnEHjq9euz8522l0p3e4uc0sfVB4o5LCTIeN07q6u07NQk0b5uNFlUZmg
           GetOrdersClass.GetOrders(token);
          }
     }
}
