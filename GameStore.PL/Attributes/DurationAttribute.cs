using System.ComponentModel.DataAnnotations;

namespace GameStore.PL.Attributes
{
    public class DurationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return uint.TryParse(value.ToString(), out uint val) && val > 0;
        }
    }
}