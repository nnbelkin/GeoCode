using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using psgSQLHelper;
using System.Data.SqlClient;
using System.Data;

namespace SQLBIProjects
{
    class invoices : psgSQLHelper.Configuration
    {


        #region Invoices
        static void getInvoices()
        {
            Int32 Comp_Id = 22428; //TripAdvizor 

            DateTime InvoiceDate = getLastInvoiceDate();
            DirectoryInfo InvDir = getInvoiceFolder(InvoiceDate);
            String Dir_From = InvDir.FullName;
            List<String> InvoiceFiles = getInvoiceFiles(Comp_Id, InvoiceDate, Dir_From);

            String SqlSrvName = System.Environment.MachineName;
            if (SqlSrvName.Contains("DEV")) { SqlSrvName = "BOSPSGSQL5"; }
            String Dir_To = @"\\" + SqlSrvName + @"\TS2\Electronic Invoice\TripAdvisor";//\" + InvDir.Name;

            getFileCopy(Dir_To, Dir_From, InvoiceFiles);
            //getZip(Dir_Zip, InvDir.Name, InvoiceFiles); 

        }
        #endregion

        #region Load Files

        static Boolean getFileCopy(String lDir_To, String lDir_From, List<String> lInvoiceFiles)
        {
            foreach (String InvoiceFile in lInvoiceFiles)
            {

                try
                {
                    String Path_To = lDir_To + @"\" + InvoiceFile;
                    String Path_From = lDir_From + @"\" + InvoiceFile;
                    if (File.Exists(Path_To))
                    { File.Delete(Path_To); }
                    File.Copy(Path_From, Path_To);
                }

                finally
                {

                }


            }
            return true;
        }

        static Boolean getZip(String lDir_Zip, String lInvoiceDate, List<String> lInvoiceFiles)
        {
            try
            {
                String ZipFileName = lDir_Zip + @"\PSG_" + lInvoiceDate + "_TripAdvizor.zip";
                FileStream destFile = File.Create(ZipFileName);
                GZipStream compStream = new GZipStream(destFile, CompressionMode.Compress);
                foreach (String InvoiceFile in lInvoiceFiles)
                {
                    FileStream sourceFile = File.OpenRead(InvoiceFile);

                    try
                    {
                        Int32 theByte = sourceFile.ReadByte();
                        while (theByte != -1)
                        {
                            compStream.WriteByte((Byte)theByte);
                            theByte = sourceFile.ReadByte();
                        }
                    }

                    finally
                    {

                    }
                }

                compStream.Dispose();

                return true;
            }
            catch (Exception e)
            {
                throw e;


            }
            finally
            {
            }
        }


        #endregion

        #region SQL Helper

        #region Get Invoices

        static List<String> getInvoiceFiles(Int32 lComp_Id, DateTime lInvoiceDate, String lDir)
        {

            try
            {
                List<String> InvoiceFiles = new List<String>();
                String sql = "Select * from [dbo].[fn_Comp_Get_Invoice_List](" + lComp_Id + ",'" + lInvoiceDate + "')";
                SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sql);
                do
                {
                    while (dr.Read())
                    {
                        String InvoiceFile = Convert.ToString(dr["InvoiceNo"]) + ".pdf";
                        InvoiceFiles.Add(InvoiceFile);
                    }
                }
                while (dr.NextResult());
                dr.Close();


                return InvoiceFiles;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }

        }

        static DirectoryInfo getInvoiceFolder(DateTime lInvoiceDate)
        {

            try
            {
                String lYear = lInvoiceDate.Year.ToString();
                String lFolderName = lInvoiceDate.ToString("MM/dd/yy").Replace("/", "");
                String lDirName = getFileSrvName() + @"\InvoiceImages\" + lYear + @"\" + lFolderName;
                System.IO.DirectoryInfo dr = new DirectoryInfo(lDirName);
                //return GetNewestDirecroty(dr); 
                return dr;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
        }

        public static DirectoryInfo GetNewestDirecroty(DirectoryInfo directory)
        {
            return directory.GetDirectories()
                .Union(directory.GetDirectories().Select(d => GetNewestDirecroty(d)))
                .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
                .FirstOrDefault();
        }

        public static FileInfo GetNewestFile(DirectoryInfo directory)
        {
            return directory.GetFiles()
                .Union(directory.GetDirectories().Select(d => GetNewestFile(d)))
                .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
                .FirstOrDefault();
        }

        static String getFileSrvName()
        {

            try
            {

                String sql = "select dbo.fn_Get_TypeDescr('Path_FileServer',1)";
                return Convert.ToString(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql));

            }
            catch (Exception e)
            {
                return @"\\bospsgnas4\nasshare\";
                throw e;
            }
            finally
            {

            }
        }
        //[dbo].[fn_Get_LastInvoiceDate]() 
        static DateTime getLastInvoiceDate()
        {

            try
            {

                String sql = "select dbo.fn_Get_LastInvoiceDate()";
                return Convert.ToDateTime(SqlHelper.ExecuteScalar(ConnectionString, CommandType.Text, sql));

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {

            }
        }



        #endregion

        #endregion 
    }
}
