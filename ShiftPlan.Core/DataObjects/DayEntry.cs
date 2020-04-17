using System;
using Newtonsoft.Json;

namespace ShiftPlan.Core.DataObjects
{
    /// <summary>
    /// Represents a day entry
    /// </summary>
    public sealed class DayEntry
    {
        /// <summary>
        /// Gets or sets the number / sequence
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets the date of the day
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the value of the day (person or name of the holiday)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the type of the day (default = normal)
        /// </summary>
        public CustomEnums.DayType Type { get; set; } = CustomEnums.DayType.Normal;

        /// <summary>
        /// Backing field for <see cref="IsHoliday"/>
        /// </summary>
        private bool _isHoliday;

        /// <summary>
        /// Gets or sets the value which indicates if the day is a holiday or not
        /// </summary>
        [JsonIgnore]
        public bool IsHoliday
        {
            get => _isHoliday;
            set
            {
                _isHoliday = value;
                Type = value ? CustomEnums.DayType.Holiday : CustomEnums.DayType.Normal;
            }
        }

        #region Properties for the Person Control (only view)

        /// <summary>
        /// Gets the date (only for the view)
        /// </summary>
        [JsonIgnore]
        public string DateView => $"{Date.DayOfWeek}, {Date:dd.MM.yyyy}";

        /// <summary>
        /// Gets the week number (only for the view)
        /// </summary>
        [JsonIgnore]
        public string CalendarWeek => Helper.GetCalendarWeek(Date).ToString();

        /// <summary>
        /// Gets or sets the value which indicates if the entry is enabled or not
        /// </summary>
        [JsonIgnore]
        public bool Enabled => Type != CustomEnums.DayType.Weekend;
        #endregion
    }
}
