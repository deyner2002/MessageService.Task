using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMessage.Enum;

namespace TaskMessage.Model
{
    public class InfoRecurrence
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool Active { get; set; }
        public DateTime ActivationTime { get; set; }
        public Recurrence Recurrence { get; set; }
    }
}
