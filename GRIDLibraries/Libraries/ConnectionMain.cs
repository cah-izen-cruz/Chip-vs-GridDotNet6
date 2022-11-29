using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using System;

namespace GRIDLibraries.Libraries
{
    partial class GridLib
    {
        public void CloseMainDbConnection()
        {
            this.gridMainDbConnection.Close();
        }

        public bool CheckMainDbConnection()
        {
            this.gridMainDbConnection = new SqlConnection();
            this.gridMainDbCommand = new SqlCommand();
            this.gridMainDbConnection.ConnectionString = conStringMain;

            if(this.gridMainDbConnection.State == ConnectionState.Open)
            {
                this.CloseMainDbConnection();
            }

            try
            {
                this.gridMainDbConnection.Open();
                this.gridMainDbCommand.Connection = this.gridMainDbConnection;

                if(this.gridMainDbConnection.State == ConnectionState.Open)
                {
                    this.gridMainDbConnectionState = true;
                    return true;
                }
                else
                {
                    this.gridMainDbConnectionState = false;
                    return false;
                }

            }
            catch (System.IO.IOException)
            {
                return false;
            }



        }

        public bool OpenMainDbConnection()
        {
            this.gridMainDbConnection = new SqlConnection();
            this.gridMainDbCommand = new SqlCommand();


            this.gridMainDbConnection.ConnectionString = conStringMain;



            // If it's left open, then close it first
            if (this.gridMainDbConnection.State == ConnectionState.Open)
                this.CloseMainDbConnection();

            try
            {
                this.gridMainDbConnection.Open();

                // The COMMAND object uses the same CONNECTION object
                this.gridMainDbCommand.Connection = this.gridMainDbConnection;

                if (this.gridMainDbConnection.State == ConnectionState.Open)
                {
                    // 'Me.hwkLogFile.InfoLog("Successfully connected to DB")
                    this.gridMainDbConnectionState = true;
                    return true;
                }
                else
                {
                    this.gridMainDbConnectionState = false;
                    return false;
                }
            }
            catch (SqlException)
            {

                // Me.hwkLogFile.ErrLog("Failed to connect to DB")
                // Me.hwkLogFile.ErrLog("EXCEPTION_MSG: " & ex.Message)
                return false;
            }

         
        }

        public bool OpenMainAHSQAConnection()
        {
            this.gridMainDbConnection = new SqlConnection();
            this.gridMainDbCommand = new SqlCommand();


            this.gridMainDbConnection.ConnectionString = conStringAHS_QA;



            // If it's left open, then close it first
            if (this.gridMainDbConnection.State == ConnectionState.Open)
                this.CloseMainDbConnection();

            try
            {
                this.gridMainDbConnection.Open();

                // The COMMAND object uses the same CONNECTION object
                this.gridMainDbCommand.Connection = this.gridMainDbConnection;

                if (this.gridMainDbConnection.State == ConnectionState.Open)
                {
                    // 'Me.hwkLogFile.InfoLog("Successfully connected to DB")
                    this.gridMainDbConnectionState = true;
                    return true;
                }
                else
                {
                    this.gridMainDbConnectionState = false;
                    return false;
                }
            }
            catch (SqlException)
            {

                // Me.hwkLogFile.ErrLog("Failed to connect to DB")
                // Me.hwkLogFile.ErrLog("EXCEPTION_MSG: " & ex.Message)
                return false;
            }


        }

        public string InitializeException(string _EID)
        {
            string InitializeExceptionRet = default;


            InitializeExceptionRet = "";

            if (this.OpenMainDbConnection())
            {

                this.gridMainDbCommand.Parameters.Clear();
                this.gridMainDbCommand.Parameters.AddWithValue("@EID", _EID);
                this.gridMainDbCommand.CommandText = "SELECT * FROM tblUserInfo WITH (NOLOCK) WHERE EID=@EID;";
                this.gridMainDbCommand.CommandTimeout = 0;

                SqlDataReader dr;
                dr = this.gridMainDbCommand.ExecuteReader();

                try
                {
                    if (dr.Read())
                    {
                    }
                    else
                    {

                    }
                }
                catch (Exception ex)
                {
                    InitializeExceptionRet = ex.Message;
                }

                try
                {
                    dr.Close();
                }
                catch (Exception ex)
                {
                }

                this.CloseMainDbConnection();

            }

            return InitializeExceptionRet;
        }

    }
}
