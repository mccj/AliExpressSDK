using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDK.Platform.AliExpressApi.Model
{

    public class ListResult
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
    }

    public class SellerEvaluationOrderList : AliSuccessType
    {
        [JsonProperty("listResult")]
        public ListResult[] ListResult { get; set; }

        [JsonProperty("totalItem")]
        public int TotalItem { get; set; }
    }
}
