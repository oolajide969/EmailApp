// See https://aka.ms/new-console-template for more information

using Microsoft.WindowsAzure.Storage.Table;

namespace EmailApp
{
    public class EmailAtt : TableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SenderEmail { get; set; }
        public string RecipientEmail { get; set; }
        public DateTime TimeSent { get; set; }
        public string TicketName { get; set; }
        public string TicketNumber { get; set; }
        public string TicketCost { get; set; }

        public string Subject { get; set; }
        public string Content { get; set; }
        public DateTime LastStatusUpdate { get; set; }
        public string Status { get; set; }
        public EmailAtt(string emailType, string emailSev)
        {
            this.PartitionKey = emailType;
            this.RowKey = emailSev;
        }
    }
}

