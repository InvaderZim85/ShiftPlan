using System;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using ShiftPlan.Core.DataObjects;

namespace ShiftPlan.Core.Business
{
    /// <summary>
    /// Provides the logic for the interaction with the mail server
    /// </summary>
    public static class MailManager
    {
        /// <summary>
        /// Checks the mails for a given mail
        /// </summary>
        /// <param name="settings">The mail settings</param>
        /// <param name="fromDate">The start date</param>
        public static string GetMailContent(MailSettings settings, DateTime fromDate)
        {
            try
            {
                ServiceLogger.Info("Check mails...");
                using (var client = new ImapClient())
                {
                    // Create a connection
                    client.Connect(settings.Incoming, settings.Port, true);
                    client.Authenticate(settings.User, settings.Password);

                    // Get the inbox
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    // Get only specified messages
                    var query = SearchQuery.DeliveredAfter(fromDate)
                        .And(SearchQuery.SubjectContains(settings.Indicator));

                    // Set the order
                    var orderBy = new[] { OrderBy.ReverseArrival };

                    // Get the mails
                    var mails = inbox.Sort(query, orderBy);

                    if (!mails.Any())
                    {
                        ServiceLogger.Info("No mail available.");
                        return "";
                    }

                    // Get the "latest" mail id
                    var mailId = mails.FirstOrDefault();

                    // Get the message
                    var message = inbox.GetMessage(mailId);

                    // Get the content
                    var body = message.TextBody;

                    if (string.IsNullOrEmpty(body))
                    {
                        ServiceLogger.Info("Content is empty.");
                        return "";
                    }

                    return body;
                }
            }
            catch (Exception ex)
            {
                ServiceLogger.Error("An error has occured while checking the mails.", ex);
                return "";
            }
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="settings">The settings</param>
        /// <param name="data">The mail data</param>
        public static void SendMail(MailSettings settings, MailData data)
        {

        }
    }
}
