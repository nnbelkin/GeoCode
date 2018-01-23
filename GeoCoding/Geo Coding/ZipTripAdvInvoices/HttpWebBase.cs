using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Collections;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

/// <summary>
/// Summary description for HttpWebBase
/// </summary>
/// 
namespace SQLBIProjects
{
    public enum EncodingType
    { 
        UTF8,
        ASCII,
        BigEndianUnicode,
        Unicode,
        UTF7, UTF32
    }

    public class HttpWebBase : IDisposable
    {
        protected string UserName;
        protected string Password;
        protected string RequestMethod;
        protected DateTime RequestDate;
        protected string content_type;
        protected string ProxyServer = "";
        protected int ProxyPort;
        protected EncodingType EcodingEnum;
        private string responds = string.Empty;

        protected HttpWebBase()
        {

        }

        protected HttpWebRequest CreateHttpWebRequest(string uri, Hashtable HeaderCollection)
        {
            return CreateWebRequest(uri, HeaderCollection);
        }

        private HttpWebRequest CreateWebRequest(string uri, Hashtable HeaderCollection)
        {
            HttpWebRequest WebReq = null;
            if(!string.IsNullOrEmpty(uri))
            {
                if(!string.IsNullOrEmpty(RequestMethod))
                {
                        WebReq = (HttpWebRequest)WebRequest.Create(uri);
                        try
                        {
                            if (string.IsNullOrEmpty(content_type))
                            {
                                WebReq.ContentType = "application/x-www-form-urlencoded";
                            }
                            else
                            {
                                WebReq.ContentType = content_type;
                            }

                            if (!string.IsNullOrEmpty(RequestMethod))
                            {
                                WebReq.Method = RequestMethod;
                                WebReq.Date = RequestDate;
                            }

                            if (ProxyServer.Length > 0)
                            {
                                WebProxy webProx = new WebProxy(ProxyServer, ProxyPort);
                                WebReq.Proxy = webProx;
                            }

                            if (HeaderCollection.Count > 0)
                            {
                                ICollection keys = HeaderCollection.Keys;
                                foreach (string key in keys)
                                {
                                    WebReq.Headers.Add(key, HeaderCollection[key].ToString());
                                }
                            }

                            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
                            {
                                CredentialCache CreadCache = new CredentialCache();
                                CreadCache.Add(new Uri(uri), "Basic", new NetworkCredential(UserName, Password));
                                WebReq.Credentials = CreadCache;
                            }    
                       
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            HeaderCollection.Clear();
                        }
                    }                
                    else throw new WebException("No Request Method");
            }
            else throw new WebException("No Request URI");
            return WebReq;
        }

        protected virtual string SendHttpRequest(HttpWebRequest WebReq, string HttpRequest)
        {          
            if(RequestMethod == "POST")
            {
                byte[] buffer = Encoding.UTF8.GetBytes(HttpRequest); // Default
                switch (EcodingEnum)
                {
                    case EncodingType.UTF8:
                        buffer = Encoding.UTF8.GetBytes(HttpRequest);
                        break;
                    case EncodingType.ASCII:
                        buffer = Encoding.ASCII.GetBytes(HttpRequest);
                        break;
                    case EncodingType.BigEndianUnicode:
                        buffer = Encoding.BigEndianUnicode.GetBytes(HttpRequest);
                        break;
                    case EncodingType.Unicode:
                        buffer = Encoding.Unicode.GetBytes(HttpRequest);
                        break;
                    case EncodingType.UTF32:
                        buffer = Encoding.UTF32.GetBytes(HttpRequest);
                        break;
                    case EncodingType.UTF7:
                        buffer = Encoding.UTF7.GetBytes(HttpRequest);
                        break;
                }
                WebReq.ContentLength = buffer.Length;

                Stream PostData = WebReq.GetRequestStream();

                PostData.Write(buffer, 0, buffer.Length);

                PostData.Close();
            }
            return GetHttpRespond(WebReq);
        }

        private string GetHttpRespond(HttpWebRequest WebReq)
        {

            try
            {
                WebResponse webresponse = WebReq.GetResponse();

                Encoding encode = Encoding.GetEncoding(1252);

                Stream HttpAnswer = webresponse.GetResponseStream();

                StreamReader read = new StreamReader(HttpAnswer, encode);

                responds = read.ReadToEnd();

                read.Close();

                webresponse.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                WebReq = null;
            }
            return responds;
        }
        public void Dispose()
        {

        }
        protected string XmlGetElementContentAsString(string Xml,string Element)
        {
                StringBuilder returnstr = new StringBuilder();
                using (XmlReader reader = XmlReader.Create(new StringReader(Xml)))
                {
                    try
                    {
                        reader.ReadToFollowing(Element);
                        returnstr.AppendLine(reader.ReadElementContentAsString());
                    }
                    catch
                    {
                        throw new InvalidOperationException("Element not found");
                    }                   
                }        
            return returnstr.ToString();
        }

        protected T ConvertXMLIntoObject<T>(string XML)
        {
            try
            {
                string newString = XML.Replace(" xmlns=\"http://tempuri.org/\"", string.Empty);
                byte[] byteArray = Encoding.UTF8.GetBytes(newString);
                MemoryStream stream = new MemoryStream(byteArray);
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                T i = (T)serializer.Deserialize(stream);
                return i;
            }
            catch
            {
                throw new InvalidOperationException("Xml is Invalid");
            }
        }
             
        protected T ConvertJsonIntoObject<T>(string JSON)
        {
            var table = JsonConvert.DeserializeObject<T>(JSON);
            return table;
        }

        protected DataTable ConvertJsonIntoDataTable(string JSON)
        {
            DataTable table = (DataTable)JsonConvert.DeserializeObject(JSON, (typeof(DataTable)));
            return table;
        }

        protected DataSet ConvertJsonIntoDataSet(string JSON)
        {
            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(JSON);
            return dataSet;
        }

        protected dynamic ConvertJsonLinqIntoDynamic(string JSON)
        {
            dynamic oDynamic = JArray.Parse(JSON);
            return oDynamic;
        }

        protected string ParseJsonToString(string JSON, string Element)
        {
            try
            {
                JObject objec = JObject.Parse(JSON);
                return objec[Element].ToString();
            }
            catch
            {
                throw new JsonReaderException("Element to parse not found");
            }  
        }

        protected Dictionary<object, object> JsonConvertToDictionary(string JSON, object[] ListSearchKey)
        {
            try
            {
                Dictionary<object, object> OutListOfParseJson = new Dictionary<object, object>();
                var InputListOfParseJson = ConvertJsonIntoObject<Dictionary<object, object>>(JSON);
                foreach (object getList in ListSearchKey)
                {
                    OutListOfParseJson.Add(getList, InputListOfParseJson[getList]);
                }
                return OutListOfParseJson;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally { }
        }
        
        protected Dictionary<object, object> XmlConvertToDictionary(string Xml, object[] ListSearchKey)
        {
            Dictionary<object, object> OutListOfParseJson = new Dictionary<object, object>();
            foreach (object getList in ListSearchKey)
            {
                OutListOfParseJson.Add(getList, XmlGetElementContentAsString(Xml,getList.ToString()));
            }
            return OutListOfParseJson;
        }

        protected void WriteToFile(string Path, string FileName)
        {
            try
            {
                using (StreamWriter ResponseWriter = new StreamWriter(Path + "/" +FileName + ".txt", true))
                {
                    ResponseWriter.WriteLine(responds);
                }
            }
            catch
            {
                throw new IOException("Invalid file Path");
            }
        }

        protected XDocument XmlResponseAsXDocument(string XML)
        {
            TextReader tr = new StringReader(XML);
            XDocument doc = XDocument.Load(tr);
            return doc;
        }
    }
}