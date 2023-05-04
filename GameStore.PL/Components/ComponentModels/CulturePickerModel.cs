using System.Collections.Generic;
using System.Globalization;

namespace GameStore.PL.Components.ComponentModels
{
    public class CulturePickerModel
    {
        public CultureInfo CurrentUICulture { get; set; }

        public List<CultureInfo> SupportedCultures { get; set; }
    }
}
