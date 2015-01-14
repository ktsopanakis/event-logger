using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketIOClient;
using Newtonsoft.Json;

namespace EventLogger
{
    public static class ServerConnection
    {
        private static Uri WebSocketURI;
        private static Client socket;
        private static string messages;

        public static string ConnectToServer()
        {
            WebSocketURI = new Uri("http://162.209.79.120:29017/");
            socket = new Client(WebSocketURI.ToString());

            socket.Opened += SocketOpened;
            socket.Error += SocketError;
            //socket.Message += SocketMessage;

            System.Net.WebRequest.DefaultWebProxy = null;

            try
            {
                socket.Connect();
            }
            catch
            {
            }
            ;
            socket.On("connect", fn => { Console.WriteLine("On connect message: " + fn.MessageText); });

            socket.On("open", fn => { Console.WriteLine("On open message" + fn.MessageText); });

            
            socket.On("unreadErrorMessages", fn =>
            {
                messages = fn.Json.Args[0];
            });

            return messages;

        }

        public static string DataFromServer()
        {
            return "";
        }


        #region Event Handlers

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

       //static void SocketMessage(object sender, MessageEventArgs e)
       //{
       //    Console.WriteLine("message event handler");
       //    Console.WriteLine(e.Message);
       //}

        #endregion
    }
}
