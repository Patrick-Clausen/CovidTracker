using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DatabaseLibrary
{
    public class Municipality
    {
        public Municipality()
        {
            Id = ObjectId.GenerateNewId().ToString();
            LocationsInMunicipality = new List<Location>();
            CitizensInMunicipality = new List<Citizen>();
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;
        public string Name { get; set; }
        public int Population { get; set; }

        private List<string> locationsId = new List<string>();

        [BsonElement("LocationsId")]
        public List<string> LocationsId
        {
            get
            {
                if (locationsId.Count == 0 && LocationsInMunicipality.Count != 0)
                {
                    locationsId = LocationsInMunicipality.Select(l => l.Id).ToList();
                }

                return locationsId;
            }
            set
            {
                locationsId = value;
            }
        }
        [BsonIgnore]
        public List<Location> LocationsInMunicipality { get; set; }

        private List<string> citizensId = new List<string>();
        [BsonElement("CitizensId")]
        public List<string> CitizensId
        {
            get
            {
                if (citizensId.Count == 0 && CitizensInMunicipality.Count != 0)
                {
                    citizensId = CitizensInMunicipality.Select(c => c.Id).ToList();
                }

                return citizensId;
            }
            set
            {
                citizensId = value;
            }
        }
        [BsonIgnore]
        public List<Citizen> CitizensInMunicipality { get; set; }
    }
}