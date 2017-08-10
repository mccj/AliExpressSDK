using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDK.Platform.AliExpressApi.Model
{
    public class AliErrorType
    {
        /// <summary>
        /// 错误
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// 错误
        /// </summary>
        [JsonProperty("error")]
        public string Error { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("error_code")]
        public virtual string ErrorCode { get; set; }

        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("exception")]
        public string Exception { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("errorMsg")]
        public string ErrorMsg { get; set; }
        [JsonProperty("localizedMessage")]
        public string LocalizedMessage { get; set; }

        [JsonProperty("stackTrace")]
        public StackTrace[] StackTrace { get; set; }
        [JsonProperty("errorDesc")]
        public string ErrorDesc { get; set; }

    }
    public class StackTrace
    {

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("lineNumber")]
        public int LineNumber { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        [JsonProperty("nativeMethod")]
        public bool NativeMethod { get; set; }
    }
    public class AliSuccessType : AliErrorType
    {

        [JsonProperty("success")]
        public virtual bool Success { get; set; }
    }

}
