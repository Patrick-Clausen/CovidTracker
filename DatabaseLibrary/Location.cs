using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseLibrary
{
    public class Location
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;
        public Location()
        {
            Id = ObjectId.GenerateNewId().ToString();
            Address = string.Empty;
            TestCentersAtLocation = new List<TestCenter>();
            Visits = new List<LocationVisitDay>();
        }
        
        public string Address { get; set; }

        [BsonIgnore]
        public Municipality IsIn { get; set; }

        private string municipalityId = string.Empty;

        [BsonElement("MunicipalityId")]
        public string MunicipalityId
        {
            get
            {
                if (municipalityId == string.Empty && IsIn != null)
                {
                    municipalityId = IsIn.Id;
                }

                return municipalityId;
            }
            set
            {
                municipalityId = value;
            }
        }
        [BsonIgnore]
        public List<LocationVisitDay> Visits { get; set; }


        private List<string> visitIds = new List<string>();
        [BsonElement("VisitIds")]
        public List<string> VisitIds
        {
            get
            {
                if (visitIds.Count == 0 && Visits.Count != 0)
                {
                    visitIds = Visits.Select(v => v.Id).ToList();
                }

                return visitIds;
            }
            set
            {
                visitIds = value;
            }
        }


        private List<string> testCentersId = new List<string>();

        [BsonElement("TestCentersId")]
        public List<string> TestCentersId
        {
            get
            {
                if (testCentersId.Count == 0 && TestCentersAtLocation.Count != 0)
                {
                    testCentersId = TestCentersAtLocation.Select(t => t.Id).ToList();
                }

                return testCentersId;
            }
            set
            {
                testCentersId = value;
            }
        }

        [BsonIgnore]
        public List<TestCenter> TestCentersAtLocation { get; set; }
    }
}