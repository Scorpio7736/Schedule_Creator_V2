using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Schedule_Creator_V2.Models
{
    public record ProfilePicture(int id, byte[]? picture)
    {
        public BitmapImage? GetBitmapImage()
        {
            if (picture == null)
            {
                using var stream = new MemoryStream(picture);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            else
            {
                return null;
            }
        }

        public static readonly ProfilePicture Empty = new ProfilePicture(0, Array.Empty<byte>());

    }
}
