using GRID.Pages.QAQuestion;
using GRIDLibraries.Libraries;
using LiveCharts;
using Microsoft.VisualBasic;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using static GRID.MessagesBox;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;
using Convert = System.Convert;

namespace GRID.Pages
{
    /// <summary>
    /// Interaction logic for QAQuestionnaire.xaml
    /// </summary>
    public partial class QAQuestionnaire : Page
    {
        GridLib grd = new GridLib();
        //DataTable dtLOB;
        //DataTable dtQAQuestionnaire;
        //DataTable dtQASelection;
        //DataTable dtQAMarkdownSelection;

        MainWindow MainScrn = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        private static readonly Regex _regex = new Regex("[^0-9.-]+");

        Mutex QAMutex = new Mutex();

        public DataTable dtSelection = new DataTable();
        public DataRow drSelection;

        public DataTable dtMarkdown = new DataTable();
        public DataRow drMarkdown;

        QAQuestionForm qAQuestionForm = new QAQuestionForm();

        public BackgroundWorker LoadQAQuestions = new BackgroundWorker();

        private readonly BackgroundWorker bw = new BackgroundWorker();



        long maxQID;
        string strQAForm;

        bool IsEdit = false;
        public QAQuestionnaire()
        {
            InitializeComponent();

            grpConfig.IsEnabled = false;
            BrdButtons.Visibility = Visibility.Collapsed;
            btnCancel.Visibility = Visibility.Collapsed;
            btnUpdate.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Collapsed;

            btnEditlQA.Visibility = Visibility.Visible;

            objBusyIndicator.BusyContent = "Getting Questions...";
            objBusyIndicator.IsBusy = true;

            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += bw_single_Complete;

            bw.RunWorkerAsync();
        }

        private void bw_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.QAMutex.WaitOne();

            //grd.grdData.QAQuestion.dtLOB = grd.GetQALob();
            //grd.grdData.QAQuestion.dtQAQuestionnaire = grd.GetQAQuestionnaires();
            //grd.grdData.QAQuestion.dtQASelection = grd.GetQASelection();
            //grd.grdData.QAQuestion.dtQAMarkdownSelection = grd.GetQAMarkdown();

            //dtLOB = grd.GetQALob();
            //dtQAQuestionnaire = grd.GetQAQuestionnaires();
            //dtQASelection = grd.GetQASelection();
            //dtQAMarkdownSelection = grd.GetQAMarkdown();

            this.QAMutex.ReleaseMutex();
        }

        private void bw_single_Complete(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
          
                this.QAMutex.WaitOne();
                cmbQuestionnaire.ItemsSource = grd.grdData._lstQAQuestions;
                this.QAMutex.ReleaseMutex();
            }
            catch (Exception ex)
            {

            }
            objBusyIndicator.IsBusy = false;
        }


        Notifier notifier = new Notifier(msg =>
        {
            msg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            msg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            msg.Dispatcher = Application.Current.Dispatcher;
        });

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);

            if (e.Handled == true)
                new MessagesBox("Accepts Numeric values only.", MessageType.Error, MessageButtons.Ok).ShowDialog();
        }

        public void clearObjects()
        {
            txtQuestion.Text = "";
            txtCategory.Text = "";
            txtDescription.Text = "";

            lstSelection.Items.Clear();
            txtValueSelection.Text = "";
            txtScore.Text = "";

            lstMardownType.Items.Clear();
            txtMarkdown.Text = "";
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MessagesBox("Exit QA Tool Application?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
            if (Result.Value)
            {
                //Application.Current.Shutdown();
                MainScrn.fContainer.Content = "";
            }
        }

        public void GetDatafromQuestionnaireTemplate()
        {
            int indexnum = cmbQuestionnaire.SelectedIndex;
            if (indexnum == -1)
            {
            }
            else
            {
                lstQuestions.Items.Clear();

                DataRow[] result;
                if (maxQID == 0)
                {
                    result = grd.grdData.QAQuestion.dtQAQuestionnaire.Select("LOBId = '" + cmbQuestionnaire.SelectedValue + "'");
                }
                else
                {
                    result = grd.grdData.QAQuestion.dtQAQuestionnaire.Select("LOBId = '" + maxQID + "'");
                }

                foreach (DataRow row in result)
                {
                    lstQuestions.Items.Add(new { Id = row["LOBId"], FormId = row["FormId"], QQuestionName = row["Question"], QCategory = row["Category"] });
                }

                this.grd.grdData.QAQuestion.LOBId = (int)cmbQuestionnaire.SelectedValue;
            }
        }

        private void cmbQuestionnaire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetDatafromQuestionnaireTemplate();
        }

        private void btnAddQForm_Click(object sender, RoutedEventArgs e)
        {
            
            QAFormMainte aFormMainte = new QAFormMainte(0,cmbQuestionnaire.Text, Convert.ToInt32(cmbQuestionnaire.SelectedValue));
            aFormMainte.ShowDialog();          
        }


        private void btnCancelQA_Click(object sender, RoutedEventArgs e)
        {
            grpConfig.IsEnabled = true;
            grpTemplate.IsEnabled = true;
            cmbQuestionnaire.Text = "";
            cmbQuestionnaire.IsEditable = false;
            cmbQuestionnaire.Focus();
            btnAddQForm.Visibility = Visibility.Visible;
        }

        private void btnEditQA_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MessagesBox("Modify Question Template?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
            if (Result.Value)
            {
                for (int i = lstQuestions.SelectedItems.Count - 1; i >= 0; i -= 1)
                {
                    var obj = lstQuestions.SelectedItems[i];
                    var id = obj.GetType().GetProperties().First(o => o.Name == "FormId").GetValue(obj, null);

                    DataRow[] result = grd.grdData.QAQuestion.dtQASelection.Select("FormId = '" + id + "'");
                    lstSelection.Items.Clear();

                   

                    foreach (DataRow row in result)
                    {
                        txtCategory.Text = row["Category"].ToString();
                        txtQuestion.Text = row["Question"].ToString();
                        txtDescription.Text = row["Description"].ToString();

                        lstSelection.Items.Add(new { Id = row["SId"].ToString(), Value = row["Value"].ToString(), Score = row["Score"].ToString() });
                    }

                    DataRow[] result1 = grd.grdData.QAQuestion.dtQAMarkdownSelection.Select("QID = '" + id + "'");
                    lstMardownType.Items.Clear();

                    foreach (DataRow row1 in result1)
                        lstMardownType.Items.Add(new { Id = row1["Id"].ToString(), Value = row1["Value"].ToString() });

                    this.grd.grdData.QAQuestion.QID = Convert.ToInt32(id.ToString());
                }


                grpConfig.IsEnabled = true;
                grpTemplate.IsEnabled = false;

                cmbQuestionnaire.IsEnabled = false;
                btnAddQForm.IsEnabled = false;

                btnCancel.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Visible;
                btnSave.Visibility = Visibility.Collapsed;
                BrdButtons.Visibility = Visibility.Visible;
                txtCategory.Focus();

                IsEdit = true;
            }
            else
            {
                grpConfig.IsEnabled = false;
                grpTemplate.IsEnabled = true;

                this.clearObjects();

                lstQuestions.IsEnabled = true;
                cmbQuestionnaire.IsEnabled = true;
                btnAddQForm.IsEnabled = true;

                btnCancel.Visibility = Visibility.Collapsed;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Collapsed;
            }
        }

        private void btnDeleteQA_Click(object sender, RoutedEventArgs e)
        {
            for (int i = lstQuestions.SelectedItems.Count - 1; i >= 0; i -= 1)
            {
                var obj = lstQuestions.SelectedItems[i];
                var id = obj.GetType().GetProperties().First(o => o.Name == "FormId").GetValue(obj, null);

                DataRow[] result = grd.grdData.QAQuestion.dtQAQuestionnaire.Select("FormId = '" + id + "'");

                foreach (DataRow row in result)
                {
                    strQAForm = row["Question"].ToString();
                }

                this.grd.grdData.QAQuestion.QID = Convert.ToInt32(id.ToString());
            }

            bool? Result = new MessagesBox("Deleting this template will also delete its configuration." + Constants.vbNewLine + "Do you want to proceed?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (Result.Value)
            {
                this.grd.DeleteQATemplate(this.grd.grdData.QAQuestion.QID);

                this.QAMutex.WaitOne();
                //grd.grdData.QAQuestion.dtLOB = grd.GetQALob();
                grd.grdData._lstQAQuestions = grd.GetQAFormList();
                grd.grdData.QAQuestion.dtQAQuestionnaire = grd.GetQAQuestionnaires();
                grd.grdData.QAQuestion.dtQASelection = grd.GetQASelection();
                grd.grdData.QAQuestion.dtQAMarkdownSelection = grd.GetQAMarkdown();

                //dtLOB = grd.GetQALob();
                //dtQAQuestionnaire = grd.GetQAQuestionnaires();
                //dtQASelection = grd.GetQASelection();
                //dtQAMarkdownSelection = grd.GetQAMarkdown();

                GetDatafromQuestionnaireTemplate();
                this.QAMutex.ReleaseMutex();
                notifier.ShowInformation(strQAForm + " has been deleted succesfully.");
            }

            grpConfig.IsEnabled = false;
        }

        private void btnAddTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (cmbQuestionnaire.Text != "")
            {

                this.grd.grdData.QAQuestion.LOBId = (int)cmbQuestionnaire.SelectedValue;
                grpTemplate.IsEnabled = false;

                cmbQuestionnaire.IsEnabled = false;
                btnAddQForm.IsEnabled = false;

                BrdButtons.Visibility = Visibility.Visible;
                btnCancel.Visibility = Visibility.Visible;
                btnUpdate.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Visible;
                grpConfig.IsEnabled = true;
                txtCategory.Focus();


                dtSelection = new DataTable();
                DataRow drSelection;
;               dtMarkdown = new DataTable();
                DataRow drMarkdown;

            }
            else
            {
                new MessagesBox("You cannot Add Question to a blank Form." + Constants.vbNewLine + "Please choose a Questionnaire Form.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }
        }

        private void btnDelMark_Click(object sender, RoutedEventArgs e)
        {
            bool MarkId = false;
            for (int i = lstMardownType.SelectedItems.Count - 1; i >= 0; i -= 1)
            {
                var obj = lstMardownType.SelectedItems[i];
                var id = obj.GetType().GetProperties().First(o => o.Name == "Id").GetValue(obj, null);

                if (id.ToString() != "")
                {
                    DataRow[] result = grd.grdData.QAQuestion.dtQAMarkdownSelection.Select("Id = '" + id + "'");

                    this.grd.grdData.QAQuestion.MarkId = Convert.ToInt32(id.ToString());
                    MarkId = true;
                }
            }

            if (MarkId)
                grd.DeleteMarkdown(this.grd.grdData.QAQuestion.MarkId);

            var selected = lstMardownType.SelectedItems.Cast<Object>().ToArray();
            foreach (var item in selected)
            {
                lstMardownType.Items.Remove(item);
            }
        }

        private void btnAddSel_Click(object sender, RoutedEventArgs e)
        {
            if (txtValueSelection.Text != "" && txtScore.Text != "")
            {
                qAQuestionForm.SelectionValue = txtValueSelection.Text;
                qAQuestionForm.Score = Convert.ToInt32(txtScore.Text);
               

                if (IsEdit)
                {
                    grd.AddScore(this.grd.grdData.QAQuestion.QID, qAQuestionForm.SelectionValue, (int)qAQuestionForm.Score);
                }
                else
                {
                    if (dtSelection.Rows.Count == 0)
                    {
                        lstSelection.Items.Clear();
                        dtSelection.Columns.Add("Value");
                        dtSelection.Columns.Add("Score");

                        drSelection = dtSelection.NewRow();
                        drSelection["Value"] = qAQuestionForm.SelectionValue;
                        drSelection["Score"] = Strings.Trim(qAQuestionForm.Score.ToString());

                        dtSelection.Rows.Add(drSelection);
                    }
                    else
                    {
                        drSelection = dtSelection.NewRow();
                        drSelection["Value"] = qAQuestionForm.SelectionValue;
                        drSelection["Score"] = Strings.Trim(qAQuestionForm.Score.ToString());

                        dtSelection.Rows.Add(drSelection);
                    }

                    dtSelection.AcceptChanges();
                }

                lstSelection.Items.Add(new { Id = "", Value = Strings.Trim(qAQuestionForm.SelectionValue), Score = (Strings.Trim(qAQuestionForm.Score.ToString())) });

                txtValueSelection.Text = "";
                txtScore.Text = "";
                txtValueSelection.Focus();

                notifier.ShowSuccess("Score has been added.");
            }
            else
            {
                new MessagesBox("Either Criteria or Score has no value to add." + Constants.vbNewLine + "Please check.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }
        }

        private void btnMarkdownAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtMarkdown.Text != "")
            {
                qAQuestionForm.MarkdownType = txtMarkdown.Text;

                if (IsEdit)
                {
                    grd.AddMarkdown(this.grd.grdData.QAQuestion.QID, qAQuestionForm.MarkdownType);
                }
                else
                {
                    if (dtMarkdown.Rows.Count == 0)
                    {
                        lstMardownType.Items.Clear();
                        dtMarkdown.Columns.Add("Value");

                        drMarkdown = dtMarkdown.NewRow();
                        drMarkdown["Value"] = qAQuestionForm.MarkdownType;

                        dtMarkdown.Rows.Add(drMarkdown);
                    }
                    else
                    {
                        drMarkdown = dtMarkdown.NewRow();
                        drMarkdown["Value"] = qAQuestionForm.MarkdownType;

                        dtMarkdown.Rows.Add(drMarkdown);
                    }

                    dtMarkdown.AcceptChanges();
                }

                lstMardownType.Items.Add(new { Id = "", Value = Strings.Trim(qAQuestionForm.MarkdownType) });

                txtMarkdown.Text = "";
                txtMarkdown.Focus();

                notifier.ShowSuccess("Markdown Type has been added.");
            }
            else
            {
                new MessagesBox("Markdown has no value to add." + Constants.vbNewLine + "Please check.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }
        }

        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            bool selId = false;
            for (int i = lstSelection.SelectedItems.Count - 1; i >= 0; i -= 1)
            {
                var obj = lstSelection.SelectedItems[i];
                var id = obj.GetType().GetProperties().First(o => o.Name == "Id").GetValue(obj, null).ToString();

                if (id.ToString() != "")
                {
                    DataRow[] result = grd.grdData.QAQuestion.dtQASelection.Select("SId = '" + id + "'");

                    this.grd.grdData.QAQuestion.SelId = Convert.ToInt32(id.ToString());
                    selId = true;
                }
            }

            if (selId)
                grd.DeleteScore(this.grd.grdData.QAQuestion.SelId);

            var selected = lstSelection.SelectedItems.Cast<Object>().ToArray();
            foreach (var item in selected)
            {
                lstSelection.Items.Remove(item);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            grpConfig.IsEnabled = false;
            grpTemplate.IsEnabled = true;

            this.clearObjects();

            lstQuestions.IsEnabled = true;
            cmbQuestionnaire.IsEnabled = true;
            btnAddQForm.IsEnabled = true;

            btnCancel.Visibility = Visibility.Collapsed;
            btnUpdate.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Collapsed;
            BrdButtons.Visibility = Visibility.Collapsed;

            IsEdit = false;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MessagesBox("Are all changes final?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
            if (Result.Value)
            {
                try
                {
                    grd.UpdateQATemplate(grd.grdData.QAQuestion.LOBId, txtCategory.Text, txtQuestion.Text, "Selectionlist", txtDescription.Text);
                    this.QAMutex.WaitOne();

                    //grd.grdData.QAQuestion.dtLOB = grd.GetQALob();
                    grd.grdData._lstQAQuestions = grd.GetQAFormList();
                    grd.grdData.QAQuestion.dtQAQuestionnaire = grd.GetQAQuestionnaires();
                    grd.grdData.QAQuestion.dtQASelection = grd.GetQASelection();
                    grd.grdData.QAQuestion.dtQAMarkdownSelection = grd.GetQAMarkdown();

                    //dtLOB = grd.GetQALob();
                    //dtQAQuestionnaire = grd.GetQAQuestionnaires();
                    //dtQASelection = grd.GetQASelection();
                    //dtQAMarkdownSelection = grd.GetQAMarkdown();

                    GetDatafromQuestionnaireTemplate();

                    this.QAMutex.ReleaseMutex();

                    clearObjects();

                    grpConfig.IsEnabled = false;
                    grpTemplate.IsEnabled = true;
                    cmbQuestionnaire.IsEnabled = true;
                    btnAddQForm.IsEnabled = true;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnUpdate.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;


                    notifier.ShowSuccess("Changes has been saved.");

                    IsEdit = false;
                }
                catch (Exception)
                {
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool? Result = new MessagesBox("Are all entries final?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();

            if (Result.Value)
            {
                try
                {

                    this.QAMutex.WaitOne();

                    grd.AddQATemplate(grd.grdData.QAQuestion.LOBId, txtCategory.Text, txtQuestion.Text, "Selectionlist", txtDescription.Text, dtSelection, dtMarkdown);

                    //grd.grdData.QAQuestion.dtLOB = grd.GetQALob();
                    grd.grdData._lstQAQuestions = grd.GetQAFormList();
                    grd.grdData.QAQuestion.dtQAQuestionnaire = grd.GetQAQuestionnaires();
                    grd.grdData.QAQuestion.dtQASelection = grd.GetQASelection();
                    grd.grdData.QAQuestion.dtQAMarkdownSelection = grd.GetQAMarkdown();

                    //dtLOB = grd.GetQALob();
                    //dtQAQuestionnaire = grd.GetQAQuestionnaires();
                    //dtQASelection = grd.GetQASelection();
                    //dtQAMarkdownSelection = grd.GetQAMarkdown();

                    GetDatafromQuestionnaireTemplate();

                    this.QAMutex.ReleaseMutex();

                    clearObjects();

                    grpConfig.IsEnabled = false;
                    grpTemplate.IsEnabled = true;
                    cmbQuestionnaire.IsEnabled = true;
                    btnAddQForm.IsEnabled = true;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnUpdate.Visibility = Visibility.Collapsed;
                    btnSave.Visibility = Visibility.Collapsed;

                    notifier.ShowSuccess("New Template has been created.");
                }

                catch (Exception)
                {
                }
            }
            else
            { return; }

        }

        private void btnEditlQA_Click(object sender, RoutedEventArgs e)
        {
            if(cmbQuestionnaire.Text == "")
            {
                new MessagesBox("Please select a Question form to Edit.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }
            QAFormMainte aFormMainte = new QAFormMainte(1,cmbQuestionnaire.Text, Convert.ToInt32(cmbQuestionnaire.SelectedValue));
            aFormMainte.ShowDialog();

        }
       

    }
}
