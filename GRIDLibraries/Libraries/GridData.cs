using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks; 

namespace GRIDLibraries.Libraries
{
    public class GridData
    {
        public List<gridActivity> _lstMyActivitiesOrig = new List<gridActivity>();
        public List<gridActivity> _lstProductivityOrig = new List<gridActivity>();
        public List<gridActivity> _lstBreakOrig = new List<gridActivity>();

        public List<gridPerformance> _lstOpenActivities = new List<gridPerformance>();
        public List<gridPerformance> _lstCompletedActivities = new List<gridPerformance>();

        public List<QAQuestionForm> _lstQAScoring = new List<QAQuestionForm>();
        public DataTable dtActivity { get; set; } = new DataTable();

        public static GridData gridDataStore { get; set; } = new GridData();

        public gridMainScrContent ScrContent { get; set; } = new gridMainScrContent();

        public gridComboBoxes grdCombo { get; set; } = new gridComboBoxes();

        public gridTeam TeamInfo { get; set; } = new gridTeam();

        public gridUserInfo CurrentUser { get; set; } = new gridUserInfo();

        public gridPerformance CurrentActivity { get; set; } = new gridPerformance();


        public gridPerfInfo CurrentPerfInfo;


        public List<gridPerformance> PerformanceList; // holdsteam's gridPerformance List

        public List<gridActivity> ActivityList;

        public List<QAQuestionForm> QALists;

        public GridWFHinfo WFHInfo;

        public gridMainAgentMetrics MainAgentMetrics;

        public QAQuestionForm QuestionForm { get; set; } = new QAQuestionForm();

        public object ActList { get; internal set; }

        public string GridVersion = "GRID v1.4.1.0";

        public string MainWindowAction;

        public string ApplicationDirectory;

    }



}


