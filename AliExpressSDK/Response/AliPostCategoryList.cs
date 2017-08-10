using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDK.Platform.AliExpressApi.RModel
{
    public class AliPostCategoryList : Model.AliErrorType
    {
        [JsonProperty("aeopPostCategoryList")]
        public AliCategoryType[] CategoryList;

        [JsonProperty("success")]
        public bool success;
    }
}
