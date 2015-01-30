using System;
using System.Collections;
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
        public ServerConnection serve;

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
                    var newMessages = errorMessage.Unread.SkipWhile(m => messages.Contains(m, AnonymousComparer.Create((Message ms)=> ms.Id)));
                    foreach(var m in newMessages)
                        messages.Add(m);
                }

            });
            
            OnIconUpdateNeeded(messages.Count(m => !m.IsRead));


           // UpdateIcon();

        }


        private void ErrorListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItems = (sender as System.Windows.Controls.ListView).SelectedItems;

            if (selectedItems != null)
            {
                foreach (var item in selectedItems)
                {
                    if (item != null)
                    {
                        messages.FirstOrDefault(m => m.Equals(item)).IsRead = true;

                        OnIconUpdateNeeded(messages.Count(m => !m.IsRead));

                        serve.Send(new
                        {
                            origin = ((Message)item).Origin,
                            id = ((Message)item).Id,
                            readFrom = System.Environment.MachineName + ":" + System.Environment.UserName
                        });
                    }
                }
            }
        }

        private void CopyButton_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedItems = ErrorListView.SelectedItems;
            string textToCopy="";

            if (selectedItems == null)
            {
                System.Windows.MessageBox.Show("You haven't selected any line to copy");
            }
            else
            {
                foreach (var item in selectedItems)
                {
                    var messageItem = item as Message;
                    messages.FirstOrDefault(m => m.Equals(messageItem)).IsRead = true;
                    textToCopy += messageItem.MessageToString() + Environment.NewLine;
                }

                System.Windows.Clipboard.SetText(textToCopy);

                System.Windows.MessageBox.Show("Your selection has been copied to the clipboard", "Copy to Clipboard",
                    MessageBoxButton.OK);
            }
        }

        private void MarkAllAsReadButton_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var message in messages)
            {
                if (!message.IsRead)
                {
                    message.IsRead = true;

                    serve.Send(new
                    {
                        origin = message.Origin,
                        id = message.Id,
                        readFrom = System.Environment.MachineName + ":" + System.Environment.UserName
                    });
                }
            }

            OnIconUpdateNeeded(messages.Count(m => !m.IsRead));
        }

        private void ClearReadLines_OnClick(object sender, RoutedEventArgs e)
        {
            List<Message> messagesToRemove = new List<Message>();

            foreach (var message in messages)
            {
                if (message.IsRead)
                {
                    messagesToRemove.Add(message);
                }
            }

            foreach (var message in messagesToRemove)
            {
                if (messages.Contains(message))
                    messages.Remove(message);
            }
        }
    }

}
