using System;
using System.Linq;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using ShiftPlan.Global;

namespace ShiftPlan.Business
{
    /// <summary>
    /// Provides the logic for the interaction with the mail server
    /// </summary>
    internal static class MailManager
    {
        /// <summary>
        /// Checks the mails for a given mail
        /// </summary>
        public static string GetMailContent()
        {
            try
            {
                ServiceLogger.Info("Check mails...");
                using (var client = new ImapClient())
                {
                    // Create a connection
                    client.Connect(Helper.Settings.Mail.Incoming, Helper.Settings.Mail.Port, true);
                    client.Authenticate(Helper.Settings.Mail.User, Helper.Settings.Mail.Password);

                    // Get the inbox
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly);

                    // Get only specified messages
                    var fromDate = Helper.Settings.LastRun;
                    var query = SearchQuery.DeliveredAfter(fromDate)
                        .And(SearchQuery.SubjectContains(Helper.Settings.Mail.Indicator));

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
    }
}
