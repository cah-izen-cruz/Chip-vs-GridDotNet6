using GRIDLibraries.Libraries;
using System;
using System.Linq;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using static GRID.MessagesBox;
using Convert = System.Convert;

namespace GRID.Pages.QAQuestion
{
    /// <summary>
    /// Interaction logic for QAFormMainte.xaml
    /// </summary>
    /// 


    public partial class QAFormMainte : Window
    {
        GridLib grd = new GridLib();

        MainWindow MainScrn = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        int Function;
        string FormName;
        int FormId;

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


        int Formula;

        public QAFormMainte(int Fn, string _FormName, int _Id)
        {
            InitializeComponent();
            Function = Fn;
            if (Function == 0)
            {
                grpQAForm.Header = "Add New Form";
                btnSaveQForm.Visibility = Visibility.Visible;
                btnUpdateQForm.Visibility = Visibility.Collapsed;
            }
            else
            {
                grpQAForm.Header = "Update Form";
                btnSaveQForm.Visibility = Visibility.Collapsed;
                btnUpdateQForm.Visibility = Visibility.Visible;
                FormId = _Id;
                FormName = _FormName;
                txtQAFormName.Text = _FormName; 
            }
        }

        private void btnCancelQA_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainScrn.fContainer.Navigate(new QAQuestionnaire());
        }

        private void rdoSum_Checked(object sender, RoutedEventArgs e)
        {
            Formula = 1;
        }

        private void rdoAve_Checked(object sender, RoutedEventArgs e)
        {
            Formula = 2;
        }

        private void btnSaveQForm_Click(object sender, RoutedEventArgs e)
        {
            if (txtQAFormName.Text != "")
            {
                if (rdoAve.IsChecked != false || rdoSum.IsChecked != false)
                {
                    if (txtQAFormTarget.Text != "")
                    {
                        bool? Result = new MessagesBox("Are all entries final?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
                        if (Result.Value)
                        {
                            try
                            {
                                grd.AddQAForm(txtQAFormName.Text, Formula, Convert.ToInt32(txtQAFormTarget.Text));
                                grd.grdData.QuestionForm.dtLOB = grd.GetQALob();
                                MainScrn.fContainer.Content = "";
                                MainScrn.fContainer.Navigate(new QAQuestionnaire());
                            }
                            catch (Exception)
                            {
                            }
                            notifier.ShowSuccess(txtQAFormName.Text + " has been Added.");
                            this.Close();
                        }
                    }
                    else
                    {
                        new MessagesBox("Please indicate the Target", MessageType.Error, MessageButtons.YesNo).ShowDialog();
                        return;
                    }
                }
                else
                {
                    new MessagesBox("Please choose a Formula", MessageType.Error, MessageButtons.YesNo).ShowDialog();
                    return;
                }

            }
            else
            {
                new MessagesBox("Please enter Questionnaire Form Name.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                txtQAFormName.Focus();
                return;
            }
        }

        private void btnUpdateQForm_Click(object sender, RoutedEventArgs e)
        {
            if (txtQAFormName.Text != "")
            {
                if(rdoAve.IsChecked != false || rdoSum.IsChecked != false)
                {
                    if(txtQAFormTarget.Text != "")
                    {
                        bool? Result = new MessagesBox("Are all entries final?", MessageType.Confirmation, MessageButtons.YesNo).ShowDialog();
                        if (Result.Value)
                        {
                            try
                            {
                                grd.UpdateQAForm(FormId, txtQAFormName.Text, Formula, Convert.ToInt32(txtQAFormTarget.Text));
                                grd.grdData.QuestionForm.dtLOB = grd.GetQALob();
                                MainScrn.fContainer.Content = "";
                                MainScrn.fContainer.Navigate(new QAQuestionnaire());
                            }
                            catch (Exception)
                            {
                            }

                            notifier.ShowSuccess(txtQAFormName.Text + " has been Updated.");
                            this.Close();
                        }
                    }
                    else
                    {
                        new MessagesBox("Please indicate the Target", MessageType.Error, MessageButtons.YesNo).ShowDialog();
                        return;
                    }              
                }
                else
                {
                    new MessagesBox("Please choose a Formula", MessageType.Error, MessageButtons.YesNo).ShowDialog();
                    return;
                }
              
            }
            else
            {
                new MessagesBox("Please enter a new Question Form", MessageType.Error, MessageButtons.Ok).ShowDialog();
                return;
            }
        }
    }
}
