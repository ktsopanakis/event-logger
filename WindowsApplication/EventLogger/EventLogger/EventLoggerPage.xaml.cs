using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServiceStack.Text;

namespace EventLogger
{
    /// <summary>
    /// Interaction logic for EventLoggerPage.xaml
    /// </summary>
    public partial class EventLoggerPage : System.Windows.Controls.UserControl
    {

        public ObservableCollection<Message> messages = new ObservableCollection<Message>();

        public EventLoggerPage()
        {
            InitializeComponent();
            
            App.Current.Properties["numOfMessages"] = 0;

            ServerConnection serve = new ServerConnection();
            serve.MessageReceived += (sender, message) => NewMessageReceived(message);
            serve.ConnectToServer();

            ErrorListView.ItemsSource = messages;

        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        public void NewMessageReceived(string message)
        {
            UnreadErrorMessages errorMessage = JsonSerializer.DeserializeFromString<UnreadErrorMessages>(message);


            Dispatcher.Invoke(() =>
            {
                errorMessage.Unread.ForEach(m => messages.Add(m));

            });

            App.Current.Properties["numOfMessages"] = messages.Count();

           // numOfNotifications = messages.Count();

           // UpdateIcon();

        }

        //private void UpdateIcon()
        //{
        //    if (numOfNotifications == 0)
        //        icon.Icon = Properties.Resources.all_good;
        //    else if (numOfNotifications > 99)
        //        icon.Icon = Properties.Resources.angry_smiley;
        //    else
        //    {
        //        icon.Icon = Properties.Resources.square_shape;
        //        NumberOnIcon(numOfNotifications, ref icon);
        //    }
        //}

       //#region Notification number on icon
       //
       //public void NumberOnIcon(int number, ref System.Windows.Forms.NotifyIcon icon)
       //{
       //    Graphics canvas;
       //    Bitmap iconBitmap = new Bitmap(60, 60);
       //    canvas = Graphics.FromImage(iconBitmap);
       //    canvas.DrawIcon(icon.Icon, 0, 0);
       //
       //    StringFormat format = new StringFormat();
       //    format.Alignment = StringAlignment.Center;
       //
       //    canvas.DrawString(number.ToString(), new Font("Calibri", 30), new SolidBrush(System.Drawing.Color.Crimson), new RectangleF(0, 0, 60, 60), format);
       //
       //    icon.Icon = System.Drawing.Icon.FromHandle(iconBitmap.GetHicon());
       //}
       //
       //#endregion

    }

    //public class MessagesEventArgs : EventArgs
    //{
    //    private int _numOfMessages;
    //
    //    public MessagesEventArgs(int numberOfMessages)
    //    {
    //        _numOfMessages = numberOfMessages;
    //    }
    //
    //    public int NumOfMessages{ get { return _numOfMessages; } }
    //}

}
