using System;
using System.Threading.Tasks;

namespace GameStore.BLL.PaymentMethods.Interfaces
{
    public interface IPaymentMethod
    {
        public PaymentType Type { get; }
        public Task ProceedPaymentAsync(Guid orderId);
    }
}
