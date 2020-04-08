using System;
using System.IO;
using ShiftPlan.DataObjects;
using Newtonsoft.Json;

namespace ShiftPlan.Global
{
    /// <summary>
    /// Provides several helper functions
    /// </summary>
    internal static class Helper
    {
        /// <summary>
        /// Backing field for <see cref="Settings"/>
        /// </summary>
        private static Settings _settings;

        /// <summary>
        /// Gets the settings of the service
        /// </summary>
        public static Settings Settings => _settings ?? (_settings = LoadSettings());

        /// <summary>
        /// Loads the settings of the service
        /// </summary>
        /// <returns>The settings</returns>
        private static Settings LoadSettings()
        {
            var path = Path.Combine(ZimLabs.Utility.Global.GetBaseFolder(), "Settings.json");

            if (!File.Exists(path))
                throw new FileNotFoundException("The settings are missing.", path);

            var content = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<Settings>(content);
        }

        /// <summary>
        /// Saves the last run
        /// </summary>
        public static void SaveLastRun()
        {
            Settings.LastRun = DateTime.Now;

            var path = Path.Combine(ZimLabs.Utility.Global.GetBaseFolder(), "Settings.json");

            var content = JsonConvert.SerializeObject(Settings, Formatting.Indented);

            File.WriteAllText(path, content);
        }

        /// <summary>
        /// Converts the given string into the given object type
        /// </summary>
        /// <typeparam name="T">The type of the content</typeparam>
        /// <param name="content">The JSON formatted content</param>
        /// <param name="data">The data</param>
        /// <returns>true when successful, otherwise false</returns>
        public static bool ConvertJson<T>(string content, out T data)
        {
            if (string.IsNullOrEmpty(content))
            {
                data = default;
                return false;
            }

            try
            {
                data = JsonConvert.DeserializeObject<T>(content);
                return true;
            }
            catch (Exception ex)
            {
                ServiceLogger.Warn("An error has occured while parsing the mail body.", ex);
                data = default;
                return false;
            }
        }

        /// <summary>
        /// Calculates the calendar week of the given date
        /// </summary>
        /// <param name="date">The desired date</param>
        /// <returns>The calendar week</returns>
        public static int GetCalendarWeek(DateTime date)
        {
            var a = Math.Floor((14 - date.Month) / 12D);
            var y = date.Year + 4800 - a;
            var m = date.Month + 12 * a - 3;

            var jd = date.Day + Math.Floor((153 * m + 2) / 5) +
                365 * y + Math.Floor(y / 4) - Math.Floor(y / 100) +
                Math.Floor(y / 400) - 32045;

            var d4 = (jd + 31741 - jd % 7) % 146097 % 36524 %
                     1461;
            var L = Math.Floor(d4 / 1460);
            var d1 = (d4 - L) % 365 + L;

            // Kalenderwoche ermitteln
            var calendarWeek = (int) Math.Floor(d1 / 7) + 1;

            // Die ermittelte Kalenderwoche zurückgeben
            return calendarWeek;
        }
    }
}
