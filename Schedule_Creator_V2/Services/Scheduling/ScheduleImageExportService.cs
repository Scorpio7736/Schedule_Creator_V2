using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Schedule_Creator_V2.Services.Scheduling
{
    public static class ScheduleImageExportService
    {
        // =========================================================
        // CREATE PNG
        // =========================================================

        public static byte[] CreatePng(
            DataGrid scheduleGrid)
        {
            ArgumentNullException.ThrowIfNull(
                scheduleGrid);

            double originalWidth =
                scheduleGrid.Width;

            double originalHeight =
                scheduleGrid.Height;

            double originalMaxWidth =
                scheduleGrid.MaxWidth;

            double originalMaxHeight =
                scheduleGrid.MaxHeight;

            bool originalRowVirtualization =
                scheduleGrid
                    .EnableRowVirtualization;

            bool originalColumnVirtualization =
                scheduleGrid
                    .EnableColumnVirtualization;

            ScrollBarVisibility
                originalHorizontalScrollBarVisibility =
                    scheduleGrid
                        .HorizontalScrollBarVisibility;

            ScrollBarVisibility
                originalVerticalScrollBarVisibility =
                    scheduleGrid
                        .VerticalScrollBarVisibility;

            object? originalSelectedItem =
                scheduleGrid.SelectedItem;

            List<DataGridColumn> excludedColumns =
                scheduleGrid.Columns
                    .Where(
                        IsExportExcludedColumn)
                    .ToList();

            Dictionary<
                DataGridColumn,
                Visibility>
                originalColumnVisibilities =
                    excludedColumns.ToDictionary(
                        column =>
                            column,

                        column =>
                            column.Visibility);

            try
            {
                PrepareGridForExport(
                    scheduleGrid,
                    excludedColumns);

                double imageHeight =
                    CalculateImageHeight(
                        scheduleGrid);

                double renderWidth =
                    Math.Max(
                        1,
                        scheduleGrid.ActualWidth);

                ArrangeGridForExport(
                    scheduleGrid,
                    renderWidth,
                    imageHeight);

                double visibleColumnsWidth =
                    scheduleGrid.Columns
                        .Where(column =>
                            column.Visibility ==
                            Visibility.Visible)
                        .Sum(column =>
                            column.ActualWidth);

                DpiScale dpi =
                    VisualTreeHelper.GetDpi(
                        scheduleGrid);

                RenderTargetBitmap fullBitmap =
                    RenderGrid(
                        scheduleGrid,
                        renderWidth,
                        imageHeight,
                        dpi);

                CroppedBitmap croppedBitmap =
                    CropToVisibleColumns(
                        fullBitmap,
                        visibleColumnsWidth,
                        dpi);

                return EncodePng(
                    croppedBitmap);
            }
            finally
            {
                RestoreGrid(
                    scheduleGrid,
                    originalWidth,
                    originalHeight,
                    originalMaxWidth,
                    originalMaxHeight,
                    originalRowVirtualization,
                    originalColumnVirtualization,
                    originalHorizontalScrollBarVisibility,
                    originalVerticalScrollBarVisibility,
                    originalSelectedItem,
                    originalColumnVisibilities);
            }
        }


        // =========================================================
        // FILE NAME
        // =========================================================

        public static string BuildFileName(
            string? scheduleName)
        {
            string fileName =
                scheduleName?.Trim()
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(
                    fileName))
            {
                fileName =
                    "Schedule";
            }

            foreach (char invalidCharacter
                     in Path.GetInvalidFileNameChars())
            {
                fileName =
                    fileName.Replace(
                        invalidCharacter,
                        '_');
            }

            return
                fileName + ".png";
        }


        // =========================================================
        // PREPARE GRID
        // =========================================================

        private static void PrepareGridForExport(
            DataGrid scheduleGrid,
            IEnumerable<DataGridColumn> excludedColumns)
        {
            foreach (DataGridColumn column
                     in excludedColumns)
            {
                column.Visibility =
                    Visibility.Collapsed;
            }

            scheduleGrid.UnselectAll();

            scheduleGrid.EnableRowVirtualization =
                false;

            scheduleGrid.EnableColumnVirtualization =
                false;

            scheduleGrid
                .HorizontalScrollBarVisibility =
                    ScrollBarVisibility.Disabled;

            scheduleGrid
                .VerticalScrollBarVisibility =
                    ScrollBarVisibility.Disabled;

            scheduleGrid.UpdateLayout();
        }


        // =========================================================
        // ARRANGE GRID
        // =========================================================

        private static void ArrangeGridForExport(
            DataGrid scheduleGrid,
            double renderWidth,
            double imageHeight)
        {
            scheduleGrid.Width =
                renderWidth;

            scheduleGrid.Height =
                imageHeight;

            scheduleGrid.MaxWidth =
                renderWidth;

            scheduleGrid.MaxHeight =
                imageHeight;

            scheduleGrid.Measure(
                new Size(
                    renderWidth,
                    imageHeight));

            scheduleGrid.Arrange(
                new Rect(
                    0,
                    0,
                    renderWidth,
                    imageHeight));

            scheduleGrid.UpdateLayout();
        }


        // =========================================================
        // RENDER
        // =========================================================

        private static RenderTargetBitmap RenderGrid(
            DataGrid scheduleGrid,
            double renderWidth,
            double imageHeight,
            DpiScale dpi)
        {
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

            RenderTargetBitmap bitmap =
                new RenderTargetBitmap(
                    renderPixelWidth,
                    renderPixelHeight,
                    dpi.PixelsPerInchX,
                    dpi.PixelsPerInchY,
                    PixelFormats.Pbgra32);

            bitmap.Render(
                scheduleGrid);

            return bitmap;
        }


        // =========================================================
        // CROP
        // =========================================================

        private static CroppedBitmap
            CropToVisibleColumns(
                RenderTargetBitmap bitmap,
                double visibleColumnsWidth,
                DpiScale dpi)
        {
            int croppedPixelWidth =
                Math.Max(
                    1,
                    (int)Math.Ceiling(
                        visibleColumnsWidth *
                        dpi.DpiScaleX));

            croppedPixelWidth =
                Math.Min(
                    croppedPixelWidth,
                    bitmap.PixelWidth);

            return new CroppedBitmap(
                bitmap,
                new Int32Rect(
                    0,
                    0,
                    croppedPixelWidth,
                    bitmap.PixelHeight));
        }


        // =========================================================
        // ENCODE
        // =========================================================

        private static byte[] EncodePng(
            BitmapSource bitmap)
        {
            PngBitmapEncoder encoder =
                new PngBitmapEncoder();

            encoder.Frames.Add(
                BitmapFrame.Create(
                    bitmap));

            using MemoryStream stream =
                new MemoryStream();

            encoder.Save(
                stream);

            return stream.ToArray();
        }


        // =========================================================
        // IMAGE HEIGHT
        // =========================================================

        private static double CalculateImageHeight(
            DataGrid scheduleGrid)
        {
            double rowHeight =
                scheduleGrid.RowHeight;

            if (double.IsNaN(
                    rowHeight) ||
                rowHeight <= 0)
            {
                rowHeight =
                    96;
            }

            double headerHeight =
                scheduleGrid
                    .ColumnHeaderHeight;

            if (double.IsNaN(
                    headerHeight) ||
                headerHeight <= 0)
            {
                headerHeight =
                    48;
            }

            double calculatedHeight =
                headerHeight +
                (scheduleGrid.Items.Count *
                 rowHeight) +
                4;

            return Math.Ceiling(
                Math.Max(
                    calculatedHeight,
                    scheduleGrid.ActualHeight));
        }


        // =========================================================
        // EXCLUDED COLUMNS
        // =========================================================

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


        // =========================================================
        // RESTORE GRID
        // =========================================================

        private static void RestoreGrid(
            DataGrid scheduleGrid,
            double originalWidth,
            double originalHeight,
            double originalMaxWidth,
            double originalMaxHeight,
            bool originalRowVirtualization,
            bool originalColumnVirtualization,
            ScrollBarVisibility
                originalHorizontalScrollBarVisibility,
            ScrollBarVisibility
                originalVerticalScrollBarVisibility,
            object? originalSelectedItem,
            IReadOnlyDictionary<
                DataGridColumn,
                Visibility>
                originalColumnVisibilities)
        {
            foreach (
                KeyValuePair<
                    DataGridColumn,
                    Visibility>
                    columnVisibility
                in originalColumnVisibilities)
            {
                columnVisibility
                    .Key
                    .Visibility =
                        columnVisibility.Value;
            }

            scheduleGrid.Width =
                originalWidth;

            scheduleGrid.Height =
                originalHeight;

            scheduleGrid.MaxWidth =
                originalMaxWidth;

            scheduleGrid.MaxHeight =
                originalMaxHeight;

            scheduleGrid.EnableRowVirtualization =
                originalRowVirtualization;

            scheduleGrid.EnableColumnVirtualization =
                originalColumnVirtualization;

            scheduleGrid
                .HorizontalScrollBarVisibility =
                    originalHorizontalScrollBarVisibility;

            scheduleGrid
                .VerticalScrollBarVisibility =
                    originalVerticalScrollBarVisibility;

            if (originalSelectedItem is not null &&
                scheduleGrid.Items.Contains(
                    originalSelectedItem))
            {
                scheduleGrid.SelectedItem =
                    originalSelectedItem;
            }

            scheduleGrid.InvalidateMeasure();
            scheduleGrid.InvalidateArrange();
            scheduleGrid.UpdateLayout();
        }
    }
}