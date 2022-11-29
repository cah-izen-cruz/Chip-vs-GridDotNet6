using GRID.Themes;
using GRIDLibraries.Libraries;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        GridLib grd = new GridLib();

        public bool boolTheme;
        public ImageBrush UI = new ImageBrush();
        public ImageBrush b = new ImageBrush();
        
        private readonly PaletteHelper paletteHelper = new PaletteHelper();

        MainWindow MainScrn = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

        public Settings()
        {
            InitializeComponent();

            StackPreview.Visibility = Visibility.Collapsed;
            Cmb.IsEnabled = false;
        }

        private void Themes_Click(object sender, RoutedEventArgs e)
        {
            ITheme theme = paletteHelper.GetTheme();
            ImageBrush b = new ImageBrush();

            if (Themes.IsChecked == true)
            {
                Cmb.Items.Clear();
                Cmb.Items.Add("Default");
                Cmb.Items.Add("Dark 1");
                Cmb.Items.Add("Dark 2");
                Cmb.Items.Add("Dark 3");
                Cmb.Items.Add("Dark 4");
                Cmb.Items.Add("Dark 5");
                Cmb.Items.Add("Dark 6");

                boolTheme = true;

                theme.SetBaseTheme(Theme.Dark);
                b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/DefaultDark.jpg"));

                this.MainScrn.Main.Background = b;
                grd.grdData.ScrContent.IsDark = true;
                ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);

   
            }

            else
            {
                Cmb.Items.Clear();
                Cmb.Items.Add("Default");
                Cmb.Items.Add("Light 1");
                Cmb.Items.Add("Light 2");
                Cmb.Items.Add("Light 3");
                Cmb.Items.Add("Light 4");

                boolTheme = false;

                theme.SetBaseTheme(Theme.Light);
                b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/DefaultLight.jpg"));

                this.MainScrn.Main.Background = b;
                grd.grdData.ScrContent.IsDark = false;
                ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
            }

            b.Stretch = Stretch.UniformToFill;

            StackPreview.Visibility = Visibility.Collapsed;
            grd.grdData.ScrContent.TimerStart = true;
            paletteHelper.SetTheme(theme);
        }

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lblFileName.Content = "----";
            ImageBrush b = new ImageBrush();

            switch (Cmb.SelectedIndex)
            {
                case 0:
                    if (boolTheme == true)
                    {
                        ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/DefaultDark.jpg"));
                    }

                    else
                    {
                        ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/DefaultLight.jpg"));
                    }
                    StackPreview.Visibility = Visibility.Hidden;

                    b.Stretch = Stretch.UniformToFill;

                    //((MainWindow)Application.Current.MainWindow).Main.Background = b;
                    grd.grdData.ScrContent.MainBG = b;
                    grd.grdData.ScrContent.TimerStart = true;
                    return;


                case 1:
                    if (boolTheme == true)
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark1.jpg"));
                    else
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light1.jpg"));
                    break;

                case 2:
                    if (boolTheme == true)
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark2.jpg"));
                    else
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light2.jpg"));
                    break;

                case 3:
                    if (boolTheme == true)
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark3.jpg"));
                    else
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light3.jpg"));
                    break;
                case 4:
                    if (boolTheme == true)
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark4.jpg"));
                    else
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light4.jpg"));
                    break;
                case 5:
                    if (boolTheme == true)
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark5.jpg"));
                    else
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light5.jpg"));
                    break;
                case 6:
                    if (boolTheme == true)
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark6.jpg"));
                    else
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light6.jpg"));

                    break;

                default:
                    // code block
                    break;
            }

            b.Stretch = Stretch.UniformToFill;
            BrdPreview.Background = b;
            StackPreview.Visibility = Visibility.Visible;
        }

        private void RadUser_Checked(object sender, RoutedEventArgs e)
        {
            Cmb.Items.Clear();
            if (Themes.IsChecked == true)
            {
                Cmb.Items.Clear();
                Cmb.Items.Add("Default");
                Cmb.Items.Add("Dark 1");
                Cmb.Items.Add("Dark 2");
                Cmb.Items.Add("Dark 3");
                Cmb.Items.Add("Dark 4");
                Cmb.Items.Add("Dark 5");
                Cmb.Items.Add("Dark 6");
            }
            else
            {
                Cmb.Items.Add("Default");
                Cmb.Items.Add("Light 1");
                Cmb.Items.Add("Light 2");
                Cmb.Items.Add("Light 3");
                Cmb.Items.Add("Light 4");
            }

            Cmb.IsEnabled = true;
            grd.grdData.ScrContent.TimerStart = false;
        }

        private void RadAuto_Checked(object sender, RoutedEventArgs e)
        {
            Cmb.IsEnabled = false;
            Cmb.Items.Clear();
            Cmb.Items.Add("Default");
            StackPreview.Visibility = Visibility.Hidden;

            ImageBrush b = new ImageBrush();
            if (Themes.IsChecked == true)
            {
                b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/DefaultDark.jpg"));
                ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
            }
            else
            {
                b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/DefaultLight.jpg"));
                ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
            }

            b.Stretch = Stretch.UniformToFill;
            //((MainWindow)Application.Current.MainWindow).Main.Background = b;
            grd.grdData.ScrContent.MainBG = b;
            grd.grdData.ScrContent.TimerStart = true;
        }

        private void btnSetBG_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush b = new ImageBrush();

            switch (Cmb.SelectedIndex)
            {
                case 0:
                    if (Themes.IsChecked == true)
                    {
                        boolTheme = true;
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/DefaultDark.jpg"));
                    }
                    else
                    {
                        boolTheme = false;
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/DefaultLight.jpg"));
                    }

                    //if (((MainWindow)Application.Current.MainWindow).boolTheme == true)
                    //    ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
                    //else
                    //    ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
                    break;
                case 1:
                    if (Themes.IsChecked == true)
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark1.jpg"));
                    }
                    else
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light1.jpg"));
                    }
                    break;

                case 2:
                    if (Themes.IsChecked == true)
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark2.jpg"));
                    }
                    else
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light2.jpg"));
                    }
                    break;

                case 3:
                    if (Themes.IsChecked == true)
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark3.jpg"));
                    }
                    else
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light3.jpg"));
                    }
                    break;
                case 4:
                    if (Themes.IsChecked == true)
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark4.jpg"));
                    }
                    else
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light4.jpg"));
                    }
                    break;
                case 5:
                    if (Themes.IsChecked == true)
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark5.jpg"));
                    }
                    else
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light5.jpg"));
                    }
                    break;
                case 6:
                    if (Themes.IsChecked == true)
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Dark/Dark6.jpg"));
                    }
                    else
                    {
                        b.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Themes/Light/Light6.jpg"));
                    }
                    break;

                default:
                    // code block
                    break;
            }

            if (boolTheme == true)
            {
                ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);

                ITheme theme = paletteHelper.GetTheme();
                theme.SetBaseTheme(Theme.Dark);

                paletteHelper.SetTheme(theme);
            }
            else
            {
                ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
                ITheme theme = paletteHelper.GetTheme();
                theme.SetBaseTheme(Theme.Light);

                paletteHelper.SetTheme(theme);
            }




            if (lblFileName.Content.ToString() != "----")
            {
                grd.grdData.ScrContent.TimerStart = true;
                grd.grdData.ScrContent.MainBG = UI;
                this.MainScrn.Main.Background = UI;
                //((MainWindow)Application.Current.MainWindow).Main.Background = UI;
            }
            else
            {
                b.Stretch = Stretch.UniformToFill;
                //grd.grdData.ScrContent.TimerStart = true;
                //grd.grdData.ScrContent.MainBG = b;
                this.MainScrn.Main.Background = b;
                //((MainWindow)Application.Current.MainWindow).Main.Background = b;
            }

            lblFileName.Content = "----";
            StackPreview.Visibility = Visibility.Hidden;
            btnBrowseBG.IsEnabled = true;
        }
    }
}
