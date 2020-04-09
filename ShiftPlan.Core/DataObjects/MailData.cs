namespace ShiftPlan.Core.DataObjects
{
    /// <summary>
    /// Represents the mail options
    /// </summary>
    public sealed class MailData
    {
        /// <summary>
        /// Gets or sets the receiver of the mail
        /// </summary>
        public string Receiver { get; set; }

        /// <summary>
        /// Gets or sets the subject of the mail
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body of the mail
        /// </summary>
        public string Body { get; set; }
    }
}
