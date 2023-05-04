using GameStore.BLL.PaymentMethods;
using System;
using System.Threading.Tasks;

namespace GameStore.BLL.Interfaces
{
    public interface IPaymentService
    {
        public Task PayAsync(Guid orderId, PaymentType paymentType);
    }
}
