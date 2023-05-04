using GameStore.BLL.Interfaces;
using GameStore.BLL.PaymentMethods;
using GameStore.BLL.PaymentMethods.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameStore.BLL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IEnumerable<IPaymentMethod> _payments;

        public PaymentService(IEnumerable<IPaymentMethod> payments)
        {
            _payments = payments;
        }

        public Task PayAsync(Guid orderId, PaymentType paymentType)
        {
            IPaymentMethod payment = _payments.FirstOrDefault(p => p.Type == paymentType);
            if (payment is null)
            {
                throw new ArgumentException($"Invalid payment type was chosen: {paymentType}");
            }

            return payment.ProceedPaymentAsync(orderId);
        }
    }
}
