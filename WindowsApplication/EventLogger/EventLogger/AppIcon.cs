using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace EventLogger
{
    public class AppIcon
    {
        public NotifyIcon icon = new NotifyIcon();

        public AppIcon()
        {
            icon.Icon = Properties.Resources.all_good;

            icon.Visible = true;
            icon.ShowBalloonTip(5000, "hello", "world", ToolTipIcon.Info);
            icon.Click += icon_Click;

        }


        void icon_Click(object sender, EventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Normal;
            App.Current.MainWindow.ShowInTaskbar = true;
        }

        public void UpdateIcon(int numOfNotifications)
        {
            if (numOfNotifications == 0)
                icon.Icon = Properties.Resources.all_good;
            else if (numOfNotifications > 99)
                icon.Icon = Properties.Resources.angry_smiley;
            else
            {
                icon.Icon = Properties.Resources.square_shape;
                NumberOnIcon(numOfNotifications, ref icon);
            }
        }

        #region Notification number on icon

        public void NumberOnIcon(int number, ref System.Windows.Forms.NotifyIcon icon)
        {
            Graphics canvas;
            Bitmap iconBitmap = new Bitmap(60, 60);
            canvas = Graphics.FromImage(iconBitmap);
            canvas.DrawIcon(icon.Icon, 0, 0);

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;

            canvas.DrawString(number.ToString(), new Font("Calibri", 30), new SolidBrush(System.Drawing.Color.Crimson), new RectangleF(0, 0, 60, 60), format);

            icon.Icon = System.Drawing.Icon.FromHandle(iconBitmap.GetHicon());
        }

        #endregion
    }
}
