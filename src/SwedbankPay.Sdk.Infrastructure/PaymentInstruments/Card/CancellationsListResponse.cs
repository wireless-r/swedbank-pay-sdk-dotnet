﻿using SwedbankPay.Sdk.Common;
using System;
using System.Collections.Generic;

namespace SwedbankPay.Sdk.PaymentInstruments.Card
{
    public class CancellationsListResponse : IdLink, ICancellationsListResponse
    {
        public CancellationsListResponse(Uri id, List<ITransactionResponse> cancellationList)
        {
            Id = id;
            CancellationList = cancellationList;
        }


        public List<ITransactionResponse> CancellationList { get; }
    }
}