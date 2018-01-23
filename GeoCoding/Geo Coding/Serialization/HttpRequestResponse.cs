using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Net;
using System.Reflection;
using System.Text;
using System.Data;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Linq;

/// <summary>
/// Summary description for HttpWebDerive
/// </summary>
/// 
namespace HttpWebRequestResponse
{
    public class HttpRequestResponse : HttpWebBase
    {
        private string URI;
        private string Request = "";
        private Hashtable HeaderCollection = new Hashtable();
        private string FinalResponds = string.Empty;         
#region HttpProperties
        /// <summary>
        /// Sets the basic WebRequest Credentials Username
        /// </summary>
        /// 
        public string USERNAME_HTTP
        {
            set
            {
                UserName = value;
            }
        }
        /// <summary>
        /// Sets the basic WebRequest Credentials Password
        /// </summary>
        /// 
        public string PASSWORD_HTTP
        {
            set
            {
                Password = value;
            }
        }
        /// <summary>
        /// Sets the WebProxy Server Address
        /// </summary>
        /// 
        public string PROXY_SERVER
        {
            set
            {
                ProxyServer = value;
            }
        }
        /// <summary>
        /// Sets the WebProxy Server Port
        /// </summary>
        /// 
        public int PROXY_PORT
        {
            set
            {
                ProxyPort = value;
            }
        }
        /// <summary>
        /// Sets the WebRequest Method
        /// </summary>
        /// 
        public string HTTP_REQUEST_METHOD
        {
            set
            {
                RequestMethod = value;
            }
        }
        /// <summary>
        /// Sets the WebRequest Date
        /// </summary>
        /// 
        public DateTime HTTP_REQUEST_DATE
        {
            set
            {
                RequestDate = value;
            }
        }
        /// <summary>
        /// Sets the WebRequest Content Type
        /// </summary>
        /// 
        public string HTTP_CONTENT_TYPE
        {
            set
            {
                content_type = value;
            }
        }
        /// <summary>
        /// Sets the WebRequest URI
        /// </summary>
        /// 
        public string HTTP_REQUEST_URI
        {
            set
            {
                URI = value;
            }
        }
        /// <summary>
        /// Sets the WebRequest string Buffer
        /// </summary>
        /// 
        public string PostRequestBuffer
        {
            set
            {
                Request = value;
            }
        }
        /// <summary>
        /// Sets the collection of header name/value pairs associated with the request.
        /// </summary>
        /// 
        public void AddRequestHeader(string Name, object Value)
        {
            HeaderCollection.Add(Name, Value);
        }
        /// <summary>
        /// Sets the WebRequest Request encoding type;
        /// </summary>
        /// 
        public EncodingType REQUEST_ENCODING
        {
            set
            {
                EcodingEnum = value;
            }
        }
    #endregion

#region Constructors
        public HttpRequestResponse(string uri)
        {
            URI = uri;
        }
        public HttpRequestResponse()
        {

        }
        public HttpRequestResponse(string uri, string request)
        {
            URI = uri;
            Request = request;
        }
#endregion
        
#region for XML Methods
        /// <summary>
        /// If the Response from Server is XML, get the value of the element base on the parameter
        /// </summary>
        /// 
        public string ParseXmlResponseAsString(string Element)
        {
            return XmlGetElementContentAsString(HttpRequestRespond, Element);
        }      

        public string ParseXmlElementResponseAsString(string XML, string Element)
        {
            return XmlGetElementContentAsString(XML, Element);
        }
        /// <summary>
        /// If the Response from Server is XML, Deserialize XML into Object
        /// </summary>
        /// 
        public T DeserializeXmlResponseAsObject<T>()
        {
            return ConvertXMLIntoObject<T>(HttpRequestRespond);
        }

        public T DeserializeXmlResponseAsObject<T>(string XML)
        {
            return ConvertXMLIntoObject<T>(XML);
        }
        /// <summary>
        /// If the Response from Server is XML, parse XMl response based on the Array key object
        /// </summary>
        /// 
        public Dictionary<object, object> ParseXmlResponseAsDictionary(object[] ListSearchKey)
        {
            return XmlConvertToDictionary(HttpRequestRespond,  ListSearchKey);        
        }
        /// <summary>
        /// Get Xml response as XDocument
        /// </summary>
        /// 
        public XDocument GetXmlResponseAsXDocument
        { 
            get
            {
              return XmlResponseAsXDocument(HttpRequestRespond);
            }
        }
#endregion
        
#region for JSON Methods
        /// <summary>
        /// If the Response from Server is JSON, get the value of the element base on the parameter
        /// </summary>
        /// 
        public string ParseJsonElementResponseAsString(string Element)
        {
            return ParseJsonToString(HttpRequestRespond, Element);
        }

        public string ParseJsonElementResponseAsString(string JSON, string Element)
        {
            return ParseJsonToString(JSON, Element);
        }
        /// <summary>
        /// If the Response from Server is JSON, get the value of the element base on the array objects parameter
        /// </summary>
        /// 
        public Dictionary<object, object> ParseJsonResponseAsDictionary(object[] ListSearchKey)
        {
            return JsonConvertToDictionary(HttpRequestRespond, ListSearchKey);
        }

        public Dictionary<object, object> ParseJsonResponseAsDictionary(string JSON, object[] ListSearchKey)
        {
            return JsonConvertToDictionary(JSON, ListSearchKey);
        }
        /// <summary>
        /// If the Response from Server is JSON, Deserialize JSON into Object
        /// </summary>
        /// 
        public T DeserializeJsonResponseAsObject<T>()
        {
            return ConvertJsonIntoObject<T>(HttpRequestRespond);
        }
        public T DeserializeJsonResponseAsObject<T>(string JSON)
        {
            return ConvertJsonIntoObject<T>(JSON);
        }
        /// <summary>
        /// If the Response from Server is JSON, Deserialize JSON into DataTable
        /// </summary>
        /// 
        public DataTable DeserializeJsonResponseAsDataTable()
        {          
            return ConvertJsonIntoDataTable(HttpRequestRespond);            
        }

        public DataTable DeserializeJsonResponseAsDataTable(string JSON)
        {
            return ConvertJsonIntoDataTable(JSON);
        }
        /// <summary>
        /// If the Response from Server is JSON, Deserialize JSON into DataSet
        /// </summary>
        /// 
        public DataSet DeserializeJsonResponseAsDataSet()
        {           
            return ConvertJsonIntoDataSet(HttpRequestRespond);            
        }
        public DataSet DeserializeJsonResponseAsDataSet(string JSON)
        {
            return ConvertJsonIntoDataSet(JSON);
        }
        /// <summary>
        /// If the Response from Server is XML, Deserialize XML into Dictionary
        /// </summary>
        /// 
        public Dictionary<object, object> DeserializeJsonResponseAsDictionary()
        {           
            return ConvertJsonIntoObject<Dictionary<object, object>>(HttpRequestRespond);
        }
        public Dictionary<object, object> DeserializeJsonResponseAsDictionary(string JSON)
        {
            return ConvertJsonIntoObject<Dictionary<object, object>>(JSON);
        }
        /// <summary>
        /// Writes the server response to file
        /// </summary>
        /// 
        public void WriteResponseToDrive(string Path, string FileName)
        {
            WriteToFile(Path, FileName);
        }
        #endregion

        /// <summary>
        /// Gets the final response from the server
        /// </summary>
        /// 
        public string GetServerResponse
        {
            get
            {
                return HttpRequestRespond;
            }
        }        
        private string HttpRequestRespond
        {
            get
            {
                HttpWebRequest WebReq = null;          
                   
                try
                {
                    WebReq = CreateHttpWebRequest(URI, HeaderCollection);

                    FinalResponds = SendHttpRequest(WebReq, Request);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    WebReq = null;
                }
                return FinalResponds;   
            }
        }

    }
}