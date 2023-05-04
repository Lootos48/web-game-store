using GameStore.BLL.PaymentMethods.Interfaces;
using System;
using System.Threading.Tasks;

namespace GameStore.BLL.PaymentMethods
{
    public class IBoxPayment : IPaymentMethod
    {
        public PaymentType Type => PaymentType.IBox;

        public Task ProceedPaymentAsync(Guid orderId)
        {
            return Task.CompletedTask;
        }
    }
}
