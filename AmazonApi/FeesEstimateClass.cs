using Newtonsoft.Json.Linq;
using RestSharp;
using System.Data.SqlClient;
using System.Data;
using RestSharp.Serializers.NewtonsoftJson;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using FikaAmazonAPI.AmazonSpApiSDK.Models.FbaSmallandLight;
using System.Net.Http.Headers;
using Amazon.Runtime.Internal;

namespace AmazonAPI
{
    public class FeesEstimateClass
    {
      public static string FeesEstimate(string token,  string sellerSKU,string marketplaceId)
      {
       int shippingCost;
       string endpoint = "https://sellingpartnerapi-na.amazon.com"; // Change for your regio
       var client = new RestClient(endpoint);
        var request = new RestRequest($"/products/fees/v0/listings/{sellerSKU}/feesEstimate", Method.Post);

        // Add Headers
        request.AddHeader("Authorization", $"Bearer {token}");
        request.AddHeader("x-amz-access-token", token);
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/json");

        // Request Body
        var requestBody = new
        {
    FeesEstimateRequest = new
    {
        MarketplaceId = marketplaceId,
        IsAmazonFulfilled = true,
        PriceToEstimateFees = new
        {
            ListingPrice = new
            {
                CurrencyCode = "CAD",//"USD",
                Amount = 10
            },
            Shipping = new
            {
                CurrencyCode = "CAD",//"USD",
                Amount = 10 // Pass the shipping cost as a parameter
            },
            Points = new
            {
                PointsNumber = 0,
                PointsMonetaryValue = new
                {
                    CurrencyCode = "CAD",//"USD",
                    Amount = 0
                }
            }
        },
        Identifier = "UmaS1"
    }
};
 request.AddJsonBody(JsonConvert.SerializeObject(requestBody));
        // Execute request
         RestResponse response =  client.ExecuteAsync(request).GetAwaiter().GetResult();
return null;
     }
    }
}

//{
//  "payload": {
//    "FeesEstimateResult": {
//      "Status": "Success"
//                  ,"FeesEstimateIdentifier": {
//                "MarketplaceId": "ATVPDKIKX0DER"
//                          ,"IdType": "SellerSKU"
//                                  ,"SellerId": "A3SR1OERCH239J"
//                        ,"SellerInputIdentifier": "UmaS1"
//                        ,"IsAmazonFulfilled": true
//                        ,"IdValue": "FUEL HOSE"
//                        ,"PriceToEstimateFees": {
//                    "ListingPrice": {
//            "CurrencyCode": "USD",
//            "Amount": 10
//          }
//                              ,"Shipping": {
//            "CurrencyCode": "USD",
//            "Amount": 10
//          }
//                                ,"Points": {
//                            "PointsNumber": 0
//                            ,"PointsMonetaryValue": {
//                "CurrencyCode": "USD",
//                "Amount": 0
//              }
//                          }
//                  }
//              }
//                  ,"FeesEstimate": {
//                "TimeOfFeesEstimation": "2025-03-04T17:47:58.000Z"
//                          ,"TotalFeesEstimate": {
//            "CurrencyCode": "USD",
//            "Amount": 15.4
//          }
//                          ,"FeeDetailList": [
//                          {
//                "FeeType": "ReferralFee",
//                "FeeAmount": {
//                  "CurrencyCode": "USD",
//                  "Amount": 11.5
//                },
//                "FinalFee": {
//                  "CurrencyCode": "USD",
//                  "Amount": 11.5
//                }
//                                                  ,"FeePromotion": {
//                    "CurrencyCode": "USD",
//                    "Amount": 0.0
//                  }
//                                                              }
//              ,                          {
//                "FeeType": "VariableClosingFee",
//                "FeeAmount": {
//                  "CurrencyCode": "USD",
//                  "Amount": 0.0
//                },
//                "FinalFee": {
//                  "CurrencyCode": "USD",
//                  "Amount": 0.0
//                }
//                                                  ,"FeePromotion": {
//                    "CurrencyCode": "USD",
//                    "Amount": 0.0
//                  }
//                                                              }
//              ,                          {
//                "FeeType": "PerItemFee",
//                "FeeAmount": {
//                  "CurrencyCode": "USD",
//                  "Amount": 0.0
//                },
//                "FinalFee": {
//                  "CurrencyCode": "USD",
//                  "Amount": 0.0
//                }
//                                                  ,"FeePromotion": {
//                    "CurrencyCode": "USD",
//                    "Amount": 0.0
//                  }
//                                                              }
//              ,                          {
//                "FeeType": "FBAFees",
//                "FeeAmount": {
//                  "CurrencyCode": "USD",
//                  "Amount": 3.9
//                },
//                "FinalFee": {
//                  "CurrencyCode": "USD",
//                  "Amount": 3.9
//                }
//                                                  ,"FeePromotion": {
//                    "CurrencyCode": "USD",
//                    "Amount": 0.0
//                  }
//                                                                ,"IncludedFeeDetailList": [
//                                      {
//                      "FeeType": "FBAWeightHandling",
//                      "FeeAmount": {
//                        "CurrencyCode": "USD",
//                        "Amount": 0.0
//                      },
//                      "FinalFee": {
//                        "CurrencyCode": "USD",
//                        "Amount": 0.0
//                      }
//                                                                  ,"FeePromotion": {
//                        "CurrencyCode": "USD",
//                        "Amount": 0.0
//                      }
//                                                                }
//                    ,                                      {
//                      "FeeType": "FBAPickAndPack",
//                      "FeeAmount": {
//                        "CurrencyCode": "USD",
//                        "Amount": 3.9
//                      },
//                      "FinalFee": {
//                        "CurrencyCode": "USD",
//                        "Amount": 3.9
//                      }
//                                                                  ,"FeePromotion": {
//                        "CurrencyCode": "USD",
//                        "Amount": 0.0
//                      }
//                                                                }
//                    ,                                      {
//                      "FeeType": "FBAOrderHandling",
//                      "FeeAmount": {
//                        "CurrencyCode": "USD",
//                        "Amount": 0.0
//                      },
//                      "FinalFee": {
//                        "CurrencyCode": "USD",
//                        "Amount": 0.0
//                      }
//                                                                  ,"FeePromotion": {
//                        "CurrencyCode": "USD",
//                        "Amount": 0.0
//                      }
//                                                                }
//                                                      ]
//                              }
//                                    ]
//              }
//                }
//  }
//}
