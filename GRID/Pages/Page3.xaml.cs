using GRID.Controls;
using GRIDLibraries.Libraries;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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





        private void btnUploadSS_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog radOpenFileDialog = new OpenFileDialog() { Multiselect = true };
            bool? response = radOpenFileDialog.ShowDialog();

            if (response == true)
            {
                string[] files = radOpenFileDialog.FileNames;

                for (int i = 0; i < files.Length; i++)
                {
                    string filename = System.IO.Path.GetFileName(files[i]);
                    FileInfo fileInfo = new FileInfo(files[i]);

                    UploadFileList.Items.Add(new Upload()
                    {
                        FileName = filename,
                        FileSize = string.Format("{0} {1}", (fileInfo.Length / 1.049e+6).ToString("0.0"), "Mb"),
                        UploadProgress = 100

                    });
                }
            }
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string filename = System.IO.Path.GetFileName(files[0]);

                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = System.IO.Path.GetFileName(files[i]);
                    FileInfo fileInfo = new FileInfo(files[i]);

                    UploadFileList.Items.Add(new Upload()
                    {
                        FileName = filename,
                        FileSize = string.Format("{0} {1}", (fileInfo.Length / 1.049e+6).ToString("0.0"), "Mb"),
                        UploadProgress = 100

                    });
                }
            }
        }

       
    }
}
