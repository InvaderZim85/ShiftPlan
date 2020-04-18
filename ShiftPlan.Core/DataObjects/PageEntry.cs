using System;

namespace ShiftPlan.Core.DataObjects
{
    /// <summary>
    /// Represents a page entry
    /// </summary>
    public sealed class PageEntry
    {
        /// <summary>
        /// Gets or sets the id of the entry
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the calendar week
        /// </summary>
        public int CalendarWeek { get; set; }

        /// <summary>
        /// Gets or sets the date view entry
        /// </summary>
        public string DateView { get; set; }

        /// <summary>
        /// Gets or sets the name of the person
        /// </summary>
        public string Person { get; set; }

        /// <summary>
        /// Gets or sets the value which indicates if the entry is marked as active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Converts a <see cref="DayEntry"/> into a <see cref="PageEntry"/>
        /// </summary>
        /// <param name="entry">The <see cref="DayEntry"/></param>
        public static explicit operator PageEntry(DayEntry entry)
        {
            return new PageEntry
            {
                Date = entry.Date,
                CalendarWeek = entry.CalendarWeek,
                DateView = entry.DateView,
                Person = entry.Value,
                Active = true
            };
        }
    }
}
