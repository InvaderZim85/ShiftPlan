using System;
using System.Linq;
using System.Timers;
using ShiftPlan.Core;
using ShiftPlan.Core.Business;
using ShiftPlan.Core.DataObjects;

namespace ShiftPlan.Business
{
    /// <summary>
    /// Provides the logic for the interaction with the mail and the ftp
    /// </summary>
    internal sealed class ShiftPlanManager : IDisposable
    {
        /// <summary>
        /// Contains the value which indicates if the class was already disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Timer for the check interval
        /// </summary>
        private Timer _checkTimer;

        /// <summary>
        /// Contains the value which indicates if its the first run
        /// </summary>
        private bool _firstExecution = true;

        /// <summary>
        /// Contains the hash value of the mail body
        /// </summary>
        private string _bodyHash;

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
            var body = MailManager.GetMailContent(Helper.Settings.Mail, Helper.Settings.LastRun);
            
            if (string.IsNullOrEmpty(body))
                return;

            // Compare the hash code
            var currentHash = body.GetMd5();

            if (!string.IsNullOrEmpty(_bodyHash) && _bodyHash.Equals(currentHash))
            {
                ServiceLogger.Info("Content equals. Skip page creation.");
                return;
            }

            _bodyHash = currentHash;

            // Converts the body
            var data = ConvertMailBody(body);

            // Save the data
            using (var pagePlanManager = new PagePlanManager())
            {
                pagePlanManager.Save(data);
            }

            // Set the last run
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
            var dayCount = 0;
            foreach (var line in lines.Where(w => !w.StartsWith("#")))
            {
                if (count == 0)
                {
                    if (DateTime.TryParse(line, out var date))
                    {
                        result.Start = date;
                    }
                    else
                    {
                        ServiceLogger.Info("Can't parse data.");
                        return null;
                    }
                }
                else
                {
                    var holiday = line.Contains("|h");

                    var dayEntry = new DayEntry
                    {
                        Date = result.Start.AddDays(dayCount),
                        Value = line.Replace("|h", ""),
                        Type = holiday ? CustomEnums.DayType.Holiday : CustomEnums.DayType.Normal
                    };

                    result.Days.Add(dayEntry);
                    dayCount++;
                }

                count++;
            }

            return result;
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
