using GameStore.DomainModels.Models;

namespace GameStore.BLL.Util.Interfaces
{
    public interface IInvoicePdfGenerator
    {
        byte[] Generate(Invoice invoice);
    }
}
