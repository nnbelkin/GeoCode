using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLBIProjects
{
    class resume
    {
         #region Resume Copy


        //static void ResumeCopy()
        //{
        //   String srv =  getFileSrvName();
        //   String dir_from = @"PSG Resumes\";
        //   String dir_to = @"PSG Resumes FTPTEST\";
        //   List<String> resumes = getResumes();
        //   Int32 i = 0; 
        //   foreach (String res in resumes)
        //   {
        //       if (File.Exists(srv + dir_to + res) == false)
        //       {
        //           i += 1; 
        //           if (File.Exists(srv + dir_from + res))
        //           {
        //               Console.WriteLine(i + " : " + srv + dir_to + res); 
        //               File.Copy(srv + dir_from + res, srv + dir_to + res);
        //           }
        //       }
        //   }
        
        //}

        //static List<String> getResumes()
        //{

        //    try
        //    {
        //        List<String> Resumes = new List<String>();
        //        String sql = "select Resume, DocName from tblEMPLOYEE_resume where EnteredDate >= dateadd(year, -1, getdate()) and ActiveFlag = 0 and DeleteFlag = 0 ";
        //        SqlDataReader dr = SqlHelper.ExecuteReader(ConnectionString, CommandType.Text, sql);
        //        do
        //        {
        //            while (dr.Read())
        //            {
        //                String res = Convert.ToString(dr["DocName"]);
        //                Resumes.Add(res);
        //            }
        //        }
        //        while (dr.NextResult());
        //        dr.Close();


        //        return Resumes;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    finally
        //    {

        //    }

        //}

        #endregion 
   
    }
}
