using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DatabaseLibrary;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Prism.Commands;
using Prism.Mvvm;

namespace CovidTrackerApp
{
    public class TestCaseTabViewModel : BindableBase
    {
        private MainWindow window;
        public TestCaseTabViewModel(MainWindow mainWindow)
        {
            window = mainWindow;
        }

        public ObservableCollection<TestCenter> TestCenters
        {
            get
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var testCenterCollection = db.GetCollection<TestCenter>("TestCenters");

                return new ObservableCollection<TestCenter>(testCenterCollection.FindSync(o => true).ToList());
            }
        }

        public ObservableCollection<Citizen> Citizens
        {
            get
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var citizenCollection = db.GetCollection<Citizen>("Citizens");

                return new ObservableCollection<Citizen>(citizenCollection.FindSync(o => true).ToList());
            }
        }

        private CitizenTestedAtTestCenter testCaseUnderCreation = new CitizenTestedAtTestCenter();

        public CitizenTestedAtTestCenter TestCaseUnderCreation
        {
            get
            {
                return testCaseUnderCreation;
            }
            set
            {
                testCaseUnderCreation = value;
                RaisePropertyChanged();
            }
        }

        private ICommand testCaseSaveCommand;

        public ICommand TestCaseSaveCommand
        {
            get
            {
                return testCaseSaveCommand ?? (testCaseSaveCommand = new DelegateCommand(TestCaseSaveCommandHandler));
            }
        }

        private async void TestCaseSaveCommandHandler()
        {
            bool verificationFailed = false;
            if (TestCaseUnderCreation.TestedAt == null)
            {
                window.TestCaseTestCenterRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCaseTestCenterRequired.Visibility = Visibility.Hidden;
            }

            if (TestCaseUnderCreation.TestedCitizen == null)
            {
                window.TestCaseCitizenRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCaseCitizenRequired.Visibility = Visibility.Hidden;
            }

            if (TestCaseUnderCreation.Status == string.Empty)
            {
                window.TestCaseStatusRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCaseStatusRequired.Visibility = Visibility.Hidden;
            }

            if (TestCaseUnderCreation.Date == default)
            {
                window.TestCaseDateRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.TestCaseDateRequired.Visibility = Visibility.Hidden;
            }

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
                    var citizenCollection = db.GetCollection<Citizen>("Citizens");

                    var update = Builders<Citizen>.Update.AddToSet("Tests", TestCaseUnderCreation);

                    citizenCollection.FindOneAndUpdate(o => o.Id == TestCaseUnderCreation.CitizenId,update);

                    TestCaseUnderCreation = new CitizenTestedAtTestCenter();
                    RaisePropertyChanged("TestCases");
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
