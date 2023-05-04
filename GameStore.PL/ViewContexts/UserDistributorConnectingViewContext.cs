using GameStore.PL.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class UserDistributorConnectingViewContext
    {
        public UserDTO User { get; set; }

        public List<DistributorDTO> Distributors { get; set; }

        public string DistributorId { get; set; }

        public IEnumerable<SelectListItem> GetDistributorsAsSelectList()
        {
            return new SelectList(Distributors, "Id", "CompanyName");
        }
    }
}
