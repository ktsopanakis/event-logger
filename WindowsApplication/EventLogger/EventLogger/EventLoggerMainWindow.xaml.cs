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
using FirstFloor.ModernUI.Windows.Controls;

namespace EventLogger
{
    /// <summary>
    /// Interaction logic for EventLoggerMainWindow.xaml
    /// </summary>
    public partial class EventLoggerMainWindow : ModernWindow
    {
        AppIcon Icon = new AppIcon();

        public EventLoggerMainWindow()
        {
            InitializeComponent();

            this.WindowState = WindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Collapsed;

            System.Windows.Application.Current.Exit += Current_Exit;
            this.LoggerPage.IconUpdateNeeded += (sender, message) => Icon.UpdateIcon(message);

        }

       void Current_Exit(object sender, ExitEventArgs e)
       {
           Icon.icon.Dispose();
       }
     
       
       protected override void OnClosing( CancelEventArgs e)
       {
           e.Cancel = true;
       
           this.WindowState = WindowState.Minimized;
           this.ShowInTaskbar = false;
           
           this.LoggerPage.messages.Clear();
           
           Icon.UpdateIcon(0);
       }
       
    }
}
