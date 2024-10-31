using Azure;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailApp
{
    public class EmailPush
    {
        public bool SendEmail(EmailAtt emailAtt)
        {
            bool isSent = false;
            var connectionString = ConfigurationManager.ConnectionStrings["Email"].ConnectionString;
            EmailClient emailClient = new EmailClient(connectionString);

            EmailContent emailContent = new EmailContent(emailAtt.Subject);

            emailContent.Html = emailAtt.Content;

            List<EmailAddress> emailAddresses = new List<EmailAddress>
            {
               new EmailAddress(emailAtt.RecipientEmail){ DisplayName = "{name}"}
            };
            EmailRecipients emailRecipients = new EmailRecipients(emailAddresses);
            EmailMessage emailMessage = new EmailMessage(emailAtt.SenderEmail, emailContent, emailRecipients);

            SendEmailResult emailResult = emailClient.Send(emailMessage, CancellationToken.None);

            Console.WriteLine($"MessageId = {emailResult.MessageId}");
            Response<SendStatusResult> messageStatus = null; 
            messageStatus = emailClient.GetSendStatus(emailResult.MessageId);
            Console.WriteLine(messageStatus);
            Console.WriteLine($"MessageStatus = {messageStatus.Value.Status}");
            emailAtt.Status = messageStatus.Value.Status.ToString();
            UpdateTable(emailAtt);
            TimeSpan duration = TimeSpan.FromMinutes(3);
            long start = DateTime.Now.Ticks;
            do
            {
                messageStatus = emailClient.GetSendStatus(emailResult.MessageId);
                if (messageStatus.Value.Status != SendStatus.Queued)
                {
                    Console.WriteLine($"MessageStatus = {messageStatus.Value.Status}");
                    emailAtt.Status = messageStatus.Value.Status.ToString();
                    emailAtt.LastStatusUpdate = DateTime.Now;
                    UpdateTable(emailAtt);
                    isSent = true;
                    break;
                }
                Thread.Sleep(10000);
                Console.WriteLine($"...");

            } while (DateTime.Now.Ticks - start < duration.Ticks);

            return isSent;
        }

        public bool UpdateTable(EmailAtt emailAtt)
        {
            var connString = ConfigurationManager.AppSettings["StorageConnectionString"];
            CloudStorageAccount account = CloudStorageAccount.Parse(connString);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("EmailSent");


            bool isUpdated = false;
            var insertBatchOperation = new TableBatchOperation();
            insertBatchOperation.InsertOrReplace(emailAtt);
            table.ExecuteBatchAsync(insertBatchOperation);


            return isUpdated;
        }
    }
}
