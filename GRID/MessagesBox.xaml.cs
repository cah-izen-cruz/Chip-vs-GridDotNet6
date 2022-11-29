using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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
using Telerik.Windows.Documents.Model.Drawing.Charts;

namespace GRID
{
    /// <summary>
    /// Interaction logic for MessagesBox.xaml
    /// </summary>
    public partial class MessagesBox : Window
    {
        public MessagesBox(string Message, MessageType Type, MessageButtons Buttons)
        {
            InitializeComponent();

            lblMessageContent.Content = Message;
            switch (Type)
            {
                case MessageType.Info:
                    txtMessageTitle.Text = "Information";
                    break;
                case MessageType.Confirmation:
                    txtMessageTitle.Text = "Confirmation";
                    break;
                case MessageType.Success:
                    {
                        string defaultColor = "#4527a0";
                        Color bkColor = (Color)ColorConverter.ConvertFromString(defaultColor);
                        changeBackgroundThemeColor(Colors.Green);
                        txtMessageTitle.Text = "Success";
                    }
                    break;
                case MessageType.Warning:
                    {
                        string defaultColor = "#F98602";
                        Color bkColor = (Color)ColorConverter.ConvertFromString(defaultColor);
                        changeBackgroundThemeColor(bkColor);
                        changeBackgroundThemeColor(Colors.Gold);
                        txtMessageTitle.Text = "Warning";
                    }
                    break;
                case MessageType.Error:
                    {
                        string defaultColor = "#F44336";
                        Color bkColor = (Color)ColorConverter.ConvertFromString(defaultColor);
                        changeBackgroundThemeColor(bkColor);
                        changeBackgroundThemeColor(Colors.Red);
                        txtMessageTitle.Text = "Error";
                    }
                    break;
            }

            switch (Buttons)
            {
                case MessageButtons.OkCancel:
                    btnYes.Visibility = Visibility.Collapsed; btnNo.Visibility = Visibility.Collapsed;
                    break;
                case MessageButtons.YesNo:
                    btnOk.Visibility = Visibility.Collapsed; btnCancel.Visibility = Visibility.Collapsed;
                    break;
                case MessageButtons.Ok:
                    btnOk.Visibility = Visibility.Visible;
                    btnCancel.Visibility = Visibility.Collapsed;
                    btnYes.Visibility = Visibility.Collapsed; btnNo.Visibility = Visibility.Collapsed;
                    break;
            }


        
        }

        public void changeBackgroundThemeColor(Color newColor)
        {
            CardHeader.Background = new SolidColorBrush(newColor);
            btnCloseMessage.Foreground = new SolidColorBrush(newColor);
            btnYes.Background = new SolidColorBrush(newColor);
            btnNo.Background = new SolidColorBrush(newColor);

            btnOk.Background = new SolidColorBrush(newColor);
            btnCancel.Background = new SolidColorBrush(newColor);
        }

        public enum MessageType
        {
            Info,
            Confirmation,
            Success,
            Warning,
            Error,
        }
        public enum MessageButtons
        {
            OkCancel,
            YesNo,
            Ok,
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btnCloseMessage_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
