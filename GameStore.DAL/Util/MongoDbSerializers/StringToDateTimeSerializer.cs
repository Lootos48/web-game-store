using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;

namespace GameStore.DAL.Util.MongoDbSerializers
{
    class StringToDateTimeSerializer : SerializerBase<DateTime?>
    {
        public override DateTime? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.String)
            {
                var value = context.Reader.ReadString();
                if (DateTime.TryParse(value, out DateTime date))
                {
                    return date;
                }

                return null;
            }

            context.Reader.SkipValue();
            return null;
        }
    }
}
