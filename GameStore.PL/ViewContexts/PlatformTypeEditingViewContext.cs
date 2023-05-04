using GameStore.PL.DTOs.EditDTOs;
using System;
using System.Collections.Generic;

namespace GameStore.PL.ViewContexts
{
    public class PlatformTypeEditingViewContext
    {
        public  EditPlatformTypeRequestDTO PlatformType { get; set; }

        public Dictionary<string, Guid> AvailableLocalizations { get; set; }
    }
}
