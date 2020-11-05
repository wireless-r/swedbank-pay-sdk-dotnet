﻿using SwedbankPay.Sdk.Extensions;
using SwedbankPay.Sdk.PaymentOrders;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace SwedbankPay.Sdk.PaymentInstruments.Card
{
    public class CardPaymentOperations : OperationsBase, ICardPaymentOperations
    {
        internal CardPaymentOperations(IOperationList operations, HttpClient client)
        {
            foreach (var httpOperation in operations)
            {
                switch (httpOperation.Rel.Value)
                {
                    case PaymentResourceOperations.UpdatePaymentAbort:
                        Abort = async payload => {
                            var dto = await client.SendAsJsonAsync<CardPaymentResponseDto>(httpOperation.Method, httpOperation.Href, payload);
                            return new CardPaymentResponse(dto, client);
                        };
                        break;
                    case PaymentResourceOperations.RedirectAuthorization:
                        RedirectAuthorization = httpOperation;
                        break;

                    case PaymentResourceOperations.ViewAuthorization:
                        ViewAuthorization = httpOperation;
                        break;

                    case PaymentResourceOperations.DirectAuthorization:
                        DirectAuthorization = async payload => {
                            var dto = await client.SendAsJsonAsync<CardPaymentAuthorizationResponseDto>(httpOperation.Method, httpOperation.Href, payload);
                            return new CardPaymentAuthorizationResponse(dto.Payment, dto.Authorization.MapToCard());
                        };
                        break;

                    case PaymentResourceOperations.CreateCapture:
                        Capture = async payload => {
                            var dto = await client.SendAsJsonAsync<CaptureResponseDto>(httpOperation.Method, httpOperation.Href, payload);
                            return new CaptureResponse(dto.payment, dto.Capture.Map());
                        };
                        break;

                    case PaymentResourceOperations.CreateCancellation:
                        Cancel = async payload => {
                            var dto = await client.SendAsJsonAsync<CancellationResponseDto>(httpOperation.Method, httpOperation.Href, payload);
                            return new CancellationResponse(dto.Payment, dto.Cancellation.Map());
                        };
                        break;

                    case PaymentResourceOperations.CreateReversal:
                        Reverse = async payload => {
                            var dto = await client.SendAsJsonAsync<ReversalResponseDto>(httpOperation.Method, httpOperation.Href, payload);
                            return new ReversalResponse(dto.Payment, dto.Reversal.Map());
                        };
                        break;

                    case PaymentResourceOperations.RedirectVerification:
                        RedirectVerification = httpOperation;
                        break;

                    case PaymentResourceOperations.ViewVerification:
                        ViewVerification = httpOperation;
                        break;

                    case PaymentResourceOperations.DirectVerification:
                        DirectVerification = httpOperation;
                        break;

                    case PaymentResourceOperations.PaidPayment:
                        PaidPayment = httpOperation;
                        break;
                    default:
                        break;
                }
                Add(httpOperation.Rel, httpOperation);
            }
        }

        public Func<CardPaymentCancelRequest, Task<ICancellationResponse>> Cancel { get; }
        public Func<CardPaymentCaptureRequest, Task<ICaptureResponse>> Capture { get; }
        public Func<CardPaymentAuthorizationRequest, Task<CardPaymentAuthorizationResponse>> DirectAuthorization { get; }
        public Func<CardPaymentReversalRequest, Task<IReversalResponse>> Reverse { get; }
        public Func<CardPaymentAbortRequest, Task<ICardPaymentResponse>> Abort { get; }
        public HttpOperation DirectVerification { get; internal set; }
        public HttpOperation PaidPayment { get; internal set; }
        public HttpOperation RedirectAuthorization { get; internal set; }
        public HttpOperation RedirectVerification { get; internal set; }
        public HttpOperation ViewAuthorization { get; internal set; }
        public HttpOperation ViewVerification { get; internal set; }
    }
}