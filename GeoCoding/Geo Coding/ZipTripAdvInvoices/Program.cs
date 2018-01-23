using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization; 
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml; 

using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Collections;

using psgSQLHelper;
using System.Data.SqlClient;

namespace SQLBIProjects
{
    class Program : psgSQLHelper.Configuration
    {
        static void Main(string[] args)
        {
            String _debugmode = "";
            Int32 count = 0; 
            
            DateTime _startdatetime = DateTime.Now;
            String _utype = "Emp";
            geocode g = new geocode(_debugmode, _utype, ref count);
            _utype = "EmpInt";
            g = new geocode(_debugmode, _utype, ref count);
            _utype = "Client";
            g = new geocode(_debugmode, _utype, ref count);
            
            //if (count < 2500)
            //{
            //    _utype = "Emp";
            //    g = new geocode(_debugmode, _utype, ref count);
            //}
            //if (count < 2500)
            //{

            //    _utype = "Client";
            //    g = new geocode(_debugmode, _utype, ref count);
            //}

        }




        #region Send SQL Email

        static Int32 SendSQLEmail(String Subject, String Body)
        {

            try
            {
                //[dbo].[sp_SMTPemail]
                //    (
                //        @From as nvarchar(100),
                //        @To as nvarchar(max),
                //        @Subject as nvarchar(500),
                //        @Body as varchar(max),
                //        @isHTML as tinyint
                //    ) 
                SqlParameter[] param = {  
                        new SqlParameter( "@From", "webmaster@psgstaffing.com"),                        
                        new SqlParameter( "@To", "webmaster@psgstaffing.com" ),                
                        new SqlParameter( "@Subject", Subject),
                        new SqlParameter( "@Body", Body),
                        new SqlParameter( "@isHTML", 1)

                  };




                return Convert.ToInt32(SqlHelper.ExecuteScalar(ConnectionString, "sp_SMTPemail", param));


            }
            catch (Exception e)
            {
                return -1;
                throw e;
            }
            finally
            {

            }
        }

        #endregion

    }

      
}
