using System;
using System.Threading.Tasks;

namespace GameStore.BLL.Workflows.Interfaces
{
    public interface IInvoiceGenerationWorkflow
    {
        Task<byte[]> GetBankInvoiceAsync(Guid id);
    }
}