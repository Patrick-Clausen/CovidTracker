using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseLibrary
{
    public class TestCenter
    {
        public TestCenter()
        {
            Id = ObjectId.GenerateNewId().ToString();
            Name = string.Empty;
            Hours = string.Empty;
            Tests = new List<CitizenTestedAtTestCenter>();
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;
        public string Name { get; set; }
        public string Hours { get; set; }

        [BsonIgnore]
        public Location PlacedIn { get; set; }

        private string locationId = string.Empty;

        [BsonElement("LocationId")]
        public string LocationId
        {
            get
            {
                if (locationId == string.Empty && PlacedIn != null)
                {
                    locationId = PlacedIn.Id;
                }
                return locationId;
            }
            set
            {
                locationId = value;
            }
        }

        public TestCenterManagement HasManagement { get; set; }

        private List<string> testsId = new List<string>();
        [BsonElement("TestsId")]
        public List<string> TestsId { 
            get
            {
                if (testsId.Count == 0 && Tests.Count != 0)
                {
                    testsId = Tests.Select(t => t._id).ToList();
                }

                return testsId;
            }
            set
            {
                testsId = value;
            }
        }

        [BsonIgnore]
        public List<CitizenTestedAtTestCenter> Tests { get; set; }
    }
}