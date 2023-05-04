using GameStore.DAL.Attributes;
using GameStore.DAL.Util.MongoDbSerializers;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace GameStore.DAL.Entities.MongoEntities
{
    [CollectionName("orders")]
    public class OrderMongoEntity
    {
        [BsonElement("OrderID")]
        public int? OrderId { get; set; }

        [BsonElement("CustomerID")]
        public string CustomerId { get; set; }

        [BsonElement("EmployeeID")]
        public int? EmployeeId { get; set; }

        [BsonSerializer(typeof(StringToDateTimeSerializer))]
        public DateTime? OrderDate { get; set; }

        [BsonSerializer(typeof(StringToDateTimeSerializer))]
        public DateTime? RequiredDate { get; set; }

        [BsonSerializer(typeof(StringToDateTimeSerializer))]
        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }

        [BsonSerializer(typeof(StringToDoubleSerializer))]
        public double? Freight { get; set; }

        public string ShipName { get; set; }

        public string ShipAddress { get; set; }

        public string ShipCity { get; set; }

        public string ShipRegion { get; set; }

        [BsonSerializer(typeof(StringToIntSerializer))]
        public string ShipPostalCode { get; set; }

        public string ShipCountry { get; set; }
    }
}
