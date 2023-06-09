﻿using System;
using System.Collections.Generic;

namespace GameStore.DAL.Entities
{
    public class PublisherEntity : Entity
    {
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

        public UserEntity User { get; set; }

        public ICollection<GameEntity> PublishedGames { get; set; } = new List<GameEntity>();
    }
}
