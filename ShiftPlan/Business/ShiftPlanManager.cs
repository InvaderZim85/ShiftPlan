using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using ShiftPlan.DataObjects;
using ShiftPlan.Global;
using ZimLabs.Utility.Extensions;

namespace ShiftPlan.Business
{
    /// <summary>
    /// Provides the logic for the interaction with the mail and the ftp
    /// </summary>
    internal sealed class ShiftPlanManager : IDisposable
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
            var minInterval = firstExecution ? 1 : Helper.Settings.CheckInterval;

            ServiceLogger.Info($"Timer set. Interval: {minInterval}");

            var interval = TimeSpan.FromMinutes(minInterval).TotalMilliseconds;

            _checkTimer = new Timer(interval);
            _checkTimer.Elapsed += CheckTimer_Elapsed;
            _checkTimer.Start();
        }

        /// <summary>
        /// Checks the mails for a given mail
        /// </summary>
        private void CheckMail()
        {
            var body = MailManager.GetMailContent();

            if (string.IsNullOrEmpty(body))
                return;

            CreateHtmlPage(body);
        }

        /// <summary>
        /// Creates the html page
        /// </summary>
        /// <param name="content">The content</param>
        private void CreateHtmlPage(string content)
        {
            ServiceLogger.Info("Create html page");

            // Step 1: Try to convert the body
            var data = ConvertMailBody(content);

            var personList = new List<DayEntry>();

            var count = 0;
            var dateCount = 0;
            var endDate = data.Start;
            while (count < data.Days.Count)
            {
                var date = data.Start.AddDays(dateCount);
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    var dayEntry = new DayEntry
                    {
                        Date = date,
                        Value = date.DayOfWeek.ToString(),
                        Holiday = true
                    };

                    personList.Add(dayEntry);
                    dateCount++;
                    continue;
                }

                var tmpDayEntry = data.Days[count];
                tmpDayEntry.Date = date;

                personList.Add(tmpDayEntry);

                endDate = date;
                dateCount++;
                count++;
            }

            // Create the table body
            var tableBody = new StringBuilder();

            foreach (var entry in personList)
            {
                if (entry.Holiday)
                    tableBody.AppendLine("<tr class=\"table-dark text-dark\">");
                else
                    tableBody.AppendLine("<tr>");

                tableBody.AppendLine($"<td>{Helper.GetCalendarWeek(entry.Date)}</td>");
                tableBody.AppendLine($"<td>{entry.Date.DayOfWeek}, {entry.Date:dd.MM.yyyy}</td>");
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
        /// Converts the mail body
        /// </summary>
        /// <param name="body">The mail body (plain text)</param>
        /// <returns></returns>
        private ShiftPlanData ConvertMailBody(string body)
        {
            if (Helper.ConvertJson<ShiftPlanData>(body, out var data))
            {
                return data;
            }

            ServiceLogger.Info("Mail body is not JSON. Try custom format");

            // The custom format is: 
            // First line: start date
            // Second line and following: persons
            var lines = body.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            var result = new ShiftPlanData();

            var count = 0;
            foreach (var line in lines)
            {
                if (count == 0)
                {
                    result.Start = line.ToDateTime();
                }
                else
                {
                    var holiday = line.Contains("|h");

                    var dayEntry = new DayEntry
                    {
                        Value = line.Replace("|h", ""),
                        Holiday = holiday
                    };

                    result.Days.Add(dayEntry);
                }

                count++;
            }

            return result;
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
