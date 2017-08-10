using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDK.Platform.AliExpressApi.Model
{
    /// <summary>
    /// 阿里授权令牌
    /// </summary>
    public class AliTokenType : Model.AliErrorType
    {
        private class TimeSpanConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(TimeSpan);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return new TimeSpan(Convert.ToInt64(reader.Value) * 10000000L);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var d = (TimeSpan)value;
                writer.WriteValue(d.TotalSeconds);
            }
        }
        /// <summary>
        /// appKey
        /// </summary>
        [JsonProperty("aliId")]
        public string AliId { get; set; }
        /// <summary>
        /// ISV
        /// </summary>
        [JsonProperty("resource_owner")]
        public string ResourceOwner { get; set; }
        /// <summary>
        /// 凭证有效时间，单位：秒
        /// </summary>
        [JsonProperty("expires_in")]
        [JsonConverter(typeof(TimeSpanConverter))]
        public System.TimeSpan ExpiresIn { get; set; }
        /// <summary>
        /// 长时令牌，有效期半年。当access_token过期后，可以使用refresh_token换取新的access_token访问用户数据。
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        /// <summary>
        /// 长时令牌到期时间
        /// </summary>
        [JsonProperty("refresh_token_timeout")]
        public System.DateTimeOffset? RefreshTokenTimeout { get; set; }
        /// <summary>
        /// 用户授权令牌，为用户一次会话的授权标识，有效期10小时。在获得code后，通过调用开放平台后台接口getToken来获取access_token App在访问用户隐私数据时，需要带上access_token，也只有accessToken才能作为访问的凭证，其他token如code和refreshToken都不能直接作为访问凭证，需要转换为accessToken之后才能访问用户隐私数据。
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("memberId")]
        public string MemberId { get; set; }
    }
}
