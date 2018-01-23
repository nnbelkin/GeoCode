using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using psgSQLHelper; 

namespace SQLBIProjects
{

    public class GeoCodeRecord : psgSQLHelper.Configuration
    {
        public GeoCodeRecord()
        {
        }

       
        public XDocument Document { get; set; }
        public String location { get; set; }
        public String street1 { get; set; }
        public String street2 { get; set; }
        public String city { get; set; }
        public String state { get; set; }
        public String zip { get; set; }
        public String formattedLocation { get; set; }
        public String latitude { get; set; }
        public String longitude { get; set; }
        public String locationType { get; set; }
        public Int32 address_id { get; set; }
        public String status { get; set; }
    }

    public class geocode : psgSQLHelper.Configuration
    {
        List<GeoCodeRecord> records = new List<GeoCodeRecord>();
        String debugmode = "";
        String utype = "";
        String connstring = psgSQLHelper.Configuration.ConnectionString;
        Boolean toProcess = true;
        Int32 count = 0;

        String APIKey = getAPIKeybyMachine(); 

        static String getAPIKeybyMachine()
        {
           String key = ""; 
           switch (System.Environment.MachineName)
            {
               case  "BOSPSGVDEV2" :            
                   key = "AIzaSyDhHc7AYq1NGt-QUWf-zliEdkB8jtUxWQU"; 
                   break;
               case "BOSPSGDEV3" :
                  // key = "AIzaSyA25vCdhLYc3JuzZXVxS70xQti-DPxJ678";
                  key = "AIzaSyCny3yOcUTWOqT9ZzDrA1k3nCLWit7T1r8";
                 //  key = "AIzaSyDhHc7AYq1NGt-QUWf-zliEdkB8jtUxWQU"; 
                   break;
               case "BOSPSGSQL5":
                   key = "AIzaSyA25vCdhLYc3JuzZXVxS70xQti-DPxJ678";
                   break;
               case "BOSPSGW6":
                   key = "AIzaSyAEXBs-x8T7KI_72WdMoKKxI_2ZGxL1kx0"; 
                  // key = "AIzaSyCTnyl--1X9NletAx50TtOrpbU26gX3LZk";

                 // key = "AIzaSyDhHc7AYq1NGt-QUWf-zliEdkB8jtUxWQU"; 
                 //  key = "AIzaSyA25vCdhLYc3JuzZXVxS70xQti-DPxJ678";
                   break;
                case "D7QLYJM1":
                   key = "AIzaSyA25vCdhLYc3JuzZXVxS70xQti-DPxJ678";
                   break;
            }

           return key; 
        }
        
        //key for my desktop 
        //AIzaSyCezLqIA8mT8URqGUv8iXwFSuoguiKanws
        //key for dev2
        // "AIzaSyDhHc7AYq1NGt-QUWf-zliEdkB8jtUxWQU";
        //key for sql5
        //"AIzaSyA25vCdhLYc3JuzZXVxS70xQti-DPxJ678";

        public geocode(String _debugmode, String _utype, ref Int32 _count)
        {
            debugmode = _debugmode;
            utype = _utype;
            count = _count; 
            if (_debugmode !="")
            {
                connstring = connstring.Replace("TS2", "TS2" + debugmode).Replace("bospsgsql5", "bospsgsql6"); 
            }
            try
            {
                while (toProcess)
                {
                    getAddresses();

                    foreach (GeoCodeRecord _rec in records)
                    {
                        Delay(500);
                        if (processGeoAddresses(_rec) == false)
                        {
                            toProcess = false;
                            break;
                        }
                        if (count >= 2500)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            { Console.Write("count " + count + " " + ex.Message);}
            finally {}
        }

        #region methods


        Boolean processGeoAddresses(GeoCodeRecord rec)
        {
            XElement x;
            Boolean result = true; 

            try
            {
                

               // String geocodeURL = "http://maps.google.com/maps/geo?key=" + APIKey + "&q=" + rec.location + "&sensor=false&output=xml";

               String geocodeURL = "https://maps.googleapis.com/maps/api/geocode/xml?address=" + rec.location + "&key=" + APIKey;// "&sensor=false";

               rec.Document = System.Xml.Linq.XDocument.Load(geocodeURL);

               rec.status = rec.Document.Descendants().First(p => p.Name.LocalName == "status").Value;

               if (rec.status =="OK")
               {

                   x = rec.Document.Descendants().SingleOrDefault(p => p.Name.LocalName == "location");

                  // XElement x = rec.Document.Descendants().First(p => p.Name.LocalName == "location");

                  rec.latitude = x.Descendants().SingleOrDefault(p => p.Name.LocalName == "lat").Value;
                  rec.longitude = x.Descendants().SingleOrDefault(p => p.Name.LocalName == "lng").Value;
                  rec.formattedLocation = rec.Document.Descendants().SingleOrDefault(p => p.Name.LocalName == "formatted_address").Value;
                  rec.status = rec.Document.Descendants().SingleOrDefault(p => p.Name.LocalName == "formatted_address").Value;
                  Console.WriteLine(rec.location + " " + rec.latitude + " " + rec.longitude + " processed");
                  updateAddress(rec);
                  count += 1;
                  result= true;        

               }
               else if (rec.status =="ZERO_RESULTS")
               {
                   rec.latitude = "--";
                   rec.longitude = "--";
                   rec.formattedLocation = rec.status;
                   Console.WriteLine(rec.location + " " + rec.latitude + " " + rec.longitude + "  zero results");
                   updateAddress(rec);
                   count += 1;
                   result= true; 
               }
               else if (rec.status == "OVER_QUERY_LIMIT")
               {
                   Console.WriteLine(rec.location + " " + " err" + rec.status);
                   //Console.Read();
                   result = false;
               }
               else
               {
                   Console.WriteLine( utype + ":" + rec.address_id + " " + rec.location + " " + " err" + rec.Document.ToString());
                   Console.Read();
                   result = false;
               }


               return result; 

            }
            catch (Exception ex)
            {
               Console.WriteLine(utype + ":" + + rec.address_id + " " + rec.location + " " + ex.Message);
               try
               {


                   if (rec.status == "OK")
                   {
                       x = rec.Document.Descendants().First(p => p.Name.LocalName == "location");

                       // XElement x = rec.Document.Descendants().First(p => p.Name.LocalName == "location");

                       rec.latitude = x.Descendants().First(p => p.Name.LocalName == "lat").Value;
                       rec.longitude = x.Descendants().First(p => p.Name.LocalName == "lng").Value;
                       rec.formattedLocation = rec.Document.Descendants().First(p => p.Name.LocalName == "formatted_address").Value;
                       Console.WriteLine(rec.location + " " + rec.latitude + " " + rec.longitude + " first rec processed");
                       updateAddress(rec);
                       count += 1; 
                       result = true; 
                   }
                   else
                   {
                       Console.WriteLine(utype + ":" + rec.address_id + " " +  rec.location + " " +  " err" + ex.Message);
                      // Console.Read();
                       result = false; 
                   }
               }
               catch (Exception ex1)
               {
                   Console.WriteLine(utype + ":" + rec.address_id + " " +  rec.location + " " + ex1.Message + " err");
                   String status = rec.Document.Descendants().First(p => p.Name.LocalName == "status").Value;
                   rec.latitude = "--";
                   rec.longitude = "--";
                   rec.formattedLocation =status + " " +  ex1.Message;
                   updateAddress(rec);
                   count += 1;
                   result = true; 
                }
               
               return result;
            }
            finally { }
        }


       

        String CreateGeoURL(String location)
        {
            try
            {
                return  "http://maps.googleapis.com/maps/api/geocode/xml?address=" + location + "&key=" + APIKey;//&sensor=false"

               // return "https://maps.googleapis.com/maps/api/geocode/json?address=" + location + "&key=" + APIKey; 

                //geoDoc = System.Xml.Linq.XDocument.Load(geocodeURL);

                //return true; 

            }
            catch (Exception ex)
            {
                Console.Write(location + " " + ex.Message);
                return "";
            }
            finally { }
        }

          


        Boolean getAddresses()
        {

            try
            {
                toProcess = false;
                records = new List<GeoCodeRecord>();
            String sql = "exec stp_LoadAddressQueue '" + utype + "'";
                SqlDataReader dr = SqlHelper.ExecuteReader(connstring, CommandType.Text, sql);
                do
                {
                    while (dr.Read())
                    {
                        GeoCodeRecord rec = new GeoCodeRecord();
                        rec.location = ParseStreet(Convert.ToString(dr["Address"]));
                        rec.street1 = ParseStreet(Convert.ToString(dr["Street1"]));
                        rec.street2 = ParseStreet(Convert.ToString(dr["Street1"]));
                        rec.city = ParseStreet(Convert.ToString(dr["City"]));
                        rec.state = ParseStreet(Convert.ToString(dr["State"]));
                        rec.zip = ParseStreet(Convert.ToString(dr["Zip"]));
                        rec.address_id = Convert.ToInt32(dr["Address_Id"]); 
                        records.Add(rec);
                        toProcess = true; 
                    }
                }
                while (dr.NextResult());
                dr.Close();


                return true;
            }
            catch (Exception e)
            {
                Console.Write("Err:" + e.Message);
                return false;
            }
            finally{}

        }

        Boolean updateAddress(GeoCodeRecord geo)
        {
             String sql = "";
            try
            {
                String table = "";
                String id = "";

                switch (utype)
                {
                    case "Emp":
                        {
                            table = "tblEmployee_Address"; id = "Address_Id";
                            break;
                        }
                    case "EmpInt":
                        {
                            table = "tblEmployee_Address"; id = "Address_Id";
                            break;
                        }
                    case "Client":
                        {
                            table = "tblCOMPANY_CONTACT_ADDRESS"; id = "CompContAddress_Id";
                            break;
                        }
                    case "Order":
                        {
                            table = "PSGBoston.dbo.OrderDetails"; id = "OrderId";
                            break;
                        }
                    case "Zip":
                        {
                            table = "tblxZIPCODE"; id = "Pk_Id";
                            break;
                        }
                
                }
               
                if (utype != "Order")
                {
                    sql = "UPDATE " + table + " SET Geo_Lat='" + geo.latitude + "', Geo_Long='" + geo.longitude + "'," +
                               "GeoAddressUsed='" + geo.formattedLocation.Replace("'", "''") + "', ModifiedDate=GetDate() WHERE " + id + "=" + geo.address_id;
                }
                else
                {
                    sql = "UPDATE " + table + " SET GeoLatitude='" + geo.latitude + "', GeoLongitude='" + geo.longitude + "'" +
                            "WHERE " + id + "=" + geo.address_id;

                }

                psgSQLHelper.SqlHelper.ExecuteScalar(connstring, CommandType.Text, sql); 

                return true;
            }
            catch (Exception e)
            {
                Console.Write("Err:" + sql + " " + e.Message);
                return false; 
            }
            finally { }

        }

        #endregion 

        #region delay

        private void Delay(Int32 mlsec)
        {

            Thread.Sleep(mlsec);

        }


        #endregion 

        #region fix address

        private string  ParseStreet(String street) 
        {
            street = TruncateString(street, "SUITE");
            street = TruncateString(street, "STE");
            street = TruncateString(street, "#");
            street = TruncateString(street, "UNIT ");
            street = TruncateString(street, "Room ");
  


            street = TruncateString(street, "USED");
            street = RemoveStreet(street, "HAND DELV");
            street = RemoveStreet(street, "Remit");

           


            return street;
       }

        String RemoveStreet(String street, String badText) 
            {

                street = street.Replace(badText, ""); 
                return street;
            }

        String TruncateString(String street , String removeText) 
        {
        if (street.Contains(removeText))
            {
            if (street.IndexOf(removeText) == 0 )
            {
                street = street.Substring(0, street.IndexOf(removeText));
            }
            else 
            {
                street = street.Substring(0, street.IndexOf(removeText) - 1);
            }

            }

        return street;
        }

        #endregion 

    }
}


#region commented
/*
 Boolean ProcessRequest(GeoCodeRecord rec)
        {
            try
            {
                String requestUrl = CreateGeoURL(rec.location);

               // using (HttpRequestResponse HttpWeb = new HttpRequestResponse(requestUrl))
              //  {
                
                    String test = "{\n   \"results\" : [\n      {\n         \"address_components\" : [\n            {\n               \"long_name\" : \"Belchertown\",\n               \"short_name\" : \"Belchertown\",\n               \"types\" : [ \"locality\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Belchertown\",\n               \"short_name\" : \"Belchertown\",\n               \"types\" : [ \"administrative_area_level_3\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Hampshire County\",\n               \"short_name\" : \"Hampshire County\",\n               \"types\" : [ \"administrative_area_level_2\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Massachusetts\",\n               \"short_name\" : \"MA\",\n               \"types\" : [ \"administrative_area_level_1\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"United States\",\n               \"short_name\" : \"US\",\n               \"types\" : [ \"country\", \"political\" ]\n            }\n         ],\n         \"formatted_address\" : \"Belchertown, MA, USA\",\n         \"geometry\" : {\n            \"bounds\" : {\n               \"northeast\" : {\n                  \"lat\" : 42.358762,\n                  \"lng\" : -72.33164189999999\n               },\n               \"southwest\" : {\n                  \"lat\" : 42.1858049,\n                  \"lng\" : -72.47248900000001\n               }\n            },\n            \"location\" : {\n               \"lat\" : 42.2770346,\n               \"lng\" : -72.4008884\n            },\n            \"location_type\" : \"APPROXIMATE\",\n            \"viewport\" : {\n               \"northeast\" : {\n                  \"lat\" : 42.358762,\n                  \"lng\" : -72.33164189999999\n               },\n               \"southwest\" : {\n                  \"lat\" : 42.1858049,\n                  \"lng\" : -72.47248900000001\n               }\n            }\n         },\n         \"place_id\" : \"ChIJKcp6WHvI5okRS4SxYyFiLOM\",\n         \"types\" : [ \"locality\", \"political\" ]\n      },\n      {\n         \"address_components\" : [\n            {\n               \"long_name\" : \"Belchertown\",\n               \"short_name\" : \"Belchertown\",\n               \"types\" : [ \"neighborhood\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Belchertown\",\n               \"short_name\" : \"Belchertown\",\n               \"types\" : [ \"locality\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Belchertown\",\n               \"short_name\" : \"Belchertown\",\n               \"types\" : [ \"administrative_area_level_3\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Hampshire County\",\n               \"short_name\" : \"Hampshire County\",\n               \"types\" : [ \"administrative_area_level_2\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"Massachusetts\",\n               \"short_name\" : \"MA\",\n               \"types\" : [ \"administrative_area_level_1\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"United States\",\n               \"short_name\" : \"US\",\n               \"types\" : [ \"country\", \"political\" ]\n            },\n            {\n               \"long_name\" : \"01007\",\n               \"short_name\" : \"01007\",\n               \"types\" : [ \"postal_code\" ]\n            }\n         ],\n         \"formatted_address\" : \"Belchertown, Belchertown, MA 01007, USA\",\n         \"geometry\" : {\n            \"bounds\" : {\n               \"northeast\" : {\n                  \"lat\" : 42.300247,\n                  \"lng\" : -72.37827890000001\n               },\n               \"southwest\" : {\n                  \"lat\" : 42.240972,\n                  \"lng\" : -72.43755299999999\n               }\n            },\n            \"location\" : {\n               \"lat\" : 42.2770362,\n               \"lng\" : -72.4009176\n            },\n            \"location_type\" : \"APPROXIMATE\",\n            \"viewport\" : {\n               \"northeast\" : {\n                  \"lat\" : 42.300247,\n                  \"lng\" : -72.37827890000001\n               },\n               \"southwest\" : {\n                  \"lat\" : 42.240972,\n                  \"lng\" : -72.43755299999999\n               }\n            }\n         },\n         \"place_id\" : \"ChIJxSVUYEbI5okRLnfEPHWIaSc\",\n         \"types\" : [ \"neighborhood\", \"political\" ]\n      }\n   ],\n   \"status\" : \"OK\"\n}\n";

                    HttpRequestResponse HttpWeb = new HttpRequestResponse(1, requestUrl, test); 
 
                
                    Dictionary<object, object> oDictionary = new Dictionary<object, object>();
                    HttpWeb.HTTP_REQUEST_METHOD = "GET";
                   // HttpWeb.AddRequestHeader("Authorization", "SAMPLESIGNATURE");
                    //object[] ListToParse = new object[] {"formatted_address", "location_type", "location", "lat", "long" };
                    //oDictionary = HttpWeb.ParseJsonResponseAsDictionary
                    //    (ListToParse); // execute ParseJsonResponseAsDictionary method
                    
                 
                    //    rec.formattedLocation = Convert.ToString(oDictionary["formatted_address"]);
                    //    rec.locationType = Convert.ToString(oDictionary["location_type"]);
                    //    rec.latitude = Convert.ToString(oDictionary["lat"]);
                    //    rec.longitude = Convert.ToString(oDictionary["long"]);
                    //    Console.WriteLine(oDictionary["error_code"]);

                        String results = HttpWeb.ParseJsonElementResponseAsString("results");
                        HttpRequestResponse oresults = new HttpRequestResponse(1, requestUrl, results);
                        rec.formattedLocation = HttpWeb.ParseJsonElementResponseAsString("formatted_address");
                        String geometry = HttpWeb.ParseJsonElementResponseAsString("location");
                        Console.Write(results);
                        //rec.formattedLocation = HttpWeb.ParseJsonElementResponseAsString("formatted_address");
                        //rec.locationType = HttpWeb.ParseJsonElementResponseAsString("location_type");
                        //rec.latitude = HttpWeb.ParseJsonElementResponseAsString("lat");
                        //rec.longitude = HttpWeb.ParseJsonElementResponseAsString("long"); 
                        
                 //  updateAddress(rec); 
                    
               // }

              

                
               
                


                return true;
            }
            catch (Exception ex)
            {
                Console.Write(rec.location + " " + ex.Message);
                return false; 
            }
            
        }

*/

#endregion 