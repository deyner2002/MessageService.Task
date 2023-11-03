using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TaskMessage.Logic
{
    public class Email
    {
        private SmtpClient cliente;
        private static string correo = "bajaimes@unicesar.edu.co";
        private static string contrasenia = "zfad mctn msqz dncw";
        private static IConfiguration Configuration { get; set; }
        private MailMessage email;
        public Email()
        {
            
            cliente = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(correo, contrasenia)
            };
        }
        public void EnviarCorreo(string destinatario, string asunto, string mensaje, bool esHtlm = false)
        {
            email = new MailMessage(correo, destinatario, asunto, mensaje);
            email.IsBodyHtml = esHtlm;
            cliente.Send(email);
        }
        public void EnviarCorreo(MailMessage message)
        {
            cliente.Send(message);
        }
        public async Task EnviarCorreoAsync(MailMessage message)
        {
            await cliente.SendMailAsync(message);
        }

    }
}
