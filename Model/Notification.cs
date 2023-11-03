using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMessage.Enum;
namespace TaskMessage.Model
{
    public class Notification
    {
        public string Id { get; set; }
        public bool IsRecurring { get; set; }
        public InfoRecurrence InfoRecurrence { get; set; }
        public List<Channel> Channels { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<Template> Templates { get; set; }
    }
}
