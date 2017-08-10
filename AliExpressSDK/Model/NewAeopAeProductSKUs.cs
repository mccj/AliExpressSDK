using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDK.Platform.AliExpressApi.Model
{
    public class NewAeopAeProductSKUs
    {


        public NewAeopSKUProperty[] aeopSKUProperty { get; set; }
        public string currencyCode { get; set; }
        public string id { get; set; }
        public long ipmSkuStock { get; set; }
        public string skuCode { get; set; }
        public string skuPrice { get; set; }
        public bool skuStock
        {
            get; set;
        }
    }
}
