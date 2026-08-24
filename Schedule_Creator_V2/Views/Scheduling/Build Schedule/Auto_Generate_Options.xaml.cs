using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace Schedule_Creator_V2
{
    public partial class Auto_Generate_Options : Window
    {
        private sealed class StaffExclusionOption
        {
            public int StaffId { get; init; }

            public string Name { get; init; } = string.Empty;

            public bool IsExcluded { get; set; }
        }

        private readonly ObservableCollection<StaffExclusionOption> _staffOptions = new ObservableCollection<StaffExclusionOption>();

        public int StaffPerDay { get; private set; }

        public HashSet<int> ExcludedStaffIds { get; } = new HashSet<int>();

        public Auto_Generate_Options()
        {
            InitializeComponent();

            List<Staff> allStaff = DatabaseRead.ReadStaff()
                .OrderBy(staff => staff.displayName)
                .ToList();

            foreach (Staff staff in allStaff)
            {
                _staffOptions.Add(new StaffExclusionOption
                {
                    StaffId = staff.id,
                    Name = staff.displayName,
                    IsExcluded = false
                });
            }

            StaffCheckboxList.ItemsSource = _staffOptions;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(StaffPerDayBox.Text, out int requestedStaffPerDay) || requestedStaffPerDay < 1)
            {
                Messages.Display(new Error(1005, "Please enter a valid number of staff per day (1 or greater)."));
                return;
            }

            StaffPerDay = requestedStaffPerDay;
            ExcludedStaffIds.Clear();

            foreach (StaffExclusionOption option in _staffOptions.Where(option => option.IsExcluded))
            {
                ExcludedStaffIds.Add(option.StaffId);
            }

            DialogResult = true;
            Close();
        }
    }
}
