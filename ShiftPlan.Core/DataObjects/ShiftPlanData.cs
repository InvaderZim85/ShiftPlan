using System;
using System.Collections.Generic;

namespace ShiftPlan.Core.DataObjects
{
    /// <summary>
    /// Represents the shift data
    /// </summary>
    public sealed class ShiftPlanData
    {
        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets the day entries
        /// </summary>
        public List<DayEntry> Days { get; set; } = new List<DayEntry>();
    }
}
