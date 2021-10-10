using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFChat.MVVM.Model
{
    class UserModel
    {
        public string Username { get; set; }
        public string UID { get; set; }
        public string ImageSource { get; set; }
        public ObservableCollection<MessageModel> Messages { get; set; }
        public string Status { get; set; }
    }
}
