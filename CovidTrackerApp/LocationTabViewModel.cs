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
    public class LocationTabViewModel : BindableBase
    {
        private MainWindow window;
        public LocationTabViewModel(MainWindow mainWindow)
        {
            window = mainWindow;
        }

        public ObservableCollection<Municipality> Municipalities
        {
            get
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var municipalityCollection = db.GetCollection<Municipality>("Municipalities");
                return new ObservableCollection<Municipality>(municipalityCollection.FindSync(o => true).ToList());
            }
        }

        private Location locationUnderCreation = new Location();

        public Location LocationUnderCreation
        {
            get
            {
                return locationUnderCreation;
            }
            set
            {
                locationUnderCreation = value;
                RaisePropertyChanged();
            }
        }

        private ICommand locationSaveCommand;

        public ICommand LocationSaveCommand
        {
            get
            {
                return locationSaveCommand ?? (locationSaveCommand = new DelegateCommand(LocationSaveCommandHandler));
            }
        }

        private async void LocationSaveCommandHandler()
        {
            bool verificationFailed = false;
            if (LocationUnderCreation.Address == string.Empty)
            {
                window.AddressRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.AddressRequired.Visibility = Visibility.Hidden;
            }

            if (LocationUnderCreation.IsIn == null)
            {
                window.LocationMunicipalityRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.LocationMunicipalityRequired.Visibility = Visibility.Hidden;
            }

            if (verificationFailed)
            {
                return;
            }

            try
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var collection = db.GetCollection<Location>("Locations");
                collection.InsertOne(locationUnderCreation);
                var municipalityCollection = db.GetCollection<Municipality>("Municipalities");
                var update = Builders<Municipality>.Update.AddToSet("LocationsId", LocationUnderCreation.Id);
                municipalityCollection.FindOneAndUpdate(m => m.Id == LocationUnderCreation.MunicipalityId, update);
                LocationUnderCreation = new Location();
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
