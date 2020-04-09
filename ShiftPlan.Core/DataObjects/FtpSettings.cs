namespace ShiftPlan.Core.DataObjects
{
    /// <summary>
    /// Represents the ftp settings
    /// </summary>
    public sealed class FtpSettings : UserEntry
    {
        /// <summary>
        /// Gets or sets the address of the ftp server
        /// </summary>
        public string Server { get; set; }
    }
}
