using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.SqlClient;
using System.Data;
using RestSharp.Serializers.NewtonsoftJson;

namespace AmazonAPI
{
    public class GetInventoryClass
    {
      public static string GetInventory(string token, string marketPlace, string invDate)
      {

         string nextToken = null;
         var options = new RestClientOptions("https://sellingpartnerapi-na.amazon.com")
           {
             MaxTimeout = -1,
            };
        RestClient client = new RestClient(options);
        var request = new RestRequest("/fba/inventory/v1/summaries", Method.Get);
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("x-amz-access-token",token);
        request.AddQueryParameter("details",true);
        request.AddQueryParameter("granularityType","Marketplace");
        request.AddQueryParameter("granularityId",marketPlace);
        request.AddQueryParameter("marketplaceIds",marketPlace);
        //request.AddQueryParameter("startDateTime",invDate);
        request.AddQueryParameter("nextToken",null); 
        //request.AddQueryParameter("MaxResultsPerPage",100);
         ////////////////////////////
         ////////////////////////////

         RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
        if(response.StatusCode != System.Net.HttpStatusCode.OK)
           {
                UtilityMethods.WriteToLog(
                "GetInventory returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                Console.WriteLine
                ("GetInventory e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                return null;
               }
         if((JObject.Parse(response.Content))["pagination"] != null)  
            nextToken = (string)(JObject.Parse(response.Content))["pagination"]["nextToken"];
        
         if(marketPlace == SD.USMarketplace)
            marketPlace = "US";
         else
           marketPlace = "CA";
        if(response.Content != null)
          {
           deleteInventoryTable(marketPlace);
            if (nextToken == null) // there are less then 100 and no paging
             {
              bool result  = loopInventory(JObject.Parse(response.Content),marketPlace);  
             }
             else
             {
               bool result;
               while(nextToken != null)
               {
                  result  = loopInventory(JObject.Parse(response.Content),marketPlace); 
                  request.AddOrUpdateParameter("nextToken", nextToken, ParameterType.QueryString);
                  response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
                  if(response.StatusCode != System.Net.HttpStatusCode.OK)
                   {
                    UtilityMethods.WriteToLog(
                    "GetInventory2 returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                    Console.WriteLine
                    ("GetInventory2 e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                    return null;
                   }
                if((JObject.Parse(response.Content))["pagination"] != null)  
                   nextToken = (string)(JObject.Parse(response.Content))["pagination"]["nextToken"];
                else
                   nextToken = null;
               }
              //at the end of the loop nextToken is null so we are at the last page
              result  = loopInventory(JObject.Parse(response.Content),marketPlace); 
             }
          }
       return "";
}
     public static bool loopInventory(JObject jobj,string marketPlace)
      {
      try{ 
           int i =0;
           JArray ordersArray = (JArray)jobj["payload"]["inventorySummaries"];
           foreach(JObject obj in ordersArray)
           {
             if(((int)obj["totalQuantity"]) == 0)
               continue;
             string asin = (string)obj["asin"];
             int availableQuantity = (int)obj["inventoryDetails"]["fulfillableQuantity"];
             int inboundShippedQuantity = (int)obj["inventoryDetails"]["inboundShippedQuantity"]; 
             int inboundReceivingQuantity = (int)obj["inventoryDetails"]["inboundReceivingQuantity"]; 
             int reservedQuantity = (int)obj["inventoryDetails"]["reservedQuantity"]["totalReservedQuantity"];
             bool existInAsinToSku = ExistInAsinToSku(asin);
             if(existInAsinToSku)
                 AddInventoryToKT(asin,availableQuantity,inboundShippedQuantity,inboundReceivingQuantity,
                              reservedQuantity, marketPlace);
              
            }
             return true;
           }
          catch{
          UtilityMethods.WriteToLog(
                    "EXCEPTION in loopInventory-" + UtilityMethods.IsraelDateTime());
                    Console.WriteLine
                    ("EXCEPTION in loopInventory-" + UtilityMethods.IsraelDateTime());
                    return false;
            }
      }
 public static bool ExistInAsinToSku(string asin)
        {
            
            SqlConnection con = new SqlConnection(SD.connectionStr);
            string sql = "SELECT 1 FROM [dbo].AsinToSku " +
"                         WHERE Asin = @ASIN";
            try
            {
                con.Open();
                
                SqlCommand cmd = new SqlCommand(sql, con);
              cmd.Parameters.Add("@ASIN", SqlDbType.VarChar, 20).Value = asin;
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
                UtilityMethods.WriteToLog("Exception in ExistInAsinToSku: "+ex.Message);
                Console.WriteLine("Exception in ExistInAsinToSku: "+ex.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
                return false;
            }
        }
public static  void AddInventoryToKT(string asin, int availableQuantity,
                                       int inboundShippedQuantity,int inboundReceivingQuantity,
                                       int reservedQuantity, string marketPlace)
      {
       SqlConnection con = new SqlConnection(SD.connectionStr);
   try{          
       string sql ="";
         
  sql = "INSERT INTO [dbo].AmazonInventories " +
"                  (MarketPlace,UpdateDate,Asin,AvailableQty,InboundShippedQty," +
"                   InboundReceivingQty, ReservedQty) VALUES"+
"                  (" +
"                    @MP,@UPDATE,@ASIN,@AVQ,@SHIPPEDQ,@RECEIVQ,@RESERVQ)";
         con.Open();
        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@MP", SqlDbType.VarChar, 5).Value = marketPlace;
        cmd.Parameters.Add("@UPDATE", SqlDbType.DateTime, 100).Value = UtilityMethods.IsraelDateTime();
        cmd.Parameters.Add("@ASIN", SqlDbType.VarChar, 20).Value = asin;
        cmd.Parameters.Add("@AVQ", SqlDbType.Int, 100).Value = availableQuantity;
        cmd.Parameters.Add("@SHIPPEDQ", SqlDbType.Int, 100).Value = inboundShippedQuantity;
        cmd.Parameters.Add("@RECEIVQ", SqlDbType.Int, 100).Value = inboundReceivingQuantity;
        cmd.Parameters.Add("@RESERVQ", SqlDbType.Int, 100).Value = reservedQuantity; 
        int effectedRows = cmd.ExecuteNonQuery();
        con.Close();
       // Console.WriteLine("Update of sku " + sku + " with quantity " + qty);
        //updateInventory(sku,qty,con);//UPDATE INVENTORY
       if(effectedRows != 1)
          {
             Console.WriteLine("ERROR AddInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.WriteToLog("ERROR AddInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
          }
      }
   catch(Exception e)
            {
   UtilityMethods.WriteToLog("Exception on AddInventoryToKT with asin " + asin+ "-" + UtilityMethods.IsraelDateTime());
                Console.WriteLine("Exception on AddInventoryToKT with asin "  + asin+ "-" + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();
   UtilityMethods.WriteToLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
}
        }
public static bool deleteInventoryTable(string marketPlace)
        {
        SqlConnection con = new SqlConnection(SD.connectionStr);
        try
        {
         con.Open();
         string sql = "DELETE From [dbo].AmazonInventories WHERE MarketPlace = '"+marketPlace+"'";
                SqlCommand cmd = new SqlCommand(sql, con);
               
                cmd.ExecuteNonQuery();
                con.Close();
              return true;
            }
            catch(Exception e)
            {
                UtilityMethods.WriteToLog("Exception on AmazonInventories " + UtilityMethods.IsraelDateTime());
                Console.WriteLine("Exception on AmazonInventories " + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();  
               UtilityMethods.WriteToLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
               return false;
            }
        }
  
}
}

