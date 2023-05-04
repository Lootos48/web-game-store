using GameStore.DAL.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GameStore.DAL.Entities.MongoEntities
{
    [CollectionName("products")]
    public class ProductMongoEntity
    {
        public string ProductKey { get; set; }

        [BsonElement("ProductID")]
        public int? ProductId { get; set; }

        public string ProductName { get; set; }

        [BsonElement("SupplierID")]
        public int? SupplierId { get; set; }

        [BsonElement("CategoryID")]
        public int? CategoryId { get; set; }

        public string QuantityPerUnit { get; set; }

        [BsonRepresentation(BsonType.Decimal128)]
        public decimal? UnitPrice { get; set; }

        public int? UnitsInStock { get; set; }

        public int? UnitsInOrder { get; set; }

        public int? ReorderLevel { get; set; }

        [BsonRepresentation(BsonType.Boolean)]
        public bool? Discontinued { get; set; }

        public int? ViewCount { get; set; }
    }
}
