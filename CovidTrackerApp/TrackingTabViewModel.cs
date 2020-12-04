using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using DatabaseLibrary;
using MongoDB.Driver;
using Prism.Mvvm;

namespace CovidTrackerApp
{
    public class TrackingTabViewModel : BindableBase
    {
        private MainWindow window;
        public TrackingTabViewModel(MainWindow mainWindow)
        {
            window = mainWindow;
        }

        public ObservableCollection<PossibleInfection> PossibleInfectionList
        {
            get
            {
                ObservableCollection<PossibleInfection> possibleInfectionList = new ObservableCollection<PossibleInfection>();

                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var citizenCollection = db.GetCollection<Citizen>("Citizens");
                var citizens = citizenCollection.FindSync(o => true).ToList();
                var locationVisitCollection = db.GetCollection<LocationVisitDay>("LocationVisitDays");
                var locationVisits = locationVisitCollection.FindSync(o => true).ToList();
                var locationCollection = db.GetCollection<Location>("Locations");

                foreach (var locationVisit in locationVisits)
                {
                    foreach (Citizen citizen in citizens.Where(c => locationVisit.VisitingCitizensId.Contains(c.Id)).ToList())
                    {
                        if (locationVisit.VisitingCitizensId.Contains(citizen.Id) && citizen.Tests.Any(t =>
                            t.Result == "Positive" && locationVisit.DateOfVisit.Date > t.Date.Date.AddDays(-3) &&
                            locationVisit.DateOfVisit.Date < t.Date.Date.AddDays(14)))
                        {
                            //IF TRUE, LOCATION VISIT HAS POSSIBLE INFECTIONS
                            foreach (Citizen possiblyInfectedCitizen in (citizens.Where(c =>
                                locationVisit.VisitingCitizensId.Contains(c.Id))))
                            {
                                if (possiblyInfectedCitizen != citizen)
                                {
                                    Location locationOfInfection = locationCollection
                                        .Find(o => o.Id == locationVisit.VisitedLocationId).ToList().First();
                                    possibleInfectionList.Add(new PossibleInfection() { DateOfInfection = locationVisit.DateOfVisit.ToShortDateString(), Infectee = possiblyInfectedCitizen.FullName, Infector = citizen.FullName, LocationOfInfection = locationOfInfection.Address });
                                }
                            }
                        }
                    }
                }


                return possibleInfectionList;
            }
        }
    }

    public class PossibleInfection
    {
        public string Infectee { get; set; }
        public string Infector { get; set; }
        public string LocationOfInfection { get; set; }
        public string DateOfInfection { get; set; }
    }
}
