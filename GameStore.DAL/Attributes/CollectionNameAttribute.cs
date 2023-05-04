using System;

namespace GameStore.DAL.Attributes
{
    public class CollectionNameAttribute : Attribute
    {
        public string Name { get; set; }

        public CollectionNameAttribute(string collectionName)
        {
            Name = collectionName;
        }
    }
}
