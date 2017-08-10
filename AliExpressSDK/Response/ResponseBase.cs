//namespace AliExpressSDK
//{
//    using Newtonsoft.Json;
//    using System;
//    using System.Dynamic;
//    using System.Runtime.CompilerServices;

//    public class ResponseBase : DynamicObject, CustomAPI.IErrorResponse
//    {
//        [JsonProperty("code")]
//        public int? ErrorCode { get; set; }

//        //[JsonProperty("data")]
//        //public object Data { get; set; }

//        [JsonProperty("message")]
//        public string ErrorMessage { get; set; }






//        public string ErrorType { get { return string.Empty; } }

//        public bool IsError
//        {
//            get
//            {
//                return this.ErrorCode.HasValue && this.ErrorCode.Value > 0;
//            }
//        }
//    }
//}

