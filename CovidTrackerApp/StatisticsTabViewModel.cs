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
    public class StatisticsTabViewModel : BindableBase
    {
        private MainWindow window;

        public StatisticsTabViewModel(MainWindow mainWindow)
        {
            window = mainWindow;
        }

        public ObservableCollection<SexCase> SexCaseList
        {
            get
            {
                ObservableCollection<SexCase> sexCaseList = new ObservableCollection<SexCase>();
                sexCaseList.Add(new SexCase() { Sex = "Male" });
                sexCaseList.Add(new SexCase() { Sex = "Female" });
                sexCaseList.Add(new SexCase() { Sex = "Other" });

                SexCase sexCase = new SexCase();


                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var citizenCollection = db.GetCollection<Citizen>("Citizens");
                List<Citizen> citizens = citizenCollection.FindSync(o => true).ToList();

                //GET TOTAL AMOUNT OF REGISTERED PEOPLE OF ALL GENDERS
                sexCaseList[0].RegisteredPeople = citizens
                    .Count(c => c.Sex == "M");
                sexCaseList[1].RegisteredPeople = citizens
                    .Count(c => c.Sex == "F");
                sexCaseList[2].RegisteredPeople = citizens
                    .Count(c => c.Sex == "O");

                //GET ACTIVE CASES BY GENDER
                sexCaseList[0].ActiveCases = citizens
                    .Where(c => c.Sex == "M")
                    .Count(c => c.Tests.Any(
                        t => 
                            t.Result == "Positive" &&
                            t.Date.Date > DateTime.UtcNow.Date.AddDays(-14)
                    ));
                sexCaseList[1].ActiveCases = citizens
                    .Where(c => c.Sex == "F")
                    .Count(c => c.Tests.Any(
                        t =>
                            t.Result == "Positive" &&
                            t.Date.Date > DateTime.UtcNow.Date.AddDays(-14)
                    ));

                sexCaseList[2].ActiveCases = citizens
                    .Where(c => c.Sex == "O")
                    .Count(c => c.Tests.Any(
                        t =>
                            t.Result == "Positive" &&
                            t.Date.Date > DateTime.UtcNow.Date.AddDays(-14)
                    ));

                //GET POSITIVE TESTS
                sexCaseList[0].PositiveTests = 0;
                foreach (Citizen citizen in citizens)
                {
                    if (citizen.Sex == "M")
                    {
                        foreach (var test in citizen.Tests)
                        {
                            sexCaseList[0].TotalTests++;
                            if (test.Result == "Positive")
                            {
                                sexCaseList[0].PositiveTests++;
                            }
                        }
                    }
                }
                sexCaseList[1].PositiveTests = 0;
                foreach (Citizen citizen in citizens)
                {
                    if (citizen.Sex == "F")
                    {
                        foreach (var test in citizen.Tests)
                        {
                            sexCaseList[1].TotalTests++;
                            if (test.Result == "Positive")
                            {
                                sexCaseList[1].PositiveTests++;
                            }
                        }
                    }
                }
                sexCaseList[2].PositiveTests = 0;
                foreach (Citizen citizen in citizens)
                {
                    if (citizen.Sex == "O")
                    {
                        sexCaseList[2].TotalTests++;
                        foreach (var test in citizen.Tests)
                        {
                            if (test.Result == "Positive")
                            {
                                sexCaseList[2].PositiveTests++;
                            }
                        }
                    }
                }

                return sexCaseList;
            }
        }

        public ObservableCollection<AgeCase> AgeCaseList
        {
            get
            {
                ObservableCollection<AgeCase> ageCaseList = new ObservableCollection<AgeCase>();

                var client = new MongoClient("mongodb://127.0.0.1:27017");
                var db = client.GetDatabase("CovidTracking");
                var citizenCollection = db.GetCollection<Citizen>("Citizens");
                List<Citizen> citizens = citizenCollection.FindSync(o => true).ToList();

                for (int i = 0; i < 100; i += 10)
                {
                    ageCaseList.Add(new AgeCase());

                    ageCaseList[i / 10].AgeGroup = String.Format($"{i} - {i + 9}");


                        ageCaseList[i / 10].RegisteredPeople = citizens
                            .Count(c => (c.Age >= i && c.Age < (i + 10)));

                        ageCaseList[i / 10].ActiveCases = citizens
                            .Where(c => c.Age >= i && c.Age < (i + 10))
                            .Count(c => c.Tests.Any(
                                t => t.Result == "Positive"
                                     && t.Date.Date > DateTime.UtcNow.Date.AddDays(-14)
                            ));

                    ageCaseList[i / 10].PositiveTests = 0;
                    foreach (Citizen citizen in citizens)
                    {
                        if (citizen.Age >= i && citizen.Age < (i + 10))
                        {
                            ageCaseList[i / 10].TotalTests++;
                            foreach (var test in citizen.Tests)
                            {
                                if (test.Result == "Positive")
                                {
                                    ageCaseList[i / 10].PositiveTests++;
                                }
                            }
                        }
                    }
                }

                return ageCaseList;
            }
        }
    }

    public class SexCase
    {
        public string Sex { get; set; }
        public int RegisteredPeople { get; set; }
        public int ActiveCases { get; set; }
        public int PositiveTests { get; set; }
        public int TotalTests { get; set; }
    }

    public class AgeCase
    {
        public string AgeGroup { get; set; }
        public int RegisteredPeople { get; set; }
        public int ActiveCases { get; set; }
        public int PositiveTests { get; set; }
        public int TotalTests { get; set; }
    }
}
