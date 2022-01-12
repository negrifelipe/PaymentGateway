﻿using Microsoft.AspNetCore.WebUtilities;
using RestoreMonarchy.PaymentGateway.API.Abstractions;
using RestoreMonarchy.PaymentGateway.API.Models;
using RestoreMonarchy.PaymentGateway.API.Results;
using RestoreMonarchy.PaymentGateway.API.Services;
using RestoreMonarchy.PaymentGateway.Providers.PayPal.Components;
using RestoreMonarchy.PaymentGateway.Providers.PayPal.Models;

namespace RestoreMonarchy.PaymentGateway.Providers.PayPal
{
    public class PayPalPaymentProvider : PaymentProvider
    {
        public override string Name => "PayPal";
        public override Type FormComponentType => typeof(PayPalParametersForm);
        public override Type InfoComponentType => typeof(PayPalPaymentInfo);

        private readonly IPaymentService paymentService;
        private readonly IBaseUrl baseUrl;

        public PayPalPaymentProvider(IPaymentService paymentService, IBaseUrl baseUrl)
        {
            this.paymentService = paymentService;
            this.baseUrl = baseUrl;
        }

        public override async Task<UserAction> StartPaymentAsync(Guid publicId)
        {
            PaymentWithParameters<PayPalParameters> pwp = await paymentService.GetPaymentWithParameters<PayPalParameters>(publicId);

            Dictionary<string, string> dict = new()
            {
                { "cmd", "_cart" },
                { "upload", "1" },
                { "business", pwp.Parameters.GetReceiver(pwp.Payment.Receiver) },
                { "custom", pwp.Payment.PublicId.ToString() },
                { "currency_code", pwp.Payment.Currency },
                { "amount", pwp.Payment.Amount.ToString() },
                { "no_shipping", "1" },
                { "no_note", "1" },
                { "notify_url", baseUrl.Get("api/payments/notify/paypal") },
                { "return", pwp.Payment.Store.ReturnUrl },
                { "cancel_return", pwp.Payment.Store.CancelUrl }
            };

            for (int i = 1; i <= pwp.Payment.Items.Count; i++)
            {
                PaymentItemInfo item = pwp.Payment.Items[i - 1];
                dict.Add("item_name_" + i, item.Name);
                dict.Add("quantity_" + i, item.Quantity.ToString());
                dict.Add("amount_" + i, item.Price.ToString());
            }

            return Redirect(QueryHelpers.AddQueryString(pwp.Parameters.GetUrl(), dict));
        }
    }
}