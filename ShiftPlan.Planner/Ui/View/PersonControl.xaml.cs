using System.Windows;
using ShiftPlan.Core.DataObjects;

namespace ShiftPlan.Planner.Ui.View
{
    /// <summary>
    /// Interaction logic for PersonControl.xaml
    /// </summary>
    public partial class PersonControl
    {
        /// <summary>
        /// The dependency property of <see cref="Entry"/>
        /// </summary>
        public static readonly DependencyProperty EntryProperty = DependencyProperty.Register(
            nameof(Entry), typeof(DayEntry), typeof(PersonControl), new PropertyMetadata(default(DayEntry)));

        /// <summary>
        /// Gets or sets the day entry
        /// </summary>
        public DayEntry Entry
        {
            get => (DayEntry) GetValue(EntryProperty);
            set => SetValue(EntryProperty, value);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="PersonControl"/>
        /// </summary>
        public PersonControl()
        {
            InitializeComponent();
        }
    }
}
