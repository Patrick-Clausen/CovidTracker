using System;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseLibrary
{
    public class CitizenTestedAtTestCenter
    {
        public CitizenTestedAtTestCenter()
        {
            Result = string.Empty;
            Status = string.Empty;
            _id = ObjectId.GenerateNewId().ToString();
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id;
        [BsonIgnore]
        public Citizen TestedCitizen { get; set; }

        private string citizenId = string.Empty;
        [BsonElement("CitizenId")]
        public string CitizenId {
            get
            {
                if (citizenId == string.Empty && TestedCitizen != null)
                {
                    citizenId = TestedCitizen.Id;
                }

                return citizenId;
            }
            set
            {
                citizenId = value;
            }
        }

        [BsonIgnore]
        public TestCenter TestedAt { get; set; }

        private string testCenterId = string.Empty;
        [BsonElement("TestCenterId")]
        public string TestCenterId {
            get
            {
                if (testCenterId == string.Empty && TestedAt != null)
                {
                    testCenterId = TestedAt.Id;
                }

                return testCenterId;
            }
            set
            {
                testCenterId = value;
            }
        }
        public string Result { get; set; }

        private DateTime date = DateTime.UtcNow.Date;
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, DateTimeKind.Utc);
            }
        }

        public string Status { get; set; }
    }
}