using GameStore.BLL.PaymentMethods;
using System;

namespace GameStore.BLL.Extensions
{
    public static class PaymentExtensions
    {
        private static readonly int BankTimeOut = 3;
        private static readonly int IBoxTimeOut = 1;
        private static readonly int VisaTimeOut = 1;

        public static DateTime CalculateOrderPaymentExpirationDate(this DateTime paymentExpirationDate, PaymentType type)
        {

            switch (type)
            {
                case PaymentType.Bank:
                    paymentExpirationDate = paymentExpirationDate.AddDays(BankTimeOut);
                    break;
                case PaymentType.IBox:
                    paymentExpirationDate = paymentExpirationDate.AddMinutes(IBoxTimeOut);
                    break;
                case PaymentType.Visa:
                    paymentExpirationDate = paymentExpirationDate.AddHours(VisaTimeOut);
                    break;
                default:
                    break;
            }

            return paymentExpirationDate;
        }
    }
}
