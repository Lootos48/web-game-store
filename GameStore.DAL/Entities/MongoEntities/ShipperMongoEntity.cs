using GameStore.DAL.Attributes;
using GameStore.DAL.Util.MongoDbSerializers;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [CollectionName("shippers")]
    public class ShipperMongoEntity
    {
        [BsonElement("ShipperID")]
        public int? ShipperId { get; set; }

        public string CompanyName { get; set; }

        [BsonSerializer(typeof(StringToIntSerializer))]
        public string Phone { get; set; }
    }
}
