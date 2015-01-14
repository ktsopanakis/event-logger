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
using SocketIOClient;

namespace EventLogger
{
    /// <summary>
    /// Interaction logic for EventLoggerMainWindow.xaml
    /// </summary>
    public partial class EventLoggerMainWindow : Window
    {
        System.Windows.Forms.NotifyIcon icon = new NotifyIcon();

        public EventLoggerMainWindow()
        {
            InitializeComponent();
            icon.Icon = Properties.Resources.favicon;
            icon.Visible = true;
            icon.ShowBalloonTip(5000, "hello", "world", ToolTipIcon.Info);
            icon.Click += icon_Click;

            ConnectToServer();
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

        public void ConnectToServer()
        {
            Uri WebSocketURI = new Uri("http://panacea-ts.dotbydot.eu:1337/");
            Client socket = new Client(WebSocketURI.ToString());

            socket.Opened += SocketOpened;
            socket.Error += SocketError;
            socket.Message += SocketMessage;

            socket.Connect();

            socket.On("connect", fn => { Console.WriteLine("On connect message: " + fn.MessageText); });

            socket.On("open", fn => { Console.WriteLine("On open message" + fn.MessageText);});

            
        }

        static void SocketOpened(object sender, EventArgs e)
        {
            Console.WriteLine("opened event handler");
            Console.WriteLine(e.ToString());
        }

        static void SocketError(object sender, SocketIOClient.ErrorEventArgs e)
        {
            Console.WriteLine("error event handler");
            Console.WriteLine(e.Message);
        }

        static void SocketMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("message event handler");
            Console.WriteLine(e.Message);
        }

    }
}
