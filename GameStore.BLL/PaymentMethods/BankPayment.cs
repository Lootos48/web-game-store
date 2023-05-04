using GameStore.BLL.PaymentMethods.Interfaces;
using System;
using System.Threading.Tasks;

namespace GameStore.BLL.PaymentMethods
{
    public class BankPayment : IPaymentMethod
    {
        public PaymentType Type => PaymentType.Bank;

        public Task ProceedPaymentAsync(Guid orderId)
        {
            return Task.CompletedTask;
        }
    }
}
