using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Schedule_Creator_V2.Models;
using Schedule_Creator_V2.Services;
using Xceed.Wpf.Toolkit;

namespace Schedule_Creator_V2
{
    /// <summary>
    /// Interaction logic for Add_Job_Settings.xaml
    /// </summary>
    public partial class Add_Job_Settings : Page
    {
        private PageInputsAndLabels GetInputsAndLabels()
        {
            List<PageInput> inputs = new List<PageInput>()
                {
                    new PageInput(EveryDayCheckBox, "Every Day Box", true),

                };

            List<Label> labels = new List<Label>()
                {

                };

            return new PageInputsAndLabels(inputs, labels);
        }

        public Add_Job_Settings()
        {
            InitializeComponent();

        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }


    }
}
