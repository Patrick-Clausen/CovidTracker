using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseLibrary
{
    public class LocationVisitDay
    {
        public LocationVisitDay()
        {
            Id = ObjectId.GenerateNewId().ToString();
            VisitingCitizens = new List<Citizen>();
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;


        [BsonIgnore]
        public List<Citizen> VisitingCitizens { get; set; }

        private List<string> visitingCitizensId = new List<string>();

        [BsonElement("VisitingCitizensId")]
        public List<string> VisitingCitizensId
        {
            get
            {
                if (visitingCitizensId.Count == 0 && VisitingCitizens.Count != 0)
                {
                    visitingCitizensId = VisitingCitizens.Select(c => c.Id).ToList();
                }

                return visitingCitizensId;
            }
            set
            {
                visitingCitizensId = value;
            }
        }

        private DateTime dateOfVisit = DateTime.UtcNow.Date;
        public DateTime DateOfVisit
        {
            get
            {
                return dateOfVisit;
            }
            set
            {
                //Copy in date to ensure utc - Even though it's a bit dumb
                dateOfVisit = new DateTime(value.Year, value.Month,value.Day,value.Hour, value.Minute, value.Second, DateTimeKind.Utc);
            }
        }

        [BsonIgnore]
        public Location VisitedLocation { get; set; }

        private string visitedLocationId = string.Empty;
        [BsonElement("VisitedLocationId")]
        public string VisitedLocationId {
            get
            {
                if (visitedLocationId == string.Empty && VisitedLocation != null)
                {
                    visitedLocationId = VisitedLocation.Id;
                }

                return visitedLocationId;
            }
            set
            {
                visitedLocationId = value;
            }
        }
    }
}