using System;
using System.Linq;
using ShiftPlan.Core.Data;
using ShiftPlan.Core.DataObjects;

namespace ShiftPlan.Core.Business
{
    /// <summary>
    /// Provides the functions for the interaction with the page plan data
    /// </summary>
    public sealed class PagePlanManager : IDisposable
    {
        /// <summary>
        /// Contains the value which indicates if the class was already disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Contains the instance of the repo
        /// </summary>
        private readonly Repo _repo;

        /// <summary>
        /// Creates a new instance of the <see cref="PagePlanManager"/>
        /// </summary>
        public PagePlanManager()
        {
            // Init the repo
            _repo = new Repo(Helper.Settings.Database);
        }

        /// <summary>
        /// Saves the shift plan data
        /// </summary>
        /// <param name="data">The data</param>
        public void Save(ShiftPlanData data)
        {
            // Step 1: set all entries to inactive
            _repo.SetInactive(data.Start);

            // Step 2: Load all existing entries to check if there are already entries with the same date exists
            var entries = _repo.LoadPageEntries(data.Start, data.Days.Last().Date);

            // Step 2: Save the new data
            foreach (var dayEntry in data.Days)
            {
                var tmpEntry = entries.FirstOrDefault(f => f.Date.Day == dayEntry.Date.Day);
                if (tmpEntry != null)
                {
                    tmpEntry.Person = dayEntry.Value;
                    tmpEntry.Active = true;

                    _repo.Update(tmpEntry);
                }
                else
                {
                    _repo.Save((PageEntry) dayEntry);
                }
            }
        }

        /// <summary>
        /// Disposes the class
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            _repo?.Dispose();

            _disposed = true;
        }
    }
}
