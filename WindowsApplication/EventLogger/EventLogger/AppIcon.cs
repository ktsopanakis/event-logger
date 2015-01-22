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
            icon.ShowBalloonTip(5000, "Click here to", "open app window", ToolTipIcon.Info);
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
                icon.Icon = NumberOnIcon(numOfNotifications);
            }
        }

        #region Notification number on icon

        public Icon NumberOnIcon(int number)
        {
            var bitmap = new Bitmap(32, 32);

            var numberdIcon = Properties.Resources.square_shape;
            var drawFont = new System.Drawing.Font("Calibri", 16, System.Drawing.FontStyle.Bold);
            var drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Crimson);

            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);

            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            graphics.DrawIcon(numberdIcon, 0, 0);

            graphics.DrawString(number.ToString(), drawFont, drawBrush, 1, 2);

            Icon createdIcon = Icon.FromHandle(bitmap.GetHicon());

            drawFont.Dispose();
            drawBrush.Dispose();
            graphics.Dispose();
            bitmap.Dispose();

            return createdIcon;
        }

        #endregion
    }
}
