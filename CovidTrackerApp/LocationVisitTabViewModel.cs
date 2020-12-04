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
    public class LocationVisitTabViewModel : BindableBase
    {
        private MainWindow window;

        public LocationVisitTabViewModel(MainWindow mainWindow)
        {
            window = mainWindow;
            locationVisitDayUnderCreation = new LocationVisitDay();
            locationVisitDayUnderCreation.VisitingCitizens.Add(new Citizen());
        }

        public ObservableCollection<Location> Locations
        {
            get
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var locationCollection = db.GetCollection<Location>("Locations");
                return new ObservableCollection<Location>(locationCollection.FindSync(o => true).ToList());
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

        private LocationVisitDay locationVisitDayUnderCreation;

        public LocationVisitDay LocationVisitDayUnderCreation
        {
            get
            {
                return locationVisitDayUnderCreation;
            }
            set
            {
                locationVisitDayUnderCreation = value;
                RaisePropertyChanged();
            }
        }

        private ICommand citizenWasAtLocationSaveCommand;

        public ICommand CitizenWasAtLocationSaveCommand
        {
            get
            {
                return citizenWasAtLocationSaveCommand ?? (citizenWasAtLocationSaveCommand = new DelegateCommand(CitizenWasAtLocationSaveCommandHandler));
            }
        }

        public async void CitizenWasAtLocationSaveCommandHandler()
        {
            bool verificationFailed = false;
            if (LocationVisitDayUnderCreation.VisitedLocation == null)
            {
                window.LocationVisitLocationRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.LocationVisitLocationRequired.Visibility = Visibility.Hidden;
            }
            if (LocationVisitDayUnderCreation.VisitingCitizens[0].Age == -1)
            {
                window.LocationVisitCitizenRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.LocationVisitCitizenRequired.Visibility = Visibility.Hidden;
            }

            if (LocationVisitDayUnderCreation.DateOfVisit == default)
            {
                window.LocationVisitDateRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.LocationVisitDateRequired.Visibility = Visibility.Hidden;
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
                    var locationVisitCollection = db.GetCollection<LocationVisitDay>("LocationVisitDays");
                    var citizenCollection = db.GetCollection<Citizen>("Citizens");
                    var locationsCollection = db.GetCollection<Location>("Locations");
                    var foundVisit = locationVisitCollection.FindSync(l =>
                        l.VisitedLocationId == LocationVisitDayUnderCreation.VisitedLocationId &&
                        l.DateOfVisit == LocationVisitDayUnderCreation.DateOfVisit);

                    if (foundVisit.ToList().Count != 0)
                    {
                        var update = Builders<LocationVisitDay>.Update.AddToSet("VisitingCitizensId",
                            LocationVisitDayUnderCreation.VisitingCitizensId[0]);
                        locationVisitCollection.FindOneAndUpdate(
                            l => l.VisitedLocationId == LocationVisitDayUnderCreation.VisitedLocationId &&
                                 l.DateOfVisit == LocationVisitDayUnderCreation.DateOfVisit, update);
                    }
                    else
                    {
                        locationVisitCollection.InsertOne(LocationVisitDayUnderCreation);
                        var updateCitizen =
                            Builders<Citizen>.Update.AddToSet("VisitsId", LocationVisitDayUnderCreation.Id);
                        var updateLocation = Builders<Location>.Update.AddToSet("VisitIds", LocationVisitDayUnderCreation.Id);
                        citizenCollection.FindOneAndUpdate(c =>
                            c.Id == LocationVisitDayUnderCreation.VisitingCitizensId[0], updateCitizen);
                        locationsCollection.FindOneAndUpdate(
                            l => l.Id == LocationVisitDayUnderCreation.VisitedLocationId, updateLocation);
                    }

                    LocationVisitDayUnderCreation = new LocationVisitDay();
                    locationVisitDayUnderCreation.VisitingCitizens.Add(new Citizen());

                    RaisePropertyChanged("Locations");
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
