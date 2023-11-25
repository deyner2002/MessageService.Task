using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace TaskMessage.Logic
{
    public class Whatsapp
    {
        public Whatsapp() { }
        async public void EnviarMensaje(string telefono, string texto)
        {
            //Token
            string token = "EAAJSPTZA1iz8BO0JPuLTkrOc6F5iIleL9haL3JvygDaQ3wkxUEsYTkm2Ft7UXFnWY7ZChGyN32QMzZALGcZCR1arWMZBvJLqFgHNtoWDx4A1f5aItpz1YcTM4IAqWYZBSGevVFyIBYZAKplv4KvZAKxSMoSy8ZCCl5BymhXbaB4Bj9ZAOJXFkGpcccCvMS8V6aFGSW0oSFqc1jDfdhfh5mZCwZDZD";
            //Identificador de número de teléfono
            string idTelefono = "142915658913096";
            //Nuestro telefono
            telefono = "57" + telefono;
            telefono = "573145313922";
            //Nombre de la plantilla
            //Console.WriteLine(templateName);
            string templateName = "hello_world";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v17.0/" + idTelefono + "/messages");
            request.Headers.Add("Authorization", "Bearer " + token);
            var payload = new
            {
                messaging_product = "whatsapp",
                to = telefono,
                type = "template",
                template = new
                { 
                    name = templateName,
                    language = new
                    {
                        code = "en_US"
                    }
                }
            };
            var serializedPayload = JsonConvert.SerializeObject(payload);
            request.Content = new StringContent(serializedPayload, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);

            // Declara la variable responseBody en el ámbito global
            string responseBody;

            // Lee el cuerpo de la respuesta
            responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                // Mensaje enviado correctamente
                Console.WriteLine("Mensaje enviado correctamente");
            }
            else
            {
                // Se produjo un error
                Console.WriteLine("Error al enviar el mensaje:", responseBody);
            }
        }
    }
}
