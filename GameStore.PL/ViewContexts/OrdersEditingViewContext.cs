using GameStore.PL.DTOs;
using GameStore.PL.DTOs.EditDTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class OrdersEditingViewContext
    {
        public EditOrderRequestDTO OrderToEdit { get; set; }

        public List<ShipperDTO> Shippers { get; set; }

        public IEnumerable<SelectListItem> GetShippersAsSelectItem()
        {
            return new SelectList(Shippers, "ShipperId", "CompanyName");
        }
    }
}
