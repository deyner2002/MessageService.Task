using Newtonsoft.Json.Linq;
using TaskMessage.Enum;
namespace TaskMessage.Model
{
    public class Notification
    {
        public bool IsProgrammed { get; set; }
        public ProgrammingInfo ProgrammingInfo { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<Template> Templates { get; set; }
        public bool GetObject { get; set; }
        public string? GetObjectUrl { get; set; }
        public string? ObjectTemplate { get; set; }
    }
}
