using System;
using System.Collections.Generic;

namespace GameStore.DomainModels.Models
{
    public class Distributor
    {
        public string Id { get; set; }

        public string CompanyName { get; set; }

        public string Description { get; set; }

        public string HomePage { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public Guid? UserId { get; set; }

        public List<Goods> DistributedGoods { get; set; } = new List<Goods>();
    }
}
