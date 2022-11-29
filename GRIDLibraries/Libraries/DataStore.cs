using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRIDLibraries.Libraries
{
    public class gridDataStore
    {
        public gridDataStore()
        {
           
            CurrentUser = new gridUserInfo();

            CurrentActivity = new gridPerformance();

            //MainAgentMetrics = new gridMainAgentMetrics();

            //IVHotKey = new DataTable();

            GridVersion = "GRID v1.4.1.0";

            LOBProcess = "";
            LOBUploadInprogress = false;

            ActivityList = new List<gridActivity>();
            PerformanceList = new List<gridPerformance>();

        }

     

        public bool LOBUploadInprogress;

        public gridUserInfo CurrentUser; // holds current active user

        public gridPerformance CurrentActivity; // holds current activity


        public gridTeam TeamInfo; // holds current active team

        public List<gridActivity> ActivityList; // holdsteam's Activity List
        public List<gridPerformance> PerformanceList; // holdsteam's gridPerformance List


        //public GridWFHinfo WFHInfo;

        public string MainWindowAction;

        public string ApplicationDirectory;
        public string GridVersion;

        public string LOBProcess;


        // USED IN MAIN FORM
        public gridPerformance CurrentPerformance;
        public List<gridPerformance> MainOpenActivities;
        public List<gridPerformance> MainClosedActivities;
        //public gridMainAgentMetrics MainAgentMetrics;



        // USED IN PERF INFO
        public gridPerfInfo CurrentPerfInfo;

        // USED IN UTILIZATION THRESHOLD
        public double UtilThresLowerLmt;
        public double UtilThresUpperLmt;

        // USED IN EFFICIENCY THRESHOLD
        public double EfcnThresLowerLmt;
        public double EfcnThresUpperLmt;

        public bool IsMainWindowActivive;

    }
}
