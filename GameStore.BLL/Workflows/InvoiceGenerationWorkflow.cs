using GameStore.BLL.Interfaces;
using GameStore.BLL.Util.Interfaces;
using GameStore.BLL.Workflows.Interfaces;
using GameStore.DomainModels.Models;
using System;
using System.Threading.Tasks;

namespace GameStore.BLL.Workflows
{
    public class InvoiceGenerationWorkflow : IInvoiceGenerationWorkflow
    {
        private readonly IOrderService _orderService;
        private readonly IInvoicePdfGenerator _invoicePdfGenerator;

        public InvoiceGenerationWorkflow(
            IOrderService orderService,
            IInvoicePdfGenerator invoicePdfGenerator)
        {
            _orderService = orderService;
            _invoicePdfGenerator = invoicePdfGenerator;
        }

        public async Task<byte[]> GetBankInvoiceAsync(Guid id)
        {
            Order foundedOrder = await _orderService.GetOrderByIdAsync(id);
            Invoice invoice = new Invoice()
            {
                Order = foundedOrder,
                TotalPrice = foundedOrder.TotalPrice
            };

            return _invoicePdfGenerator.Generate(invoice);
        }
    }
}
