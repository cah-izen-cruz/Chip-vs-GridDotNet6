using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using System;
using System.Windows.Controls;

namespace GRIDLibraries.Libraries
{
    partial class GridLib
    { 
        public bool OpenDbConnection3(string conStr)
        {
            this.gridDbConnection3 = new SqlConnection();
            this.gridDbCommand3 = new SqlCommand();


            this.gridDbConnection3.ConnectionString = conStr;

            if (this.gridDbConnection3.State == ConnectionState.Open)
                this.CloseDbConnection3();

            try
            {
                this.gridDbConnection3.Open();

                this.gridDbCommand3.Connection = this.gridDbConnection3;

                if (this.gridDbConnection3.State == ConnectionState.Open)
                {
                    this.gridDbConnectionState3 = true;
                    return true;
                }
                else
                {
                    this.gridDbConnectionState3 = false;
                    return false;
                }
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public void CloseDbConnection3()
        {
            try
            {
                this.gridDbConnection3.Close();
            }
            catch (Exception)
            {
            }
        }

        public bool OpenDbConnection()
        {
            bool OpenDbConnectionRet = default;

            this.gridDbConnection = new SqlConnection();
            this.gridDbCommand = new SqlCommand();

            this.gridDbConnection.ConnectionString = conString;

            if (this.gridDbConnection.State == ConnectionState.Open)
            {
                this.CloseDbConnection();
            }

            try
            {

                this.gridDbConnection.Open();
                this.gridDbCommand.Connection = this.gridDbConnection;
                if (this.gridDbConnection.State == ConnectionState.Open)
                {
                    this.gridDbConnectionState = true;
                    OpenDbConnectionRet = true;
                    return true;
                }
                else
                {
                    this.gridDbConnectionState = false;
                    OpenDbConnectionRet = false;
                    return false;
                }
            }

            catch (SqlException)
            {

                OpenDbConnectionRet = false;
                return false;

            }

            return OpenDbConnectionRet;



        }

        public void CloseDbConnection()
        {
            this.gridDbConnection.Close();
        }



        public bool OpenDbTimecardConnection()
        {
            bool OpenDbTimecardConnectionRet = default;

            this.gridDbTimecardConnection = new SqlConnection();
            this.gridDbTimecardCommand = new SqlCommand();

            this.gridDbTimecardConnection.ConnectionString = conString;

            if (this.gridDbTimecardConnection.State == ConnectionState.Open)
            {
                this.CloseDbTimecardConnection();
            }

            try
            {

                this.gridDbTimecardConnection.Open();
                this.gridDbTimecardCommand.Connection = this.gridDbTimecardConnection;
                if (this.gridDbTimecardConnection.State == ConnectionState.Open)
                {
                    this.gridDbTimecardConnectionState = true;
                    OpenDbTimecardConnectionRet = true;
                }
                else
                {
                    this.gridDbTimecardConnectionState = false;
                    OpenDbTimecardConnectionRet = false;
                }
            }

            catch (SqlException ex)
            {

                OpenDbTimecardConnectionRet = false;

            }

            return OpenDbTimecardConnectionRet;



        }

        public void CloseDbTimecardConnection()
        {

            this.gridDbTimecardConnection.Close();
            try
            {
                this.gridDbTimecardConnectionState = false;
            }
            catch (Exception ex)
            {

            }
        }


    }
}
