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
    public sealed partial class AliExpressStrClient
    {
        private readonly string _fieldAccessToken;
        private string _accessToken { get { return _accessTokenFun(); } }
        private readonly Func<string> _accessTokenFun;
        private readonly string _openapiIP;
        private readonly string _appKey;
        private readonly string _url;
        private readonly System.Func<string, string, string, bool, byte[], Dictionary<string, object>, bool, bool, string> _postWebRequest;
        internal AliExpressStrClient(string appKey, Func<string> accessToken, string openapiIP, string url, string fieldAccessToken,
            System.Func<string, string, string, bool, byte[], Dictionary<string, object>, bool, bool, string> postWebRequest)
        {
            this._appKey = appKey;
            this._openapiIP = openapiIP;
            this._accessTokenFun = accessToken;
            this._url = url;
            this._fieldAccessToken = fieldAccessToken;
            this._postWebRequest = postWebRequest;
        }
        private string postWebRequest(string openapiIP, string appKey, string urlPath, bool isFile = false, byte[] stream = null, Dictionary<string, object> paramDic = null, bool paramIsSign = false, bool paramIsAppKey = false)
        {
            return _postWebRequest(openapiIP, appKey, urlPath, isFile, stream, paramDic, paramIsSign, paramIsAppKey);
        }
    }
}