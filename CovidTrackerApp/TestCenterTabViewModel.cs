using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DatabaseLibrary;
using MongoDB.Driver;
using Prism.Commands;
using Prism.Mvvm;

namespace CovidTrackerApp
{
    public class TestCenterTabViewModel : BindableBase
    {
        private MainWindow window;

        public TestCenterTabViewModel(MainWindow mainWindow)
        {
            window = mainWindow;
        }

        public ObservableCollection<Location> Locations
        {
            get
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var locationCollection = db.GetCollection<Location>("Locations");
                List<Location> locations = locationCollection.FindSync(o => true).ToList();


                return new ObservableCollection<Location>(locations);
            }
        }

        private TestCenterManagement testCenterManagementUnderCreation = new TestCenterManagement();

        public TestCenterManagement TestCenterManagementUnderCreation
        {
            get
            {
                return testCenterManagementUnderCreation;
            }
            set
            {
                testCenterManagementUnderCreation = value;
                RaisePropertyChanged();
            }
        }

        private TestCenter testCenterUnderCreation = new TestCenter();

        public TestCenter TestCenterUnderCreation
        {
            get
            {
                return testCenterUnderCreation;
            }
            set
            {
                testCenterUnderCreation = value;
                RaisePropertyChanged();
            }
        }




        private ICommand testCenterSaveCommand;

        public ICommand TestCenterSaveCommand
        {
            get
            {
                return testCenterSaveCommand ?? (testCenterSaveCommand = new DelegateCommand(TestCenterSaveCommandHandler));
            }
        }

        private async void TestCenterSaveCommandHandler()
        {
            bool verificationFailed = false;

            if (TestCenterManagementUnderCreation.Email == string.Empty)
            {
                window.TestCenterManagementEmailRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCenterManagementEmailRequired.Visibility = Visibility.Hidden;
            }

            if (TestCenterManagementUnderCreation.PhoneNumber == 0)
            {
                window.TestCenterManagementPhoneNumberRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCenterManagementPhoneNumberRequired.Visibility = Visibility.Hidden;
            }

            if (TestCenterManagementUnderCreation.Name == string.Empty)
            {
                window.TestCenterManagementNameRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCenterManagementNameRequired.Visibility = Visibility.Hidden;
            }

            if (TestCenterUnderCreation.PlacedIn == null)
            {
                window.TestCenterLocationRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCenterLocationRequired.Visibility = Visibility.Hidden;
            }

            if (TestCenterUnderCreation.Name == string.Empty)
            {
                window.TestCenterNameRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCenterNameRequired.Visibility = Visibility.Hidden;
            }

            if (TestCenterUnderCreation.Hours == string.Empty)
            {
                window.TestCenterHoursRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCenterHoursRequired.Visibility = Visibility.Hidden;
            }


            TestCenterUnderCreation.HasManagement = TestCenterManagementUnderCreation;

            if (verificationFailed)
            {
                return;
            }
            else
            {
                try
                {
                    var client = new MongoClient("mongodb://127.0.0.1:27017");
                    var db = client.GetDatabase("CovidTracking");
                    var testCenterCollection = db.GetCollection<TestCenter>("TestCenters");
                    var locationCollection = db.GetCollection<Location>("Locations");
                    testCenterCollection.InsertOne(TestCenterUnderCreation);
                    var update = Builders<Location>.Update.AddToSet("TestCentersId", TestCenterUnderCreation.Id);
                    locationCollection.FindOneAndUpdate(l => l.Id == TestCenterUnderCreation.LocationId, update);
                    TestCenterUnderCreation = new TestCenter();
                    TestCenterManagementUnderCreation = new TestCenterManagement();
                    RaisePropertyChanged("TestCenters");

                }
                catch (Exception e)
                {
                    StringBuilder exceptionString = new StringBuilder();
                    while (e != null)
                    {
                        exceptionString.AppendLine(e.Message);
                        e = e.InnerException;
                    }
                    MessageBox.Show(exceptionString.ToString());
                }
            }

        }
    }
}
