using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Models.Records;
using Schedule_Creator_V2.Services.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for View_Schedule.xaml
    /// </summary>
    public partial class View_Schedule : Page
    {
        private readonly Dictionary<int, string>
            _staffNameCache = new();

        public View_Schedule()
        {
            InitializeComponent();

            ScheduleComboBox.ItemsSource =
                DatabaseRead.ReadAllScheduleNames();

            SetVisibility();
        }

        private void SetVisibility()
        {
            /*
             * Reset every column first in case the job settings
             * have changed since the page was last loaded.
             */
            MonCol.Visibility = Visibility.Hidden;
            TueCol.Visibility = Visibility.Hidden;
            WedCol.Visibility = Visibility.Hidden;
            ThuCol.Visibility = Visibility.Hidden;
            FriCol.Visibility = Visibility.Hidden;
            SatCol.Visibility = Visibility.Hidden;
            SunCol.Visibility = Visibility.Hidden;

            List<DayOfWeek> jobDays =
                DatabaseRead.ReadJobSettingsDays();

            foreach (DayOfWeek day in jobDays)
            {
                switch (day)
                {
                    case DayOfWeek.Monday:
                        MonCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Tuesday:
                        TueCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Wednesday:
                        WedCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Thursday:
                        ThuCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Friday:
                        FriCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Saturday:
                        SatCol.Visibility =
                            Visibility.Visible;
                        break;

                    case DayOfWeek.Sunday:
                        SunCol.Visibility =
                            Visibility.Visible;
                        break;
                }
            }
        }

        private void FillBoxes(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (ScheduleComboBox.SelectedItem
                    is not string scheduleName ||
                string.IsNullOrWhiteSpace(scheduleName))
            {
                ScheduleGrid.ItemsSource =
                    Array.Empty<ViewScheduleRow>();

                return;
            }

            List<ScheduleRow> savedShifts =
                DatabaseRead.ReadScheduleByScheduleName(
                    scheduleName);

            if (savedShifts.Count == 0)
            {
                ScheduleGrid.ItemsSource =
                    Array.Empty<ViewScheduleRow>();

                return;
            }

            /*
             * Group the shifts by day.
             *
             * Each day's shifts are sorted by their start time.
             * The first shift from each day appears in row one,
             * the second shift appears in row two, and so on.
             */
            Dictionary<DayOfWeek, List<ScheduleRow>>
                shiftsByDay =
                    savedShifts
                        .GroupBy(shift =>
                            shift.dayOfWeek)
                        .ToDictionary(
                            group => group.Key,
                            group => group
                                .OrderBy(shift =>
                                    shift.startTime)
                                .ThenBy(shift =>
                                    shift.endTime)
                                .ThenBy(shift =>
                                    shift.staffID)
                                .ToList());

            int displayRowCount =
                shiftsByDay.Values
                    .Max(shifts => shifts.Count);

            List<ViewScheduleRow> displayRows =
                new();

            for (int rowIndex = 0;
                 rowIndex < displayRowCount;
                 rowIndex++)
            {
                ViewScheduleRow displayRow =
                    new();

                ApplyShift(
                    displayRow,
                    DayOfWeek.Monday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Monday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Tuesday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Tuesday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Wednesday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Wednesday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Thursday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Thursday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Friday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Friday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Saturday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Saturday,
                        rowIndex));

                ApplyShift(
                    displayRow,
                    DayOfWeek.Sunday,
                    GetShiftAtIndex(
                        shiftsByDay,
                        DayOfWeek.Sunday,
                        rowIndex));

                displayRows.Add(displayRow);
            }

            ScheduleGrid.ItemsSource =
                displayRows;
        }

        private static ScheduleRow? GetShiftAtIndex(
            Dictionary<DayOfWeek, List<ScheduleRow>>
                shiftsByDay,
            DayOfWeek day,
            int index)
        {
            if (!shiftsByDay.TryGetValue(
                    day,
                    out List<ScheduleRow>? shifts))
            {
                return null;
            }

            if (index < 0 ||
                index >= shifts.Count)
            {
                return null;
            }

            return shifts[index];
        }

        private void ApplyShift(
            ViewScheduleRow displayRow,
            DayOfWeek day,
            ScheduleRow? shift)
        {
            if (shift is null)
            {
                return;
            }

            string staffName =
                shift.staffID.HasValue
                    ? GetStaffName(
                        shift.staffID.Value)
                    : "Missing";

            switch (day)
            {
                case DayOfWeek.Monday:
                    displayRow.AvailMon =
                        staffName;

                    displayRow.MonStartTime =
                        shift.startTime;

                    displayRow.MonEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Tuesday:
                    displayRow.AvailTue =
                        staffName;

                    displayRow.TueStartTime =
                        shift.startTime;

                    displayRow.TueEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Wednesday:
                    displayRow.AvailWed =
                        staffName;

                    displayRow.WedStartTime =
                        shift.startTime;

                    displayRow.WedEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Thursday:
                    displayRow.AvailThu =
                        staffName;

                    displayRow.ThuStartTime =
                        shift.startTime;

                    displayRow.ThuEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Friday:
                    displayRow.AvailFri =
                        staffName;

                    displayRow.FriStartTime =
                        shift.startTime;

                    displayRow.FriEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Saturday:
                    displayRow.AvailSat =
                        staffName;

                    displayRow.SatStartTime =
                        shift.startTime;

                    displayRow.SatEndTime =
                        shift.endTime;
                    break;

                case DayOfWeek.Sunday:
                    displayRow.AvailSun =
                        staffName;

                    displayRow.SunStartTime =
                        shift.startTime;

                    displayRow.SunEndTime =
                        shift.endTime;
                    break;
            }
        }

        private string GetStaffName(
            int staffId)
        {
            if (_staffNameCache.TryGetValue(
                    staffId,
                    out string? cachedName))
            {
                return cachedName;
            }

            Staff staff =
                DatabaseRead.ReadStaffByID(
                    staffId);

            string displayName =
                string.IsNullOrWhiteSpace(
                    staff.displayName)
                    ? $"Staff ID: {staffId}"
                    : staff.displayName;

            _staffNameCache[staffId] =
                displayName;

            return displayName;
        }

        private void CopyScheduleBase64Button_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (ScheduleGrid.Items.Count == 0)
            {
                MessageBox.Show(
                    "Select a schedule before copying its image.",
                    "No Schedule Selected",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                ScheduleComboBox.Focus();
                ScheduleComboBox.IsDropDownOpen =
                    true;

                return;
            }

            Button? copyButton =
                sender as Button;

            try
            {
                if (copyButton is not null)
                {
                    copyButton.IsEnabled =
                        false;
                }

                string imageDataUri =
                    CreateScheduleGridPngDataUri();

                CopyTextToClipboard(
                    imageDataUri);

                MessageBox.Show(
                    "The schedule image was converted to PNG " +
                    "Base64 and copied to the clipboard.\n\n" +
                    "You can paste it directly into an email " +
                    "image source field.",
                    "Schedule Image Copied",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    "The schedule image could not be copied.\n\n" +
                    exception.Message,
                    "Schedule Image Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                if (copyButton is not null)
                {
                    copyButton.IsEnabled =
                        true;
                }
            }
        }

        private string CreateScheduleGridPngDataUri()
        {
            double originalWidth =
                ScheduleGrid.Width;

            double originalHeight =
                ScheduleGrid.Height;

            double originalMaxWidth =
                ScheduleGrid.MaxWidth;

            double originalMaxHeight =
                ScheduleGrid.MaxHeight;

            bool originalRowVirtualization =
                ScheduleGrid.EnableRowVirtualization;

            bool originalColumnVirtualization =
                ScheduleGrid.EnableColumnVirtualization;

            ScrollBarVisibility originalHorizontalScrollBarVisibility =
                ScheduleGrid.HorizontalScrollBarVisibility;

            ScrollBarVisibility originalVerticalScrollBarVisibility =
                ScheduleGrid.VerticalScrollBarVisibility;

            object? originalSelectedItem =
                ScheduleGrid.SelectedItem;

            List<DataGridColumn> excludedColumns =
                ScheduleGrid.Columns
                    .Where(IsExportExcludedColumn)
                    .ToList();

            Dictionary<DataGridColumn, Visibility>
                originalColumnVisibilities =
                    excludedColumns.ToDictionary(
                        column => column,
                        column => column.Visibility);

            try
            {
                foreach (DataGridColumn column in excludedColumns)
                {
                    column.Visibility =
                        Visibility.Collapsed;
                }

                ScheduleGrid.UnselectAll();

                ScheduleGrid.EnableRowVirtualization =
                    false;

                ScheduleGrid.EnableColumnVirtualization =
                    false;

                ScheduleGrid.HorizontalScrollBarVisibility =
                    ScrollBarVisibility.Disabled;

                ScheduleGrid.VerticalScrollBarVisibility =
                    ScrollBarVisibility.Disabled;

                ScheduleGrid.UpdateLayout();

                double imageHeight =
                    CalculateScheduleImageHeight();

                /*
                 * Keep the grid at its current displayed width while rendering.
                 * The extra scrollbar gutter will be cropped afterward.
                 */
                double renderWidth =
                    Math.Max(
                        1,
                        ScheduleGrid.ActualWidth);

                ScheduleGrid.Width =
                    renderWidth;

                ScheduleGrid.Height =
                    imageHeight;

                ScheduleGrid.MaxWidth =
                    renderWidth;

                ScheduleGrid.MaxHeight =
                    imageHeight;

                ScheduleGrid.Measure(
                    new Size(
                        renderWidth,
                        imageHeight));

                ScheduleGrid.Arrange(
                    new Rect(
                        0,
                        0,
                        renderWidth,
                        imageHeight));

                ScheduleGrid.UpdateLayout();

                /*
                 * Only include the combined width of visible columns.
                 * This removes the blank scrollbar/delete-column gutter.
                 */
                double visibleColumnsWidth =
                    ScheduleGrid.Columns
                        .Where(column =>
                            column.Visibility ==
                            Visibility.Visible)
                        .Sum(column =>
                            column.ActualWidth);

                DpiScale dpi =
                    VisualTreeHelper.GetDpi(
                        ScheduleGrid);

                int renderPixelWidth =
                    Math.Max(
                        1,
                        (int)Math.Ceiling(
                            renderWidth *
                            dpi.DpiScaleX));

                int renderPixelHeight =
                    Math.Max(
                        1,
                        (int)Math.Ceiling(
                            imageHeight *
                            dpi.DpiScaleY));

                RenderTargetBitmap fullBitmap =
                    new RenderTargetBitmap(
                        renderPixelWidth,
                        renderPixelHeight,
                        dpi.PixelsPerInchX,
                        dpi.PixelsPerInchY,
                        PixelFormats.Pbgra32);

                fullBitmap.Render(
                    ScheduleGrid);

                int croppedPixelWidth =
                    Math.Max(
                        1,
                        (int)Math.Ceiling(
                            visibleColumnsWidth *
                            dpi.DpiScaleX));

                croppedPixelWidth =
                    Math.Min(
                        croppedPixelWidth,
                        fullBitmap.PixelWidth);

                CroppedBitmap croppedBitmap =
                    new CroppedBitmap(
                        fullBitmap,
                        new Int32Rect(
                            0,
                            0,
                            croppedPixelWidth,
                            fullBitmap.PixelHeight));

                PngBitmapEncoder encoder =
                    new PngBitmapEncoder();

                encoder.Frames.Add(
                    BitmapFrame.Create(
                        croppedBitmap));

                using MemoryStream stream =
                    new MemoryStream();

                encoder.Save(
                    stream);

                string base64 =
                    Convert.ToBase64String(
                        stream.ToArray());

                return
                    "data:image/png;base64," +
                    base64;
            }
            finally
            {
                foreach (
                    KeyValuePair<DataGridColumn, Visibility>
                        columnVisibility
                    in originalColumnVisibilities)
                {
                    columnVisibility.Key.Visibility =
                        columnVisibility.Value;
                }

                ScheduleGrid.Width =
                    originalWidth;

                ScheduleGrid.Height =
                    originalHeight;

                ScheduleGrid.MaxWidth =
                    originalMaxWidth;

                ScheduleGrid.MaxHeight =
                    originalMaxHeight;

                ScheduleGrid.EnableRowVirtualization =
                    originalRowVirtualization;

                ScheduleGrid.EnableColumnVirtualization =
                    originalColumnVirtualization;

                ScheduleGrid.HorizontalScrollBarVisibility =
                    originalHorizontalScrollBarVisibility;

                ScheduleGrid.VerticalScrollBarVisibility =
                    originalVerticalScrollBarVisibility;

                if (originalSelectedItem is not null &&
                    ScheduleGrid.Items.Contains(
                        originalSelectedItem))
                {
                    ScheduleGrid.SelectedItem =
                        originalSelectedItem;
                }

                ScheduleGrid.InvalidateMeasure();
                ScheduleGrid.InvalidateArrange();
                ScheduleGrid.UpdateLayout();
            }
        }

        private double CalculateScheduleImageWidth()
        {
            double visibleColumnsWidth =
                ScheduleGrid.Columns
                    .Where(column =>
                        column.Visibility ==
                        Visibility.Visible)
                    .Sum(column =>
                        Math.Max(
                            column.ActualWidth,
                            column.MinWidth));

            /*
             * The additional pixels prevent the final cell
             * border from being clipped.
             */
            return Math.Ceiling(
                Math.Max(
                    ScheduleGrid.ActualWidth,
                    visibleColumnsWidth + 4));
        }

        private static bool IsExportExcludedColumn(
    DataGridColumn column)
        {
            string header =
                column.Header?
                    .ToString()?
                    .Trim()
                ?? string.Empty;

            return
                header.Equals(
                    "Remove",
                    StringComparison.OrdinalIgnoreCase) ||
                header.Equals(
                    "Delete",
                    StringComparison.OrdinalIgnoreCase);
        }

        private double CalculateScheduleImageHeight()
        {
            double rowHeight =
                ScheduleGrid.RowHeight;

            if (double.IsNaN(rowHeight) ||
                rowHeight <= 0)
            {
                rowHeight = 96;
            }

            double headerHeight =
                ScheduleGrid.ColumnHeaderHeight;

            if (double.IsNaN(headerHeight) ||
                headerHeight <= 0)
            {
                headerHeight = 48;
            }

            double calculatedHeight =
                headerHeight +
                (ScheduleGrid.Items.Count *
                 rowHeight) +
                4;

            return Math.Ceiling(
                Math.Max(
                    calculatedHeight,
                    ScheduleGrid.ActualHeight));
        }

        private static void CopyTextToClipboard(
            string text)
        {
            const int maximumAttempts = 5;

            for (int attempt = 1;
                 attempt <= maximumAttempts;
                 attempt++)
            {
                try
                {
                    Clipboard.SetDataObject(
                        text,
                        true);

                    return;
                }
                catch (COMException)
                    when (attempt < maximumAttempts)
                {
                    /*
                     * Another application can briefly lock
                     * the Windows clipboard.
                     */
                    Thread.Sleep(75);
                }
            }

            throw new InvalidOperationException(
                "Windows did not allow access to the clipboard.");
        }
    }
}