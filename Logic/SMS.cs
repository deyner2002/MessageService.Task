using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace TaskMessage.Logic
{
    public class SMS
    {
        

        public SMS() { }

        public void EnviarMensaje(string telefono,string mensaje)
        {
            var accountSid = "AC7e02c382deabe7e92f2f10b88bce1294";
            var authToken = "c9c6dc0617e5c0c93287bb5782459279";
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber("+573145313922"));
            messageOptions.From = new PhoneNumber("+12015375970");
            messageOptions.Body = mensaje;


            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);
        }

    }
}
