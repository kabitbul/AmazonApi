
using System.Data.SqlClient;
using System.Data;

namespace AmazonAPI
{
    public class AAmzInventoryCostClass
    {
      public static void populateDailyInventoryCostTable(int storeId,string marketplace)
       {
         SqlConnection con = new SqlConnection(SD.connectionStr);
         double totalFBACost = 0;
         double totalAWDCost = 0;
         double totalOnTheWayCost = 0;
         string sql;
         string marketplaceName = DataByStoreClass.getMarketplaceName(marketplace);
          double total = 0;
        //FBA
       //select fba.Asin, (AvailableQty+InboundShippedQty + InboundReceivingQty + ReservedQty) totalQ, ats.Cost,
      //((fba.AvailableQty+fba.InboundShippedQty + fba.InboundReceivingQty +fba.ReservedQty)*ats.Cost) totalcost
     //from AAmzFBAInventory fba,AAmzAsinToSku ats
    //where ats.StoreId = fba.StoreId and ats.Asin = fba.Asin and fba.StoreId = 1 and MarketPlace = 'US' 
       
sql = "SELECT SUM(((fba.AvailableQty+fba.InboundShippedQty + fba.InboundReceivingQty +fba.ReservedQty)*ats.Cost)) totalCost, 'FBA' invType" +
"      FROM AAmzFBAInventory fba,AAmzAsinToSku ats" +
"      WHERE ats.StoreId = fba.StoreId and ats.Asin = fba.Asin and fba.StoreId = "+storeId+" and MarketPlace = '"+marketplaceName+"' " +
"      and(fba.MarketPlace = 'US' OR (fba.MarketPlace = 'CA' and ats.IsCanadaAsin = 1 and ats.RestockCA = 1))" +
"UNION" +
"      SELECT SUM(((awd.totalInboundQuantity + totalOnhandQuantity)*ats.Cost))totalcost, 'AWD' invType" +
"      FROM AAmzAWDInventory awd,AAmzAsinToSku ats" +
"      WHERE ats.StoreId = awd.StoreId and ats.Asin = awd.Asin and awd.StoreId = "+storeId+" and MarketPlace = '"+marketplaceName+"'" +
"      UNION" +
"      SELECT SUM((s.Quantity*ats.Cost)) totalcost, 'OTW' invType" +
"      FROM AAmzStockPurchase s, AAmzAsinToSku ats" +
"      WHERE ats.StoreId = s.StoreId and ats.Asin = s.ProductAsin and s.InboundUpdated = 0 and s.StoreId = "+storeId+" and s.MarketPlace = '"+marketplaceName+"'";
        try
            {
                con.Open();
                
                SqlCommand cmd = new SqlCommand(sql, con);
                 SqlDataReader reader = cmd.ExecuteReader();
                 while (reader.Read())
                 {
                   string invtype = reader.GetString(1);
                   total = reader.IsDBNull(0) ? 0 : Convert.ToDouble(reader[0]);
                   if(invtype == "FBA") 
                      totalFBACost = total;
                   else if(invtype == "AWD")
                       totalAWDCost = total;
                   else if (invtype == "OTW")
                      totalOnTheWayCost = total;

                 }
                 reader.Close();
                 if (totalFBACost == 0 && totalAWDCost == 0 && totalOnTheWayCost == 0)
                   {
                      con.Close();
                      return;
                    }
                //add record
                 try{    
                      sql = "INSERT INTO aamzInventoryCost (StoreId,MarketPlace,DateCreated,FBACost,AWDCost,OnTheWayCost) VALUES " +
                           "(@STRID,@MP,@DATECREATED,@FBACOST,@AWDCOST,@OTWCOST)";
                      cmd = new SqlCommand(sql, con);
                      cmd.Parameters.Add("@STRID", SqlDbType.Int,100).Value = storeId;
                      cmd.Parameters.Add("@MP", SqlDbType.VarChar, 5).Value = marketplaceName;
                      cmd.Parameters.Add("@DATECREATED", SqlDbType.DateTime, 100).Value = UtilityMethods.IsraelDateTime();
                      cmd.Parameters.Add("@FBACOST", SqlDbType.Decimal, 100).Value = totalFBACost;
                      cmd.Parameters.Add("@AWDCOST", SqlDbType.Decimal, 100).Value = totalAWDCost;
                      cmd.Parameters.Add("@OTWCOST", SqlDbType.Decimal, 100).Value = totalOnTheWayCost;
                    }catch(Exception e){
                        Console.WriteLine(e.Message);

                       }
                  int effectedRows = cmd.ExecuteNonQuery();
                con.Close();
                if(effectedRows != 1)
                {
                 Console.WriteLine(DataByStoreClass.getStoreName(storeId)+" ERROR- effected rows are " + effectedRows + " while adding new record to  aamzInventoryCost");
                }
          }catch(Exception e)
           {
             Console.WriteLine(e.Message);
             con.Close();
           }
        }


public static void AddOrderToKT(string orderId, string marketPlace,DateTime purchaseDate,int qty,
                                string asin,int storeId)
{
   SqlConnection con = new SqlConnection(SD.connectionStr);
   string storeName = DataByStoreClass.getStoreName(storeId);
   try{          
       string sql ="";
         
  sql = "INSERT INTO [dbo].AAmzOrders " +
"                  (AmazonOrdId,MarketPlace,PurchaseDate,Qty,Asin,StoreId" +
"                  ) VALUES " +
"                  (" +
"                    @ORDID,@MP,@PDATE,@QTY,@ASIN,@STRID)";
         con.Open();
        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.Parameters.Add("@ORDID", SqlDbType.VarChar, 100).Value = orderId;
        cmd.Parameters.Add("@MP", SqlDbType.VarChar, 5).Value = marketPlace;
        purchaseDate = UtilityMethods.PDTDateTime(purchaseDate);
        cmd.Parameters.Add("@PDATE", SqlDbType.DateTime, 100).Value = purchaseDate;
        cmd.Parameters.Add("@QTY", SqlDbType.Int, 100).Value = qty;
        cmd.Parameters.Add("@ASIN", SqlDbType.VarChar, 100).Value = asin;
        cmd.Parameters.Add("@STRID", SqlDbType.Int,100).Value = storeId;
        int effectedRows = cmd.ExecuteNonQuery();
        con.Close();
       // Console.WriteLine("Update of sku " + sku + " with quantity " + qty);
        //updateInventory(sku,qty,con);//UPDATE INVENTORY
       if(effectedRows != 1)
          {
             Console.WriteLine(storeName+" ERROR- effected rows are " + effectedRows + " while adding order with asin " + asin);
            UtilityMethods.WriteToTextLog(storeName+" ERROR- effected rows are " + effectedRows + " while adding order with asin " + asin,"ERR");
 UtilityMethods.SendErrMail(storeName+" ERROR- effected rows are " + effectedRows + " while adding order with asin " + asin);
          }
      else
       {
        Console.WriteLine(storeName+" Added Order "+orderId+" From MarketPlace " + marketPlace+" purchase date " +purchaseDate);
        UtilityMethods.WriteToTextLog(storeName+ "Added Order "+orderId+" From MarketPlace " + marketPlace+" purchase date " +purchaseDate,"INF");
       }
      }
   catch(Exception e)
            {
   UtilityMethods.WriteToTextLog(storeName+ " Exception on AddOrderToKT with asin " + asin+ "-" ,"ERR");
                Console.WriteLine(storeName+" Exception on AddOrderToKT with asin "  + asin+ "-" + UtilityMethods.IsraelDateTime());
                if (con.State == ConnectionState.Open)
                    con.Close();
   UtilityMethods.WriteToTextLog(e.Message.Length <= 1999 ? e.Message: e.Message.Substring(0, 1999),"ERR");
UtilityMethods.SendErrMail(storeName+" Exception on AddOrderToKT with asin " + asin );
}
}
}
}
