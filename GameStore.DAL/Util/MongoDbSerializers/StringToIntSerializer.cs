using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GameStore.DAL.Util.MongoDbSerializers
{
    public class StringToIntSerializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Int32)
            {
                return context.Reader.ReadInt32().ToString();
            }
            else if (context.Reader.CurrentBsonType == BsonType.String)
            {
                var value = context.Reader.ReadString();
                if (string.IsNullOrWhiteSpace(value))
                {
                    return null;
                }

                return value;
            }

            context.Reader.SkipValue();
            return null;
        }
    }
}
