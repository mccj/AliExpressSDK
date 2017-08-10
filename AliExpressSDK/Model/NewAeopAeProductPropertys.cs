using Newtonsoft.Json;
using SDK.Platform.AliExpressApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDK.Platform.AliExpressApi.Model
{
   public class NewAeopAeProductPropertys
    {

        [JsonProperty("attrName")]
        public string attrName { get; set; }
        [JsonProperty("attrNameId")]
        public int attrNameId { get; set; }
        [JsonProperty("attrValue")]
        public string attrValue { get; set; }
        [JsonProperty("attrValueId")]
        public int attrValueId { get; set; }
    }
}
