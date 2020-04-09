namespace ShiftPlan.Core.DataObjects
{
    /// <summary>
    /// Provides the settings for the topshelf service
    /// </summary>
    public class ServiceSettings
    {
        /// <summary>
        /// Gets or sets the name of the service
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the service
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description of the service
        /// </summary>
        public string Description { get; set; }
    }
}
