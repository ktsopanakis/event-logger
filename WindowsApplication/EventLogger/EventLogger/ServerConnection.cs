using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using SimpleJson;
using SocketIOClient;
using Newtonsoft.Json;
using SocketIOClient.Messages;

namespace EventLogger
{
    public class ServerConnection
    {
        private static Uri WebSocketURI;
        private static Client socket;
        private static string message;
        public event EventHandler<string> MessageReceived;
        public bool IsAlive;

        void OnMessageReceived(string mess)
        {
            if (MessageReceived != null) MessageReceived(this, mess);
        }
        public void ConnectToServer()
        {
            WebSocketURI = new Uri("http://162.209.79.120:29017/");
            socket = new Client(WebSocketURI.ToString());
            socket.RetryConnectionAttempts = 500;

            socket.Opened += SocketOpened;
            socket.Error += SocketError;
            socket.SocketConnectionClosed += SocketConnectionClosed;
            //socket.Message += SocketMessage;
            
            System.Net.WebRequest.DefaultWebProxy = null;

            try
            {
                socket.Connect();
                IsAlive = socket.IsConnected;
            }
            catch
            {
            }
            ;
            socket.On("connect", fn => { Console.WriteLine("On connect message: " + fn.MessageText); });

            socket.On("open", fn => { Console.WriteLine("On open message" + fn.MessageText); });

            socket.On("initialList", fn =>
            {
                message = fn.Json.Args[0].ToString();
                OnMessageReceived(message);
            });

            socket.On("unreadErrorMessages", fn =>
            {
                message = fn.Json.Args[0].ToString();
                OnMessageReceived(message);
            });

        }


        public void Close()
        {
            socket.Close();
        }

        public void Send(dynamic message)
        {
            socket.Emit("messageRead", message);
        }
        
        #region Event Handlers

        void SocketOpened(object sender, EventArgs e)
        {
            Console.WriteLine("opened event handler");
            Console.WriteLine(e.ToString());
        }

        void SocketError(object sender, SocketIOClient.ErrorEventArgs e)
        {
            Console.WriteLine("error event handler");
            Console.WriteLine(e.Message);
            try
            {
                Console.WriteLine("Connection closed trying to reconnect...");
                socket.Connect();
                IsAlive = socket.IsConnected;
            }
            catch
            {
            }
        }

        void SocketConnectionClosed(object sender, EventArgs e)
        {
            Console.WriteLine("error event handler");
            Console.WriteLine(e.ToString());
            try
            {
                Console.WriteLine("Connection closed trying to reconnect...");
                socket.Connect();
                IsAlive = socket.IsConnected;
            }
            catch
            {
            }
        }

        #endregion
    }
}
