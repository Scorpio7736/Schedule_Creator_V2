using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using Schedule_Creator_V2.ExtensionMethods;
using System.Windows;
using System.Windows.Controls;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Avail.xaml
    /// </summary>
    public partial class Add_Avail : Page
    {
        public Add_Avail()
        {
            InitializeComponent();
            StaffComboBox.ItemsSource = DatabaseRead.ReadStaff();
        }

        private void FillGrid(object sender, RoutedEventArgs e)
        {
            List<Availability> availabilities = DatabaseRead.ReadAvailability();

            if (availabilities.Count > 0)
            {
                MakeNewRows(availabilities);
            }
            else
            {
                MakeNewRow();
            }
        }

        private void MakeNewRows(List<Availability> availabilities)
        {
            foreach (Availability avail in availabilities)
            {
                AvailGrid.Items.Add(new AvailRow(avail));
            }
        }

        private void MakeNewRow(object? sender = null, RoutedEventArgs? e = null)
        {
            AvailGrid.Items.Add(new AvailRow());
        }

        private void RemoveRow(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn)
            {
                return;
            }

            int index = AvailGrid.GetRowIndexFromButton(btn);

            if (index < 0 || index >= AvailGrid.Items.Count)
            {
                return;
            }

            if (AvailGrid.Items.Count > 1)
            {
                AvailGrid.Items.RemoveAt(index);
            }
        }


        private void Save_Btn_Click(object sender, RoutedEventArgs e)
        {

            foreach (AvailRow item in AvailGrid.Items)
            {

                if (item.IsThereNull() == false)
                {
                    int staffID = ((Staff)StaffComboBox.SelectedItem).id;

                    DatabaseCreate.RemoveAllAvailability(staffID);

                    DatabaseCreate.AddAvailability(
                        new Availability(
                        staffID,
                        (DayOfWeek)item.dayOfTheWeek.SelectedItem,
                        TimeOnly.FromDateTime(item.startTimePicker.Value.Value),
                        TimeOnly.FromDateTime(item.endTimePicker.Value.Value)
                    ));
                }
                else
                {
                    Messages.Display(
                        new Error(
                            1000,
                            "Null Value Error"
                        ));
                    return;
                }
            }

            Messages.Display(new Message("All availability data for this staff member has been saved.", "Availability Saved"));


        }

        private void Clear_Btn_Click(object sender, RoutedEventArgs e)
        {
            int staffID = ((Staff)StaffComboBox.SelectedItem).id;

            DatabaseCreate.RemoveAllAvailability(staffID);

            Cancel_Btn_Click();

            Messages.Display(new Message("All saved availability data for this staff member has been cleared.", "Availability Reset"));
        }

        private void Cancel_Btn_Click(object? sender = null, RoutedEventArgs? e = null)
        {
            while (AvailGrid.Items.Count > 0)
            {
                AvailGrid.Items.RemoveAt(0);
            }

            MakeNewRow();
        }

    }
}
