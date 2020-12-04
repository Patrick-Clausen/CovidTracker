using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseLibrary
{
    public class Citizen
    {
        [BsonId] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;
        public Citizen()
        {
            Id = ObjectId.GenerateNewId().ToString();
            FirstName = string.Empty;
            LastName = string.Empty;
            Sex = "\0";
            Age = -1;
            SSN = string.Empty;
            Tests = new List<CitizenTestedAtTestCenter>();
            Visits = new List<LocationVisitDay>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }

        [BsonIgnore]
        public Municipality LivesIn { get; set; }

        [BsonElement("MunicipalityId")]
        public string MunicipalityId
        {
            get
            {
                if (LivesIn != null)
                {
                    return LivesIn.Id;
                }

                return string.Empty;
            }
        }

        public string SSN { get; set; }

        public List<CitizenTestedAtTestCenter> Tests { get; set; }

        [BsonIgnore]
        public List<LocationVisitDay> Visits
        {
            get; set; 

        }

        private List<string> visitsId = new List<string>();

        [BsonElement("VisitsId")]         
        public List<string> VisitsId {
            get
            {
                if (visitsId.Count == 0 && Visits.Count != 0)
                {
                    visitsId = Visits.Select(v => v.Id).ToList();
                }

                return visitsId;
            }
            set
            {
                visitsId = value;
            }
        }

        [BsonIgnore]
        public string FullName
        {
            get
            {
                return (FirstName + " " + LastName);

            }
        }
    }
}