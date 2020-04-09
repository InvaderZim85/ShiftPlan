using System;

namespace ShiftPlan.Core.DataObjects
{
    /// <summary>
    /// Represents a day entry
    /// </summary>
    public sealed class DayEntry
    {
        /// <summary>
        /// Gets or sets the date of the day
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the value of the day (person or name of the holiday)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the value which indicates if the day is a holiday
        /// </summary>
        public bool Holiday { get; set; }
    }
}
