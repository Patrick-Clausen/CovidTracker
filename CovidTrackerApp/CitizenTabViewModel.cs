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
    class CitizenTabViewModel : BindableBase
    {
        private MainWindow window;

        public CitizenTabViewModel(MainWindow mainWindow)
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

        private Citizen citizenUnderCreation = new Citizen();

        public Citizen CitizenUnderCreation
        {
            get
            {
                return citizenUnderCreation;
            }
            set
            {
                citizenUnderCreation = value;
                RaisePropertyChanged();
            }
        }

        private ICommand citizenSaveCommand;

        public ICommand CitizenSaveCommand
        {
            get
            {
                return citizenSaveCommand ?? (citizenSaveCommand = new DelegateCommand(CitizenSaveCommandHandler));
            }
        }

        async void CitizenSaveCommandHandler()
        {
            bool verificationFailed = false;
            if (CitizenUnderCreation.FirstName == string.Empty)
            {
                window.FirstNameRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.FirstNameRequired.Visibility = Visibility.Hidden;
            }

            if (CitizenUnderCreation.LastName == string.Empty)
            {
                window.LastNameRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.LastNameRequired.Visibility = Visibility.Hidden;
            }

            if (CitizenUnderCreation.Sex == "\0")
            {
                window.SexRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.SexRequired.Visibility = Visibility.Hidden;
            }

            if (CitizenUnderCreation.LivesIn == null)
            {
                window.MunicipalityRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.MunicipalityRequired.Visibility = Visibility.Hidden;
            }

            if (CitizenUnderCreation.SSN == string.Empty)
            {
                window.SSNRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.SSNRequired.Visibility = Visibility.Hidden;
            }

            if (CitizenUnderCreation.Age == -1)
            {
                window.AgeRequired.Visibility = Visibility.Visible;
                verificationFailed = true;
            }
            else
            {
                window.AgeRequired.Visibility = Visibility.Hidden;
            }

            if (verificationFailed)
            {
                return;
            }

            try
            {
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var collection = db.GetCollection<Citizen>("Citizens");
                await collection.InsertOneAsync(citizenUnderCreation);
                var municipalityCollection = db.GetCollection<Municipality>("Municipalities");
                var update = Builders<Municipality>.Update.AddToSet("CitizensId", CitizenUnderCreation.Id);
                municipalityCollection.FindOneAndUpdate(m => m.Id == CitizenUnderCreation.MunicipalityId, update);
                RaisePropertyChanged("Citizens");
                CitizenUnderCreation = new Citizen();
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
