using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDK.Platform.AliExpressApi.RModel
{
    public class AliAttributesType : Model.AliErrorType
    {
        [JsonProperty("attributes")]
        public AliAttributesList[] attributes { get; set; }
        [JsonProperty("success")]
        public bool success { get; set; }
    }
    public class AliAttributesList {

        [JsonProperty("spec")]
        public int Spec { get; set; }
        [JsonProperty("visible")]
        public bool Visible { get; set; }
        [JsonProperty("customizedName")]
        public bool CustomizedName { get; set; }
        [JsonProperty("customizedPic")]
        public bool CustomizedPic { get; set; }
        [JsonProperty("keyAttribute")]
        public bool KeyAttribute { get; set; }
        [JsonProperty("sku")]
        public bool SKU { get; set; }
        [JsonProperty("id")]
        public Int32 Id { get; set; }
        [JsonProperty("values")]
        public AliAttributeValueList[] AttributeValueList { get; set; }
        [JsonProperty("names")]
        public AliAttributeNameList AttributeName { get; set; }
        [JsonProperty("inputType")]
        public string InputType { get; set; }
        [JsonProperty("required")]
        public bool Required { get; set; }
        [JsonProperty("attributeShowTypeValue")]
        public string AttributeShowTypeValue { get; set; }
    }
    
    public class AliAttributeValueList{
        [JsonProperty("id")]
        public Int32 Id{get;set;}
        [JsonProperty("names")]
        public AliAttributeNameList AliAttributeNameList { get; set; }
    }
    public class AliAttributeNameList{
        [JsonProperty("zh")]
        public string Zh{get;set;}
        [JsonProperty("en")]
        public string En{get;set;}
    }
}
