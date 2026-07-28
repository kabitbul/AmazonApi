using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.SqlClient;
using System.Data;
using RestSharp.Serializers.NewtonsoftJson;
using System.Net.Mail;
using System.Net;
using FikaAmazonAPI.AmazonSpApiSDK.Models.FbaSmallandLight;
using System.Security.Cryptography.X509Certificates;

namespace AmazonAPI
{
    public class AAmzGetInventoryClass
    {
public static string GetInventoryBysellerSKU(string token, string marketPlace, string sellerSKU,int storeId)
      {
       string storeName = DataByStoreClass.getStoreName(storeId);
       try{ 
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
        request.AddQueryParameter("sellerSkus",sellerSKU);
        //request.AddQueryParameter("startDateTime",invDate);
        request.AddQueryParameter("nextToken",null); 
        //request.AddQueryParameter("MaxResultsPerPage",100);
         ////////////////////////////
         ////////////////////////////

         RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
         JObject jobj =  JObject.Parse(response.Content);
         if(response.StatusCode != System.Net.HttpStatusCode.OK)
          {
            UtilityMethods.WriteToTextLog(storeName+
                    " GetInventoryBysellerSKU returned status " + response.StatusCode + "with sellerSKU "+sellerSKU ,"ERR");
                  UtilityMethods.SendErrMail(storeName+" GetInventoryBysellerSKU returned status " + response.StatusCode + "with sellerSKU "+sellerSKU);
                    Console.WriteLine
                    (storeName+" GetInventoryBysellerSKU returned status " + response.StatusCode + "with sellerSKU "+sellerSKU + "-" + UtilityMethods.IsraelDateTime());
                    return null;
          }
        JArray ordersArray = (JArray)jobj["payload"]["inventorySummaries"];
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
                    storeName+" EXCEPTION in GetInventoryBysellerSKU with sellerSKU " + sellerSKU,"ERR");
UtilityMethods.SendErrMail(storeName+ " EXCEPTION in GetInventoryBysellerSKU with sellerSKU " + sellerSKU + " " + e.Message);
                    Console.WriteLine
                    (storeName+" EXCEPTION in GetInventoryBysellerSKU with sellerSKU " +sellerSKU +" " + UtilityMethods.IsraelDateTime());
          UtilityMethods.WriteToTextLog(e.Message,"ERR");
                    return null;
            }
}
      public static string GetInventory(string token, string marketPlace, string invDate,int storeId, bool forTemp)
      {
         string storeName = DataByStoreClass.getStoreName(storeId);
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
                UtilityMethods.WriteToTextLog(storeName +
                " GetInventory returned status " + response.StatusCode,"ERR");
                 UtilityMethods.SendErrMail(storeName+" GetInventory returned status " + response.StatusCode);
                Console.WriteLine
                (storeName+" GetInventory e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
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
var previousInventory =
        new Dictionary<string, FbaInventorySnapshot>(
            StringComparer.OrdinalIgnoreCase);
            if(!forTemp)
{
    previousInventory =
            GetPreviousFbaInventory(storeId, marketPlace);

        deleteInventoryTable(marketPlace, storeId);
}
            UtilityMethods.WriteToTextLog("=================START ADD INVENTORY TO "+storeName+ " " + marketPlace+"=============","INF");
            if (nextToken == null) // there are less then 100 and no paging
             {
              bool result  = loopInventory(JObject.Parse(response.Content),marketPlace,storeId, forTemp,
previousInventory);  
              if (result)
               {
                  UtilityMethods.WriteToTextLog(storeName+" END inventory update successfully","INF");
                }
              else
              {
                 UtilityMethods.WriteToTextLog(storeName+ " FAILED during inventory update","ERR");
               UtilityMethods.SendErrMail(storeName+" FAILED during inventory update");
               }
             }
             else
             {
               bool result;
               while(nextToken != null)
               {
                  result  = loopInventory(JObject.Parse(response.Content),marketPlace,storeId,forTemp,previousInventory); 
                  request.AddOrUpdateParameter("nextToken", nextToken, ParameterType.QueryString);
                  response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
                  if(response.StatusCode != System.Net.HttpStatusCode.OK)
                   {
                    UtilityMethods.WriteToTextLog(
                    storeName+" GetInventory2 returned status " + response.StatusCode ,"ERR");
                  UtilityMethods.SendErrMail(storeName+" GetInventory2 returned status " + response.StatusCode);
                    Console.WriteLine
                    (storeName+" GetInventory2 e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                    return null;
                   }
                if((JObject.Parse(response.Content))["pagination"] != null)  
                   nextToken = (string)(JObject.Parse(response.Content))["pagination"]["nextToken"];
                else
                   nextToken = null;
               }
              //at the end of the loop nextToken is null so we are at the last page
              result  = loopInventory(JObject.Parse(response.Content),marketPlace,storeId,forTemp,previousInventory); 
              if (result)
               {
                  UtilityMethods.WriteToTextLog(storeName+" END inventory update successfully","INF");
                }
              else
              {
                 UtilityMethods.WriteToTextLog(storeName+" FAILED during inventory update","ERR");
               UtilityMethods.SendErrMail(storeName+" FAILED during inventory update");
                }
             }
          }
       return "";
}
  public static string GetAWDInventory(string token,int storeId)
      {
         string storeName = DataByStoreClass.getStoreName(storeId);
         string nextToken = null;
         var options = new RestClientOptions("https://sellingpartnerapi-na.amazon.com")
           {
             MaxTimeout = -1,
            };
        RestClient client = new RestClient(options);
        var request = new RestRequest("awd/2024-05-09/inventory", Method.Get);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("x-amz-access-token",token);
        request.AddQueryParameter("details","HIDE");
        request.AddQueryParameter("nextToken",null);
         ////////////////////////////
         ////////////////////////////

         RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
        if(response.StatusCode != System.Net.HttpStatusCode.OK)
           {
                UtilityMethods.WriteToTextLog(
                storeName +" GetAWDInventory returned status " + response.StatusCode,"ERR");
                 UtilityMethods.SendErrMail(storeName+" GetAWDInventory returned status " + response.StatusCode);
                Console.WriteLine
                (storeName+" GetAWDInventory e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                return null;
               }
         if((JObject.Parse(response.Content))["nextToken"] != null)  
            nextToken = (string)(JObject.Parse(response.Content))["nextToken"];
        if(response.Content != null)
          {
           deleteAWDInventoryTable(storeId);
            UtilityMethods.WriteToTextLog("=================START ADD "+storeName+" AWD INVENTORY=============","INF");
            if (nextToken == null) // there are less then 100 and no paging
             {
              bool result  = loopAWDInventory(JObject.Parse(response.Content),storeId);  
              if (result)
               {
                  UtilityMethods.WriteToTextLog(storeName+" END AWD inventory update successfully","INF");
                }
              else
              {
                 UtilityMethods.WriteToTextLog(storeName+" FAILED during AWD inventory update","ERR");
               UtilityMethods.SendErrMail(storeName+" FAILED during AWD inventory update");
               }
             }
             else
             {
               bool result;
               while(nextToken != null)
               {
                  result  = loopAWDInventory(JObject.Parse(response.Content),storeId); 
                  request.AddOrUpdateParameter("nextToken", nextToken, ParameterType.QueryString);
                  response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
                  if(response.StatusCode != System.Net.HttpStatusCode.OK)
                   {
                    UtilityMethods.WriteToTextLog(storeName+
                    " GetAWDInventory2 returned status " + response.StatusCode ,"ERR");
                  UtilityMethods.SendErrMail(storeName+" GetAWDInventory2 returned status " + response.StatusCode);
                    Console.WriteLine
                    (storeName+" GetAWDInventory2 e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                    return null;
                   }
                if((JObject.Parse(response.Content))["nextToken"] != null)  
                   nextToken = (string)(JObject.Parse(response.Content))["nextToken"];
                else
                   nextToken = null;
               }
              //at the end of the loop nextToken is null so we are at the last page
              result  = loopAWDInventory(JObject.Parse(response.Content),storeId); 
              if (result)
               {
                  UtilityMethods.WriteToTextLog(storeName+" END AWD inventory update successfully","INF");
                }
              else
              {
                 UtilityMethods.WriteToTextLog(storeName+" FAILED during AWD inventory update","ERR");
               UtilityMethods.SendErrMail(storeName+" FAILED during AWD inventory update");
                }
             }
          }
       return "";
}

     private static bool loopInventory(JObject jobj,string marketPlace,int storeId,bool forTemp,
    Dictionary<string, FbaInventorySnapshot> previousInventory)
      {
       string storeName = DataByStoreClass.getStoreName(storeId);
      try{ 
           int i =0;
           JArray ordersArray = (JArray)jobj["payload"]["inventorySummaries"];
           foreach(JObject obj in ordersArray)
           {
             if(forTemp)
              {
               AddTempSkuAsin((string)obj["sellerSku"],(string)obj["asin"],storeId);
               continue;
                    }
             string asin = (string)obj["asin"];
             if(((int)obj["totalQuantity"]) == 0 || (string)obj["asin"] == "B0CBQHLCWS"
                                                 || (string)obj["asin"] == "B08G9NLFGC"
                                                 || (string)obj["asin"] == "B0CND1P9YD")
               continue;
            
             int availableQuantity = (int)obj["inventoryDetails"]["fulfillableQuantity"];
             int inboundShippedQuantity = (int)obj["inventoryDetails"]["inboundShippedQuantity"]; 
             int inboundReceivingQuantity = (int)obj["inventoryDetails"]["inboundReceivingQuantity"]; 
             int reservedQuantity = (int)obj["inventoryDetails"]["reservedQuantity"]["totalReservedQuantity"];
             bool existInAsinToSku = ExistInAsinToSku(asin,storeId);
            if (existInAsinToSku)
{
    if (previousInventory.TryGetValue(
            asin,
            out FbaInventorySnapshot previous))
    {
bool receivingStarted =
    previous.InboundReceivingQty == 0
    && inboundReceivingQuantity > 0;

bool availableThresholdReachedWhileReceiving =
    previous.AvailableQty < 30
    && availableQuantity >= 30
    && (
        previous.InboundReceivingQty > 0
        || inboundReceivingQuantity > 0
    );

bool receivingFinishedAndAvailableIncreased =
    previous.InboundReceivingQty > 0
    && inboundReceivingQuantity == 0
    && availableQuantity > previous.AvailableQty;

bool shippedQuantityMovedIntoAvailable =
    previous.InboundShippedQty > inboundShippedQuantity
    && availableQuantity > previous.AvailableQty;

bool receivingIndication =
    receivingStarted
    || availableThresholdReachedWhileReceiving
    || receivingFinishedAndAvailableIncreased
    || shippedQuantityMovedIntoAvailable;

        bool actionableQuantity =
            availableQuantity >= 30;

        if (receivingIndication && actionableQuantity)
        {
            string detectionReason =
    receivingStarted
        ? "ReceivingStarted"
        : availableThresholdReachedWhileReceiving
            ? "AvailableThresholdReached"
            : receivingFinishedAndAvailableIncreased
                ? "ReceivingFinishedAvailableUp"
                : "InboundShippedDownAvailableUp";

            TryCreateFbaReceivingAlert(
                storeId,
                marketPlace,
                asin,
                availableQuantity,
                inboundShippedQuantity,
                inboundReceivingQuantity,
                reservedQuantity,
                detectionReason);
        }
    }

    AddInventoryToKT(
        asin,
        availableQuantity,
        inboundShippedQuantity,
        inboundReceivingQuantity,
        reservedQuantity,
        marketPlace,
        storeId);
}
              
            }

             return true;
           }
          catch( Exception e){
          UtilityMethods.WriteToTextLog(
                    storeName+" EXCEPTION in loopInventory-","ERR");
UtilityMethods.SendErrMail(storeName+" EXCEPTION in loopInventory" + e.Message);
                    Console.WriteLine
                    (storeName+" EXCEPTION in loopInventory-" + UtilityMethods.IsraelDateTime());
          UtilityMethods.WriteToTextLog(e.Message,"ERR");
                    return false;
            }
      }
///////////////////////////////////////////////////////////////////
private static void TryCreateFbaReceivingAlert(
    int storeId,
    string marketplace,
    string asin,
    int availableQty,
    int inboundShippedQty,
    int inboundReceivingQty,
    int reservedQty,
    string detectionReason)
{
    const string sql = @"
        INSERT INTO dbo.AAmzFBAReceivingAlerts
        (
            StoreId,
            Marketplace,
            Asin,
            AvailableQty,
            InboundShippedQty,
            InboundReceivingQty,
            ReservedQty,
            DetectionReason,
            CreatedDate,
            IsHandled,
            HandledDate
        )
        SELECT
            @StoreId,
            @Marketplace,
            @Asin,
            @AvailableQty,
            @InboundShippedQty,
            @InboundReceivingQty,
            @ReservedQty,
            @DetectionReason,
            @CreatedDate,
            0,
            NULL
        WHERE NOT EXISTS
        (
            SELECT 1
            FROM dbo.AAmzFBAReceivingAlerts
            WHERE StoreId = @StoreId
              AND Marketplace = @Marketplace
              AND Asin = @Asin
              AND CreatedDate >= DATEADD(DAY, -7, @CreatedDate)
        );";

    using (SqlConnection connection = new SqlConnection(SD.connectionStr))
    using (SqlCommand command = new SqlCommand(sql, connection))
    {
       DateTime createdDate = UtilityMethods.IsraelDateTime();

        command.Parameters.Add(
            "@StoreId",
            SqlDbType.Int).Value = storeId;

        command.Parameters.Add(
            "@Marketplace",
            SqlDbType.NVarChar,
            3).Value = marketplace;

        command.Parameters.Add(
            "@Asin",
            SqlDbType.NVarChar,
            20).Value = asin;

        command.Parameters.Add(
            "@AvailableQty",
            SqlDbType.Int).Value = availableQty;

        command.Parameters.Add(
            "@InboundShippedQty",
            SqlDbType.Int).Value = inboundShippedQty;

        command.Parameters.Add(
            "@InboundReceivingQty",
            SqlDbType.Int).Value = inboundReceivingQty;

        command.Parameters.Add(
            "@ReservedQty",
            SqlDbType.Int).Value = reservedQty;

        command.Parameters.Add(
            "@DetectionReason",
            SqlDbType.NVarChar,
            50).Value = detectionReason;

        command.Parameters.Add(
            "@CreatedDate",
            SqlDbType.DateTime2).Value = createdDate;

        connection.Open();

        int insertedRows = command.ExecuteNonQuery();

        if (insertedRows > 0)
        {
            UtilityMethods.WriteToTextLog(
                "FBA receiving alert created: " +
                $"StoreId={storeId}, " +
                $"Marketplace={marketplace}, " +
                $"ASIN={asin}, " +
                $"Available={availableQty}, " +
                $"Reason={detectionReason}",
                "INF");
        }
    }
}
///////////////////////////////////////////////////////////////////
public static bool loopAWDInventory(JObject jobj,int storeId)
      {
       string storeName = DataByStoreClass.getStoreName(storeId);
      try{ 
           int i =0;
           JArray ordersArray = (JArray)jobj["inventory"];
           foreach(JObject obj in ordersArray)
           {
             // get the asin based on SKU
             string asin = getAsinFromTemp((string)obj["sku"],storeId ) ;
             int inboundQuantity = (int)obj["totalInboundQuantity"];
             int OnhandQuantity = (int)obj["totalOnhandQuantity"];
             bool existInAsinToSku = ExistInAsinToSku(asin,storeId);
             if(existInAsinToSku)
                 AddAWDInventoryToKT(asin,inboundQuantity,OnhandQuantity,storeId);
              
            }

             return true;
           }
          catch( Exception e){
          UtilityMethods.WriteToTextLog(
                   storeName+ " EXCEPTION in loopInventory-","ERR");
UtilityMethods.SendErrMail(storeName+" EXCEPTION in loopInventory" + e.Message);
                    Console.WriteLine
                    (storeName+" EXCEPTION in loopInventory-" + UtilityMethods.IsraelDateTime());
          UtilityMethods.WriteToTextLog(e.Message,"ERR");
                    return false;
            }
      }
///////////////////////////////////////////////////////////////////
 public static bool ExistInAsinToSku(string asin,int storeId)
        {
            string storeName = DataByStoreClass.getStoreName(storeId);
            SqlConnection con = new SqlConnection(SD.connectionStr);
            string sql = "SELECT 1 FROM [dbo].AAmzAsinToSku " +
"                         WHERE Asin = @ASIN AND StoreId = @STRID";
            try
            {
                con.Open();
                
                SqlCommand cmd = new SqlCommand(sql, con);
              cmd.Parameters.Add("@ASIN", SqlDbType.VarChar, 20).Value = asin;
              cmd.Parameters.Add("@STRID", SqlDbType.Int, 100).Value = storeId;
                 SqlDataReader reader = cmd.ExecuteReader();
                
              while (reader.Read())
                {
                 con.Close();
                 return true;
                }
                    con.Close();
                UtilityMethods.WriteToTextLog(storeName+" ASIN " + asin + " Needs to be added to table AsinToSku" , "ERR");
                Console.WriteLine(storeName+" ASIN " + asin + " Needs to be added to table AsinToSku" );
                SendSkuNeedsEmail(asin,storeId);
                return false;
            }
            catch (Exception ex)
            {
                UtilityMethods.WriteToTextLog(storeName+" Exception in ExistInAsinToSku: "+ex.Message,"ERR");
                UtilityMethods.SendErrMail(storeName+" Exception in ExistInAsinToSku: "+ex.Message);
                Console.WriteLine(storeName+" Exception in ExistInAsinToSku: "+ex.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
                return false;
            }
        }
public static  void AddInventoryToKT(string asin, int availableQuantity,
                                       int inboundShippedQuantity,int inboundReceivingQuantity,
                                       int reservedQuantity, string marketPlace,int storeId)
      {
       SqlConnection con = new SqlConnection(SD.connectionStr);
       string  storeName = DataByStoreClass.getStoreName(storeId);
   try{          
       string sql ="";
         
  sql = "INSERT INTO [dbo].AAmzFBAInventory " +
"                  (MarketPlace,UpdateDate,Asin,AvailableQty,InboundShippedQty," +
"                   InboundReceivingQty, ReservedQty,StoreId) VALUES"+
"                  (" +
"                    @MP,@UPDATE,@ASIN,@AVQ,@SHIPPEDQ,@RECEIVQ,@RESERVQ,@STRID)";
         con.Open();
        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@MP", SqlDbType.VarChar, 5).Value = marketPlace;
        cmd.Parameters.Add("@UPDATE", SqlDbType.DateTime, 100).Value = UtilityMethods.IsraelDateTime();
        cmd.Parameters.Add("@ASIN", SqlDbType.VarChar, 20).Value = asin;
        cmd.Parameters.Add("@AVQ", SqlDbType.Int, 100).Value = availableQuantity;
        cmd.Parameters.Add("@SHIPPEDQ", SqlDbType.Int, 100).Value = inboundShippedQuantity;
        cmd.Parameters.Add("@RECEIVQ", SqlDbType.Int, 100).Value = inboundReceivingQuantity;
        cmd.Parameters.Add("@RESERVQ", SqlDbType.Int, 100).Value = reservedQuantity; 
       cmd.Parameters.Add("@STRID", SqlDbType.Int, 100).Value = storeId; 
        int effectedRows = cmd.ExecuteNonQuery();
        con.Close();
       // Console.WriteLine("Update of sku " + sku + " with quantity " + qty);
        //updateInventory(sku,qty,con);//UPDATE INVENTORY
       if(effectedRows != 1)
          {
             Console.WriteLine(storeName+" ERROR AddInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.SendErrMail(storeName+ " AddInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.WriteToTextLog(storeName+" AddInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin,"ERR");
          }
      }
   catch(Exception e)
            {
   UtilityMethods.WriteToTextLog(storeName+" Exception on AddInventoryToKT with asin " + asin,"ERR");
                Console.WriteLine(storeName+" Exception on AddInventoryToKT with asin "  + asin+ "-" + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();
   UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
}
        }
///////////////////////////////////////////////////////////////
public static  void AddAWDInventoryToKT(string asin, int inboundQuantity,
                                       int onHandQuantity,int storeId)
      {
       SqlConnection con = new SqlConnection(SD.connectionStr);
       string storeName = DataByStoreClass.getStoreName(storeId);
   try{          
       string sql ="";
         
  sql = "INSERT INTO [dbo].AAmzAWDInventory " +
"                  (MarketPlace,UpdateDate,Asin,totalInboundQuantity,totalOnhandQuantity,StoreId) VALUES"+
"              (" +"@MP,          @UPDATE,@ASIN,    @INBOUNDQ,            @ONHANDQ,        @STRID)";
         con.Open();
        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@MP", SqlDbType.VarChar, 5).Value = "US";
        cmd.Parameters.Add("@UPDATE", SqlDbType.DateTime, 100).Value = UtilityMethods.IsraelDateTime();
        cmd.Parameters.Add("@ASIN", SqlDbType.VarChar, 20).Value = asin;
        cmd.Parameters.Add("@INBOUNDQ", SqlDbType.Int, 100).Value = inboundQuantity;
        cmd.Parameters.Add("@ONHANDQ", SqlDbType.Int, 100).Value = onHandQuantity;
        cmd.Parameters.Add("@STRID", SqlDbType.Int, 100).Value = storeId;
        int effectedRows = cmd.ExecuteNonQuery();
        con.Close();

       if(effectedRows != 1)
          {
             Console.WriteLine(storeName+" ERROR AddAWDInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.SendErrMail(storeName+" AddAWDInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.WriteToTextLog(storeName+" AddAWDInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin,"ERR");
          }
      }
   catch(Exception e)
            {
   UtilityMethods.WriteToTextLog(storeName+" Exception on AddAWDInventoryToKT with asin " + asin,"ERR");
                Console.WriteLine(storeName+" Exception on AddAWDInventoryToKT with asin "  + asin+ "-" + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();
   UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
}
        }
//////////////////////////////////////////////////////////////
public static bool deleteInventoryTable(string marketPlace,int storeId)
        {
        SqlConnection con = new SqlConnection(SD.connectionStr);
        string storeName = DataByStoreClass.getStoreName(storeId);
        try
        {
         con.Open();
         string sql = "DELETE From [dbo].AAmzFBAInventory WHERE MarketPlace = '"+marketPlace+"' AND StoreId = "+storeId;
                SqlCommand cmd = new SqlCommand(sql, con);
               
                cmd.ExecuteNonQuery();
                con.Close();
               UtilityMethods.WriteToTextLog(storeName+" successfully deleted inventory on "+DataByStoreClass.getStoreName(storeId) + 
               " marketplace " + DataByStoreClass.getMarketplaceName(marketPlace),"INF");
               Console.WriteLine("successfully deleted inventory on "+DataByStoreClass.getStoreName(storeId) + 
               " marketplace " + DataByStoreClass.getMarketplaceName(marketPlace));
              return true;
            }
            catch(Exception e)
            {
                UtilityMethods.WriteToTextLog(storeName+" Exception on deleteInventoryTable ","ERR");
                Console.WriteLine(storeName+" Exception on deleteInventoryTable " + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();  
               UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
 UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
               return false;
            }
        }
//
public static bool deleteAWDInventoryTable(int storeId)
        {
        SqlConnection con = new SqlConnection(SD.connectionStr);
        string storeName = DataByStoreClass.getStoreName(storeId);
        try
        {
         con.Open();
         string sql = "DELETE From [dbo].AAmzAWDInventory WHERE StoreId = " + storeId;
                SqlCommand cmd = new SqlCommand(sql, con);
               
                cmd.ExecuteNonQuery();
                con.Close();
               UtilityMethods.WriteToTextLog(storeName+" successfully deleted AWD inventory on KT","INF");
               Console.WriteLine(storeName+" successfully deleted AWD inventory on KT");
              return true;
            }
            catch(Exception e)
            {
                UtilityMethods.WriteToTextLog(storeName+" Exception on deleteAWDInventoryTable ","ERR");
                Console.WriteLine(storeName+" Exception on deleteAWDInventoryTable " + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();  
               UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
 UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
               return false;
            }
        }
private static void SendSkuNeedsEmail(string asin,int storeId)
        {
          string storeName = DataByStoreClass.getStoreName(storeId);
            try
            {
                // Configure your SMTP client settings.
                // For example, using Gmail's SMTP server.
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("ktonlinemarketing1@gmail.com", SD.gmailPassSMTP),
                    EnableSsl = true
                };

                // Create the email message.
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("ktonlinemarketing1@gmail.com"),
                    Subject = "Tomer - Ya Tahat",
                    Body = "You Forgot to add Asin: " + asin + " To the screen \"Assin By Sku\" on store " + storeName
                };

                // Send the email to yourself.
                mailMessage.To.Add("ktonlinemarketing1@gmail.com");

                // Send the email.
                smtpClient.Send(mailMessage);
            }
            catch (Exception emailEx)
            {
                // If the email fails, write to the console (or log appropriately).
                Console.WriteLine(storeName+" Failed to send exception email: " + emailEx.Message);
            }
        }
public bool DeleteOldRec()
        {
SqlConnection con = new SqlConnection(SD.connectionStr);
        try
        {
         con.Open();
         string sql =   "DELETE FROM AAmzOrders" +
"                        WHERE purchaseDate < DATEADD(MONTH, -13, GETDATE())";
                SqlCommand cmd = new SqlCommand(sql, con);
               
                cmd.ExecuteNonQuery();
                con.Close();
               UtilityMethods.WriteToTextLog("successfully deleted inventory on KT","INF");
               Console.WriteLine("successfully deleted inventory on KT");
              return true;
            }
            catch(Exception e)
            {
                UtilityMethods.WriteToTextLog("Exception on deleteInventoryTable ","ERR");
                Console.WriteLine("Exception on deleteInventoryTable " + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();  
               UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
 UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
               return false;
            }
}//delete rec
//-------------------------------
public static  void AddTempSkuAsin(string sellerSku, string asin,int storeId)
      {
       SqlConnection con = new SqlConnection(SD.connectionStr);
       string storeName = DataByStoreClass.getStoreName(storeId);
   try{          
       string sql ="";
         
  sql = "INSERT INTO [dbo].TempSkuAsins " +
"                  (StoreId,sku,asin) VALUES"+
"              (" +"@STRID,@SKU,@ASIN)";
         con.Open();
        SqlCommand cmd = new SqlCommand(sql, con);
cmd.Parameters.Add("@STRID", SqlDbType.Int, 100).Value = storeId;
cmd.Parameters.Add("@SKU", SqlDbType.VarChar, 100).Value = sellerSku;
cmd.Parameters.Add("@ASIN", SqlDbType.VarChar, 20).Value = asin;
        
        int effectedRows = cmd.ExecuteNonQuery();
        con.Close();

       if(effectedRows != 1)
          {
             Console.WriteLine(storeName+" ERROR AddAWDInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.SendErrMail(storeName+" AddAWDInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.WriteToTextLog(storeName+" AddAWDInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin,"ERR");
          }
      }
   catch(Exception e)
            {
   UtilityMethods.WriteToTextLog(storeName+" Exception on AddAWDInventoryToKT with asin " + asin,"ERR");
                Console.WriteLine(storeName+" Exception on AddAWDInventoryToKT with asin "  + asin+ "-" + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();
   UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
}
        }
public static string getAsinFromTemp(string sku,int storeId ) 
{
            string storeName = DataByStoreClass.getStoreName(storeId);
            SqlConnection con = new SqlConnection(SD.connectionStr);
            string sql = "SELECT ASIN FROM [dbo].TempSkuAsins " +
"                         WHERE sku = @SKU AND StoreId = @STRID";
            string asin;
            try
            {
                con.Open();
                
                SqlCommand cmd = new SqlCommand(sql, con);
              cmd.Parameters.Add("@SKU", SqlDbType.VarChar, 100).Value = sku;
              cmd.Parameters.Add("@STRID", SqlDbType.Int, 100).Value = storeId;
                 SqlDataReader reader = cmd.ExecuteReader();
                
              while (reader.Read())
                {
                 asin = reader.GetString(0);
                 con.Close();
                 return asin;
                }

                
                return "";
            }
            catch (Exception ex)
            {
                UtilityMethods.WriteToTextLog(storeName+" Exception in getAsinFromTemp: "+ex.Message,"ERR");
                UtilityMethods.SendErrMail(storeName+" Exception in getAsinFromTemp: "+ex.Message);
                Console.WriteLine(storeName+" Exception in getAsinFromTemp: "+ex.Message);
                if (con.State == ConnectionState.Open)
                    con.Close();
                return "";
            }
        }
public static bool DeleteTempSkuAsin(int storeId)
        {
SqlConnection con = new SqlConnection(SD.connectionStr);
string storeName = DataByStoreClass.getStoreName(storeId);
        try
        {
         con.Open();
         string sql =   "DELETE FROM [dbo].TempSkuAsins" +
"                        WHERE StoreId = @STRID";
                SqlCommand cmd = new SqlCommand(sql, con);
               cmd.Parameters.Add("@STRID", SqlDbType.Int, 100).Value = storeId;
                cmd.ExecuteNonQuery();
                con.Close();
               UtilityMethods.WriteToTextLog("successfully deleted TempSkuAsins from store "+storeName,"INF");
               Console.WriteLine("successfully deleted TempSkuAsins from store "+storeName);
              return true;
            }
            catch(Exception e)
            {
                UtilityMethods.WriteToTextLog("Exception on from store ","ERR");
                Console.WriteLine("Exception on from store " + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();  
               UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
 UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
               return false;
            }
}//delete rec
private static Dictionary<string, FbaInventorySnapshot> GetPreviousFbaInventory(
    int storeId,
    string marketplace)
{
    var result = new Dictionary<string, FbaInventorySnapshot>(
        StringComparer.OrdinalIgnoreCase);

    const string sql = @"
        SELECT
            Asin,
            AvailableQty,
            InboundShippedQty,
            InboundReceivingQty,
            ReservedQty
        FROM AAmzFBAInventory
        WHERE StoreId = @StoreId
          AND Marketplace = @Marketplace";

    using (SqlConnection connection = new SqlConnection(SD.connectionStr))
    using (SqlCommand command = new SqlCommand(sql, connection))
    {
        command.Parameters.Add("@StoreId", SqlDbType.Int).Value = storeId;
        command.Parameters.Add("@Marketplace", SqlDbType.NVarChar, 3).Value = marketplace;

        connection.Open();

        using (SqlDataReader reader = command.ExecuteReader())
        {
            int asinOrdinal = reader.GetOrdinal("Asin");
            int availableOrdinal = reader.GetOrdinal("AvailableQty");
            int shippedOrdinal = reader.GetOrdinal("InboundShippedQty");
            int receivingOrdinal = reader.GetOrdinal("InboundReceivingQty");
            int reservedOrdinal = reader.GetOrdinal("ReservedQty");

            while (reader.Read())
            {
                string asin = reader.IsDBNull(asinOrdinal)
                    ? string.Empty
                    : reader.GetString(asinOrdinal);

                if (string.IsNullOrWhiteSpace(asin))
                {
                    continue;
                }

                result[asin] = new FbaInventorySnapshot
                {
                    Asin = asin,

                    AvailableQty = reader.IsDBNull(availableOrdinal)
                        ? 0
                        : reader.GetInt32(availableOrdinal),

                    InboundShippedQty = reader.IsDBNull(shippedOrdinal)
                        ? 0
                        : reader.GetInt32(shippedOrdinal),

                    InboundReceivingQty = reader.IsDBNull(receivingOrdinal)
                        ? 0
                        : reader.GetInt32(receivingOrdinal),

                    ReservedQty = reader.IsDBNull(reservedOrdinal)
                        ? 0
                        : reader.GetInt32(reservedOrdinal)
                };
            }
        }
    }

    return result;
}
private sealed class FbaInventorySnapshot
{
    public string Asin { get; set; } = string.Empty;
    public int AvailableQty { get; set; }
    public int InboundShippedQty { get; set; }
    public int InboundReceivingQty { get; set; }
    public int ReservedQty { get; set; }
}
}
}

