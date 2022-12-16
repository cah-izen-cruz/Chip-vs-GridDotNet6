using GRIDLibraries.Libraries;
using LiveCharts.Wpf;
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
using System.Windows.Shapes;
using static GRID.MessagesBox;

namespace GRID
{
    /// <summary>
    /// Interaction logic for FrmChipUsers.xaml
    /// </summary>
    /// 

    public partial class FrmChipUsers : Window
    {
        GridLib grd = new GridLib();
        public FrmChipUsers()
        {
            InitializeComponent();         
        }

        private void btnEmpNo_Click(object sender, RoutedEventArgs e)
        {
            if(txtAgentName.Text != "")
            lstOwner.ItemsSource =  grd.GetEmployee(txtAgentName.Text);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (!(lstOwner.SelectedItem == null))
            {
                QAQuestionForm obj = (QAQuestionForm)lstOwner.SelectedItem;
                if (obj != null)
                {
                    grd.grdData.QuestionForm.UserId = obj.UserId;
                    grd.grdData.QuestionForm.EmpName = obj.EmpName;
                    grd.grdData.QuestionForm.SupEmpNo = obj.SupEmpNo;
                    grd.grdData.QuestionForm.Supervisor = obj.Supervisor;

                    this.Close();
                }

            }
            else
            {
                new MessagesBox("Please choose the Owner of your Questionnaire.", MessageType.Error, MessageButtons.Ok).ShowDialog();
                txtAgentName.Focus();
                return;
            }
           
        }

        private void btnUpdateQForm_Click(object sender, RoutedEventArgs e)
        {
            grd.grdData.QuestionForm.UserId = 0;
            grd.grdData.QuestionForm.EmpName = "";
            this.Close();
        }
    }
}
