using GameStore.PL.DTOs;
using System;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class OrdersFiltrationByDatesViewContext
    {
        public List<OrderDTO> Orders { get; set; }

        public DateTime? MinDate { get; set; }

        public DateTime? MaxDate { get; set; }
    }
}
