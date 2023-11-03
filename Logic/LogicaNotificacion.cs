using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TaskMessage.Model;
using System.Threading.Channels;
using Newtonsoft.Json;
using TaskMessage.Enum;
using System.Net.Mail;




namespace TaskMessage.Logic
{
    public class LogicaNotificacion
    {
        public Notification notificacion;
        public LogicaNotificacion(string cadenaJson) 
        {
            notificacion = MapearDeJson(cadenaJson);
        }
        //zfad mctn msqz dncw
        private Notification MapearDeJson(string cadenaJson)
        {
            var _notificacion = JsonConvert.DeserializeObject<Notification>(cadenaJson);
            notificacion = new Notification();
            notificacion.IsProgrammed = _notificacion.IsProgrammed;
            notificacion.ProgrammingInfo = _notificacion.ProgrammingInfo;
            notificacion.Channels = _notificacion.Channels;
            notificacion.Contacts= _notificacion.Contacts;
            notificacion.Templates = _notificacion.Templates;
            return notificacion;
        }

    }
}
