using GRIDLibraries.Libraries;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;

namespace GRID.Pages
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {
        GridLib grd = new GridLib();
        int TabCtr;
        DataTable dtCompleted;

        MainWindow MainScrn = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        #region "OpenCompletedActivities"
        private void LoadOpenActivity()
        {

            lvOpenActivities.ItemsSource = null;

            var AddRF = new List<string>();
            int ActId = 0;

            if (cmbOpenActName.SelectedIndex == 0)
            {
                AddRF = grd.GetConfigFieldName(0);
            }
            else
            {
                try
                {
                    gridActivity act = (gridActivity)cmbOpenActName.SelectedItem;
                    ActId = act.Id;
                    AddRF = grd.GetConfigFieldName(act.Id);
                }

                catch (Exception ex)
                {

                }

            }

            var gView = new GridView();

            //gView.AllowsColumnReorder = true;
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "ID", DisplayMemberBinding = new Binding("Id"), Width = 0 });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "ActId", DisplayMemberBinding = new Binding("Activity.Id"), Width = 0 });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Activity Name", DisplayMemberBinding = new Binding("Activity.ActName") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Time Started", DisplayMemberBinding = new Binding("TimeStart") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Duration", DisplayMemberBinding = new Binding("TimeElapsed") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Type", DisplayMemberBinding = new Binding("Activity.Type") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Process", DisplayMemberBinding = new Binding("Activity.Process") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Ref No.", DisplayMemberBinding = new Binding("ReferenceId") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Item Id", DisplayMemberBinding = new Binding("LOBItemId"), Width = 0 });


            if (!(AddRF == null))
            {
                if (AddRF.Count > 0)
                {

                    for (int i = 0, loopTo = AddRF.Count - 1; i <= loopTo; i++)
                        gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = AddRF[i].ToString(), DisplayMemberBinding = new Binding("PerfConfigData.RF" + Strings.Format(i + 1, "00")), Width = 100 });

                }
            }

            lvOpenActivities.View = gView;

            lvOpenActivities.ItemsSource = grd.GetPerformances(ActId, 1);



            TabItem1.Header = "(" + lvOpenActivities.Items.Count + ") " + " Open";

        }

        private void LoadCompletedActivity()
        {


            dtCompleted = new DataTable();

            lvClosedActivities.ItemsSource = null;

            //lblTotalAudit.Visibility = Visibility.Hidden;
            //btnExportCompleted.Visibility = Visibility.Hidden;
            var AddRF = new List<string>();

            int ActId = 0;
            if (cmbCompletedActName.SelectedIndex == 0)
            {
                AddRF = grd.GetConfigFieldName(0);
            }
            else
            {
                try
                {
                    gridActivity act = (gridActivity)cmbCompletedActName.SelectedItem;
                    ActId = act.Id;
                    AddRF = grd.GetConfigFieldName(act.Id);
                }
                catch (Exception ex)
                {

                }

            }

            var gView = new GridView();

            dtCompleted.Columns.Add("ID");
            dtCompleted.Columns.Add("Activity.ActName");
            dtCompleted.Columns.Add("Activity.Type");
            dtCompleted.Columns.Add("Activity.Process");
            dtCompleted.Columns.Add("TimeStart");
            dtCompleted.Columns.Add("TimeEnd");
            dtCompleted.Columns.Add("TimeElapsed");
            dtCompleted.Columns.Add("ReferenceId");


            gView.AllowsColumnReorder = true;
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "ID", DisplayMemberBinding = new Binding("Id"), Width = 0 });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Activity Name", DisplayMemberBinding = new Binding("Activity.ActName") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Type", DisplayMemberBinding = new Binding("Activity.Type") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Process", DisplayMemberBinding = new Binding("Activity.Process") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Time Started", DisplayMemberBinding = new Binding("TimeStart") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Time Ended", DisplayMemberBinding = new Binding("TimeEnd") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Duration", DisplayMemberBinding = new Binding("TimeElapsed") });
            gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = "Ref No.", DisplayMemberBinding = new Binding("ReferenceId") });



            if (!(AddRF == null))
            {
                if (AddRF.Count > 0)
                {

                    for (int i = 0, loopTo = AddRF.Count - 1; i <= loopTo; i++)
                    {
                        try
                        {
                            dtCompleted.Columns.Add(AddRF[i].ToString());
                        }
                        catch (Exception ex)
                        {

                        }


                        gView.Columns.Add(new System.Windows.Controls.GridViewColumn() { Header = AddRF[i].ToString(), DisplayMemberBinding = new Binding("PerfConfigData.RF" + Strings.Format(i + 1, "00")), Width = 100 });
                    }

                }
            }



            lvClosedActivities.View = gView;

            lvClosedActivities.ItemsSource = grd.GetPerformances(ActId, 2);

            tabItem2.Header = "(" + lvClosedActivities.Items.Count + ")" + " Completed";
        }

        #endregion
        public Page1()
        {
            InitializeComponent();

            grd.conString = "Data Source=WPEC5009GRDRP01;" + "Initial Catalog=" + grd.grdData.TeamInfo.DBName + ";" + "Persist Security Info=True;" + "Integrated Security=SSPI;" + "Connect Timeout=3000;";

            try
            {
                cmbOpenActName.ItemsSource = null;
                cmbCompletedActName.ItemsSource = null;

                cmbCompletedActName.ItemsSource = grd.GetDistinctPerfActivity(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2, 2);
                cmbCompletedActName.SelectedIndex = 0;
                cmbOpenActName.ItemsSource = grd.GetDistinctPerfActivity(grd.grdData.CurrentUser.EmpNo, grd.grdData.CurrentUser.TransactionDate2, 1);
                cmbOpenActName.SelectedIndex = 0;

                cmbCompletedActName.SelectedIndex = 0;
                cmbOpenActName.SelectedIndex = 0;

                this.LoadOpenActivity();
                this.LoadCompletedActivity();

                if (TabCtr == 1)
                {
                    TabControl1.SelectedIndex = 1;
                }
                else
                {
                    TabControl1.SelectedIndex = 0;
                }

            }
            catch (Exception)
            {
            }
        }


    }
}
