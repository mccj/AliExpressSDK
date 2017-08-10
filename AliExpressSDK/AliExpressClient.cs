using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SDK.Platform.AliExpressApi.Model;
using System.Net.Http;
using SDK.Platform.AliExpressApi.RModel;

namespace SDK.Platform.AliExpressApi
{
    public partial class AliExpressClient : SDK.BaseAPI.BaseHttpClient
    {
        #region 常量
        ////private const string UrlRefreshToken = "param2/1/system.oauth2/refreshToken";
        //private const string UrlPostponeToken = "param2/1/system.oauth2/postponeToken";
        //private const string UrlgetToken2 = "param2/1/system.oauth2/getToken";
        //private const string UrlCurrentTime = "param2/1/system/currentTime";//获取速卖通服务器时间

        //////
        protected const string openapiIP = "https://gw.api.alibaba.com/openapi/";
        //private const string fileapiIP = "http://gw.api.alibaba.com/fileapi/";
        //private const string UrlGetToken = "http/1/system.oauth2/getToken";
        protected const string Url = "param2/1/aliexpress.open/";
        private const string fieldAopSignature = "_aop_signature";
        protected const string fieldAccessToken = "access_token";
        private const string fieldAopTimeStamp = "_aop_timestamp";
        private bool _throw = true;
        #endregion 常量
        public AliExpressClient(string appKey, string appSecret, string accessToken = null, bool _throw = true)
        : base(SDK.BaseAPI.MediaType.Json)
        {
            this.AppKey = appKey;
            this.AppSecret = appSecret;
            this.AccessToken = accessToken;
            //this.DateTimeConvert = new DateTimeConverter();
            //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式 
            //this.DateTimeConvert = new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyyMMddHHmmssFFFzz'00'" };
            //this.DateTimeConvert = new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyyMMddHHmmssFFF'-0800'" };
            //this.DateTimeConvert = new AliDateTimeConverter { DateTimeFormat = "yyyyMMddHHmmssFFF" };
            //this.DateTimeConvert = new AliDateTimeConverter { DateTimeFormat = "yyyyMMddHHmmssFFFzz'00'" };
            this.DateTimeConvert = new AliExpressConverter { DateTimeOffsetFormat = "yyyyMMddHHmmssFFFzz'00'" };
            this._throw = _throw;
            this.StrClient = new AliExpressStrClient(appKey, () => this.AccessToken, AliExpressClient.openapiIP, AliExpressClient.Url, AliExpressClient.fieldAccessToken, this.PostWebRequest);
            this.ModelClient = new AliExpressModelClient(this.StrClient, DeserializeObject);
        }
        #region 保护属性
        protected AliExpressStrClient StrClient { get; }
        protected AliExpressModelClient ModelClient { get; }
        #endregion 保护属性
        #region 公共属性
        /// <summary>
        /// appKey
        /// </summary>
        public string AppKey { get; private set; }
        /// <summary>
        /// AppSecret 签名串
        /// </summary>
        protected string AppSecret { get; private set; }
        /// <summary>
        /// 访问令牌
        /// </summary>
        protected string AccessToken { get; private set; }
        public JsonConverter DateTimeConvert { get; private set; }
        //public int Api调用次数 { get; private set; }
        #endregion 公共属性
        #region 私有方法
        /// <summary>
        /// 获得授权签名
        /// </summary>
        /// <param name="urlPath">基础url部分，格式为protocol/apiVersion/namespace/apiName/appKey，如 json/1/system/currentTime/1；如果为客户端授权时此参数置为空串""</param>
        /// <param name="paramDic">请求参数，即queryString + request body 中的所有参数</param>
        /// <param name="isAppKey">是否需要传入AppKey</param>
        /// <returns></returns>
        private string Sign(string urlPath, Dictionary<string, object> paramDic, bool isAppKey = false)
        {
            if (isAppKey)
            {
                urlPath += "/" + this.AppKey;
            }
            byte[] signatureKey = Encoding.UTF8.GetBytes(this.AppSecret);//此处用自己的签名密钥
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, object> kv in paramDic)
            {
                list.Add(kv.Key + System.Web.HttpUtility.UrlDecode(Convert.ToString(kv.Value)));
                //list.Add(kv.Key + HttpUtility.UrlDecode(kv.Value));
            }
            list.Sort();
            string tmp = urlPath;
            foreach (string kvstr in list)
            {
                tmp = tmp + kvstr;
            }
            //HMAC-SHA1
            System.Security.Cryptography.HMACSHA1 hmacsha1 = new System.Security.Cryptography.HMACSHA1(signatureKey);
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(tmp));
            /*
            hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(urlPath));
            foreach (string kvstr in list)
            {
                hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(kvstr));
            }
             */
            byte[] hash = hmacsha1.Hash;
            //TO HEX
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
        }
        ///// <summary>
        ///// 获得签名
        ///// </summary>
        ///// <param name="urlPath">网址</param>
        ///// <param name="paramDic">参数</param>
        ///// <returns></returns>
        //public  string Sign(string urlPath, Dictionary<string, string> paramDic, bool isAppKey = true)
        //{
        //    if (isAppKey)
        //    {
        //        urlPath += "/" + this.AppKey;
        //    }
        //    byte[] signatureKey = Encoding.UTF8.GetBytes(this.AppSecret);
        //    List<string> list = new List<string>();
        //    foreach (KeyValuePair<string, string> kv in paramDic)
        //    {
        //        list.Add(kv.Key + HttpUtility.UrlDecode(kv.Value));
        //    }
        //    list.Sort();
        //    string tmp = urlPath;
        //    foreach (string kvstr in list)
        //    {
        //        tmp = tmp + kvstr;
        //    }
        //    HMACSHA1 hmacshal = new HMACSHA1(signatureKey);
        //    hmacshal.ComputeHash(Encoding.UTF8.GetBytes(tmp));
        //    byte[] hash = hmacshal.Hash;
        //    return BitConverter.ToString(hash).Replace("-", string.Empty).ToUpper();
        //}
        //private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        //{   // 总是接受    
        //    return true;
        //}
        /// <summary>
        /// 请求服务器
        /// </summary>
        /// <param name="openapiIP">请求的openapi地址</param>
        /// <param name="appKey">appKey</param>
        /// <param name="urlPath">授权签名的基础url部分</param>
        /// <param name="isFile">是否传输文件</param>
        /// <param name="stream">文件流</param>
        /// <param name="paramDic">设置参数集合</param>
        /// <param name="paramIsSign">是否需要授权签名</param>
        /// <param name="paramIsAppKey">授权签名的基础上是否需要加appkey</param>
        /// <returns></returns>
        protected string PostWebRequest(string openapiIP, string appKey, string urlPath, bool isFile = false, byte[] stream = null, Dictionary<string, object> paramDic = null, bool paramIsSign = false, bool paramIsAppKey = false)
        {
            //string c = PostWebRequest(openapiIP + urlPath + "/" + appKey,
            //   this.GetParamUrl(paramDic, isSign: paramIsSign, isAppKey: paramIsAppKey, urlPath: urlPath),
            //   isFile: isFile, stream: stream);
            //return c;

            int 重试次数 = 0;
            重试:
            var postUrl = openapiIP + urlPath + "/" + appKey;
            var paramData = this.GetParamUrl(paramDic, isSign: paramIsSign, isAppKey: paramIsAppKey, urlPath: urlPath);
            var html = "";
            if (isFile)
            {
                //html = this.PostBytesAsync<string>(this.UrlAddQuery(postUrl, paramData.QueryString()), stream);
                html = this.PostBytesAsync(this.UrlAddQuery(postUrl, paramData.QueryString()), stream).GetResultContent();
            }
            else
            {
                html = this.PostWebRequest2(postUrl, paramData);
            }

            dynamic sss = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Dynamic.ExpandoObject2>(html);
            if (sss.exception == "Request need user authorized")
            {
                //授权错误
                重试次数++;
                if (重试次数 >= 3)
                {
                    return html;
                }
                if (this.GetAccessToken != null)
                {
                    this.SetAccessToken(this.GetAccessToken());
                    paramDic[AliExpressClient.fieldAccessToken] = this.AccessToken;//用户授权令牌

                    goto 重试;
                }
            }

            return html;
        }
        protected string PostWebRequest2(string url, SDK.BaseAPI.ParamR param)
        {
            //return this.PostFormUrlAsync<System.String>(url, param.ToDictionary().ToDictionary(f => f.Key, f => Convert.ToString(f.Value)).ToArray());
            //return this.PostBytesAsync<System.String>(url, param.ToBytes(), param.ContentType);
                return this.PostBytesAsync(url, param.ToBytes(), param.ContentType).GetResultContent();
        }
        //        ///// <summary>
        //        ///// 请求服务器
        //        ///// </summary>
        //        ///// <param name="postUrl"></param>
        //        ///// <param name="paramData">请求参数</param>
        //        ///// <param name="isFile">是否传输文件</param>
        //        ///// <param name="stream">文件流</param>
        //        ///// <returns></returns>
        //        //private string PostWebRequest(string postUrl, ParamR paramData, bool isFile = false, byte[] stream = null)
        //        //{
        //        //    if (isFile)
        //        //    {
        //        //        return this.PostWebRequest2(UrlAddQuery(postUrl, paramData.QueryString()), stream);
        //        //    }
        //        //    else
        //        //    {
        //        //        return this.PostWebRequest2(postUrl, paramData);
        //        //    }
        //        //    //if (paramData == null) paramData = new ParamR();
        //        //    //var url = postUrl; //UrlAddQuery(postUrl, paramData.QueryString());
        //        //    ////var url = new Uri(postUrl);
        //        //    ////var paramQuery = paramData.QueryString();
        //        //    ////if (isFile && !string.IsNullOrWhiteSpace(paramQuery))
        //        //    ////{
        //        //    ////    var query = HttpUtility.ParseQueryString(url.Query);
        //        //    ////    var param = HttpUtility.ParseQueryString(paramQuery);
        //        //    ////    foreach (var item in param.AllKeys)
        //        //    ////    {
        //        //    ////        query[item] = param[item];
        //        //    ////    }
        //        //    ////    url = new Uri(url, "?" + query.ToString());
        //        //    ////}
        //        //    //超时链接:
        //        //    //string ret = "";
        //        //    //int timeoutLength = 0;
        //        //    //try
        //        //    //{
        //        //    //    if (ServicePointManager.ServerCertificateValidationCallback == null)
        //        //    //        ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(CheckValidationResult);
        //        //    //    //postUrl = "http://gw.api.alibaba.com:80/fileapi/param2/1/aliexpress.open/api.uploadTempImage/6722251?srcFileName=1.jpg&access_token=3bb82aa5-47ab-4751-9431-445c318287f4&_aop_signature=92A24DAB0D7F7DADE819A1822ED35F43AC92F415";

        //        //    //    //Dictionary<string,object> d=new Dictionary<string,object>();
        //        //    //    //d.Add("srcFileName","1.jpg");
        //        //    //    //d.Add("access_token","3bb82aa5-47ab-4751-9431-445c318287f4");
        //        //    //    //string s= Sign("param2/1/aliexpress.open/api.uploadTempImage", d, true);

        //        //    //    //if (s == "92A24DAB0D7F7DADE819A1822ED35F43AC92F415")
        //        //    //    //{

        //        //    //    //}

        //        //    //    // 创建request对象
        //        //    //    HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
        //        //    //    //webReq.KeepAlive = false;
        //        //    //    //webrequest.ServicePoint.ConnectionLimit = 100;
        //        //    //    webrequest.Method = "POST";
        //        //    //    // 这个可以是改变的，也可以是下面这个固定的字符串
        //        //    //    Stream newStream = null;
        //        //    //    if (isFile)
        //        //    //    {
        //        //    //        webrequest.ContentLength = stream.Length;
        //        //    //        newStream = webrequest.GetRequestStream();
        //        //    //        newStream.Write(stream, 0, stream.Length);
        //        //    //        newStream.Close();
        //        //    //    }
        //        //    //    else
        //        //    //    {
        //        //    //        webrequest.ContentType = paramData.ContentType;
        //        //    //        byte[] byteArray = paramData.ToBytes(Encoding.UTF8); //转化
        //        //    //        webrequest.ContentLength = byteArray.Length;
        //        //    //        newStream = webrequest.GetRequestStream();
        //        //    //        newStream.Write(byteArray, 0, byteArray.Length); //写入参数
        //        //    //        newStream.Close();
        //        //    //    }
        //        //    //    HttpWebResponse response = (HttpWebResponse)webrequest.GetResponse();
        //        //    //    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
        //        //    //    ret = sr.ReadToEnd();
        //        //    //    sr.Close();
        //        //    //    response.Close();
        //        //    //    newStream.Close();
        //        //    //    lock (this)
        //        //    //    {
        //        //    //        Api调用次数++;
        //        //    //    }
        //        //    //}
        //        //    //catch (WebException ex)
        //        //    //{
        //        //    //    if ((ex.Status == WebExceptionStatus.Timeout || ex.Status == WebExceptionStatus.ReceiveFailure) && timeoutLength <= 3)
        //        //    //    {//超时重试3次
        //        //    //        timeoutLength++;
        //        //    //        goto 超时链接;
        //        //    //    }
        //        //    //    if (ex.Response != null)
        //        //    //    {
        //        //    //        StreamReader sr = new StreamReader(ex.Response.GetResponseStream(), Encoding.UTF8);
        //        //    //        ret = sr.ReadToEnd();
        //        //    //    }
        //        //    //    else
        //        //    //    {
        //        //    //        throw ex;
        //        //    //        //ret = ex.Message;
        //        //    //    }
        //        //    //}
        //        //    //catch (System.Net.Sockets.SocketException ex)
        //        //    //{
        //        //    //    if (timeoutLength <= 3)
        //        //    //    {//超时重试3次
        //        //    //        timeoutLength++;
        //        //    //        goto 超时链接;
        //        //    //    }
        //        //    //    throw ex;
        //        //    //}
        //        //    //catch (Exception ex)
        //        //    //{
        //        //    //    throw ex;
        //        //    //}
        //        //    //return ret;
        //        //}
        /// <summary>
        /// 把paramDic转换成url数据
        /// </summary>
        /// <param name="paramDic">设置参数集合</param>
        /// <param name="isSign">是否需要授权签名</param>
        /// <param name="urlPath">授权签名的基础url部分</param>
        /// <param name="isAppKey">授权签名的基础上是否需要加appkey</param>
        /// <param name="isTimeStamp">是否需要时间戳</param>
        /// <returns></returns>
        protected SDK.BaseAPI.ParamR GetParamUrl(Dictionary<string, object> paramDic, bool isSign = false, string urlPath = null, bool isAppKey = false, bool isTimeStamp = false)
        {
            if (paramDic == null)
                return new SDK.BaseAPI.ParamR();
            //移除内容为空的参数
            paramDic = paramDic.Where(f => !string.IsNullOrWhiteSpace(f.Key) && f.Value != null && !string.IsNullOrWhiteSpace(f.Value.ToString())).ToDictionary(f => f.Key, f => f.Value);
            if (isSign)
            {//授权签名
                paramDic.Add(AliExpressClient.fieldAopSignature, this.Sign(urlPath, paramDic, isAppKey));
            }
            if (isTimeStamp)
            {//授权签名
                paramDic.Add(AliExpressClient.fieldAopTimeStamp, this.GetTimeStamp(System.DateTime.Now));
            }
            //转换数据
            //var query = string.Join("&", paramDic.Select(f => f.Key + "=" + f.Value).ToArray());
            //return query;


            //string tmp = "";
            //foreach (KeyValuePair<string, object> kv in paramDic)
            //{
            //    tmp += kv.Key + "=" + kv.Value + "&";
            //}
            //tmp = tmp.Trim('&');
            //return tmp;
            return new SDK.BaseAPI.ParamR(paramDic);
        }
        protected string ConvertTimeParam(System.DateTimeOffset? dt)
        {//格式: mm/dd/yyyy hh:mm:ss,如10/09/2013 00:00:00
            if (dt.HasValue)
                //速卖通使用的是 UTC-7 的时区
                //return dt.Value.ToUniversalTime().AddHours(-7).ToString("MM\\/dd\\/yyyy HH:mm:ss");
                return dt.Value.ToOffset(new TimeSpan(-7, 0, 0)).ToString("MM\\/dd\\/yyyy HH:mm:ss");
            else
                return null;
        }
        //protected string ConvertDataParam(System.DateTime? dt)
        //{//例如：yyyy-mm-dd
        //    if (dt.HasValue)
        //        //速卖通使用的是 UTC-7 的时区
        //        return dt.Value.ToUniversalTime().AddHours(-7).ToString("yyyy-MM-dd");
        //    else
        //        return null;
        //}
        protected string ConvertDataParam(System.DateTimeOffset? dt)
        {//例如：yyyy-mm-dd
            if (dt.HasValue)
                //速卖通使用的是 UTC-7 的时区
                //return dt.Value.ToUniversalTime().AddHours(-7).ToString("yyyy-MM-dd");
                return dt.Value.ToOffset(new TimeSpan(-7, 0, 0)).ToString("yyyy-MM-dd");
            else
                return null;
        }
        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        private long GetTimeStamp(System.DateTime dt)
        {
            //TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan ts = dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
        private object DeserializeObject(Type type, string value)
        {
            //try
            //{
            var s = JsonConvert.DeserializeObject(value, type, this.DateTimeConvert);
            if (_throw && s is AliErrorType)
            {
                if (s is AliSuccessType)
                {
                    var err = (s as AliSuccessType);
                    if (!err.Success)
                    {
                        throw new AliException(err);
                    }
                }
                else
                {
                    var err = (s as AliErrorType);
                    if (!string.IsNullOrWhiteSpace(err.Exception) || !string.IsNullOrWhiteSpace(err.ErrorMessage))
                        throw new AliException(err);
                    //if (!string.IsNullOrWhiteSpace(err.Exception))
                    //    throw new AliException(err.Exception);
                }
            }
            return s;
            //}
            //catch (Exception)
            //{
            //    throw new Exception(value);
            //}
        }
        protected T DeserializeObject<T>(string value)
        {
            return (T)DeserializeObject(typeof(T), value);
        }
        #endregion 私有方法
        protected override object ErrorHandle(Type type, HttpResponseMessage message)
        {
            //return base.ErrorHandle(type, message);
            return message.GetResultContent();
        }
        private class AliExpressConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                if (objectType == null)
                {
                    throw new ArgumentNullException("objectType");
                }
                bool nullable = (objectType.IsGenericType && (objectType.GetGenericTypeDefinition() == typeof(Nullable<>)));
                Type t = (nullable) ? Nullable.GetUnderlyingType(objectType) : objectType;
                if (
                    t == typeof(System.DateTimeOffset) ||
                    t == typeof(System.DateTime) ||
                    t == typeof(System.TimeSpan)
                )
                    return true;
                else
                    return false;
            }
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (objectType == null)
                {
                    throw new ArgumentNullException("objectType");
                }
                bool nullable = (objectType.IsGenericType && (objectType.GetGenericTypeDefinition() == typeof(Nullable<>)));
                Type t = (nullable) ? Nullable.GetUnderlyingType(objectType) : objectType;
                if (t == typeof(System.DateTimeOffset))
                {
                    if (reader.Value is long)
                        return new System.DateTimeOffset(new System.DateTime(Convert.ToInt64(reader.Value)));
                    else if (reader.Value is string)
                    {
                        string dateText = reader.Value as string;
                        if (!string.IsNullOrEmpty(this.DateTimeOffsetFormat))
                            return DateTimeOffset.ParseExact(dateText, this.DateTimeOffsetFormat, this.Culture, this.DateTimeStyles);
                        else
                            return DateTimeOffset.Parse(dateText, Culture, this.DateTimeStyles);
                    }
                }
                else if (t == typeof(System.DateTime))
                {
                    if (reader.Value is long)
                        return new System.DateTime(Convert.ToInt64(reader.Value));
                    else if (reader.Value is string)
                    {
                        string dateText = reader.Value as string;
                        if (!string.IsNullOrEmpty(this.DateTimeFormat))
                            return DateTime.ParseExact(dateText, this.DateTimeFormat, this.Culture, this.DateTimeStyles);
                        else
                            return DateTime.Parse(dateText, Culture, this.DateTimeStyles);
                    }
                }
                else if (t == typeof(System.TimeSpan))
                {
                    if (reader.Value is long)
                        return new System.TimeSpan(Convert.ToInt64(reader.Value));
                    else if (reader.Value is string)
                    {
                        string dateText = reader.Value as string;
                        if (!string.IsNullOrEmpty(this.TimeSpanFormat))
                            return TimeSpan.ParseExact(dateText, this.TimeSpanFormat, this.Culture);
                        else
                            return TimeSpan.Parse(dateText, Culture);
                    }
                }
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                //writer.WriteValue("");
            }

            public string TimeSpanFormat { get; set; }
            public string DateTimeFormat { get; set; }

            public string DateTimeOffsetFormat { get; set; }
            public System.Globalization.DateTimeStyles DateTimeStyles
            {
                get { return _dateTimeStyles; }
                set { _dateTimeStyles = value; }
            }
            private System.Globalization.DateTimeStyles _dateTimeStyles = System.Globalization.DateTimeStyles.RoundtripKind;
            public System.Globalization.CultureInfo Culture
            {
                get { return _culture ?? System.Globalization.CultureInfo.CurrentCulture; }
                set { _culture = value; }
            }
            private System.Globalization.CultureInfo _culture;
        }
    }
}