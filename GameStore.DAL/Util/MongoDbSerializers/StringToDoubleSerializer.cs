using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace GameStore.DAL.Util.MongoDbSerializers
{
    public class StringToDoubleSerializer : SerializerBase<double?>
    {
        public override double? Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType == BsonType.Int32)
            {
                return context.Reader.ReadInt32();
            }
            else if (context.Reader.CurrentBsonType == BsonType.String)
            {
                var value = context.Reader.ReadString();

                if (double.TryParse(value, out double result))
                {
                    return result;
                }

                return null;
            }

            context.Reader.SkipValue();
            return null;
        }
    }
}
