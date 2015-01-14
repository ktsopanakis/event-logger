using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography.X509Certificates;
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
using System.Drawing;
using Newtonsoft.Json.Serialization;
using ServiceStack.Text;

namespace EventLogger
{
    /// <summary>
    /// Interaction logic for EventLoggerMainWindow.xaml
    /// </summary>
    public partial class EventLoggerMainWindow : Window
    {
        System.Windows.Forms.NotifyIcon icon = new NotifyIcon();
        public ObservableCollection<Message> messages = new ObservableCollection<Message>();
        public int numOfNotifications;

        public EventLoggerMainWindow()
        {
            InitializeComponent();

            ServerConnection serve = new ServerConnection();
            serve.MessageReceived += (sender, message) => NewMessageReceived(message);
            serve.ConnectToServer();


            //numOfNotifications = 0;
            //numOfNotifications = 50;
            numOfNotifications = 100;

            if (numOfNotifications == 0)
                icon.Icon = Properties.Resources.all_good;
            else if (numOfNotifications > 99)
                icon.Icon = Properties.Resources.angry_smiley;
            else
            {
                icon.Icon = Properties.Resources.square_shape;
                NumberOnIcon(numOfNotifications, ref icon);
            }
            
            icon.Visible = true;
            icon.ShowBalloonTip(5000, "hello", "world", ToolTipIcon.Info);
            icon.Click += icon_Click;

            ErrorListView.ItemsSource = messages;

            System.Windows.Application.Current.Exit += Current_Exit;


        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            icon.Dispose();
        }

        void icon_Click(object sender, EventArgs e)
        {
            this.WindowState= WindowState.Normal;
            this.ShowInTaskbar = true;
        }

        protected override void OnClosing( CancelEventArgs e)
        {
            e.Cancel = true;

            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        public void NewMessageReceived(string message)
        {
            UnreadErrorMessages errorMessage = JsonSerializer.DeserializeFromString<UnreadErrorMessages>(message);

            
            Dispatcher.Invoke(() =>
            {
                errorMessage.Unread.ForEach(m=>messages.Add(m));
                
            });
        }
       
        #region Notification number on icon

        public void NumberOnIcon(int number,  ref System.Windows.Forms.NotifyIcon icon)
        {
            Graphics canvas;
            Bitmap iconBitmap = new Bitmap(60,60);
            canvas = Graphics.FromImage(iconBitmap);
            canvas.DrawIcon(icon.Icon, 0, 0);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;

            canvas.DrawString(number.ToString(),new Font("Calibri",30), new SolidBrush(System.Drawing.Color.Crimson), new RectangleF(0,0,60,60), format );

            icon.Icon = System.Drawing.Icon.FromHandle(iconBitmap.GetHicon());
        }

        #endregion

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
