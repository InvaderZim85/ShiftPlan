using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MahApps.Metro.Controls.Dialogs;
using ShiftPlan.Core;
using ShiftPlan.Core.DataObjects;
using ZimLabs.WpfBase;

namespace ShiftPlan.Planner.Ui.ViewModel
{
    /// <summary>
    /// Provides the logic for the main window
    /// </summary>
    internal sealed class MainWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Contains the instance of the mah apps dialog coordinator
        /// </summary>
        private IDialogCoordinator _dialogCoordinator;

        /// <summary>
        /// Backing field for <see cref="StartDate"/>
        /// </summary>
        private DateTime _startDate = DateTime.Now;

        /// <summary>
        /// Gets or sets the start date
        /// </summary>
        public DateTime StartDate
        {
            get => _startDate;
            set => SetField(ref _startDate, value);
        }

        /// <summary>
        /// Backing field for <see cref="WorkDays"/>
        /// </summary>
        private int _workDays = 10;

        /// <summary>
        /// Gets or sets the amount of work days
        /// </summary>
        public int WorkDays
        {
            get => _workDays;
            set => SetField(ref _workDays, value);
        }

        /// <summary>
        /// Backing field for <see cref="DayEntries"/>
        /// </summary>
        private ObservableCollection<DayEntry> _dayEntries;

        /// <summary>
        /// Gets or sets the list with the day entries
        /// </summary>
        public ObservableCollection<DayEntry> DayEntries
        {
            get => _dayEntries;
            set => SetField(ref _dayEntries, value);
        }

        /// <summary>
        /// Backing field for <see cref="IsEnabled"/>
        /// </summary>
        private bool _isEnabled;

        /// <summary>
        /// Gets or sets the value which indicates if the input mask is enabled or not
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetField(ref _isEnabled, value);
        }

        /// <summary>
        /// Init the view model
        /// </summary>
        /// <param name="dialogCoordinator">The instance of the mah apps dialog coordinator</param>
        public void InitViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;
        }

        /// <summary>
        /// The command to set the values
        /// </summary>
        public ICommand SetCommand => new DelegateCommand(SetValues);

        /// <summary>
        /// The command to reset the values
        /// </summary>
        public ICommand ResetCommand => new DelegateCommand(Reset);

        /// <summary>
        /// The command to send the data
        /// </summary>
        public ICommand SendCommand => new DelegateCommand(Send);

        /// <summary>
        /// Sets the values according to the given date and work day count
        /// </summary>
        private void SetValues()
        {
            var tmpList = new List<DayEntry>(WorkDays);
            var count = 0;
            var dayCount = 0;
            while (count < WorkDays)
            {
                var date = StartDate.AddDays(dayCount++);

                DayEntry dayEntry;
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    dayEntry = new DayEntry
                    {
                        Number = dayCount,
                        Date = date,
                        Value = date.DayOfWeek.ToString(),
                        Type = CustomEnums.DayType.Weekend
                    };
                }
                else
                {
                    dayEntry = new DayEntry
                    {
                        Number = dayCount,
                        Date = date,
                        Value = ""
                    };
                }

                tmpList.Add(dayEntry);
                if (dayEntry.Type != CustomEnums.DayType.Weekend)
                    count++;
            }

            IsEnabled = tmpList.Any();
            DayEntries = new ObservableCollection<DayEntry>(tmpList);
        }

        /// <summary>
        /// Resets the input
        /// </summary>
        private void Reset()
        {
            WorkDays = 10;
            StartDate = DateTime.Now;
            DayEntries = new ObservableCollection<DayEntry>();
            IsEnabled = false;
        }

        /// <summary>
        /// Sends the data via mail...
        /// </summary>
        private async void Send()
        {
            // Step 1: Create the JSON object
            var data = new
            {
                Start = StartDate,
                Days = DayEntries
            };

            var jsonContent = data.ToJson();

            await _dialogCoordinator.ShowMessageAsync(this, "JSON Data", jsonContent);
        }
    }
}
