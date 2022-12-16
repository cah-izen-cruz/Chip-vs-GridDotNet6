using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace GRIDLibraries.Libraries
{

    partial class GridLib
    {

        public List<QAQuestionForm> GetQAFormList()
        {
            var tempQAList = new List<QAQuestionForm>();

            if (this.OpenMainAHSQAConnection())
            {
                SqlDataReader dr;
                this.gridMainDbCommand.CommandText = "SELECT [ID], [NAME], CASE [FORMULA] WHEN 1 THEN 'SUM' ELSE 'AVERAGE' END AS FORMULA, [TARGET] FROM [dbo].[tblQAForm] ORDER BY [NAME];";

                dr = this.gridMainDbCommand.ExecuteReader();

                while (dr.Read())
                    tempQAList.Add(new QAQuestionForm() { LOBId = (int)dr["Id"], Name = (string)dr["Name"], Formula = (string)dr["FORMULA"], Target = Convert.ToInt32(dr["Target"]) });
                dr.Close();
                this.CloseMainDbConnection();
            }


            return tempQAList;

        }

        public List<QAQuestionForm> GetEmployee(string _EmpName)
        {
            var tempAgentList = new List<QAQuestionForm>();

            if (this.OpenMainAHSQAConnection())
            {
                SqlDataReader dr;
                //this.gridMainDbCommand.CommandText = "SELECT EmpNo, EmpName FROM [dbo].[tblUserInfo] WITH (NOLOCK) WHERE EmpName LIKE '%" + _EmpName + "%' AND [Status]=1  ORDER BY [EmpName];";
                this.gridMainDbCommand.CommandText = "SELECT A.EmpNo AS UserId, A.EmpName, C.EmpNo AS SupEmpNo, C.EmpName As Supervisor FROM[dbo].[tblUserInfo] A WITH(NOLOCK) INNER JOIN[dbo].[tblUserInfo] C ON A.SupEmpNo = C.EmpNo WHERE A.EmpName LIKE '%" + _EmpName + "%' AND A.[Status]=1  ORDER BY A.[EmpName];";

                dr = this.gridMainDbCommand.ExecuteReader();

                while (dr.Read())
                    tempAgentList.Add(new QAQuestionForm() { UserId = Convert.ToInt32(dr["UserId"]), EmpName = (string)dr["EmpName"], SupEmpNo = Convert.ToInt32(dr["SupEmpNo"]), Supervisor = (string)dr["Supervisor"] });
                dr.Close();
                this.CloseMainDbConnection();
            }

            return tempAgentList;
        }



        //public DataTable GetQALob()
        //{
        //    var temp = new DataTable();

        //    var da = new SqlDataAdapter();

        //    da = new SqlDataAdapter("SELECT Id,Name,Formula,Target FROM dbo.[tblQAForm] Order By Id;", conStringAHS_QA);
        //    da.SelectCommand.CommandTimeout = 1000;
        //    try
        //    {
        //        da.Fill(temp);
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    return temp;
        //}


        public DataTable GetQAQuestionnaires()
        {
            var temp = new DataTable();

            var da = new SqlDataAdapter();

            da = new SqlDataAdapter("SELECT DISTINCT Id AS LOBId,Name,FormId,Question,ObjectType,Description,Category FROM [dbo].[vQuery_QA_Questionnaire];", conStringAHS_QA);
            da.SelectCommand.CommandTimeout = 1000;
            try
            {
                da.Fill(temp);
            }
            catch (Exception ex)
            {
            }

            return temp;
        }

        public DataTable GetQASelection()
        {
            var temp = new DataTable();
            var da = new SqlDataAdapter();

            da = new SqlDataAdapter("SELECT * FROM[dbo].[vQuery_QA_Questionnaire];", conStringAHS_QA);
            da.SelectCommand.CommandTimeout = 1000;
            try
            {
                da.Fill(temp);
            }
            catch (Exception ex)
            {
            }
            return temp;
        }


        public DataTable GetQAContainers()
        {
            var temp = new DataTable();
            var da = new SqlDataAdapter();

            da = new SqlDataAdapter("SELECT * FROM[dbo].[vQuery_QA_Container];", conStringAHS_QA);
            da.SelectCommand.CommandTimeout = 1000;
            try
            {
                da.Fill(temp);
            }
            catch (Exception ex)
            {
            }
            return temp;
        }

        public DataTable GetQAMarkdown()
        {
            var temp = new DataTable();
            var da = new SqlDataAdapter();

            da = new SqlDataAdapter("SELECT * FROM [dbo].[vQuery_QA_Markdown];", conStringAHS_QA);
            da.SelectCommand.CommandTimeout = 1000;
            try
            {
                da.Fill(temp);
            }
            catch (Exception ex)
            {
            }
            return temp;
        }

        public bool AddQAForm(string _OALOBName, int _Formula, int _Target)
        {

            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {


                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@Name", _OALOBName);
                this.gridMainDbCommand.Parameters.AddWithValue("@Formula", _Formula);
                this.gridMainDbCommand.Parameters.AddWithValue("@Target", _Target);
                this.gridMainDbCommand.Parameters.AddWithValue("@ModifiedBy", this.grdData.CurrentUser.EID);
                this.gridMainDbCommand.Parameters.AddWithValue("@ModifiedDate", this.ConvertTimeZone(this.grdData.TeamInfo.OffSet));
                this.gridMainDbCommand.Parameters.AddWithValue("@Status", true);

                this.gridMainDbCommand.CommandText = "INSERT INTO [dbo].[tblQAForm] ([Name],[Formula],[Target],[ModifiedBy],[ModifiedDate],[Status]) VALUES (@Name,@Formula,@Target,@ModifiedBy,@ModifiedDate,@Status);";

                try
                {
                    if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                        temp = true;
                }
                catch (Exception)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();

            return temp;

        }

        public bool SaveQAItemsSCOPE_IDENTITY(DateTime _InteractionDate, int _Owner, string _Workgroup)
        {

            bool temp = false;
            this.grdMutexx.WaitOne();
       
            if (this.OpenMainAHSQAConnection())
            {
                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@InteractionDate", Strings.Format(_InteractionDate, "MM/dd/yyyy").ToString()); //Strings.Format(TransDate, "MM/dd/yyyy").ToString()
                this.gridMainDbCommand.Parameters.AddWithValue("@Owner", _Owner);
                this.gridMainDbCommand.Parameters.AddWithValue("@Workgroup", _Workgroup);
                this.gridMainDbCommand.CommandText = "INSERT INTO [dbo].[tblQAItem] ([InteractionDate],[Owner],[Workgroup]) VALUES (@InteractionDate,@Owner,@Workgroup);" + "SELECT IDENT_CURRENT('[dbo].[tblQAItem]')";
                    
                try
                {
                    //if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                    //temp = true;
                    var izen = this.gridMainDbCommand.ExecuteScalar();
                    grdData.QuestionForm.ItemId = Convert.ToInt32(izen.ToString());

                }
                catch (Exception)
                {
                }
                this.CloseMainDbConnection();
            }


          

            this.grdMutexx.ReleaseMutex();

            return temp;

        }

        public bool SaveQAScores(int _ItemId, int _FormId, string _FormName, string _Category, int _QID, string _Question, int _Score, string _Markdown, string _Remarks)
        {

            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@ItemId", _ItemId); //-- From Account Tab
                this.gridMainDbCommand.Parameters.AddWithValue("@FormId", _FormId); //---QA FORM ID
                this.gridMainDbCommand.Parameters.AddWithValue("@FormName", _FormName); //---QA FORM NAME
                this.gridMainDbCommand.Parameters.AddWithValue("@Category", _Category);
                this.gridMainDbCommand.Parameters.AddWithValue("@QuestionId", _QID);
                this.gridMainDbCommand.Parameters.AddWithValue("@Question", _Question);
                this.gridMainDbCommand.Parameters.AddWithValue("@Score", _Score);
                this.gridMainDbCommand.Parameters.AddWithValue("@Markdown", _Markdown);
                this.gridMainDbCommand.Parameters.AddWithValue("@Remarks", _Remarks);

                this.gridMainDbCommand.CommandText = "INSERT INTO [dbo].[tblQAScore] ([ItemId],[FormId],[FormName],[Category],[QuestionId],[Question],[Score],[Markdown],[Remarks]) "
                     + "VALUES (@ItemId,@FormId,@FormName,@Category,@QuestionId,@Question,@Score,@Markdown,@Remarks);";

                try
                {
                    if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                        temp = true;

                }
                catch (Exception)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();

            return temp;

        }

        public bool SaveQAAttachments(int _ItemId, byte[] _SSFileName)
        {

            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@ItemId", _ItemId);
                this.gridMainDbCommand.Parameters.AddWithValue("@Image", _SSFileName);
                this.gridMainDbCommand.CommandText = "INSERT INTO [dbo].[tblQAScreenShot] ([ItemId],[Image]) VALUES (@ItemId,@Image);";

                try
                {
                    if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                        temp = true;
                }
                catch (Exception)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();

            return temp;

        }  

        public bool GetQASumFormula(int _Id)
        {
            bool temp = false;
            this.grdMutexx.WaitOne();
            if (this.OpenMainAHSQAConnection())
            {

                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@Id", _Id);
                this.gridMainDbCommand.CommandText = "SELECT FORMULA,TARGET FROM [dbo].[tblQAForm] WHERE Id=@Id;";
                try
                {
                    SqlDataReader dr = this.gridMainDbCommand.ExecuteReader();
                    while (dr.Read())
                    {
                        this.grdData.QuestionForm.Formula = dr["Formula"].ToString();
                        this.grdData.QuestionForm.Target = Convert.ToInt32(dr["Target"].ToString());
                        temp = true;
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
                }
                this.CloseMainDbConnection();

            }
            this.grdMutexx.ReleaseMutex();
            return temp;
        }

        [Obsolete]
        public bool UpdateQAForm(int _Id, string _OALOBName, int _Formula, int _Target)
        {

            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                try
                {
                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.Parameters.AddWithValue("@Id", _Id);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Name", _OALOBName);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Formula", _Formula);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Target", _Target);
                    this.gridMainDbCommand.Parameters.AddWithValue("@ModifiedBy", this.grdData.CurrentUser.EID);
                    this.gridMainDbCommand.Parameters.AddWithValue("@ModifiedDate", this.ConvertTimeZone(grdData.TeamInfo.OffSet));
                    this.gridMainDbCommand.Parameters.AddWithValue("@Status", true);

                    this.gridMainDbCommand.CommandText = "UPDATE [dbo].[tblQAForm] SET [Name]=@Name,[Formula]=@Formula,[Target]=@Target," +
                                "[ModifiedBy]=@ModifiedBy,[ModifiedDate]=ModifiedDate,[Status]=@Status WHERE [Id]=@Id;";

                    try
                    {
                        if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                            temp = true;
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (Exception ex)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();

            return temp;

        }

        public bool AddQATemplate(int _FormId, string _Category, string _QAQuestion, string _QAObjType, string _QADescription, DataTable dt1, DataTable dt2)
        {
            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                this.gridMainDbCommand.CommandText = "SELECT COALESCE(MAX(FormId)+1,1) AS MAXQID FROM [dbo].[tblQAQuestions]";
                try
                {
                    SqlDataReader dr = this.gridMainDbCommand.ExecuteReader();
                    while (dr.Read())
                    {
                        this.grdData.QuestionForm.MaxQID = Convert.ToInt32(dr["MAXQID"]);
                    }
                    dr.Close();

                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.Parameters.AddWithValue("@Id", this.grdData.QuestionForm.LOBId);
                    this.gridMainDbCommand.Parameters.AddWithValue("@FormId", this.grdData.QuestionForm.MaxQID);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Category", _Category);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Question", _QAQuestion);
                    this.gridMainDbCommand.Parameters.AddWithValue("@ObjType", _QAObjType);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Description", _QADescription);
                    this.gridMainDbCommand.Parameters.AddWithValue("@ModifiedBy", this.grdData.CurrentUser.EID);
                    this.gridMainDbCommand.Parameters.AddWithValue("@ModifiedDate", this.ConvertTimeZone(grdData.TeamInfo.OffSet));
                    this.gridMainDbCommand.Parameters.AddWithValue("@Status", true);

                    this.gridMainDbCommand.CommandText = "INSERT INTO [dbo].[tblQAQuestions] ([Id],[FormId],[Category],[Question],[ObjectType],[Description],[ModifiedBy],[ModifiedDate],[Status])" +
                                " VALUES (@Id,@FormId,@Category,@Question,@ObjType,@Description,@ModifiedBy,@ModifiedDate,@Status);";
                    try
                    {
                        if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                            temp = true;
                    }
                    catch (Exception)
                    {
                    }

                    foreach (DataRow row in dt1.Rows)
                    {
                        this.grdData.QuestionForm.SelectionValue = row["Value"].ToString();
                        this.grdData.QuestionForm.Score = Convert.ToInt32(row["Score"].ToString());

                        this.gridMainDbCommand.CommandText = "INSERT INTO [dbo].[tblQAScoring] ([QID],[Value],[Score])" +
                                        " VALUES ('" + this.grdData.QuestionForm.MaxQID + "','" + this.grdData.QuestionForm.SelectionValue + "','" + this.grdData.QuestionForm.Score + "');";
                        try
                        {
                            this.gridMainDbCommand.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    foreach (DataRow row2 in dt2.Rows)
                    {
                        this.grdData.QuestionForm.MarkdownType = row2["Value"].ToString();

                        this.gridMainDbCommand.CommandText = "INSERT INTO [dbo].[tblQAMarkdown] ([QID],[Value])" +
                                        " VALUES ('" + this.grdData.QuestionForm.MaxQID + "','" + this.grdData.QuestionForm.MarkdownType + "');";
                        try
                        {
                            this.gridMainDbCommand.ExecuteNonQuery();
                        }
                        catch (Exception)
                        {
                        }
                    }

                }
                catch (Exception ex)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();

            return temp;
        }

        public bool UpdateQATemplate(int _LOBId, string _Category, string _QAQuestion, string _QAObjType, string _QADescription)
        {
            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                try
                {
                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.Parameters.AddWithValue("@Id", this.grdData.QuestionForm.LOBId);
                    this.gridMainDbCommand.Parameters.AddWithValue("@FormId", this.grdData.QuestionForm.QID);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Category", _Category);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Question", _QAQuestion);
                    this.gridMainDbCommand.Parameters.AddWithValue("@ObjType", _QAObjType);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Description", _QADescription);
                    this.gridMainDbCommand.Parameters.AddWithValue("@ModifiedBy", this.grdData.CurrentUser.EID);
                    this.gridMainDbCommand.Parameters.AddWithValue("@ModifiedDate", this.ConvertTimeZone(grdData.TeamInfo.OffSet));
                    this.gridMainDbCommand.Parameters.AddWithValue("@Status", true);

                    this.gridMainDbCommand.CommandText = "UPDATE [dbo].[tblQAQuestions] SET [Category]=@Category,[Question]=@Question,[ObjectType]=@ObjType," +
                                "[Description]=@Description,[ModifiedBy]=@ModifiedBy,[ModifiedDate]=ModifiedDate,[Status]=@Status WHERE [Id]=@Id AND FormId=@FormId;";

                    try
                    {
                        if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                            temp = true;
                    }
                    catch (Exception)
                    {
                    }
                }
                catch (Exception ex)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();

            return temp;

        }

        public bool AddScore(int _QID, string _SValue, int _SScore)
        {
            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                try
                {
                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.Parameters.AddWithValue("@QID", _QID);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Value", _SValue);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Score", Strings.Trim(_SScore.ToString()));

                    this.gridMainDbCommand.CommandText = "INSERT INTO [dbo].[tblQAScoring] ([QID],[Value],[Score])" +
                                    " VALUES (@QID,@Value,@Score);";
                    try
                    {
                        this.gridMainDbCommand.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                    }

                }
                catch (Exception)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();
            return temp;
        }

        public bool AddMarkdown(int _QID, string _MValue)
        {
            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                try
                {
                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.Parameters.AddWithValue("@QID", _QID);
                    this.gridMainDbCommand.Parameters.AddWithValue("@Value", _MValue);

                    this.gridMainDbCommand.CommandText = "INSERT INTO [dbo].[tblQAMarkdown] ([QID],[Value])" +
                                    " VALUES (@QID,@Value);";
                    try
                    {
                        this.gridMainDbCommand.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                    }

                }
                catch (Exception)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();
            return temp;
        }

        public bool DeleteScore(int _Id)
        {
            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                try
                {
                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.Parameters.AddWithValue("@Id", _Id);
                    this.gridMainDbCommand.CommandText = "DELETE FROM [dbo].[tblQAScoring] WHERE [Id]=@Id";
                    if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                        temp = true;

                }
                catch (Exception)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();
            return temp;
        }

        public bool DeleteMarkdown(int _Id)
        {
            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                try
                {
                    this.gridMainDbCommand.Parameters.Clear();
                    this.gridMainDbCommand.Parameters.AddWithValue("@Id", _Id);
                    this.gridMainDbCommand.CommandText = "DELETE FROM [dbo].[tblQAMarkdown] WHERE [Id]=@Id";
                    if (this.gridMainDbCommand.ExecuteNonQuery() > 0)
                        temp = true;

                }
                catch (Exception)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();
            return temp;
        }

        public bool DeleteQATemplate(int _QID)
        {
            bool temp = false;
            this.grdMutexx.WaitOne();

            if (this.OpenMainAHSQAConnection())
            {
                try
                {
                    this.gridMainDbCommand = new SqlCommand("sp_QA_DelTemplate", gridMainDbConnection);
                    this.gridMainDbCommand.CommandType = CommandType.StoredProcedure;
                    this.gridMainDbCommand.Parameters.AddWithValue("@QID", _QID);
                    this.gridMainDbCommand.ExecuteNonQuery();
                }
                catch (Exception)
                {
                }
                this.CloseMainDbConnection();
            }
            this.grdMutexx.ReleaseMutex();
            return temp;
        }
    }
}




