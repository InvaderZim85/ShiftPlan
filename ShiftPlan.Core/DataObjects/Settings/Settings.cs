using System;

namespace ShiftPlan.Core.DataObjects.Settings
{
    /// <summary>
    /// Provides the settings 
    /// </summary>
    public sealed class Settings
    {
        /// <summary>
        /// Gets or sets the service settings
        /// </summary>
        public ServiceSettings Service { get; set; }

        /// <summary>
        /// Gets or sets the mail settings
        /// </summary>
        public MailSettings Mail { get; set; }

        /// <summary>
        /// Gets or sets the database settings
        /// </summary>
        public Database Database { get; set; }

        /// <summary>
        /// Gets or sets the check interval (in minutes)
        /// </summary>
        public int CheckInterval { get; set; }

        /// <summary>
        /// Gets or sets the date of the last execution
        /// </summary>
        public DateTime LastRun { get; set; }
    }
}
