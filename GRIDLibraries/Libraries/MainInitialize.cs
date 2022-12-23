using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Data.OleDb;
using Microsoft.VisualBasic;
using System.Windows;
using System.Threading;
using System.Reflection;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;

namespace GRIDLibraries.Libraries
{
    
    partial class GridLib
    {
        Mutex grdMutexx = new Mutex();
   

        #region "ActivityList"
        public List<gridActivity> MIGetActivityList(int TeamId, int GlobalTeamId)
        {
            var tempActList = new List<gridActivity>();

            string strActivityId = "";
            int i;

            var dtActivity = new DataTable();
            var dtConfig = new DataTable();
            var dtConfigItem = new DataTable();


            if (this.OpenDbConnection() == true)
            {

                string sql_NewQuery = "";
                var da = new SqlDataAdapter();
                //sql_NewQuery = "SELECT A.Id,A.TeamId,A.Name as [Name],A.AHT, B.Name as [Type], A.Process,A.LOBId,A.ConfigId, A.IsPublic, A.[Status] from tblActivity A inner join tblActivityType B on A.Type = B.Id Where A.TeamId = '" + TeamId + "' AND A.[Status]=1";
                sql_NewQuery = "SELECT * from tblActivity Where TeamId = '" + TeamId + "' AND [Status]=1";

                da = new SqlDataAdapter(sql_NewQuery, gridDbConnection);

                dtActivity = new DataTable();
                da.SelectCommand.CommandTimeout = 1000;
                try
                {
                    da.Fill(dtActivity);
                }
                catch (Exception)
                {
                }

                this.CloseDbConnection();
            }


            if (!(dtActivity == null))
            {

                if (dtActivity.Rows.Count > 0)
                {
                    var loopTo = dtActivity.Rows.Count - 1;

                    for (i = 0; i <= loopTo; i++)
                    {
                        var hActivity = new gridActivity();

                        if (string.IsNullOrEmpty(strActivityId))
                        {
                            switch (dtActivity.Rows[i]["ConfigId"])
                            {
                                case 0:
                                    break;
                                case >0:
                                    dtActivity.Rows[i]["Id"].ToString();
                                    break;
                                default:
                                    dtActivity.Rows[i]["Id"].ToString();
                                    break;
                            }
                            strActivityId = "([ActivityId]=" + dtActivity.Rows[i]["Id"].ToString();
                            //strActivityId = Conversions.ToString(Operators.ConcatenateObject("([ActivityId]=", (izen > 0), dtActivity.Rows(i).Item("ConfigId") : dtActivity.Rows(i).Item("Id")));
                        }
                        else
                        {
                            strActivityId = strActivityId + " OR [ActivityId]=" + dtActivity.Rows[i]["Id"].ToString();
                            //strActivityId = Conversions.ToString(Operators.ConcatenateObject(strActivityId + " OR [ActivityId]=", dtActivity.Rows(i).Item("ConfigId") > 0 ? dtActivity.Rows(i).Item("ConfigId") : dtActivity.Rows(i).Item("Id")));
                        }


                    }


                    if (!string.IsNullOrEmpty(strActivityId))
                    {

                        strActivityId = strActivityId + ")";

                        if (this.OpenDbConnection())
                        {

                            var da = new SqlDataAdapter();
                            da = new SqlDataAdapter("SELECT * FROM tblPerfConfig WHERE " + strActivityId + " ORDER BY [Sequence];", gridDbConnection);
                            //da = new SqlDataAdapter("SELECT * FROM tblActivityConfig WHERE " + strActivityId + " ORDER BY [Sequence];", gridDbConnection);
                            //da = new SqlDataAdapter("SELECT A.Id, A.ActivityId, A.FieldName, A.WithItem, A.IsRequired, C.Name AS ObjectType, A.Status, B.Name AS DataType, A.Sequence, A.[Desc] FROM  dbo.tblActivityConfig AS A WITH (NOLOCK) INNER JOIN    dbo.tblActivityConfigDataType AS B WITH (NOLOCK) ON A.DataType = B.Id INNER JOIN dbo.tblActivityConfigObjType AS C WITH (NOLOCK) ON A.ObjectType = C.Id WHERE  " + strActivityId + " AND (A.Status = 1) ORDER BY A.Sequence", gridDbConnection);

                            //
                            dtConfig = new DataTable();
                            da.SelectCommand.CommandTimeout = 1000;
                            try
                            {
                                da.Fill(dtConfig);
                            }
                            catch (Exception)
                            {
                            }
                            this.CloseDbConnection();
                        }

                    }

                }

            }


            if (!(dtConfig == null))
            {

                if (dtConfig.Rows.Count > 0)
                {
                    string strConfigId = "";

                    var loopTo1 = dtConfig.Rows.Count - 1;

                    for (i = 0; i <= loopTo1; i++)
                    {
                        switch (dtConfig.Rows[i]["WithItem"])
                        {
                            case true:
                                if (string.IsNullOrEmpty(strConfigId))                            
                                    strConfigId = "([PerfConfigId]=" + dtConfig.Rows[i]["Id"];
                                else
                                    strConfigId = strConfigId + " OR [PerfConfigId]=" + dtConfig.Rows[i]["Id"];
                                break;

                            case false:
                                break;                           
                        }
                    }


                    if (!string.IsNullOrEmpty(strConfigId))
                    {
                        strConfigId = strConfigId + ")";

                        if (this.OpenDbConnection())
                        {
                            var da = new SqlDataAdapter();
                            //da = new SqlDataAdapter("SELECT * FROM tblActivityConfigItem WHERE " + strConfigId + " ORDER BY Id;", gridDbConnection);
                            da = new SqlDataAdapter("SELECT * FROM tblPerfConfigItem WHERE " + strConfigId + " ORDER BY Id;", gridDbConnection);

                            dtConfigItem = new DataTable();
                            da.SelectCommand.CommandTimeout = 1000;
                            try
                            {
                                da.Fill(dtConfigItem);
                            }
                            catch (Exception)
                            {
                            }
                            this.CloseDbConnection();
                        }
                    }
                }
            }

            if (!(dtActivity == null))
            {
                if (dtActivity.Rows.Count > 0)
                {
                    var loopTo2 = dtActivity.Rows.Count - 1;
                    for (i = 0; i <= loopTo2; i++)
                    {
                        var hActivity = new gridActivity();

                        try
                        { 
                            hActivity.Id = (int)dtActivity.Rows[i]["Id"];
                            hActivity.TeamId = TeamId;
                            hActivity.ActName = (string)dtActivity.Rows[i]["ActName"];
                            //hActivity.ActName = (string)dtActivity.Rows[i]["Name"];
                            hActivity.Type = (string)dtActivity.Rows[i]["Type"];
                            hActivity.AHT = (string)dtActivity.Rows[i]["AHT"];
                            hActivity.Process = (string)dtActivity.Rows[i]["Process"];
                            hActivity.LOBId = (int)dtActivity.Rows[i]["LOBId"];
                            hActivity.ConfigId = Convert.ToInt32(dtActivity.Rows[i]["ConfigId"]);

                            hActivity.ConfigInfo=null;

                            tempActList.Add(hActivity);
                        }


                        catch (Exception)
                        {

                        }


                    }



                }
            }



            if (!(tempActList == null))
            {
                if (tempActList.Count > 0)
                {
                    if (!(dtConfig == null))
                    {
                        if (dtConfig.Rows.Count > 0)
                        {
                            var loopTo3 = tempActList.Count - 1;
                            for (i = 0; i <= loopTo3; i++)
                            {

                                var PerfConfig = new List<PerfConfig>();

                                DataRow[] tempConfig = dtConfig.Select("ActivityId=" + tempActList[i].Id, "Sequence"); //Conversions.ToInteger(tempActList[i].ConfigId > 0 ? tempActList[i].ConfigId :

                            foreach (DataRow Config in tempConfig)
                                {

                                    var retListItem = new PerfConfig();


                                    retListItem.ActivityId = tempActList[i].Id;
                                    retListItem.Id = Convert.ToInt32(Config["Id"]);
                                    retListItem.FieldName = (string)Config["FieldName"];
                                    retListItem.DataType = (string)Config["DataType"];
                                    retListItem.ObjectType = (string)Config["ObjectType"].ToString();
                                    retListItem.IsRequired = (bool)Config["IsRequired"];
                                    retListItem.WithItem = (bool)Config["WithItem"];
                                    retListItem.Status = (bool)Config["Status"];
                                    retListItem.UseCurrentDate = false;
                                    retListItem.Desc = "";

                                    //try
                                    //{
                                    //    retListItem.UseCurrentDate = (bool)Config["UseCurrentDate"];
                                    //}
                                    //catch (Exception)
                                    //{
                                    //}

                                    try
                                    {
                                        retListItem.Desc = (string)Config["Desc"];
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    try
                                    {
                                        retListItem.Sequence = (int)Config["Sequence"];
                                    }
                                    catch (Exception)
                                    {
                                        retListItem.Sequence = (int)Config["Id"];
                                    }

                                    if ((bool)Config["WithItem"] == true)
                                    {

                                        var PerfConfigIdList = new List<gridPerfConfigItem>();

                                        //DataRow[] tempConfigItem = dtConfigItem.Select("ActConfigId=" + retListItem.Id, "Id");
                                        DataRow[] tempConfigItem = dtConfigItem.Select("PerfConfigId=" + retListItem.Id, "Id");

                                        foreach (DataRow cItem in tempConfigItem)
                                        {

                                            var PerfConfigIdListItm = new gridPerfConfigItem();

                                            PerfConfigIdListItm.Id = Convert.ToInt32(cItem["Id"]);
                                            //PerfConfigIdListItm.PerfConfigId = Convert.ToInt32(cItem["ActConfigId"]);
                                            PerfConfigIdListItm.PerfConfigId = Convert.ToInt32(cItem["PerfConfigId"]);
                                            PerfConfigIdListItm.Item = (string)cItem["Item"];

                                            PerfConfigIdList.Add(PerfConfigIdListItm);

                                        }

                                        retListItem.ConfigItem = PerfConfigIdList;

                                    }

                                    PerfConfig.Add(retListItem);

                                }



                                tempActList[i].ConfigInfo = PerfConfig;

                            }


                        }
                    }
                }
            }



            return tempActList;
        }
        #endregion

        #region "QAQuestionList"

        public void MIPopulateQAList()
        {
            //var lstQuestions = new List<QAQuestionnaire>();

            //lstQuestions = this.MIGetQAList();

            //this.grdData.QALists = new List<QAQuestionnaire>();
            //this.grdData.QALists = lstQuestions;
        }
        #endregion

        public void MIPopulateActList()
        {
            var lstActivities = new List<gridActivity>();

            lstActivities = this.MIGetActivityList(this.grdData.TeamInfo.Id, this.grdData.TeamInfo.GlobalTeamId);

            this.grdData.ActivityList = new List<gridActivity>();
            this.grdData.ActivityList = lstActivities;
        }

        public bool MICurrentActivitiesToOpeMain(string UserId)
        {

            bool temp = false;

            if (this.OpenDbConnection())
            {

                this.gridDbCommand.Parameters.Clear();
                this.gridDbCommand.Parameters.AddWithValue("@UserId", UserId);
                this.gridDbCommand.Parameters.AddWithValue("@Status", "0");

                this.gridDbCommand.CommandText = "UPDATE tblPerformance Set Status='1' WHERE ([UserId]=@UserId) AND ([Status]=@Status);";

                try
                {
                    if (this.gridDbCommand.ExecuteNonQuery() > 0)
                        temp = true;
                }
                catch (Exception ex)
                {

                }

                this.CloseDbConnection();

            }


            return temp;




        }

        public bool MISavePerfToLocal()
        {

            var lstPerformances = new List<gridPerformance>();

            lstPerformances = this.MICopyPerfFromMain(this.grdData.CurrentUser.EmpNo, this.grdData.CurrentUser.TransactionDate);

            this.grdData.PerformanceList = new List<gridPerformance>();
            this.grdData.PerformanceList = lstPerformances;


            return true;


        }




    }
}
