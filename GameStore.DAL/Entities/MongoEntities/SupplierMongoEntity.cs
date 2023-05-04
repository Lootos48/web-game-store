using GameStore.DAL.Attributes;
using GameStore.DAL.Util.MongoDbSerializers;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [CollectionName("suppliers")]
    public class SupplierMongoEntity
    {
        [BsonElement("SupplierID")]
        public int? SupplierId { get; set; }

        public string CompanyName { get; set; }

        public string Description { get; set; }

        public string HomePage { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        [BsonSerializer(typeof(StringToIntSerializer))]
        public string PostalCode { get; set; }

        public string Country { get; set; }

        [BsonSerializer(typeof(StringToIntSerializer))]
        public string Phone { get; set; }

        [BsonSerializer(typeof(StringToIntSerializer))]
        public string Fax { get; set; }
    }
}
