
using TaskMessage.Model;
using Newtonsoft.Json;



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
            notificacion.Contacts= _notificacion.Contacts;
            notificacion.Templates = _notificacion.Templates;
            notificacion.ObjectTemplate = _notificacion.ObjectTemplate;
            return notificacion;
        }

    }
}
