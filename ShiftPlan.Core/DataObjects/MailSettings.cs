namespace ShiftPlan.Core.DataObjects
{
    /// <summary>
    /// Represents the mail settings
    /// </summary>
    public sealed class MailSettings : UserEntry
    {
        /// <summary>
        /// Gets or sets the address of the incoming mail server
        /// </summary>
        public string Incoming { get; set; }

        /// <summary>
        /// Gets or sets the address of the outgoing mail server
        /// </summary>
        public string Outgoing { get; set; }

        /// <summary>
        /// Gets or sets the indicator of the mail subject. When a mail with this indicator was found, the content will be loaded
        /// </summary>
        public string Indicator { get; set; }

        /// <summary>
        /// Gets or sets the port of the imap server
        /// </summary>
        public int Port { get; set; }
    }
}
