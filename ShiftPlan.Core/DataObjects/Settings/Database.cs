namespace ShiftPlan.Core.DataObjects.Settings
{
    /// <summary>
    /// Represents the database settings
    /// </summary>
    public sealed class Database : UserEntry
    {
        /// <summary>
        /// Gets or sets the name of the server
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the name of the database
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the port
        /// </summary>
        public int Port { get; set; } = 3306;
    }
}
