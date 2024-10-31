// See https://aka.ms/new-console-template for more information
using Azure.Communication.Email;
using Azure;
using Azure.Communication.Email.Models;
using Azure.Communication.Pipeline;
using Azure.Core;
using Azure.Core.Pipeline;
using System.Configuration;
using Azure.Data.Tables;
using Azure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using RazorEngine;
using RazorEngine.Templating; // For extension methods.
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EmailApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var json = "{  FirstName: '{firstname}', LastName: '{lastname}', Tickets: [  {Name: 'Ticket 1', Number: '1', Cost: '$2'}, {Name: 'Ticket 2', Number: '2', Cost: '$2'}  ]  }";

            var converter = new ExpandoObjectConverter();
            DynamicViewBag message = JsonConvert.DeserializeObject<DynamicViewBag>(json, converter);



            var emailSubject = "Hello, This is a test";
            var emailBody =
                @"<html>
                  <body>
                    Hi @Model.FirstName @Model.LastName, 
                    You ordered @Model.Tickets.Count tickets, The ticket(s) you ordered have been successfully added to pool for the week.
                    Ticket Details @foreach(var ticket in Model.Tickets){<p> Ticket Name: @ticket.Name, Ticket Number: @ticket.Number, Ticket Cost:  @ticket.Cost</p>}   
                    Good luck.
                  </body>
                  </html> "; 
            var senderEmail = "{email}";

            var obj1 = new EmailAtt("Test", "Test14")
            {       
                SenderEmail = senderEmail,
                RecipientEmail = "(email)",
                TimeSent = DateTime.Now,
                Content = emailBody,
                Subject = emailSubject,
                LastStatusUpdate = DateTime.Now,
                Status = "Unknown",



            };
            var obj2 = new EmailAtt("Test", "Test8")
            {
                FirstName = "First",
                LastName = "Last",
                SenderEmail = senderEmail,
                RecipientEmail = "{email}",
                TimeSent = DateTime.Now,
                Content = emailBody,
                Subject = emailSubject,
                LastStatusUpdate = DateTime.Now,
                Status = "Unknown",

            };

            var objList = new List<EmailAtt>
            {
                obj1, obj2
            };

            //var mailBody = Engine.Razor.RunCompile(emailBody, "templateKey", typeof(EmailAtt), obj1);
            var mailBody = Engine.Razor.RunCompile(emailBody, "templateKey", null, message);
            obj1.Content = mailBody;
            Console.WriteLine(mailBody);
            var inserted = new EmailPush().SendEmail(obj1);

            for (int i = 0; i < objList.Count; i++)
            {
                //var mailBody = Engine.Razor.RunCompile(emailBody, "templateKey", typeof(EmailAtt), objList[i]);
                //objList[i].Content = mailBody;
                //Console.WriteLine(mailBody);

                //var inserted = new EmailPush().SendEmail(objList[i]);   
            }


        }
    }
}

