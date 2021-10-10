using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFChat.Core;
using WPFChat.MVVM.Model;
using WPFChat.MVVM.Net;

namespace WPFChat.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public ObservableCollection<MessageModel> Messages { get; set; }
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<UserModel> Contacts { get; set; }

        /* Commands */
        public RelayCommand SendCommand { get; set; }
        public RelayCommand ConnectToServerCommand { get; set; }

        private Server _server;
        private UserModel _selectedContact { get; set; }

        public UserModel SelectedContact
        {
            get { return _selectedContact; }
            set 
            { 
                _selectedContact = value;
                OnPropertyChanged();
            }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
            set 
            { 
                _message = value;
                OnPropertyChanged();
            }
        }

        private string _search;

        public string Search
        {
            get { return _search; }
            set
            {
                _search = value;
                OnPropertyChanged();
            }
        }

        private string _username;

        public string username{
            get { return _username; }
            set 
            { 
                _username = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _server = new Server();
            _server.ConnectedEvent += UserConnected;
            _server.MsgReceivedEvent += MessageReceived;
            _server.UserDisconnectedEvent += RemoveUser;

            Messages = new ObservableCollection<MessageModel>();
            Users = new ObservableCollection<UserModel>();

            SendCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));

            //Contacts.Add(new UserModel
            //{
            //    Username = "Main",
            //    Status = "Hi im using CHAT",
            //    ImageSource = "/Icons/profile-placeholder.png",
            //    Messages = Messages
            //});
        }

        private void RemoveUser()
        {
            var uid = _server.packetReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var msg = _server.packetReader.ReadMessage();

            Application.Current.Dispatcher.Invoke(() => Messages.Add(AddNewMessage()));
        }

        private void UserConnected()
        {
            username = _server.packetReader.ReadMessage();

            var user = new UserModel
            {
                Username = username,
                UID = _server.packetReader.ReadMessage()
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }

        private MessageModel AddNewMessage()
        {
            return new MessageModel
            {
                Username = username,
                UsernameColor = "#409aff",
                ImageSource = "/Icons/profile-placeholder.png",
                Message = Message,
                Time = DateTime.Now,
                IsNativeOrigin = true,
                FirstMessage = true
            };
        }
    }
}
