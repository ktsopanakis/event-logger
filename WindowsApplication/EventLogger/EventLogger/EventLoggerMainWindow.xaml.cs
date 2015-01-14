using System;
using System.Collections.Generic;
using System.Linq;
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

namespace EventLogger
{
    /// <summary>
    /// Interaction logic for EventLoggerMainWindow.xaml
    /// </summary>
    public partial class EventLoggerMainWindow : Window
    {
        System.Windows.Forms.NotifyIcon icon = new NotifyIcon();
        private int numOfNotifications;

        public EventLoggerMainWindow()
        {
            InitializeComponent();

            ServerConnection.ConnectToServer();

            if (numOfNotifications == 0)
                icon.Icon = Properties.Resources.smiley;
            else if (numOfNotifications > 99)
                icon.Icon = Properties.Resources.angry_smiley;
            else
            {
                icon.Icon = Properties.Resources.square_shape;
                NumberOnIcon(556, ref icon);
            }
            
            icon.Visible = true;
            icon.ShowBalloonTip(5000, "hello", "world", ToolTipIcon.Info);
            icon.Click += icon_Click;

            

            
            System.Windows.Application.Current.Exit += Current_Exit;
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            icon.Dispose();
        }

        void icon_Click(object sender, EventArgs e)
        {
            WindowState= WindowState.Normal;
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

    }
}
