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
            //var accountName = "thelottofactorystorage";
            //var accountKey = "G5ODjspiF3XYWScQQjhn+gLbO5oI3vx5TFuLPxvEhI4hK7UD+GCMAsZUcKdPzyOhhaVI1SLjRP0/ctkE/1G8qg==";
            //var creds = new StorageCredentials(accountName, accountKey);
            //var account = new CloudStorageAccount(creds, useHttps: true);
            //var connectionString = "https://tlf-comm-service.communication.azure.com/;accesskey=R9CnmGixfsRScejJqpHxaKVfO0Fg4O5xbCv28dMy5lQaXjLXRJO/MUW7CQ7M/Lbqi2zytHVFCl1uBcvRkiXk/g=="


            var json = "{  FirstName: 'Matt', LastName: 'Comeau', Tickets: [  {Name: 'Ticket 1', Number: '1', Cost: '$2'}, {Name: 'Ticket 2', Number: '2', Cost: '$2'}  ]  }";
            //var dataInputtedNew = json.Replace("[", "").Replace("]", "").Replace(" ", "");
            //var objData = JsonConvert.DeserializeObject<JsonOutput>(dataInputtedNew);

            var converter = new ExpandoObjectConverter();
            DynamicViewBag message = JsonConvert.DeserializeObject<DynamicViewBag>(json, converter);
            //var mailBody = Engine.Razor.RunCompile(emailBody, "templateKey", null, message);



            //string[] FirstName = objData.FirstName.Split(',');
            //string[] LastName = objData.LastName.Split(',');
            //string[] Tickets = objData.Tickets.Split(',');

            //string ticketsJson = string.Join(",", Tickets);

            //var objTicket = JsonConvert.DeserializeObject<Tickets>(ticketsJson);

            //string[] Name = objTicket.Name.Split(',');
            //string[] Number = objTicket.Number.Split(',');
            //string[] Cost = objTicket.Cost.Split(',');


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
            var senderEmail = "donotreply@comms.thelottofactory.com";

            //@foreach(var ticketNumber in @Model.Tickets){ <br> @ticketNumber <br>}

            var obj1 = new EmailAtt("Test", "Test14")
            {       
                SenderEmail = senderEmail,
                RecipientEmail = "matty@thelottofactory.com",
                TimeSent = DateTime.Now,
                Content = emailBody,
                Subject = emailSubject,
                LastStatusUpdate = DateTime.Now,
                Status = "Unknown",



            };
            var obj2 = new EmailAtt("Test", "Test8")
            {
                FirstName = "Matt",
                LastName = "Comeau",
                SenderEmail = senderEmail,
                RecipientEmail = "matty@thelottofactory.com",
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

