using Schedule_Creator_V2.ExtensionMethods;
using System.Windows.Controls;

namespace Schedule_Creator_V2.Models
{
    class PageInputsAndLabels
    {
        private Dictionary<PageInput, Label> _kvp;

        public PageInputsAndLabels(List<PageInput> input, List<Label> label)
        {
            this._kvp = new Dictionary<PageInput, Label>();

            for (int i = 0; i < input.Count; i++)
            {
                this._kvp.Add(input[i], label[i]);
            }
        }

        public Dictionary<PageInput, Label> GetKeyValuePairs()
        {
            return this._kvp;
        }
        public bool CheckIfNulls()
        {
            
            foreach (PageInput kvp in _kvp.Keys)
            {
                if (kvp.CheckNull() == false)
                {
                    return false;
                }
            }
            return true;
        }

        public void ResetLabels()
        {
            foreach (Label label in _kvp.Values)
            {
                label.Reset();
            }
        }

        public void ResetInputs()
        {
            foreach (PageInput input in _kvp.Keys)
            {
                input.ResetInput();
            }
        }
        public void ResetAll()
        {
            ResetInputs();
            ResetLabels();
        }


    }
}