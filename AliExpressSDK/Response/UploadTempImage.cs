using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDK.Platform.AliExpressApi.RModel
{
    public class UploadTempImage : Model.AliSuccessType
    {
        [JsonProperty("height")]
        public int height;
        [JsonProperty("width")]
        public int width;
        [JsonProperty("srcFileName")]
        public string srcFileName;
        [JsonProperty("url")]
        public string url;
    }
    public class UploadImage : Model.AliSuccessType
    {
        [JsonProperty("height")]
        public int height;
        [JsonProperty("width")]
        public int width;
        [JsonProperty("photobankUsedSize")]
        public string photobankUsedSize;
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("photobankUrl")]
        public string photobankUrl { get; set; }
        [JsonProperty("fileName")]
        public string fileName { get; set; }
    }
}
