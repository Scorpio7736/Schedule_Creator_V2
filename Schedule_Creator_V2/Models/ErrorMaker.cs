using Schedule_Creator_V2.ExtensionMethods;
using Schedule_Creator_V2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Schedule_Creator_V2.Models
{
    class ErrorMaker
    {
        private Dictionary<PageInput, Label> _kvp;

        public ErrorMaker(PageInputsAndLabels kvp)
        {
            this._kvp = kvp.GetKeyValuePairs();
        }

        public ErrorMaker(PageInput input, Label label)
        {
            this._kvp = new Dictionary<PageInput, Label>(){{ input, label }};
        }

        public void MakeError()
        {
            List<string> errors = new List<string>();

            foreach (var input in this._kvp)
            {
                PageInput pageInput = input.Key;
                Label label = input.Value;


                if (pageInput.CheckNull() == false)
                {
                    label.ErrorOut();
                    errors.Add(pageInput.GetTitle());
                }
            }

            string errorString = $"Missing inputs: {string.Join(", ", errors)}";
            Messages.Display(new Error(1000, errorString));

        }

        public void WithVoidExceptionError()
        {
            List<string> errors = new List<string>();

            foreach (var input in this._kvp)
            {
                PageInput pageInput = input.Key;

                Label label = input.Value;


                if (pageInput.CheckNull() == false)
                {
                    label.ErrorOut();
                    errors.Add(pageInput.GetTitle());
                }
            }

            string errorString = $"Missing inputs: {string.Join(", ", errors)}";
            Messages.Display(new Error(1000, errorString));
        }

    }
}
