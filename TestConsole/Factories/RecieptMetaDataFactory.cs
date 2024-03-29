﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TestConsole.Zeeshan;

namespace TestConsole
{
    public abstract class ReceiptMetaDataFactory
    {
        private List<ATMCounter.ATMCounterResponse> Response { get; set; }
        protected ReceiptMetaDataFactory()
        {
           Initialize();
        }

        public void Initialize()
        {
            var contentJson = System.IO.File.ReadAllText("APIFullResponse.txt");
            Response = JsonConvert.DeserializeObject<List<ATMCounter.ATMCounterResponse>>(contentJson);
        }
        public abstract RecieptMetaData CreateMetaData();

        public ATMCounter.ATMCounterResponse GetResponse(string receiptName)
        {
            var receiptMetaData = Response.FirstOrDefault(x => String.Equals(x.recepientName, receiptName,StringComparison.OrdinalIgnoreCase));
            return receiptMetaData;
        }
    }
}
