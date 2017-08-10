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
    public sealed partial class AliExpressModelClient
    {
        private readonly Func<Type, string, object> _deserializeObject;
        internal AliExpressModelClient(AliExpressStrClient strClient, Func<Type, string, object> deserializeObject)
        {
            this.StrClient = strClient;
            this._deserializeObject = deserializeObject;
        }
        private T DeserializeObject<T>(string value)
        {
            return (T)_deserializeObject(typeof(T), value);
        }
        private AliExpressStrClient StrClient { get; }
    }
}