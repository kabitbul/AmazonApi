using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.SqlClient;
using System.Data;
using RestSharp.Serializers.NewtonsoftJson;
using System.Net.Mail;
using System.Net;
using FikaAmazonAPI.AmazonSpApiSDK.Models.FbaSmallandLight;

namespace AmazonAPI
{
    public class GetInventoryClass
    {
public static string GetInventoryBysellerSKU(string token, string marketPlace, string sellerSKU)
      {
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
            UtilityMethods.WriteToTextLog(
                    "GetInventoryBysellerSKU returned status " + response.StatusCode + "with sellerSKU "+sellerSKU ,"ERR");
                  UtilityMethods.SendErrMail("GetInventoryBysellerSKU returned status " + response.StatusCode + "with sellerSKU "+sellerSKU);
                    Console.WriteLine
                    ("GetInventoryBysellerSKU returned status " + response.StatusCode + "with sellerSKU "+sellerSKU + "-" + UtilityMethods.IsraelDateTime());
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
                    "EXCEPTION in GetInventoryBysellerSKU with sellerSKU " + sellerSKU,"ERR");
UtilityMethods.SendErrMail("EXCEPTION in GetInventoryBysellerSKU with sellerSKU " + sellerSKU + " " + e.Message);
                    Console.WriteLine
                    ("EXCEPTION in GetInventoryBysellerSKU with sellerSKU " +sellerSKU +" " + UtilityMethods.IsraelDateTime());
          UtilityMethods.WriteToTextLog(e.Message,"ERR");
                    return null;
            }
}
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
                UtilityMethods.WriteToTextLog(
                "GetInventory returned status " + response.StatusCode,"ERR");
                 UtilityMethods.SendErrMail("GetInventory returned status " + response.StatusCode);
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
            UtilityMethods.WriteToTextLog("=================START ADD INVENTORY TO " + marketPlace+"=============","INF");
            if (nextToken == null) // there are less then 100 and no paging
             {
              bool result  = loopInventory(JObject.Parse(response.Content),marketPlace);  
              if (result)
               {
                  UtilityMethods.WriteToTextLog("END inventory update successfully","INF");
                }
              else
              {
                 UtilityMethods.WriteToTextLog("FAILED during inventory update","ERR");
               UtilityMethods.SendErrMail("FAILED during inventory update");
               }
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
                    UtilityMethods.WriteToTextLog(
                    "GetInventory2 returned status " + response.StatusCode ,"ERR");
                  UtilityMethods.SendErrMail("GetInventory2 returned status " + response.StatusCode);
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
              if (result)
               {
                  UtilityMethods.WriteToTextLog("END inventory update successfully","INF");
                }
              else
              {
                 UtilityMethods.WriteToTextLog("FAILED during inventory update","ERR");
               UtilityMethods.SendErrMail("FAILED during inventory update");
                }
             }
          }
       return "";
}
  public static string GetAWDInventory(string token)
      {

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
                "GetAWDInventory returned status " + response.StatusCode,"ERR");
                 UtilityMethods.SendErrMail("GetAWDInventory returned status " + response.StatusCode);
                Console.WriteLine
                ("GetAWDInventory e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                return null;
               }
         if((JObject.Parse(response.Content))["pagination"] != null)  
            nextToken = (string)(JObject.Parse(response.Content))["pagination"]["nextToken"];
        if(response.Content != null)
          {
           deleteAWDInventoryTable();
            UtilityMethods.WriteToTextLog("=================START ADD AWD INVENTORY=============","INF");
            if (nextToken == null) // there are less then 100 and no paging
             {
              bool result  = loopAWDInventory(JObject.Parse(response.Content));  
              if (result)
               {
                  UtilityMethods.WriteToTextLog("END AWD inventory update successfully","INF");
                }
              else
              {
                 UtilityMethods.WriteToTextLog("FAILED during AWD inventory update","ERR");
               UtilityMethods.SendErrMail("FAILED during AWD inventory update");
               }
             }
             else
             {
               bool result;
               while(nextToken != null)
               {
                  result  = loopAWDInventory(JObject.Parse(response.Content)); 
                  request.AddOrUpdateParameter("nextToken", nextToken, ParameterType.QueryString);
                  response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
                  if(response.StatusCode != System.Net.HttpStatusCode.OK)
                   {
                    UtilityMethods.WriteToTextLog(
                    "GetAWDInventory2 returned status " + response.StatusCode ,"ERR");
                  UtilityMethods.SendErrMail("GetAWDInventory2 returned status " + response.StatusCode);
                    Console.WriteLine
                    ("GetAWDInventory2 e returned status " + response.StatusCode +"-" + UtilityMethods.IsraelDateTime());
                    return null;
                   }
                if((JObject.Parse(response.Content))["pagination"] != null)  
                   nextToken = (string)(JObject.Parse(response.Content))["pagination"]["nextToken"];
                else
                   nextToken = null;
               }
              //at the end of the loop nextToken is null so we are at the last page
              result  = loopAWDInventory(JObject.Parse(response.Content)); 
              if (result)
               {
                  UtilityMethods.WriteToTextLog("END AWD inventory update successfully","INF");
                }
              else
              {
                 UtilityMethods.WriteToTextLog("FAILED during AWD inventory update","ERR");
               UtilityMethods.SendErrMail("FAILED during AWD inventory update");
                }
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
             string asin = (string)obj["asin"];
             if(((int)obj["totalQuantity"]) == 0 || (string)obj["asin"] == "B0CBQHLCWS"
                                                 || (string)obj["asin"] == "B08G9NLFGC"
                                                 || (string)obj["asin"] == "B0CND1P9YD")
               continue;
            
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
          catch( Exception e){
          UtilityMethods.WriteToTextLog(
                    "EXCEPTION in loopInventory-","ERR");
UtilityMethods.SendErrMail("EXCEPTION in loopInventory" + e.Message);
                    Console.WriteLine
                    ("EXCEPTION in loopInventory-" + UtilityMethods.IsraelDateTime());
          UtilityMethods.WriteToTextLog(e.Message,"ERR");
                    return false;
            }
      }
///////////////////////////////////////////////////////////////////
public static bool loopAWDInventory(JObject jobj)
      {
      try{ 
           int i =0;
           JArray ordersArray = (JArray)jobj["inventory"];
           foreach(JObject obj in ordersArray)
           {
             // get the asin based on SKU
             string asin = GetInventoryBysellerSKU(SD.accessToken,SD.USMarketplace,(string)obj["sku"] ) ;
             //if(((int)obj["totalQuantity"]) == 0 || (string)obj["asin"] == "B0CBQHLCWS"
             //                                    || (string)obj["asin"] == "B08G9NLFGC"
             //                                    || (string)obj["asin"] == "B0CND1P9YD")
             //  continue;
            
             int inboundQuantity = (int)obj["totalInboundQuantity"];
             int OnhandQuantity = (int)obj["totalOnhandQuantity"];
             bool existInAsinToSku = ExistInAsinToSku(asin);
             if(existInAsinToSku)
                 AddAWDInventoryToKT(asin,inboundQuantity,OnhandQuantity);
              
            }

             return true;
           }
          catch( Exception e){
          UtilityMethods.WriteToTextLog(
                    "EXCEPTION in loopInventory-","ERR");
UtilityMethods.SendErrMail("EXCEPTION in loopInventory" + e.Message);
                    Console.WriteLine
                    ("EXCEPTION in loopInventory-" + UtilityMethods.IsraelDateTime());
          UtilityMethods.WriteToTextLog(e.Message,"ERR");
                    return false;
            }
      }
///////////////////////////////////////////////////////////////////
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
                UtilityMethods.WriteToTextLog("ASIN " + asin + " Needs to be added to table AsinToSku" , "ERR");
                Console.WriteLine("ASIN " + asin + " Needs to be added to table AsinToSku" );
                SendSkuNeedsEmail(asin);
                return false;
            }
            catch (Exception ex)
            {
                UtilityMethods.WriteToTextLog("Exception in ExistInAsinToSku: "+ex.Message,"ERR");
                UtilityMethods.SendErrMail("Exception in ExistInAsinToSku: "+ex.Message);
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
            UtilityMethods.SendErrMail("AddInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.WriteToTextLog("AddInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin,"ERR");
          }
      }
   catch(Exception e)
            {
   UtilityMethods.WriteToTextLog("Exception on AddInventoryToKT with asin " + asin,"ERR");
                Console.WriteLine("Exception on AddInventoryToKT with asin "  + asin+ "-" + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();
   UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
}
        }
///////////////////////////////////////////////////////////////
public static  void AddAWDInventoryToKT(string asin, int inboundQuantity,
                                       int onHandQuantity)
      {
       SqlConnection con = new SqlConnection(SD.connectionStr);
   try{          
       string sql ="";
         
  sql = "INSERT INTO [dbo].AmazonAWDInventories " +
"                  (MarketPlace,UpdateDate,Asin,totalInboundQuantity,totalOnhandQuantity) VALUES"+
"              (" +"@MP,          @UPDATE,@ASIN,    @INBOUNDQ,            @ONHANDQ)";
         con.Open();
        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@MP", SqlDbType.VarChar, 5).Value = "US";
        cmd.Parameters.Add("@UPDATE", SqlDbType.DateTime, 100).Value = UtilityMethods.IsraelDateTime();
        cmd.Parameters.Add("@ASIN", SqlDbType.VarChar, 20).Value = asin;
        cmd.Parameters.Add("@INBOUNDQ", SqlDbType.Int, 100).Value = inboundQuantity;
        cmd.Parameters.Add("@ONHANDQ", SqlDbType.Int, 100).Value = onHandQuantity;
        int effectedRows = cmd.ExecuteNonQuery();
        con.Close();

       if(effectedRows != 1)
          {
             Console.WriteLine("ERROR AddAWDInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.SendErrMail("AddAWDInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.WriteToTextLog("AddAWDInventoryToKT- effected rows are " + effectedRows + " while adding order with asin " + asin,"ERR");
          }
      }
   catch(Exception e)
            {
   UtilityMethods.WriteToTextLog("Exception on AddAWDInventoryToKT with asin " + asin,"ERR");
                Console.WriteLine("Exception on AddAWDInventoryToKT with asin "  + asin+ "-" + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();
   UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
}
        }
//////////////////////////////////////////////////////////////
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
        }
//
public static bool deleteAWDInventoryTable()
        {
        SqlConnection con = new SqlConnection(SD.connectionStr);
        try
        {
         con.Open();
         string sql = "DELETE From [dbo].AmazonAWDInventories";
                SqlCommand cmd = new SqlCommand(sql, con);
               
                cmd.ExecuteNonQuery();
                con.Close();
               UtilityMethods.WriteToTextLog("successfully deleted AWD inventory on KT","INF");
               Console.WriteLine("successfully deleted AWD inventory on KT");
              return true;
            }
            catch(Exception e)
            {
                UtilityMethods.WriteToTextLog("Exception on deleteAWDInventoryTable ","ERR");
                Console.WriteLine("Exception on deleteAWDInventoryTable " + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();  
               UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
 UtilityMethods.SendErrMail(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999));
               return false;
            }
        }
private static void SendSkuNeedsEmail(string asin)
        {
            try
            {
                // Configure your SMTP client settings.
                // For example, using Gmail's SMTP server.
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("ktonlinemarketing1@gmail.com", "mqgcejocdvbsmxui"),
                    EnableSsl = true
                };

                // Create the email message.
                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("ktonlinemarketing1@gmail.com"),
                    Subject = "Tomer - Ya Tahat",
                    Body = "You Forgot to add Asin: " + asin + " To the screen \"Assin By Sku\""
                };

                // Send the email to yourself.
                mailMessage.To.Add("ktonlinemarketing1@gmail.com");

                // Send the email.
                smtpClient.Send(mailMessage);
            }
            catch (Exception emailEx)
            {
                // If the email fails, write to the console (or log appropriately).
                Console.WriteLine("Failed to send exception email: " + emailEx.Message);
            }
        }
public bool DeleteOldRec()
        {
SqlConnection con = new SqlConnection(SD.connectionStr);
        try
        {
         con.Open();
         string sql =   "DELETE FROM AmazonOrders" +
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
}
}

