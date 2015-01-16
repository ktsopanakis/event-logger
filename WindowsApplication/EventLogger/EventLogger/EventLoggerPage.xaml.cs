﻿using System;
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
using System.Windows.Markup;
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
        public EventHandler<int> IconUpdateNeeded;
        private ServerConnection serve;

        void OnIconUpdateNeeded(int numOfNotifications)
        {
            if (IconUpdateNeeded != null) IconUpdateNeeded(this, numOfNotifications);
        }

        public EventLoggerPage()
        {
            InitializeComponent();
            
            serve = new ServerConnection();
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
                if(!messages.Any())
                    errorMessage.Unread.ForEach(m => messages.Add(m));
                else
                {
                    messages = new ObservableCollection<Message>(errorMessage.Unread);
                }

            });
            

            OnIconUpdateNeeded(messages.Count(m => !m.IsRead));


           // UpdateIcon();

        }


        private void ErrorListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as System.Windows.Controls.ListView).SelectedItem;

            var selectedIndex = (sender as System.Windows.Controls.ListView).SelectedIndex;

            messages.ElementAt(selectedIndex).IsRead = true;

            OnIconUpdateNeeded(messages.Count(m => !m.IsRead));

            if (selectedItem != null)
            {
                serve.Send(new
                {
                    origin = ((Message)selectedItem).Origin,
                    id = ((Message)selectedItem).Id,
                    readFrom = System.Environment.MachineName + ":" + System.Environment.UserName
                });
            }
            
        }
    }

}