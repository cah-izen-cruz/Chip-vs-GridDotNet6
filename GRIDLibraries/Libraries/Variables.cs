using System.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using System.Threading;

namespace GRIDLibraries.Libraries
{ 
    partial class GridLib
    {

        public SqlConnection gridMainDbConnection;
        public SqlCommand gridMainDbCommand;
        public bool gridMainDbConnectionState = false;

        public SqlConnection gridDbConnection3;
        public SqlCommand gridDbCommand3;
        public bool gridDbConnectionState3 = false;

        public SqlConnection gridDbConnection;
        public SqlCommand gridDbCommand;
        public bool gridDbConnectionState = false;

        public SqlConnection gridATHomeDbConnection;
        public SqlCommand gridATHomeDbCommand;
        public bool gridATHomeDbConnectionState = false;

        public SqlConnection gridDbTimecardConnection;
        public SqlCommand gridDbTimecardCommand;
        public bool gridDbTimecardConnectionState = false;

        //public OleDbConnection gridLocalDbConnection;
        //public OleDbCommand gridLocalDbCommand;

        public gridDataStore gridData;


        public readonly string AppLocalDbName = "local.mdb";
        public string AppName = "GRID";

        public string TransDate { get; set; }


    }
}



