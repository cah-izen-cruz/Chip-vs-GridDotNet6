using GRIDLibraries.Libraries;
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
using Convert = System.Convert;

namespace GRID.Pages
{
    /// <summary>
    /// Interaction logic for Page3.xaml
    /// </summary>
    public partial class Page3 : Page
    {
        GridLib grd = new GridLib();
        public Page3()
        {
            InitializeComponent();

                
          
        }

        private void TabQA_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //foreach (var QA in grd.grdData._lstQAQuestions)
            //{
            //    lstQAForm.Items.Add(new { Id = QA.LOBId, Name = QA.Name, Formula = QA.Formula, Target = QA.Target });
            //}

            //lstQAForm.Items.Clear();
            //string _Formula;
            //foreach (DataRow row in grd.grdData.QuestionForm.dtLOB.Rows)
            //{
            //    if (Convert.ToInt32(row["Formula"].ToString()) == 1)
            //        _Formula = "Sum";
            //    else
            //        _Formula = "Average";



                //    lstQAForm.Items.Add(new { Id = row["Id"], Name = row["Name"], Formula = _Formula, Target = row["Target"] });

                //}
        }
    }
}
