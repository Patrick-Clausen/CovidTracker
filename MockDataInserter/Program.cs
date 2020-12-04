using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using DatabaseLibrary;
using MongoDB.Driver;

namespace MockDataInserter
{
    class Program
    {
        static void Main(string[] args)
        {
            //CLEAR DATABASE
            /*
            using (var context = new TrackerContext())
            {
                context.CitizenTestedAtTestCenters.RemoveRange(context.CitizenTestedAtTestCenters);
                context.CitizenWasAtLocations.RemoveRange(context.CitizenWasAtLocations);
                context.Citizens.RemoveRange(context.Citizens);
                context.TestCenters.RemoveRange(context.TestCenters);
                context.Locations.RemoveRange(context.Locations);
                context.TestCenterManagements.RemoveRange(context.TestCenterManagements);
                context.Municipalities.RemoveRange(context.Municipalities);
                context.SaveChanges();
            }
            */

            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var db = client.GetDatabase("CovidTracking");
            db.DropCollection("LocationVisitDays");
            db.DropCollection("Municipalities");
            db.DropCollection("Locations");
            db.DropCollection("TestCenters");
            db.DropCollection("Citizens");
            db.CreateCollection("Citizens");
            db.CreateCollection("TestCenters");
            db.CreateCollection("Locations");
            db.CreateCollection("Municipalities");
            db.CreateCollection("LocationVisitDays");

            

            //GET MUNICIPALITIES
            List<Municipality> municipalities = new List<Municipality>();
            string[] readLines = File.ReadAllLines(@"Municipality.csv");
            foreach (string line in readLines)
            {
                string[] splitLine = line.Split(',');
                Municipality municipality = new Municipality();
                municipality.Name = splitLine[1];
                municipality.Population = int.Parse(splitLine[2]);
                municipalities.Add(municipality);
            }

            //GET TESTCENTERMANAGEMENTS
            List<TestCenterManagement> testCenterManagements = new List<TestCenterManagement>();
            readLines = File.ReadAllLines(@"TestCenterManagementData");
            foreach (string line in readLines)
            {
                string[] splitLine = line.Split(';');
                TestCenterManagement management = new TestCenterManagement();
                management.Name = splitLine[0];
                management.PhoneNumber = int.Parse(splitLine[1]);
                management.Email = splitLine[2];
                testCenterManagements.Add(management);
            }

            //GET LOCATIONS
            List<Location> locations = new List<Location>();
            readLines = File.ReadAllLines(@"LocationData");
            foreach (string line in readLines)
            {
                string[] splitLine = line.Split(';');
                Location location = new Location();
                location.Address = splitLine[0];
                location.IsIn = municipalities.Find(m => m.Name == splitLine[1]);
                //Handle circular reference
                if (location.IsIn != null)
                {
                    location.IsIn.LocationsInMunicipality.Add(location);
                }
                locations.Add(location);
            }

            //GET TEST CENTERS
            List<TestCenter> testCenters = new List<TestCenter>();
            readLines = File.ReadAllLines(@"TestCenterData");
            foreach (string line in readLines)
            {
                string[] splitLine = line.Split(';');
                TestCenter testCenter = new TestCenter();
                testCenter.Name = splitLine[0];
                testCenter.Hours = splitLine[1];
                testCenter.HasManagement = testCenterManagements.Find(t => t.Name == splitLine[2]);
                testCenter.PlacedIn = locations.Find(t => t.Address == splitLine[3]);
                testCenter.PlacedIn.TestCentersAtLocation.Add(testCenter);
                testCenters.Add(testCenter);
            }

            //GET CITIZENS
            List<Citizen> citizens = new List<Citizen>();
            readLines = File.ReadAllLines(@"CitizenData");
            foreach (string line in readLines)
            {
                string[] splitLine = line.Split(';');
                Citizen citizen = new Citizen();
                citizen.FirstName = splitLine[0];
                citizen.LastName = splitLine[1];
                citizen.Sex = splitLine[2];
                citizen.Age = int.Parse(splitLine[3]);
                citizen.SSN = splitLine[4];
                citizen.LivesIn = municipalities.Find(m => m.Name == splitLine[5]);
                citizen.LivesIn.CitizensInMunicipality.Add(citizen);
                citizens.Add(citizen);
            }

            //GET CITIZENVISITS
            List<LocationVisitDay> locationVisitDays = new List<LocationVisitDay>();
            readLines = File.ReadAllLines(@"LocationVisitedData");
            foreach (string line in readLines)
            {
                string[] splitLine = line.Split(';');
                LocationVisitDay foundDay = locationVisitDays.Find(l =>
                    l.DateOfVisit == DateTime.Parse(splitLine[1], styles: DateTimeStyles.AssumeUniversal) &&
                    l.VisitedLocation.Address == splitLine[2]);
                if (foundDay != null)
                {
                    Citizen foundCitizen = citizens.Find(c => c.SSN == splitLine[0]);
                    foundDay.VisitingCitizens.Add(foundCitizen);
                    foundCitizen.Visits.Add(foundDay);
                }
                else
                {
                    LocationVisitDay locationVisitDay = new LocationVisitDay();
                    locationVisitDay.VisitingCitizens.Add(citizens.Find(c => c.SSN == splitLine[0]));
                    locationVisitDay.DateOfVisit = DateTime.Parse(splitLine[1], styles: DateTimeStyles.AssumeUniversal);
                    locationVisitDay.VisitedLocation = locations.Find(l => l.Address == splitLine[2]);
                    locationVisitDay.VisitedLocation.Visits.Add(locationVisitDay);
                    locationVisitDays.Add(locationVisitDay);
                }
            }

            //GET TESTS
            readLines = File.ReadAllLines(@"TestResultData");
            foreach (string line in readLines)
            {
                string[] splitLine = line.Split(';');
                CitizenTestedAtTestCenter citizenTestedAtTestCenter = new CitizenTestedAtTestCenter();
                citizenTestedAtTestCenter.TestedCitizen = citizens.Find(c => c.SSN == splitLine[0]);
                citizenTestedAtTestCenter.Date = DateTime.Parse(splitLine[1], styles:DateTimeStyles.AssumeUniversal);
                citizenTestedAtTestCenter.TestedAt = testCenters.Find(t => t.Name == splitLine[2]);
                citizenTestedAtTestCenter.TestedAt.Tests.Add(citizenTestedAtTestCenter);
                citizenTestedAtTestCenter.Result = splitLine[3];
                citizenTestedAtTestCenter.Status = splitLine[4];
                citizenTestedAtTestCenter.TestedCitizen.Tests.Add(citizenTestedAtTestCenter);
            }

            var municipalityCollection = db.GetCollection<Municipality>("Municipalities");
            municipalityCollection.InsertMany(municipalities);

            var locationCollection = db.GetCollection<Location>("Locations");
            locationCollection.InsertMany(locations);

            var testCenterCollection = db.GetCollection<TestCenter>("TestCenters");
            testCenterCollection.InsertMany(testCenters);

            var citizenCollection = db.GetCollection<Citizen>("Citizens");
            citizenCollection.InsertMany(citizens);

            var locationVisitCollection = db.GetCollection<LocationVisitDay>("LocationVisitDays");
            locationVisitCollection.InsertMany(locationVisitDays);

        }
    }
}
