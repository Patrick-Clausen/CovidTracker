using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DatabaseLibrary;

namespace CovidTrackerApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new GeneralTabViewModel(this);
        }

        private void NumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SelectorSex_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)((ComboBoxItem)SexBox.SelectedItem).Content == "Female")
            {
                ((CitizenTabViewModel) DataContext).CitizenUnderCreation.Sex = "F";
            }
            else if ((string)((ComboBoxItem)SexBox.SelectedItem).Content == "Male")
            {
                ((CitizenTabViewModel) DataContext).CitizenUnderCreation.Sex = "M";
            }
            else if ((string)((ComboBoxItem)SexBox.SelectedItem).Content == "Other")
            {
                ((CitizenTabViewModel) DataContext).CitizenUnderCreation.Sex = "O";
            }
        }

        private TabItem previousItem;

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (GeneralTab.IsSelected && previousItem != GeneralTab)
            {
                previousItem = GeneralTab;
                DataContext = new GeneralTabViewModel(this);
            }
            else if (StatisticsTab.IsSelected && previousItem != StatisticsTab)
            {
                previousItem = StatisticsTab;
                DataContext = new StatisticsTabViewModel(this);
            }
            else if (TrackingTab.IsSelected && previousItem != TrackingTab)
            {
                previousItem = TrackingTab;
                DataContext = new TrackingTabViewModel(this);
            }
            else if (DataInputTab.IsSelected)
            {
                if (CitizenTab.IsSelected && previousItem != CitizenTab)
                {
                    previousItem = CitizenTab;
                    DataContext = new CitizenTabViewModel(this);
                }
                else if (TestCaseTab.IsSelected && previousItem != TestCaseTab)
                {
                    previousItem = TestCaseTab;
                    DataContext = new TestCaseTabViewModel(this);
                }
                else if (TestCenterTab.IsSelected && previousItem != TestCenterTab)
                {
                    previousItem = TestCenterTab;
                    DataContext = new TestCenterTabViewModel(this);
                }
                else if (LocationTab.IsSelected && previousItem != LocationTab)
                {
                    previousItem = LocationTab;
                    DataContext = new LocationTabViewModel(this);
                }
                else if (LocationVisitTab.IsSelected && previousItem != LocationVisitTab)
                {
                    previousItem = LocationVisitTab;
                    DataContext = new LocationVisitTabViewModel(this);
                }
            }
        }
    }
}
