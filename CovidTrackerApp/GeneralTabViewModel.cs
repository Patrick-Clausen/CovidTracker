using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DatabaseLibrary;
using MongoDB.Driver;
using Prism.Mvvm;

namespace CovidTrackerApp
{
    public class GeneralTabViewModel : BindableBase
    {
        private MainWindow window;

        public GeneralTabViewModel(MainWindow mainWindow)
        {
            window = mainWindow;
        }

        public ObservableCollection<MunicipalityCase> MunicipalityCaseList
        {
            get
            {
                ObservableCollection<MunicipalityCase> municipalityCaseList = new ObservableCollection<MunicipalityCase>();
                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var collection = db.GetCollection<Municipality>("Municipalities");

                List<Municipality> municipalities = collection.FindSync(o => true).ToList();
                foreach (Municipality municipality in municipalities)
                {
                    MunicipalityCase municipalityCase = new MunicipalityCase();
                    municipalityCase.Name = municipality.Name;
                    municipalityCase.Population = municipality.Population;
                    var citizenCollection = db.GetCollection<Citizen>("Citizens");
                    var filter = Builders<Citizen>.Filter.Eq("MunicipalityId", municipality.Id);
                    var citizens = citizenCollection
                        .Find(filter).ToList();

                    municipalityCase.PositiveTests = 0;
                    foreach (Citizen citizen in citizens)
                    {
                        foreach (CitizenTestedAtTestCenter testCase in citizen.Tests)
                        {
                            if (testCase.Result == "Positive")
                            {
                                municipalityCase.PositiveTests++;
                            }
                        }
                    }

                    municipalityCase.ActiveCases = 0;
                    foreach (Citizen citizen in citizens)
                    {
                        foreach (CitizenTestedAtTestCenter testCase in citizen.Tests)
                        {
                            if (testCase.Result == "Positive" && testCase.Date > DateTime.Now.AddDays(-14))
                            {
                                municipalityCase.ActiveCases++;
                            }
                        }
                    }

                    municipalityCase.TotalTests = 0;
                    foreach (Citizen citizen in citizens)
                    {
                        municipalityCase.TotalTests += citizen.Tests.Count;
                    }

                    municipalityCaseList.Add(municipalityCase);
                }

                return municipalityCaseList;
            }
        }
    }

    public class MunicipalityCase
    {
        public string Name { get; set; }
        public int Population { get; set; }
        public int ActiveCases { get; set; }
        public int PositiveTests { get; set; }
        public int TotalTests { get; set; }
    }
}
