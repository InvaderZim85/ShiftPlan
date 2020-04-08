using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using ShiftPlan.DataObjects;
using ShiftPlan.Global;

namespace ShiftPlan.Business
{
    /// <summary>
    /// Provides the logic for the interaction with the mail server
    /// </summary>
    public sealed class MailManager : IDisposable
    {
        /// <summary>
        /// Timer for the check interval
        /// </summary>
        private Timer _checkTimer;

        /// <summary>
        /// Contains the value which indicates if its the first run
        /// </summary>
        private bool _firstExecution = true;

        /// <summary>
        /// Contains the value which indicates if the class was already disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Init the mail manager
        /// </summary>
        public void Run()
        {
            SetTimer(_firstExecution);
        }

        /// <summary>
        /// Stops the execution
        /// </summary>
        public void Stop()
        {
            _checkTimer?.Stop();
        }

        /// <summary>
        /// Init the timer
        /// </summary>
        /// <param name="firstExecution">true when the timer should set for the first time (interval = 1 min), otherwise false (normal interval which is configured in the settings)</param>
        private void SetTimer(bool firstExecution)
        {
            var interval = TimeSpan.FromMinutes(firstExecution ? 1 : Helper.Settings.CheckInterval).TotalMilliseconds;

            _checkTimer = new Timer(interval);
            _checkTimer.Elapsed += CheckTimer_Elapsed;
            _checkTimer.Start();
        }

        /// <summary>
        /// Checks the mails for a given mail
        /// </summary>
        private void CheckMail()
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
                var orderBy = new[] {OrderBy.ReverseArrival};

                // Get the mails
                var mails = inbox.Sort(query, orderBy);

                if (!mails.Any())
                {
                    ServiceLogger.Info("No mail available.");
                    return;
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
                    return;
                }


                CreateHtmlPage(body);
            }
        }

        /// <summary>
        /// Creates the html page
        /// </summary>
        /// <param name="content">The content</param>
        private void CreateHtmlPage(string content)
        {
            ServiceLogger.Info("Create html page");

            // Step 1: Try to convert the body
            if (!Helper.ConvertJson<ShiftPlanData>(content, out var data))
            {
                ServiceLogger.Warn("The content of the mail has the wrong format.");
                return;
            }

            var personList = new SortedList<DateTime, string>();

            var count = 0;
            var dateCount = 0;
            var endDate = data.Start;
            while (count < data.Persons.Count)
            {
                var date = data.Start.AddDays(dateCount);
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    personList.Add(date, date.DayOfWeek.ToString());
                    dateCount++;
                    continue;
                }

                personList.Add(date, data.Persons[count]);

                endDate = date;
                dateCount++;
                count++;
            }

            // Create the table body
            var tableBody = new StringBuilder();

            foreach (var entry in personList)
            {
                if (entry.Key.DayOfWeek == DayOfWeek.Saturday || entry.Key.DayOfWeek == DayOfWeek.Sunday)
                    tableBody.AppendLine("<tr class=\"table-dark text-dark\">");
                else
                    tableBody.AppendLine("<tr>");

                tableBody.AppendLine($"<td>{Helper.GetCalendarWeek(entry.Key)}</td>");
                tableBody.AppendLine($"<td>{entry.Key:dddd, dd.MM.yyyy}</td>");
                tableBody.AppendLine($"<td>{entry.Value}</td></tr>");
            }

            // Load the html template
            var template = LoadTemplate();

            template = template.Replace("@Content.Start", data.Start.ToString("dd.MM.yyyy"));
            template = template.Replace("@Content.End", endDate.ToString("dd.MM.yyyy"));
            template = template.Replace("@Content.Table", tableBody.ToString());
            template = template.Replace("@Content.CreationDate", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));

            var file = SaveData(template);

            // Upload the file
            FtpManager.Upload(file);

            Helper.SaveLastRun();
        }

        /// <summary>
        /// Loads the template
        /// </summary>
        /// <returns>The content of the template</returns>
        private string LoadTemplate()
        {
            var path = Path.Combine(ZimLabs.Utility.Global.GetBaseFolder(), "index.html");

            return File.ReadAllText(path);
        }

        /// <summary>
        /// Saves the content
        /// </summary>
        /// <param name="content">The content</param>
        /// <returns>The destination file</returns>
        private FileInfo SaveData(string content)
        {
            var dir = Path.Combine(ZimLabs.Utility.Global.GetBaseFolder(), "files");
            Directory.CreateDirectory(dir);

            var file = Path.Combine(dir, $"ShiftPlan_{DateTime.Now:yyyyMMdd_HHmmss}.html");

            File.WriteAllText(file, content);

            return new FileInfo(file);
        }

        /// <summary>
        /// Occurs when the timer elapsed
        /// </summary>
        private void CheckTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Stop the timer to prevent multiple runs
            _checkTimer.Enabled = false;

            try
            {
                CheckMail();
            }
            catch (Exception ex)
            {
                ServiceLogger.Error("An error has occured.", ex);
            }

            // Check if the was executed the first time
            if (_firstExecution)
            {
                SetTimer(false);
                _firstExecution = false;
            }
            else
            {
                // Resume the timer
                _checkTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Disposes the class
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _checkTimer?.Dispose();

            _disposed = true;
        }
    }
}
