using GameStore.DAL.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [CollectionName("order-details")]
    public class OrderDetailsMongoEntity
    {
        [BsonElement("OrderID")]
        public int? OrderId { get; set; }

        [BsonElement("ProductID")]
        public int? ProductId { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? UnitPrice { get; set; }

        public int? Quantity { get; set; }

        public double? Discount { get; set; }
    }
}
