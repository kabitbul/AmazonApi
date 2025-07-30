using RestSharp;
using System.Text.Json;

namespace AmazonAPI
{
    public class DataByStoreClass
    {
      public static string getRefreshToken(int storeId)
      { 
        if(storeId == SD.KTStoreId)
           return SD.refreshToken;
         return null;

        }
      public static string getClientId(int storeId)
      { 
        if(storeId == SD.KTStoreId)
           return SD.clientID;
         return null;

       }
      public static string getClientSecret(int storeId)
      { 
        if(storeId == SD.KTStoreId)
           return SD.clientSecret;
         return null;

        }
      public static string getStoreName(int storeId)
      { 
        if(storeId == SD.KTStoreId)
           return SD.KTStoreName;
         return null;

        }
public static string getMarketplaceName(string mp)
      { 
        if(mp == SD.USMarketplace)
           return "US";
        else if(mp == SD.CAMarketplace)
            return "CA";
         return null;

        }
    }
}
