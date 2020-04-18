using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using ShiftPlan.Core.DataObjects;
using ShiftPlan.Core.DataObjects.Settings;
using ZimLabs.Database.MySql;

namespace ShiftPlan.Core.Data
{
    /// <summary>
    /// Provides several functions for the interaction with the shift plan data
    /// </summary>
    internal sealed class Repo : IDisposable
    {
        /// <summary>
        /// Contains the value which indicates if the class was already disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The instance for the connection
        /// </summary>
        private readonly Connector _connector;

        /// <summary>
        /// Creates a new instance of the <see cref="Repo"/>
        /// </summary>
        /// <param name="settings">The settings</param>
        public Repo(Database settings)
        {
            var tmpSettings = new DatabaseSettings
            {
                Server = settings.Server,
                Database = settings.Name,
                UserId = settings.User,
                Password = settings.Password.ToSecureString(),
                Port = (uint) settings.Port
            };

            _connector = new Connector(tmpSettings);
        }

        /// <summary>
        /// Loads all entries according to the given date
        /// </summary>
        /// <param name="start">The start date</param>
        /// <param name="end">The end date</param>
        /// <returns>The list with the page entries</returns>
        public List<PageEntry> LoadPageEntries(DateTime start, DateTime end)
        {
            const string query =
                @"SELECT
                    id,
                    `date`,
                    calendarWeek,
                    dateView,
                    person,
                    active,
                    creationDateTime,
                    modifiedDateTime
                FROM
                    plan
                WHERE
                    `date` BETWEEN @start AND @end;";

            return _connector.Connection.Query<PageEntry>(query, new
            {
                start,
                end
            }).ToList();
        }

        /// <summary>
        /// Saves a new entry
        /// </summary>
        /// <param name="entry">The page entry</param>
        public void Save(PageEntry entry)
        {
            if (entry == null)
                return;

            const string query = "INSERT INTO plan (date, calendarWeek, dateView, person, active, creationDateTime, modifiedDateTime) " +
                                 "VALUES (@date, @cw, @dateView, @person, @active, NOW(), NOW());" +
                                 "SELECT last_insert_id();";

            var newId = _connector.Connection.ExecuteScalar<int>(query, new
            {
                date = entry.Date,
                cw = entry.CalendarWeek,
                dateView = entry.DateView,
                person = entry.Person,
                active = entry.Active
            });

            entry.Id = newId;
        }

        /// <summary>
        /// Updates an entry
        /// </summary>
        /// <param name="entry">The entry which should be updated</param>
        public void Update(PageEntry entry)
        {
            const string query = "UPDATE plan SET person = @person, modifiedDateTime = NOW() WHERE id = @id;";

            _connector.Connection.Execute(query, new
            {
                person = entry.Person,
                id = entry.Id
            });
        }

        /// <summary>
        /// Set all entries till the given date inactive
        /// </summary>
        /// <param name="till">The till date</param>
        public void SetInactive(DateTime till)
        {
            const string query =
                @"UPDATE plan
                SET
                    active = 0
                WHERE
                    `date` < @till";

            _connector.Connection.Execute(query, new {till});
        }

        /// <summary>
        /// Disposes the class
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _connector?.Dispose();

            _disposed = true;
        }
    }
}
