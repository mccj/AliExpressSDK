using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDK.Platform.AliExpressApi.RModel
{
    public class AliCategoryType
    {
        [JsonProperty("id")]
        public long Id;
        [JsonProperty("level")]
        public int Level;
        [JsonProperty("isleaf")]
        public bool IsLeaf;
        [JsonProperty("names")]
        public CategoryNames Names;
        public long ParentId;
    }
    public class CategoryNames { 
        [JsonProperty("zh")]
        public string zh;
        [JsonProperty("pt")]
        public string pt;
        [JsonProperty("en")]
        public string en;
        [JsonProperty("ru")]
        public string ru;
        [JsonProperty("in")]
        public string ins;
        [JsonProperty("es")]
        public string es;
    }
}
