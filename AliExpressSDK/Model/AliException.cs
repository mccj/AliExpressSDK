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

namespace SDK.Platform.AliExpressApi.Model
{
    public class AliException : Exception
    {
        public AliException(AliErrorType message, Exception innerException = null) : base(message.ErrorMessage ?? message.Exception, innerException)
        {
            Error = message;
        }
        public AliErrorType Error { get; }
    }
}