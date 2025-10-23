using Schedule_Creator_V2.Models;
using System.Windows;

namespace Schedule_Creator_V2.Services
{
    internal class Messages
    {
        /// <summary>
        /// General messagebox pop up.
        /// </summary>
        /// <param name="msg"></param>
        public static void Display(Message msg)
        {
            MessageBox.Show(msg.message, msg.caption);
        }

        /// <summary>
        /// Used to inform the user on an error. This includes a code for reference and an error message that is specific to the message box.
        /// </summary>
        /// <param name="error"></param>
        public static void Display(Error error)
        {
            MessageBox.Show($"Error {error.code} - {error.message}", "Error!");
        }
    }
}

/* Error List:
 * 1000 - Null value error.
 * 1002 - missing functionality.
 *
 *
 *
 *
 *
 *
 *
 */
