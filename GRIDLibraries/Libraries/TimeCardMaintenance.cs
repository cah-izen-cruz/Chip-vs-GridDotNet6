using System;
using System.Data;
using System.Data.SqlClient;


namespace GRIDLibraries.Libraries
{
    partial class GridLib
    {
        #region "Get Function"
        public int TMGetRequiredAgent(int _teamId)
        {


            int temp = 0;

            if (this.OpenMainDbConnection())
            {

                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@TeamId", _teamId);
                this.gridMainDbCommand.Parameters.AddWithValue("@Role", 0);
                this.gridMainDbCommand.Parameters.AddWithValue("@Status", true);
                this.gridMainDbCommand.CommandText = "SELECT Count(*) AS ctr FROM tblUserInfo WITH (NOLOCK) WHERE [TeamId]=@TeamId AND [Role]=@Role AND [Status]=@Status;";

                try
                {

                    SqlDataReader dr = this.gridMainDbCommand.ExecuteReader();
                    if (dr.Read())
                        temp = dr.GetInt32("ctr");
                    dr.Close();
                }


                catch (Exception)
                {

                }

                this.CloseMainDbConnection();

            }


            return temp;


        }

        #endregion
    }
}
