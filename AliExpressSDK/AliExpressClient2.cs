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
using SDK.BaseAPI;

namespace SDK.Platform.AliExpressApi
{
    partial class AliExpressClient
    {
        #region 常量
        ////private const string UrlRefreshToken = "param2/1/system.oauth2/refreshToken";
        private const string UrlPostponeToken = "param2/1/system.oauth2/postponeToken";
        private const string UrlgetToken2 = "param2/1/system.oauth2/getToken";
        private const string UrlGetToken = "http/1/system.oauth2/getToken";



        private const string UrlCurrentTime = "param2/1/system/currentTime";//获取速卖通服务器时间
        #endregion 常量

        #region AliAPI名称
        /// <summary>
        /// 上传产品
        /// </summary>
        private const string Api_postAeProduct = "api.postAeProduct";
        /// <summary>
        /// 上传产品图片
        /// </summary>
        private const string Api_uploadTempImage = "api.uploadTempImage";
        private const string Api_uploadImage = "api.uploadImage";
        /// <summary>
        /// 单sku更新库存
        /// </summary>
        private const string Api_editSingleSkuStock = "api.editSingleSkuStock";
        /// <summary>
        /// 单sku更新价格
        /// </summary>
        private const string Api_editSingleSkuPrice = "api.editSingleSkuPrice";

        /// <summary>
        /// 更新产品
        /// </summary>
        private const string Api_editProductCidAttIdSku = "api.editProductCidAttIdSku";

        /// <summary>
        /// 根据发布类目id、父属性路径（可选）获取子属性信息
        /// </summary>
        private const string Api_getChildAttributesResultByPostCateIdAndPath = "api.getChildAttributesResultByPostCateIdAndPath";
        #endregion



        #region 授权相关
        /// <summary>
        /// app发起授权请求网址,获取临时令牌（有效期2分钟）
        /// </summary>
        /// <param name="redirectUri">
        /// app的入口地址，授权临时令牌会以queryString的形式跟在该url后返回
        /// urn:ietf:wg:oauth:2.0:oob   Code以body的方式返回到默认的alibaba页面
        /// http://gw.open.1688.com/auth/authCode.htm   Code以queryString的方式返回到该url
        /// http://gw.api.alibaba.com/auth/authCode.htm   Code以queryString的方式返回到该url
        /// </param>
        /// <param name="state">可选，app自定义参数，回跳到redirect_uri时，会原样返回</param>
        /// <returns>临时令牌（有效期2分钟）</returns>
        public virtual string GetAuthUrl(string redirectUri, string state = "")
        {
            if (string.IsNullOrEmpty(redirectUri)) { redirectUri = "http://www.weberp.com.cn"; }
            /*http://gw.api.alibaba.com/auth/authorize.htm?client_id=xxx&site=aliexpress&redirect_uri=YOUR_REDIRECT_URL&state=YOUR_PARM&_aop_signature=SIGENATURE
             * a) client_id：app注册时，分配给app的唯一标示，又称appKey
             * b) site:site参数标识当前授权的站点，直接填写aliexpress
             * c) redirect_uri: app的入口地址，授权临时令牌会以queryString的形式跟在该url后返回
             * d) state：可选，app自定义参数，回跳到redirect_uri时，会原样返回
             * e) aop_signature：签名
                  参数签名(_aop_signature)为所有参数key + value 字符串拼接后排序，把排序结果拼接为字符串data后通过bytesToHexString(HAMC-RSA1(data, appSecret))计算签名。 验证签名的方式为对参数执行同样的签名，比较传入的签名结果和计算的结果是否一致，一致为验签通过。
            */
            var dic = new Dictionary<string, object>();
            dic.Add("client_id", this.AppKey);
            dic.Add("site", "aliexpress");
            dic.Add("redirect_uri", redirectUri);
            dic.Add("state", state);
            //dic.Add(AliExpress.fieldAopSignature, this.Sign("", dic));
            return "http://gw.api.alibaba.com/auth/authorize.htm?" +
                this.GetParamUrl(dic, isSign: true, urlPath: string.Empty, isAppKey: false);

        }
        /// <summary>
        /// 获得长时令牌，refreshToken换取accessToken，有效期半年
        /// </summary>
        /// <param name="redirectUri"></param>
        /// <param name="code">临时令牌,有效期2分钟</param>
        /// <returns></returns>
        public virtual AliTokenType GetToken(string redirectUri, string code)
        {
            /*
             * https://gw.api.alibaba.com/openapi/http/1/system.oauth2/getToken/YOUR_APPKEY?grant_type=authorization_code&need_refresh_token=true&client_id= YOUR_APPKEY&client_secret= YOUR_APPSECRET&redirect_uri=YOUR_REDIRECT_URI&code=CODE
             * a) grant_type为授权类型，使用authorization_code即可
             * b) need_refresh_token为是否需要返回refresh_token，如果返回了refresh_token，原来获取的refresh_token也不会失效，除非超过半年有效期
             * c) client_id为app唯一标识，即appKey
             * d) client_secret为app密钥
             * e) redirect_uri为app入口地址
             * f) code为授权完成后返回的一次性令牌
             * g) 调用getToken接口不需要签名
             * */
            var dic = new Dictionary<string, object>();
            dic.Add("grant_type", "authorization_code");
            dic.Add("client_id", this.AppKey);
            dic.Add("client_secret", this.AppSecret);
            dic.Add("redirect_uri", redirectUri);
            dic.Add("code", code);
            dic.Add("need_refresh_token", "true");

            string c = PostWebRequest2(AliExpressClient.openapiIP + AliExpressClient.UrlGetToken + "/" + this.AppKey,
                this.GetParamUrl(dic, isSign: true));

            //{"refresh_token_timeout":"20150111011017000-0800","aliId":"17297513823","resource_owner":"cn117718701","expires_in":"36000","refresh_token":"f0be6f2b-183e-4104-a079-d5ffaed2613e","access_token":"bf150ff5-8585-4d2d-95e2-10e40c743ac4"}
            var r = this.DeserializeObject<AliTokenType>(c);
            this.AccessToken = r.AccessToken;
            return r;
        }

        /// <summary>
        /// 换取新的refreshToken流程详解隐藏
        /// 如果当前时间离refreshToken过期时间在30天以内，那么可以调用postponeToken接口换取新的refreshToken；否则会报错。
        /// </summary>
        /// <param name="refreshToken">长时令牌</param>
        /// <param name="accessToken">授权令牌</param>
        /// <returns></returns>
        public virtual AliTokenType PostponeToken(string refreshToken, string accessToken = null)
        {
            //注意：有自动功能的应用（用户不进入应用主页也能使用第三方应用，因为在应用后台可以自动调用api处理用户数据，如自动重发类的应用）才需要调用此接口。如果没有自动功能，那么不需要调用，因为用户必须在应用主页操作才能获取以及修改用户数据，所以即使refreshToken过期了，用户再次通过应用市场进入应用主页时授权一次即可 
            //换取refreshToken的url示例如下：
            //https://gw.api.alibaba.com/openapi/param2/1/system.oauth2/postponeToken/YOUR_APPKEY
            //请求参数如下：
            //client_id=YOUR_APPKEY&client_secret=YOUR_APPSECRET&refresh_token=REFRESH_TOKEN&access_token=ACCESS_TOKEN
            //{"error_code":"401","error_message":"Request need user authorized","exception":"Request need user authorized"}
            var dic = new Dictionary<string, object>();
            dic.Add("client_id", this.AppKey);
            dic.Add("client_secret", this.AppSecret);
            dic.Add("refresh_token", refreshToken);
            dic.Add("access_token", string.IsNullOrWhiteSpace(accessToken) ? this.AccessToken : accessToken);
            string c = PostWebRequest2(AliExpressClient.openapiIP + AliExpressClient.UrlPostponeToken + "/" + this.AppKey,
                this.GetParamUrl(dic, isSign: true));
            var r = this.DeserializeObject<AliTokenType>(c);
            this.AccessToken = r.AccessToken;
            return r;

            //Newtonsoft.Json.Linq.JToken token = (Newtonsoft.Json.Linq.JToken)Newtonsoft.Json.JsonConvert.DeserializeObject(html);
            //if (token["error"] != null) { throw new Exception(token["error"] + "\r\n" + token["error_description"]); }
            //this.AccessToken = token["access_token"].ToString().Replace("\"", "");
            //return this.AccessToken;
        }
        /// <summary>
        /// 刷新授权令牌，为用户一次会话的授权标识，有效期10小时。
        /// 如果已经有refreshToken并且accessToken已经过期（超过10小时），那么可以使用refresh_token换取access_token，不用重新进行授权
        /// </summary>
        public virtual AliTokenType RefreshToken(string refreshToken)
        {
            //            注意：使用此功能的必要条件是在完成授权流程时已经获取了refreshToken。另外如果accessToken还没有过期（不超过10小时），可以继续使用之前的accessToken，不用重新获取 
            //        https://gw.api.alibaba.com/openapi/param2/1/system.oauth2/getToken/YOUR_APPKEY
            //请求参数如下：
            //grant_type=refresh_token&client_id=YOUR_APPKEY&client_secret=YOUR_APPSECRET&refresh_token=REFRESH_TOKEN
            //注：此接口必须使用POST方法提交；必须使用https
            //a) 此处grant_type参数必须为refresh_token，表示通过refreshToken换取accessToken，而不是通过临时code换取
            //b) 调用getToken接口时不需要签名
            var dic = new Dictionary<string, object>();
            dic.Add("grant_type", "refresh_token");
            dic.Add("client_id", this.AppKey);
            dic.Add("client_secret", this.AppSecret);
            dic.Add("refresh_token", refreshToken);
            //string c = PostWebRequest(AliExpress.openapiIP + AliExpress.UrlgetToken2 + "/" + this.AppKey, this.GetParamUrl(dic));
            string c = PostWebRequest2(AliExpressClient.openapiIP + AliExpressClient.UrlgetToken2 + "/" + this.AppKey,
                this.GetParamUrl(dic, isSign: true, urlPath: AliExpressClient.UrlgetToken2, isAppKey: false));

            var r = this.DeserializeObject<AliTokenType>(c);
            this.AccessToken = r.AccessToken;
            return r;


            ////{"aliId":"17297513823","resource_owner":"cn117718701","expires_in":"36000","access_token":"db90da74-67b6-4cb3-a260-f325b923307e"}
            //Newtonsoft.Json.Linq.JToken token = (Newtonsoft.Json.Linq.JToken)Newtonsoft.Json.JsonConvert.DeserializeObject(html);
            //if (token["error_description"] != null) { throw new Exception(token["error_description"].ToString()); }
            //AccessToken = token["access_token"].ToString().Replace("\"", "");
            ////ResourceOwner = token["resource_owner"].ToString().Replace("\"", "");
            ////AliId = token["aliId"].ToString().Replace("\"", "");
        }
        public virtual void SetAccessToken(string accessToken)
        {
            this.AccessToken = accessToken;
        }
        public virtual Func<string> GetAccessToken { get; set; }
        ///// <summary>
        ///// 作废
        ///// 刷新授权令牌，为用户一次会话的授权标识，有效期10小时。
        ///// 如果已经有refreshToken并且accessToken已经过期（超过10小时），那么可以使用refresh_token换取access_token，不用重新进行授权
        ///// </summary>
        ///// <param name="refreshToken">刷新令牌</param>
        ///// <returns></returns>
        //[Obsolete]
        //public virtual AliTokenType RefreshToken2(string refreshToken)
        //{
        //    var dic = new Dictionary<string, object>();
        //    dic.Add("grant_type", "refresh_token");
        //    dic.Add("client_id", this.AppKey);
        //    dic.Add("client_secret", this.AppSecret);
        //    dic.Add("refresh_token", refreshToken);
        //    string c = PostWebRequest(AliExpress.openapiIP + AliExpress.UrlRefreshToken + "/" + this.AppKey,
        //        this.GetParamUrl(dic, isSign: true, urlPath: AliExpress.UrlRefreshToken, isAppKey: false));
        //    //{"aliId":"17381393572","resource_owner":"cn1001015000","expires_in":"36000","access_token":"f66eb640-d3b5-4cf9-a814-6856aa82f2de"}
        //    var r = this.DeserializeObject<AliTokenType>(c);
        //    this.AccessToken = r.AccessToken;
        //    return r;
        //}
        #endregion 授权相关
        #region 物流
        /// <summary>
        /// 根据请求地址的类型：发货地址信息，揽收地址信息，返回相应的地址列表。
        /// </summary>
        /// <returns></returns>
        public virtual AlibabaAeApiGetLogisticsSellerAddressesResponse GetLogisticsSellerAddresses(bool sender = true, bool pickup = true, bool refund = true)
        {
            var request = new[] { sender ? "sender" : null, pickup ? "pickup" : null, refund ? "refund" : null }.Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();
            return ModelClient.AlibabaAeApiGetLogisticsSellerAddresses(System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(request)));
        }
        /// <summary>
        /// 批量获取线上发货标签(线上物流发货专用接口)
        /// </summary>
        /// <param name="internationalLogisticsIds">国际运单号</param>
        /// <returns></returns>
        public virtual ApiGetPrintInfosResponse GetPrintInfos(Model.AeopWarehouseOrderQueryPdfRequest[] internationalLogistics, bool? printDetail = null)
        {
            return ModelClient.GetPrintInfos(Newtonsoft.Json.JsonConvert.SerializeObject(internationalLogistics), printDetail);
        }

        /// <summary>
        /// 修改声明发货(一个订单只能修改2次，只能修改声明发货后5日内的订单，请注意！)
        /// </summary>
        /// <param name="oldServiceName">用户需要修改的的老的发货物流服务（物流服务key：该接口根据api.listLogisticsService列出平台所支持的物流服务 进行获取目前所支持的物流。）	EMS_SH_ZX_US；EMS；SEP；FEDEX；UPSE；FEDEX_IE；RUSTON；HKPAP；CPAM；SF；HKPAM；CHP；ZTORU；ARAMEX；CPAP；TNT；ECONOMIC139；DHL；UPS；SGP；</param>
        /// <param name="oldLogisticsNo">用户需要修改的老的物流追踪号	CP123456789CN</param>
        /// <param name="newServiceName">用户需要修改的的新的发货物流服务（物流服务key：该接口根据api.listLogisticsService列出平台所支持的物流服务 进行获取目前所支持的物流。）	EMS_SH_ZX_US；EMS；SEP；FEDEX；UPSE；FEDEX_IE；RUSTON；HKPAP；CPAM；SF；HKPAM；CHP；ZTORU；ARAMEX；CPAP；TNT；ECONOMIC139；DHL；UPS；SGP；</param>
        /// <param name="newLogisticsNo">用户需要修改的新的物流追踪号	CP123456123CN</param>
        /// <param name="sendType">状态包括：全部发货(all)、部分发货(part)	all</param>
        /// <param name="outRef">用户需要发货的订单id	60769040695804</param>
        /// <param name="trackingWebsite">当serviceName=other的情况时，需要填写对应的追踪网址</param>
        /// <param name="description">备注(只能输入英文)</param>
        /// <returns></returns>
        public virtual ApiSellerModifiedShipmentResponse SellerModifiedShipment(string oldServiceName, string oldLogisticsNo, string newServiceName, string newLogisticsNo, string sendType, string outRef, string trackingWebsite = null, string description = null)
        {
            return ModelClient.SellerModifiedShipment(oldServiceName, oldLogisticsNo, newServiceName, newLogisticsNo, sendType, outRef, description, trackingWebsite);
            //return this.DeserializeObject<AliSuccessType>(c);
        }
        /// <summary>
        /// 获取邮政小包订单信息(线上物流发货专用接口)
        /// </summary>
        /// <param name="orderId">交易订单ID</param>
        /// <param name="internationalLogisticsId">国际运单号</param>
        /// <param name="chinaLogisticsId">国内快递运单号</param>
        /// <param name="logisticsStatus">物流订单状态</param>
        /// <param name="gmtCreateStartStr">开始时间</param>
        /// <param name="gmtCreateEndStr">结束时间</param>
        /// <param name="currentPage">当前页面</param>
        /// <param name="pageSize">页面大小,最大100</param>
        /// <returns></returns>
        public virtual ApiGetOnlineLogisticsInfoResponse GetOnlineLogisticsInfo(long orderId, string internationalLogisticsId = null, string chinaLogisticsId = null, string logisticsStatus = null, System.DateTimeOffset? gmtCreateStartStr = null, System.DateTimeOffset? gmtCreateEndStr = null, int? currentPage = null, int? pageSize = null)
        {
            return ModelClient.GetOnlineLogisticsInfo(orderId, internationalLogisticsId, chinaLogisticsId, logisticsStatus, this.ConvertTimeParam(gmtCreateStartStr), this.ConvertTimeParam(gmtCreateEndStr), currentPage, pageSize);
        }

        /// <summary>
        /// 获取线上发货标签(线上物流发货专用接口)
        /// </summary>
        /// <param name="internationalLogisticsId">国际运单号</param>
        /// <returns></returns>
        public virtual AliPrintInfoResponse GetPrintInfo(string internationalLogisticsId)
        {
            //return ModelClient.GetPrintInfo(internationalLogisticsId);
            var c = StrClient.api_getPrintInfo(internationalLogisticsId);
            return this.DeserializeObject<AliPrintInfoResponse>(c);
        }

        /// <summary>
        /// 根据订单号获取线上发货物流方案
        /// </summary>
        /// <param name="orderId">交易订单ID</param>
        /// <param name="goodsWeight">包裹重量</param>
        /// <param name="goodsLength">包裹长</param>
        /// <param name="goodsWidth">包裹宽</param>
        /// <param name="goodsHeight">包裹高</param>
        /// <returns></returns>
        public virtual ApiGetOnlineLogisticsServiceListByOrderIdResponse GetOnlineLogisticsServiceListByOrderId(long orderId, double? goodsWeight = null, int? goodsLength = null, int? goodsWidth = null, int? goodsHeight = null)
        {
            return ModelClient.GetOnlineLogisticsServiceListByOrderId(orderId, goodsWeight, goodsLength, goodsWidth, goodsHeight);
        }
        /// <summary>
        /// 创建线上发货物流订单 关于中国邮政平常小包+使用说明请参见http://seller.aliexpress.com/so/onlinelogistics_postbj.php 中国邮政平常小包+获取国际跟踪单号请通过http://gw.api.alibaba.com/dev/doc/api.htm?ns=aliexpress.open$n=api.getOnlineLogisticsInfo$v=1传入orderId进行获取internationalLogisticsId
        /// </summary>
        /// <param name="tradeOrderId">交易订单号	60000970354018</param>
        /// <param name="tradeOrderFrom">交易订单来源,AE订单为ESCROW ；国际站订单为“SOURCING”</param>
        /// <param name="domesticLogisticsCompanyId">国内快递ID	505(物流公司是other时,ID为-1)</param>
        /// <param name="domesticLogisticsCompany">国内快递公司名称	物流公司Id为-1时,必填</param>
        /// <param name="domesticTrackingNo">国内快递运单号,长度1-32</param>
        /// <param name="addressDTOs">地址信息,包含发货人地址,收货人地址.发货人地址key值是sender; 收货人地址key值是receiver,都必填{country为国家简称,必填;province为省/州,（必填，长度限制1-48字节）;city为城市,（必填，长度限制1-48，可以直接填写城市信息）,county为区县，（必填，长度限制1-20字节）streetAddress为街道 ,（必填，长度限制1-90字节）;name为姓名,（必填，长度限制1-90字节）;phone,mobile两者二选一,phone（长度限制1- 54字节）;mobile（长度限制1-30字节）;email邮箱必填（长度限制1-64字节）;trademanageId旺旺（必填，长度限制1-32字节）;如果是中俄航空Ruston需要揽收的订单，则再添加揽收地址信息，key值是pickup,字段同上，内容必须是中文（如无需揽收，则不必传pickup的值）</param>
        /// <param name="remark">备注</param>
        /// <param name="warehouseCarrierService">用户选择的实际发货物流服务（物流服务key,即仓库服务名称)HRB_WLB_ZTOGZ是 中俄航空 Ruston广州仓库； HRB_WLB_ZTOSH是 中俄航空 Ruston上海仓库。HRB_WLB_RUSTONHEB为哈尔滨备货仓暂不支持，该渠道请做忽略。</param>
        /// <param name="declareProductDTOs">申报产品信息,列表类型，以json格式来表达。{productId为产品ID(必填,如为礼品,则设置为0);categoryCnDesc为申报中文名称(必填,长度1-20);categoryEnDesc为申报英文名称(必填,长度1-60);productNum产品件数(必填1-999);productDeclareAmount为产品申报金额(必填,0.01-10000.00);productWeight为产品申报重量(必填0.001-2.000);isContainsBattery为是否包含锂电池(必填0/1);scItemId为仓储发货属性代码（团购订单，仓储发货必填，物流服务为RUSTON 哈尔滨备货仓 HRB_WLB_RUSTONHEB，属性代码对应AE商品的sku属性一级，暂时没有提供接口查询属性代码，可以在仓储管理--库存管理页面查看，例如： 团购产品的sku属性White对应属性代码 40414943126）;skuValue为属性名称（团购订单，仓储发货必填，例如：White）};</param>
        /// <param name="undeliverableDecision">不可达处理(退回:0/销毁:1);目前因部分ISV还未升级，系统文档中该参数当前设置为可选，默认值为-1。请ISV升级时，将参数设置为必选，默认值为1，否则将影响9月28日之后的发货功能。<value>-1</value><example>0</example></param>
        /// <returns></returns>
        public virtual ApiCreateWarehouseOrderResponse CreateWarehouseOrder(long tradeOrderId, string tradeOrderFrom, string warehouseCarrierService, AddressDTOs addressDTOs, AeopWlDeclareProductDTO[] declareProductDTOs, long domesticLogisticsCompanyId, string domesticTrackingNo, string domesticLogisticsCompany = null, string remark = null, bool undeliverableDecision = true)
        {
            return ModelClient.CreateWarehouseOrder(
                tradeOrderId: tradeOrderId,
                tradeOrderFrom: tradeOrderFrom,
                warehouseCarrierService: warehouseCarrierService,
                domesticLogisticsCompanyId: domesticLogisticsCompanyId,
                domesticTrackingNo: domesticTrackingNo,
                declareProductDTOs: System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(declareProductDTOs)),
                addressDTOs: System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(addressDTOs)),
                domesticLogisticsCompany: domesticLogisticsCompany,
                remark: remark,
                undeliverableDecision: undeliverableDecision ? 1 : 0);
        }
        /// <summary>
        /// 获取中邮小包支持的国内快递公司信息
        /// </summary>
        /// <returns></returns>
        public virtual ApiQureyWlbDomesticLogisticsCompanyResponse QureyWlbDomesticLogisticsCompany()
        {
            return ModelClient.QureyWlbDomesticLogisticsCompany();
        }
        /// <summary>
        /// 列出平台所支持的物流服务
        /// </summary>
        /// <returns></returns>
        public virtual ApiListLogisticsServiceResponse ListLogisticsService()
        {
            return ModelClient.ListLogisticsService();
        }
        /// <summary>
        /// 声明发货
        /// </summary>
        /// <param name="serviceName">用户选择的实际发货物流服务（物流服务key：该接口根据api.listLogisticsService列出平台所支持的物流服务 进行获取目前所支持的物流。）	EMS_SH_ZX_US；EMS；SEP；FEDEX；UPSE；FEDEX_IE；RUSTON；HKPAP；CPAM；SF；HKPAM；CHP；ZTORU；ARAMEX；CPAP；TNT；ECONOMIC139；DHL；UPS；SGP；</param>
        /// <param name="logisticsNo">物流追踪号</param>
        /// <param name="sendType">状态包括：全部发货(all)、部分发货(part)</param>
        /// <param name="isALL">是否全部发货</param>
        /// <param name="outRef">用户需要发货的订单id</param>
        /// <param name="trackingWebsite">当serviceName=other的情况时，需要填写对应的追踪网址</param>
        /// <param name="description">备注(只能输入英文，且长度限制在512个字符。）</param>
        /// <returns></returns>
        public virtual ApiSellerShipmentResponse SellerShipment(string outRef, bool isALL, string serviceName, string logisticsNo, /*string sendType,*/ string trackingWebsite = null, string description = null)
        {
            return ModelClient.SellerShipment(serviceName, logisticsNo, isALL ? "all" : "part", outRef, description, trackingWebsite);
        }

        /// <summary>
        /// 查询物流追踪信息
        /// </summary>
        /// <param name="serviceName">物流服务KEY	UPS</param>
        /// <param name="logisticsNo">物流追踪号	20100810142400000-0700</param>
        /// <param name="toArea">交易订单收货国家(简称)</param>
        /// <param name="origin">需要查询的订单来源 AE订单为“ESCROW”。</param>
        /// <param name="outRef">用户需要查询的订单id</param>
        /// <returns></returns>
        public virtual ApiQueryTrackingResultResponse QueryTrackingResult(string serviceName, string logisticsNo, string toArea, string origin, string outRef)
        {
            return ModelClient.QueryTrackingResult(serviceName, logisticsNo, toArea, origin, outRef);
        }
        #endregion 物流
        #region 商家
        /// <summary>
        /// 查询商家认证信息
        /// </summary>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        public virtual AlibabaAeUserauthQueryMerchantCertificationInfoResponse AlibabaAeUserauthQueryMerchantCertificationInfo(long adminMemberSeq)
        {
            return ModelClient.AlibabaAeUserauthQueryMerchantCertificationInfo(adminMemberSeq);
        }
        /// <summary>
        /// 查询商家信息
        /// </summary>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        public virtual AlibabaAeSellerMerchantResultModel AlibabaAeSellerQueryMerchant(long sellerId)
        {
            return ModelClient.AlibabaAeSellerQueryMerchant(sellerId, null);
        }
        #endregion 商家
        #region 会员服务
        /// <summary>
        /// 根据买家登录ID查询会员等级。平台对买家层级的标识，以A0-A4五个等级进行标注，数字越高，等级越高，买家越活跃，购买力越强,卖家朋友可根据买家分层更好的服务于买家。
        /// </summary>
        /// <param name="ownerMemberId"></param>
        /// <returns></returns>
        public virtual AccountResultDTO QueryAccountLevel(string ownerMemberId)
        {
            return ModelClient.QueryAccountLevel(ownerMemberId);
        }
        #endregion 会员服务
        #region 数据
        /// <summary>
        /// 查询商品交易表现
        /// </summary>
        /// <param name="productId">商品id</param>
        public virtual ApiQueryProductBusinessInfoByIdResponse QueryProductBusinessInfoById(string productId)
        {
            return ModelClient.QueryProductBusinessInfoById(productId);
            //{"addCartCount":31,"exposedCount":3141,"favoritedCount":5,"gmvPerBuyer":0,"gmvPerBuyer30d":5.25,"gmvPerOrder":0,"gmvPerOrder30d":5.25,"outputOrder":3,"refundAmt":0,"success":true,"viewedCount":188}
        }
        /// <summary>
        /// 查询商品每日浏览量(该数据仅限30天之内的时间区间数据查询）(试用）
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <param name="startDate">查询时间段的开始时间。例如：yyyy-mm-dd</param>
        /// <param name="endDate">查询时间段的截止时间。例如：yyyy-mm-dd</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="pageSize">每页结果数量，默认20个，最大值 50</param>
        /// <returns></returns>
        public virtual ApiQueryProductViewedInfoEverydayByIdResponse QueryProductViewedInfoEverydayById(string productId, System.DateTimeOffset? startDate = null, System.DateTimeOffset? endDate = null, int? currentPage = null, int? pageSize = null)
        {
            return ModelClient.QueryProductViewedInfoEverydayById(productId, ConvertDataParam(startDate), ConvertDataParam(endDate), currentPage, pageSize);
            //"{\"itemList\":[{\"count\":10,\"date\":\"2015-01-09\"},{\"count\":9,\"date\":\"2015-01-10\"},{\"count\":5,\"date\":\"2015-01-11\"},{\"count\":6,\"date\":\"2015-01-12\"},{\"count\":8,\"date\":\"2015-01-13\"},{\"count\":6,\"date\":\"2015-01-14\"},{\"count\":10,\"date\":\"2015-01-15\"},{\"count\":6,\"date\":\"2015-01-16\"},{\"count\":4,\"date\":\"2015-01-17\"},{\"count\":8,\"date\":\"2015-01-18\"},{\"count\":7,\"date\":\"2015-01-19\"},{\"count\":16,\"date\":\"2015-01-20\"},{\"count\":3,\"date\":\"2015-01-21\"},{\"count\":10,\"date\":\"2015-01-22\"},{\"count\":6,\"date\":\"2015-01-23\"},{\"count\":7,\"date\":\"2015-01-24\"},{\"count\":5,\"date\":\"2015-01-25\"},{\"count\":3,\"date\":\"2015-01-26\"},{\"count\":11,\"date\":\"2015-01-27\"},{\"count\":3,\"date\":\"2015-01-28\"}],\"success\":true,\"totalItem\":30}"
        }
        /// <summary>
        /// 查询商品每日加入购物车数据(该数据仅限30天之内的时间区间数据查询）（试用）
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <param name="startDate">查询时间段的开始时间。例如：yyyy-mm-dd</param>
        /// <param name="endDate">查询时间段的截止时间。例如：yyyy-mm-dd</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="pageSize">每页结果数量</param>
        /// <returns></returns>
        public virtual ApiQueryProductAddCartInfoEverydayByIdResponse QueryProductAddCartInfoEverydayById(string productId, System.DateTimeOffset? startDate = null, System.DateTimeOffset? endDate = null, int? currentPage = null, int? pageSize = null)
        {
            return ModelClient.QueryProductAddCartInfoEverydayById(productId, ConvertDataParam(startDate), ConvertDataParam(endDate), currentPage, pageSize);
            //return this.DeserializeObject<ApiQueryProductAddCartInfoEverydayByIdResponse>(c.Replace("\\\"", "\"").Trim('\"'));
            //"{\"itemList\":[{\"count\":0,\"date\":\"2015-01-09\"},{\"count\":3,\"date\":\"2015-01-10\"},{\"count\":1,\"date\":\"2015-01-11\"},{\"count\":0,\"date\":\"2015-01-12\"},{\"count\":0,\"date\":\"2015-01-13\"},{\"count\":0,\"date\":\"2015-01-14\"},{\"count\":3,\"date\":\"2015-01-15\"},{\"count\":2,\"date\":\"2015-01-16\"},{\"count\":0,\"date\":\"2015-01-17\"},{\"count\":2,\"date\":\"2015-01-18\"},{\"count\":1,\"date\":\"2015-01-19\"},{\"count\":7,\"date\":\"2015-01-20\"},{\"count\":1,\"date\":\"2015-01-21\"},{\"count\":2,\"date\":\"2015-01-22\"},{\"count\":0,\"date\":\"2015-01-23\"},{\"count\":0,\"date\":\"2015-01-24\"},{\"count\":1,\"date\":\"2015-01-25\"},{\"count\":0,\"date\":\"2015-01-26\"},{\"count\":2,\"date\":\"2015-01-27\"},{\"count\":0,\"date\":\"2015-01-28\"}],\"success\":true,\"totalItem\":30}"
        }
        /// <summary>
        /// 查询商品每天被曝光数据(该数据仅限30天之内的时间区间数据查询）（试用）
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <param name="startDate">查询时间段的开始时间。例如：yyyy-mm-dd</param>
        /// <param name="endDate">查询时间段的截止时间。例如：yyyy-mm-dd</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="pageSize">每页结果数量，默认20个，最大值 50</param>
        /// <returns></returns>
        public virtual ApiQueryProductExposedInfoEverydayByIdResponse QueryProductExposedInfoEverydayById(string productId, System.DateTimeOffset? startDate = null, System.DateTimeOffset? endDate = null, int? currentPage = null, int? pageSize = null)
        {
            return ModelClient.QueryProductExposedInfoEverydayById(productId, ConvertDataParam(startDate), ConvertDataParam(endDate), currentPage, pageSize);
            //return this.DeserializeObject<ApiQueryProductExposedInfoEverydayByIdResponse>(c.Replace("\\\"", "\"").Trim('\"'));
            //{"itemList":[{"count":136,"date":"2015-01-09"},{"count":131,"date":"2015-01-10"},{"count":122,"date":"2015-01-11"},{"count":100,"date":"2015-01-12"},{"count":120,"date":"2015-01-13"},{"count":102,"date":"2015-01-14"},{"count":121,"date":"2015-01-15"},{"count":81,"date":"2015-01-16"},{"count":101,"date":"2015-01-17"},{"count":104,"date":"2015-01-18"},{"count":101,"date":"2015-01-19"},{"count":90,"date":"2015-01-20"},{"count":97,"date":"2015-01-21"},{"count":99,"date":"2015-01-22"},{"count":115,"date":"2015-01-23"},{"count":94,"date":"2015-01-24"},{"count":91,"date":"2015-01-25"},{"count":158,"date":"2015-01-26"},{"count":99,"date":"2015-01-27"},{"count":93,"date":"2015-01-28"}],"success":true,"totalItem":30}
        }
        /// <summary>
        /// 查询商品每天的销量数据(该数据仅限30天之内的时间区间数据查询）（试用）
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <param name="startDate">查询时间段的开始时间。例如：yyyy-mm-dd</param>
        /// <param name="endDate">查询时间段的截止时间。例如：yyyy-mm-dd</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="pageSize">每页结果数量，默认20个，最大值 50</param>
        /// <returns></returns>
        public virtual ApiQueryProductSalesInfoEverydayByIdResponse QueryProductSalesInfoEverydayById(string productId, System.DateTimeOffset? startDate = null, System.DateTimeOffset? endDate = null, int? currentPage = null, int? pageSize = null)
        {
            return ModelClient.QueryProductSalesInfoEverydayById(productId, ConvertDataParam(startDate), ConvertDataParam(endDate), currentPage, pageSize);
            //return this.DeserializeObject<ApiQueryProductSalesInfoEverydayByIdResponse>(c.Replace("\\\"", "\"").Trim('\"'));
            //"{\"itemList\":[{\"count\":0,\"date\":\"2015-01-09\"},{\"count\":0,\"date\":\"2015-01-10\"},{\"count\":0,\"date\":\"2015-01-11\"},{\"count\":0,\"date\":\"2015-01-12\"},{\"count\":0,\"date\":\"2015-01-13\"},{\"count\":0,\"date\":\"2015-01-14\"},{\"count\":0,\"date\":\"2015-01-15\"},{\"count\":0,\"date\":\"2015-01-16\"},{\"count\":1,\"date\":\"2015-01-17\"},{\"count\":1,\"date\":\"2015-01-18\"},{\"count\":0,\"date\":\"2015-01-19\"},{\"count\":0,\"date\":\"2015-01-20\"},{\"count\":0,\"date\":\"2015-01-21\"},{\"count\":1,\"date\":\"2015-01-22\"},{\"count\":0,\"date\":\"2015-01-23\"},{\"count\":0,\"date\":\"2015-01-24\"},{\"count\":0,\"date\":\"2015-01-25\"},{\"count\":0,\"date\":\"2015-01-26\"},{\"count\":0,\"date\":\"2015-01-27\"},{\"count\":0,\"date\":\"2015-01-28\"}],\"success\":true,\"totalItem\":30}"
        }
        /// <summary>
        /// 查询商品每天被收藏的数量(该数据仅限30天之内的时间区间数据查询）（试用）
        /// </summary>
        /// <param name="productId">商品id</param>
        /// <param name="startDate">查询时间段的开始时间。例如：yyyy-mm-dd</param>
        /// <param name="endDate">查询时间段的截止时间。例如：yyyy-mm-dd</param>
        /// <param name="currentPage">当前页码</param>
        /// <param name="pageSize">每页结果数量，默认20个，最大值 50</param>
        /// <returns></returns>
        public virtual ApiQueryProductFavoritedInfoEverydayByIdResponse queryProductFavoritedInfoEverydayById(string productId, System.DateTimeOffset? startDate = null, System.DateTimeOffset? endDate = null, int? currentPage = null, int? pageSize = null)
        {
            return ModelClient.QueryProductFavoritedInfoEverydayById(productId, ConvertDataParam(startDate), ConvertDataParam(endDate), currentPage, pageSize);
            //return this.DeserializeObject<ApiQueryProductFavoritedInfoEverydayByIdResponse>(c.Replace("\\\"", "\"").Trim('\"'));
            //"{\"itemList\":[{\"count\":0,\"date\":\"2015-01-09\"},{\"count\":0,\"date\":\"2015-01-10\"},{\"count\":0,\"date\":\"2015-01-11\"},{\"count\":0,\"date\":\"2015-01-12\"},{\"count\":0,\"date\":\"2015-01-13\"},{\"count\":0,\"date\":\"2015-01-14\"},{\"count\":0,\"date\":\"2015-01-15\"},{\"count\":0,\"date\":\"2015-01-16\"},{\"count\":0,\"date\":\"2015-01-17\"},{\"count\":0,\"date\":\"2015-01-18\"},{\"count\":0,\"date\":\"2015-01-19\"},{\"count\":0,\"date\":\"2015-01-20\"},{\"count\":0,\"date\":\"2015-01-21\"},{\"count\":2,\"date\":\"2015-01-22\"},{\"count\":0,\"date\":\"2015-01-23\"},{\"count\":0,\"date\":\"2015-01-24\"},{\"count\":1,\"date\":\"2015-01-25\"},{\"count\":0,\"date\":\"2015-01-26\"},{\"count\":0,\"date\":\"2015-01-27\"},{\"count\":0,\"date\":\"2015-01-28\"}],\"success\":true,\"totalItem\":30}"
        }
        #endregion 数据
        #region 公共
        /// <summary>
        /// 查询速卖通平台公告信息
        /// </summary>
        /// <param name="anouncement">公告id，一次只能查询一个。</param>
        /// <param name="publicDatetimeStart">公告创建时间起始值，格式: mm/dd/yyyy hh:mm:ss,如10/08/2013 00:00:00</param>
        /// <param name="publicDatetimeEnd">公告创建截止值，格式: mm/dd/yyyy hh:mm:ss,如10/09/2013 00:00:00</param>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">每页个数，最大50</param>
        /// <returns></returns>
        public virtual ApiQueryAeAnouncementResponse QueryAeAnouncement(string anouncement = null, System.DateTimeOffset? publicDatetimeStart = null, System.DateTimeOffset? publicDatetimeEnd = null, int? page = null, int? pageSize = null)
        {
            return ModelClient.QueryAeAnouncement(anouncement, ConvertDataParam(publicDatetimeStart), ConvertDataParam(publicDatetimeEnd), page, pageSize);
        }
        /// <summary>
        /// 查询速卖通服务市场公告信息
        /// </summary>
        /// <param name="anouncement">公告id，一次只能查询一个。</param>
        /// <param name="publicDatetimeStart">公告创建时间起始值，格式: mm/dd/yyyy hh:mm:ss,如10/08/2013 00:00:00</param>
        /// <param name="publicDatetimeEnd">公告创建截止值，格式: mm/dd/yyyy hh:mm:ss,如10/09/2013 00:00:00</param>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">每页个数，最大50</param>
        /// <returns></returns>
        public virtual ApiQueryServiceAnouncementResponse QueryServiceAnouncement(string anouncement = null, System.DateTimeOffset? publicDatetimeStart = null, System.DateTimeOffset? publicDatetimeEnd = null, int? page = null, int? pageSize = null)
        {
            return ModelClient.QueryServiceAnouncement(anouncement, ConvertDataParam(publicDatetimeStart), ConvertDataParam(publicDatetimeEnd), page, pageSize);
        }
        /// <summary>
        /// 查询开放平台公告信息
        /// </summary>
        /// <param name="anouncement">公告id，一次只能查询一个。</param>
        /// <param name="publicDatetimeStart">公告创建时间起始值，格式: mm/dd/yyyy hh:mm:ss,如10/08/2013 00:00:00</param>
        /// <param name="publicDatetimeEnd">公告创建截止值，格式: mm/dd/yyyy hh:mm:ss,如10/09/2013 00:00:00</param>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">每页个数，最大50</param>
        public virtual ApiQueryOpenAnouncementResponse QueryOpenAnouncement(string anouncement = null, System.DateTimeOffset? publicDatetimeStart = null, System.DateTimeOffset? publicDatetimeEnd = null, int? page = null, int? pageSize = null)
        {
            return ModelClient.QueryOpenAnouncement(anouncement, ConvertDataParam(publicDatetimeStart), ConvertDataParam(publicDatetimeEnd), page, pageSize);
        }
        #endregion 公共
        #region 营销
        public enum ActStatus { not_started, releasing, release_end, closed }
        /// <summary>
        /// 查询已添加的coupon活动
        /// </summary>
        /// <param name="status">活动状态，可取值：["not_started", "releasing", "release_end", "closed"]</param>
        /// <param name="activityName">活动名称，支持模糊搜索</param>
        /// <param name="minActivityStartDate">活动开始时间区间--最小值，允许格式"mm/dd/yyyy HH:mm:ss"</param>
        /// <param name="maxActivityStartDate">活动开始时间区间--最大值，允许格式："mm/dd/yyyy HH:mm:ss"</param>
        /// <param name="currentPage">当前页的页码</param>
        /// <param name="pageSize">每页展示记录数</param>
        /// <returns></returns>
        public virtual GetActListResponse GetActList(ActStatus? status = null, string activityName = null, System.DateTimeOffset? minActivityStartDate = null, System.DateTimeOffset? maxActivityStartDate = null, int? currentPage = null, int? pageSize = null)
        {
            return ModelClient.GetActList(Convert.ToString(status), activityName, ConvertDataParam(minActivityStartDate), ConvertDataParam(maxActivityStartDate), currentPage, pageSize);
        }
        /// <summary>
        /// 获取指定活动详细信息
        /// </summary>
        /// <param name="activityId">活动ID</param>
        /// <returns></returns>
        public virtual FindSellerCouponActivityResponse FindSellerCouponActivity(long activityId)
        {
            return ModelClient.FindSellerCouponActivity(activityId);
        }
        #endregion 营销
        #region 评价
        /// <summary>
        /// 查询待卖家评价的订单信息
        /// </summary>
        /// <param name="currentPage">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="orderIds">订单ID列表</param>
        /// <param name="orderFinishTimeStart">订单完成开始时间</param>
        /// <param name="orderFinishTimeEnd">订单完成结束时间</param>
        /// <returns></returns>
        public virtual SellerEvaluationOrderList QuerySellerEvaluationOrderList(int? currentPage = null, int? pageSize = null, string[] orderIds = null)
        {
            //return ModelClient.EvaluationQuerySellerEvaluationOrderList(currentPage, pageSize, orderIds != null ? string.Join(",", orderIds) : null);
            var c = StrClient.api_evaluation_querySellerEvaluationOrderList(currentPage, pageSize, orderIds != null ? string.Join(",", orderIds) : null);
            return this.DeserializeObject<SellerEvaluationOrderList>(c);
        }
        /// <summary>
        /// 卖家对未评价的订单进行评价
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="score">用户对主订单的打分</param>
        /// <param name="feedbackContent">用户对主订单的评价内容(Max 1,000 characters. Please do not use HTML codes or Chinese characters.同时包括中文标点也不支持）</param>
        /// <returns></returns>
        public virtual AliSuccessType SaveSellerFeedback(long orderId, int? score = null, string feedbackContent = null)
        {
            var c = StrClient.api_evaluation_saveSellerFeedback(orderId, score, feedbackContent);
            return this.DeserializeObject<AliSuccessType>(c);
        }

        #endregion 评价
        #region 站内信
        #region 作废
        ///// <summary>
        ///// 根据订单号查询订单留言(即将下线)
        ///// </summary>
        ///// <param name="orderId">订单号</param>
        ///// <returns></returns>
        //public virtual OrderMsgType[] QueryOrderMsgListByOrderId(long orderId)
        //{
        //    //var dic = new Dictionary<string, object>();
        //    //dic.Add(AliExpress.fieldAccessToken, this.AccessToken);//用户授权令牌
        //    //dic.Add("orderId", orderId);

        //    //return PostWebRequest(AliExpress.openapiIP, this.AppKey, AliExpress.Url + AliExpress.Api_queryOrderMsgListByOrderId, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    return ModelClient.api_queryOrderMsgListByOrderId(orderId);
        //    return this.DeserializeObject<OrderMsgType[]>(c);
        //}
        ///// <summary>
        ///// 根据买家登录账号login_id查询站内信(即将下线)
        ///// </summary>
        ///// <param name="buyerId">买家loginId</param>
        ///// <returns></returns>
        //public virtual OrderMsgType[] QueryMessageListByBuyerId(string buyerId)
        //{
        //    //var dic = new Dictionary<string, object>();
        //    //dic.Add(AliExpress.fieldAccessToken, this.AccessToken);//用户授权令牌
        //    //dic.Add("buyerId", buyerId);

        //    //return PostWebRequest(AliExpress.openapiIP, this.AppKey, AliExpress.Url + AliExpress.Api_queryMessageListByBuyerId, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    return ModelClient.api_queryMessageListByBuyerId(buyerId);
        //    return this.DeserializeObject<OrderMsgType[]>(c);
        //}

        ///// <summary>
        ///// 查询订单留言
        ///// </summary>
        ///// <param name="orderId">订单号</param>
        ///// <param name="page">当前页码</param>
        ///// <param name="pageSize">每页数量(最大不超过50,超过50按50记,默认每页15)</param>
        ///// <param name="startTime">开始时间</param>
        ///// <param name="endTime">结束时间</param>
        ///// <returns></returns>
        //public virtual AliOrderMsgList QueryOrderMsgList(string buyerId = null, long? orderId = null, int? page = null, int? pageSize = 50, System.DateTimeOffset? startTime = null, System.DateTimeOffset? endTime = null)
        //{
        //    ////var dic = new Dictionary<string, object>();
        //    ////dic.Add(AliExpress.fieldAccessToken, this.AccessToken);//用户授权令牌
        //    ////dic.Add("currentPage", page);
        //    ////dic.Add("pageSize", pageSize);
        //    ////dic.Add("startTime", ConvertTimeParam(startTime));
        //    ////dic.Add("endTime", ConvertTimeParam(endTime));
        //    ////dic.Add("buyerId", buyerId);
        //    ////dic.Add("orderId", orderId);

        //    ////return PostWebRequest(AliExpress.openapiIP, this.AppKey, AliExpress.Url + AliExpress.Api_queryOrderMsgList, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    //return api_queryOrderMsgList(ConvertTimeParam(startTime), ConvertTimeParam(endTime), orderId, buyerId, page, pageSize);
        //    //return this.DeserializeObject<AliOrderMsgList>(c);
        //}
        ///// <summary>
        ///// 查询站内信
        ///// </summary>
        ///// <param name="buyerId">买家ID</param>
        ///// <param name="page">当前页码</param>
        ///// <param name="pageSize">每页数量(最大不超过50,超过50按50记,默认每页15)</param>
        ///// <param name="startTime">开始时间</param>
        ///// <param name="endTime">结束时间</param>
        ///// <returns></returns>
        //public virtual AliMessageList QueryMessageList(string buyerId = null, long? orderId = null, int? page = null, int? pageSize = 50, System.DateTimeOffset? startTime = null, System.DateTimeOffset? endTime = null)
        //{
        //    ////var dic = new Dictionary<string, object>();
        //    ////dic.Add(AliExpress.fieldAccessToken, this.AccessToken);//用户授权令牌
        //    ////dic.Add("currentPage", page);
        //    ////dic.Add("pageSize", pageSize);
        //    ////dic.Add("startTime", ConvertTimeParam(startTime));
        //    ////dic.Add("endTime", ConvertTimeParam(endTime));
        //    ////dic.Add("buyerId", buyerId);
        //    ////dic.Add("orderId", orderId);

        //    ////return PostWebRequest(AliExpress.openapiIP, this.AppKey, AliExpress.Url + AliExpress.Api_queryMessageList, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    //return api_queryMessageList(ConvertTimeParam(startTime), ConvertTimeParam(endTime), orderId, buyerId, page, pageSize);
        //    //return this.DeserializeObject<AliMessageList>(c);
        //}

        ///// <summary>
        ///// 新增订单留言
        ///// </summary>
        ///// <param name="orderId">订单号(站内信不需填)</param>
        ///// <param name="buyerId">买家ID（该买家曾主动发送过站内信）</param>
        ///// <param name="content">内容(只能对属于自己店铺的订单进行留言，内容不能为中文，不能包含html)</param>
        ///// <returns></returns>
        //public virtual AliSuccessType AddOrderMessage(long? orderId, string buyerId, string content)
        //{
        //    //var dic = new Dictionary<string, object>();
        //    //dic.Add("orderId", orderId);
        //    //dic.Add("buyerId", buyerId);
        //    //dic.Add("content", System.Web.HttpUtility.UrlEncode(content));
        //    //dic.Add(AliExpress.fieldAccessToken, this.AccessToken);//用户授权令牌

        //    ////return PostWebRequest(AliExpress.openapiIP, this.AppKey, AliExpress.Url + AliExpress.Api_addOrderMessage, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    //return api_addOrderMessage(orderId, buyerId, content);
        //    //try
        //    //{
        //    //    var ii = int.Parse(c);
        //    //    if (ii == 0)
        //    //        return new AliSuccessType { Success = true };
        //    //    else
        //    //        return new AliSuccessType { Success = false, ErrorCode = ii.ToString() };
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    return this.DeserializeObject<AliSuccessType>(c);
        //    //}
        //}
        ///// <summary>
        ///// 新增站内信
        ///// </summary>
        ///// <param name="orderId">订单号(站内信不需填)</param>
        ///// <param name="buyerId">买家ID（该买家曾主动发送过站内信）</param>
        ///// <param name="content">内容(同web一小时内对同一买家只能发送5次站内信，内容为英文，不能包含html)</param>
        ///// <returns></returns>
        //public virtual AliSuccessType AddMessage(long? orderId, string buyerId, string content)
        //{
        //    ////var dic = new Dictionary<string, object>();
        //    ////dic.Add("orderId", orderId);
        //    ////dic.Add("buyerId", buyerId);
        //    ////dic.Add("content", System.Web.HttpUtility.UrlEncode(content));
        //    ////dic.Add(AliExpress.fieldAccessToken, this.AccessToken);//用户授权令牌
        //    ////return PostWebRequest(AliExpress.openapiIP, this.AppKey, AliExpress.Url + AliExpress.Api_addMessage, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    //return api_addMessage(orderId, buyerId, content);
        //    //try
        //    //{
        //    //    var ii = int.Parse(c);
        //    //    if (ii == 0)
        //    //        return new AliSuccessType { Success = true };
        //    //    else
        //    //        return new AliSuccessType { Success = false, ErrorCode = ii.ToString() };
        //    //}
        //    //catch (Exception)
        //    //{
        //    //    return this.DeserializeObject<AliSuccessType>(c);
        //    //}
        //}
        ///// <summary>
        ///// 站内信更新已读（试用）
        ///// </summary>
        ///// <param name="typeId">一对用户关系ID。relationID。详见数据结构“站内信详细信息”。</param>
        ///// <returns></returns>
        //public virtual AliSuccessType UpdateReadMessage(long? typeId = null)
        //{
        //    ////var dic = new Dictionary<string, object>();
        //    ////dic.Add(AliExpress.fieldAccessToken, this.AccessToken);//用户授权令牌
        //    ////dic.Add("typeId", typeId);
        //    ////return PostWebRequest(AliExpress.openapiIP, this.AppKey, AliExpress.Url + AliExpress.Api_updateReadMessage, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    //return api_updateReadMessage(typeId);
        //    //return this.DeserializeObject<AliSuccessType>(c);
        //}
        ///// <summary>
        ///// 订单留言更新已读(试用）
        ///// </summary>
        ///// <param name="typeId">订单号</param>
        ///// <returns></returns>
        //public virtual AliSuccessType UpdateReadOrderMessage(long? typeId = null)
        //{
        //    ////var dic = new Dictionary<string, object>();
        //    ////dic.Add("typeId", typeId);
        //    ////dic.Add(AliExpress.fieldAccessToken, this.AccessToken);//用户授权令牌

        //    ////return PostWebRequest(AliExpress.openapiIP, this.AppKey, AliExpress.Url + AliExpress.Api_updateReadOrderMessage, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    //return api_updateReadOrderMessage(typeId);
        //    //return this.DeserializeObject<AliSuccessType>(c);
        //}
        #endregion 作废
        /// <summary>
        /// 站内信/订单留言更新处理状态
        /// </summary>
        /// <param name="channelId">通道ID(即关系ID)</param>
        /// <param name="dealStat">处理状态(0未处理,1已处理)</param>
        /// <returns></returns>
        public virtual EcResult UpdateMsgProcessed(string channelId, string dealStat)
        {
            return ModelClient.UpdateMsgProcessed(channelId, dealStat);
        }
        /// <summary>
        /// 站内信/订单留言打标签
        /// </summary>
        /// <param name="channelId">通道ID(即关系ID)</param>
        /// <param name="rank">标签(rank0,rank1,rank2,rank3,rank4,rank5)rank0~rank5为六种不同颜色标记依次为白，红，橙，绿，蓝，紫</param>
        /// <returns></returns>
        public virtual EcResult UpdateMsgRank(string channelId, string rank)
        {
            return ModelClient.UpdateMsgRank(channelId, rank);
        }
        /// <summary>
        /// 根据买卖家loginId查询站内信
        /// </summary>
        /// <param name="currentPage">当前页数</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="buyerId">买家loginId</param>
        /// <param name="sellerId">卖家loginId(与买家建立关系的账号，即信息所属账号)</param>
        /// <returns></returns>
        public virtual ApiQueryMsgDetailListByBuyerIdResponse QueryMsgDetailListByBuyerId(int currentPage, int pageSize, string buyerId, string sellerId)
        {
            return ModelClient.QueryMsgDetailListByBuyerId(currentPage, pageSize, buyerId, sellerId);
        }

        /// <summary>
        /// 获取当前用户下与当前用户建立消息关系的列表
        /// </summary>
        /// <param name="currentPage">当前页<value>1</value><example>1</example></param>
        /// <param name="pageSize">每页条数,pageSize取值范围(0~100) 最多返回前5000条数据<value>20</value><example>20</example></param>
        /// <param name="msgSources">查询类型<value>message_center</value><example>message_center/order_msg</example></param>
        /// <param name="filter">筛选条件(取值:dealStat/readStat/rank0/rank1/rank2/rank3/rank4/rank5)dealStat时将按未处理筛选，值为readStat时将按未读筛选，值为rank1时将按红色标签进行筛选<example>dealStat</example></param>
        /// <returns></returns>
        public virtual ApiQueryMsgRelationListResponse QueryMsgRelationList(int currentPage, int pageSize, string msgSources, string filter = null)
        {
            return ModelClient.QueryMsgRelationList(currentPage, pageSize, msgSources, filter);
        }
        /// <summary>
        /// 站内信/订单留言更新已读
        /// </summary>
        /// <param name="channelId">通道ID，即关系ID</param>
        /// <param name="msgSources">查询类型</param>
        /// <returns></returns>
        public virtual EcResult UpdateMsgRead(string channelId, string msgSources)
        {
            return ModelClient.UpdateMsgRead(channelId, msgSources);
        }

        /// <summary>
        /// 站内信/订单留言查询详情列表
        /// </summary>
        /// <param name="currentPage">当前页<value>1</value><example>1</example></param>
        /// <param name="pageSize">每页条数,pageSize取值范围(0~100) 最多返回前5000条数据<value>20</value><example>20</example></param>
        /// <param name="channelId">通道ID，即关系ID<example>22323233</example></param>
        /// <param name="msgSources">类型(message_center/order_msg)<value>message_center</value><example>message_center</example></param>
        /// <returns></returns>
        public virtual ApiQueryMsgDetailListResponse QueryMsgDetailList(int currentPage, int pageSize, string channelId, string msgSources)
        {
            return ModelClient.QueryMsgDetailList(currentPage, pageSize, channelId, msgSources);
        }
        /// <summary>
        /// 新增站内信/订单留言
        /// </summary>
        /// <param name="channelId">通道ID，即关系ID<example>334455556</example></param>
        /// <param name="buyerId">买家账号<example>uk33445</example></param>
        /// <param name="content">内容<example>hello</example></param>
        /// <param name="msgSources">类型(message_center/order_msg)<value>message_center</value><example>message_center</example></param>
        /// <param name="imgPath">图片地址<example>http://g02.a.alicdn.com/kf/HTB1U07VIVXXXXaiaXXXq6xXFXXXu.jpg</example></param>
        /// <returns></returns>

        public virtual EcResult AddMsg(string channelId, string buyerId, string content, string msgSources, string imgPath)
        {
            return ModelClient.AddMsg(channelId, buyerId, content, msgSources, imgPath);
        }
        #endregion 站内信
        #region 交易
        public enum OrderStatus
        {
            /// <summary>
            /// 等待买家付款
            /// </summary>
            PLACE_ORDER_SUCCESS,
            /// <summary>
            /// 买家申请取消
            /// </summary>
            IN_CANCEL,
            /// <summary>
            /// 等待您发货
            /// </summary>
            WAIT_SELLER_SEND_GOODS,
            /// <summary>
            /// 部分发货
            /// </summary>
            SELLER_PART_SEND_GOODS,
            /// <summary>
            /// 等待买家收货
            /// </summary>
            WAIT_BUYER_ACCEPT_GOODS,
            /// <summary>
            /// 买家确认收货后，等待退放款处理的状态
            /// </summary>
            FUND_PROCESSING,
            /// <summary>
            /// 已结束的订单
            /// </summary>
            FINISH,
            /// <summary>
            /// 含纠纷的订单，纠纷中
            /// </summary>
            IN_ISSUE,
            /// <summary>
            /// 冻结中的订单
            /// </summary>
            IN_FROZEN,
            /// <summary>
            /// 等待您确认金额
            /// </summary>
            WAIT_SELLER_EXAMINE_MONEY,
            /// <summary>
            /// 订单处于风控24小时中，从买家在线支付完成后开始，持续24小时
            /// </summary>
            RISK_CONTROL
        }
        /// <summary>
        /// 一键延长买家收货时间(订单状态需处于“买家确认收货”及“非纠纷、非冻结”状态下可支持该操作。）试用
        /// </summary>
        /// <param name="OrderId">需要延长的订单ID</param>
        /// <param name="days">请求延长的具体天数</param>
        /// <remarks>
        /// 一键延长买家收货时间(订单状态需处于“买家确认收货”及“非纠纷、非冻结”状态下可支持该操作。)
        /// </remarks>
        /// <returns>{"errorCode":0,"success":true}</returns>
        public virtual OperationResult ExtendsBuyerAcceptGoodsTime(long OrderId, int days)
        {
            return ModelClient.ExtendsBuyerAcceptGoodsTime(OrderId, days);
        }
        /// <summary>
        /// 订单交易信息查询（试用）
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public virtual OrderTradeInfo FindOrderTradeInfo(long orderId)
        {
            return ModelClient.FindOrderTradeInfo(orderId);
        }
        /// <summary>
        /// 订单收货信息查询（试用）
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public virtual TpOpenAddressDTO FindOrderReceiptInfo(long orderId)
        {
            return ModelClient.FindOrderReceiptInfo(orderId);
        }
        /// <summary>
        /// 订单基础信息查询（试用）
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public virtual OrderBaseInfo FindOrderBaseInfo(long orderId)
        {
            return ModelClient.FindOrderBaseInfo(orderId);
        }
        /// <summary>
        /// 订单列表简化查询（试用）
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">每页个数，最大50</param>
        /// <param name="createDateStart">订单创建时间起始值，格式: mm/dd/yyyy hh:mm:ss,如10/08/2013 00:00:00 倘若时间维度未精确到时分秒，故该时间条件筛选不许生效。</param>
        /// <param name="createDateEnd">订单创建时间结束值，格式: mm/dd/yyyy hh:mm:ss,如10/08/2013 00:00:00 倘若时间维度未精确到时分秒，故该时间条件筛选不许生效。	</param>
        /// <param name="orderStatus">订单状态： PLACE_ORDER_SUCCESS:等待买家付款; IN_CANCEL:买家申请取消; WAIT_SELLER_SEND_GOODS:等待您发货; SELLER_PART_SEND_GOODS:部分发货; WAIT_BUYER_ACCEPT_GOODS:等待买家收货; FUND_PROCESSING:买家确认收货后，等待退放款处理的状态; FINISH:已结束的订单; IN_ISSUE:含纠纷的订单; IN_FROZEN:冻结中的订单; WAIT_SELLER_EXAMINE_MONEY:等待您确认金额; RISK_CONTROL:订单处于风控24小时中，从买家在线支付完成后开始，持续24小时。</param>
        /// <returns></returns>
        public virtual SimpleOrderListVO FindOrderListSimpleQuery(int page = 1, int pageSize = 50, System.DateTimeOffset? createDateStart = null, System.DateTimeOffset? createDateEnd = null, OrderStatus? orderStatus = null)
        {
            return ModelClient.FindOrderListSimpleQuery(page, pageSize, ConvertTimeParam(createDateStart), ConvertTimeParam(createDateEnd), (orderStatus.HasValue ? orderStatus.Value.ToString() : null));
        }
        /// <summary>
        /// 未放款订单请款（试用）
        /// </summary>
        /// <param name="orderId">主订单id，一次只能一个。</param>
        /// <param name="files">附件保存的Url。这个值的来源是 上传请款图片接口返回的参数 。</param>
        /// <param name="memo">填写请款备注详情。</param>
        /// <returns></returns>
        public virtual RequestLoanResult RequestPaymentRelease(long orderId, string files, string memo)
        {
            return ModelClient.RequestPaymentRelease(orderId, files, memo);
        }
        /// <summary>
        /// 卖家在订单做请款时上传证明附件。（试用）
        /// </summary>
        /// <param name="filename">图片原名，上传证明文件，支持jpg和png格式，大小不超过2MB。	</param>
        /// <param name="input"></param>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        public virtual RequestLoanResult UpdateDeliveriedConfirmationFile(string filename, byte[] input = null, long? orderId = null)
        {
            return ModelClient.UpdateDeliveriedConfirmationFile(input, filename, orderId);
        }
        /// <summary>
        /// 查询订单放款信息(请注意：目前只查询进入放款中的订单信息状态，未进入放款中订单暂未做内容兼容。）
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">每页个数，最大50</param>
        /// <param name="orderId">主订单id，一次只能查询一个</param>
        /// <param name="loanStatus">订单放款状态,true 已放款，false 未放款</param>
        /// <param name="createDateStart">放款时间起始值</param>
        /// <param name="createDateEnd">放款时间截止值</param>
        /// <returns></returns>
        public virtual FundLoanListVO FindLoanListQuery(int page = 1, int pageSize = 50, long? orderId = null, bool? loanStatus = null, System.DateTimeOffset? createDateStart = null, System.DateTimeOffset? createDateEnd = null)
        {
            return ModelClient.FindLoanListQuery(page, pageSize, ConvertTimeParam(createDateStart), ConvertTimeParam(createDateEnd), (loanStatus.HasValue ? (loanStatus.Value ? "loan_ok" : "wait_loan") : null), orderId);
        }
        /// <summary>
        /// 交易订单详情查询
        /// </summary>
        /// <param name="orderId">订单Id</param>
        /// <returns></returns>
        public virtual TpOpenOrderDetailDTO FindOrderById(long orderId)
        {
            return ModelClient.FindOrderById(orderId, null, null);


            //fieldList	String	否	暂不支持。需要返回的订单对象字段。多个字段用“,”分隔。如果想返回整个子对象，该字段不设值。 目前支持以下字段：id,gmtCreate,orderStatus,sellerOperatorAliid,sellerOperatorLoginId,paymentType ,initOderAmount,orderAmount,escrowFee		
            //extInfoBitFlag	Integer	否	暂不支持。扩展信息目前支持纠纷信息，放款信息，物流信息，买方信息和退款信息，分别对应二进制位1,2,3,4,5。例如，只查询纠纷信息和物流信息，extInfoBitFlag=10100；查询全部extInfoBitFlag=11111
        }
        /// <summary>
        /// 交易订单列表查询
        /// </summary>
        /// <param name="page">当前页码</param>
        /// <param name="pageSize">每页个数，最大50</param>
        /// <param name="orderStatus">订单状态</param>
        /// <param name="createDateStart">订单创建时间起始值</param>
        /// <param name="createDateEnd">订单创建时间结束值</param>
        /// <remarks>
        /// 订单状态会多一个全新的值RISK_CONTROL 该值的含义是订单处于风控24小时中，从买家在线支付完成后开始，持续24小时。
        /// </remarks>
        /// <returns></returns>
        public virtual OrderListVO FindOrderListQuery(int page = 1, int pageSize = 50, OrderStatus? orderStatus = null, System.DateTimeOffset? createDateStart = null, System.DateTimeOffset? createDateEnd = null)
        {
            return ModelClient.FindOrderListQuery(page, pageSize, ConvertTimeParam(createDateStart), ConvertTimeParam(createDateEnd), Convert.ToString(orderStatus));
        }
        #endregion 交易
        #region 上传图片
        /// <summary>
        /// 上传图片到临时目录。适用于上传商品主图或SKU图片，临时图片至多上传6张图片用于商品介绍展示。 
        /// </summary>
        /// <param name="srcFilePath">图片本地路径</param>
        /// <returns></returns>
        public virtual UploadTempImage UploadTempImage(string srcFilePath)
        {
            if (!File.Exists(srcFilePath)) return new UploadTempImage { Success = false, Exception = "找不到本地文件 " + srcFilePath };
            var fileName = Path.GetFileName(srcFilePath);
            var fileBytes = System.IO.File.ReadAllBytes(srcFilePath);
            return UploadTempImage(fileName, fileBytes);
        }
        /// <summary>
        /// 上传图片到临时目录。适用于上传商品主图或SKU图片，临时图片至多上传6张图片用于商品介绍展示。 
        /// </summary>
        /// <param name="srcFileName">图片原名</param>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public virtual UploadTempImage UploadTempImage(string srcFileName, byte[] fileBytes)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);//用户授权令牌
            dic.Add("srcFileName", srcFileName);

            var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_uploadTempImage, isFile: true, stream: fileBytes, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
            //return api_uploadTempImage(srcFileName, days);
            return this.DeserializeObject<UploadTempImage>(c);
        }

        #endregion
        #region 图片银行
        public virtual ApiUploadImage4SDKResponse UpLoadImage4SDK(string srcFilePath, string groupId=null)
        {
            if (!File.Exists(srcFilePath)) return new ApiUploadImage4SDKResponse { Success = false, Exception = "找不到本地文件 " + srcFilePath };
            var fileName = Path.GetFileName(srcFilePath);
            var fileBytes = System.IO.File.ReadAllBytes(srcFilePath);
            return UpLoadImage4SDK(fileName, fileBytes, groupId);
        }
        public virtual ApiUploadImage4SDKResponse UpLoadImage4SDK(string fileName, byte[] fileBytes, string groupId=null)
        {
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);//用户授权令牌
            //                                                             //dic.Add("fileName", fileName);
            //                                                             //dic.Add("groupId", groupId);

            //var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_uploadImage, isFile: true, stream: fileBytes, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
            ////return api_uploadImage(fileName, groupId);
            ////return this.DeserializeObject<UploadImage>(c);
            //return null;
            //ModelClient.UploadImage4SDK("")

            //dic.Add("fileName", fileName);
            ////dic.Add("imageBytes", imageBytes);
            //dic.Add("groupId", groupId);
            ////提交请求
            //var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + "api.uploadImage4SDK", isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);

            //return null;



            return ModelClient.UploadImage4SDK(fileName, fileBytes, groupId);
            //Dictionary<string, object> dic = new Dictionary<string, object>();
            //dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);//用户授权令牌
            //dic.Add("fileName", fileName);
            //dic.Add("groupId", groupId);
            //ModelClient.UploadImage4SDK("",)
            //var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_uploadImage, isFile: true, stream: fileBytes, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
            ////return api_uploadImage(fileName, groupId);
            //return this.DeserializeObject<UploadImage>(c);
        }
        //public virtual UploadImage UploadImage(string srcFilePath, string groupId)
        //{
        //    if (!File.Exists(srcFilePath)) return new UploadImage { Success = false, Exception = "找不到本地文件 " + srcFilePath };
        //    var fileName = Path.GetFileName(srcFilePath);
        //    var fileBytes = System.IO.File.ReadAllBytes(srcFilePath);
        //    return UploadImage(fileName, groupId, fileBytes);
        //}
        ///// <summary>
        ///// 上传图片到图片银行（由于无法实现图片的上传、下载，故该测试工具不可用。）
        ///// </summary>
        ///// <param name="fileName">上传文件原名(针对图片文件显示名称”字节数（256字节以内） 限制。;)<value>$req.http.params.fileName</value></param>
        ///// <param name="groupId">图片保存的图片组，groupId为空，则图片保存在Other组中。</param>
        ///// <param name="fileBytes"></param>
        ///// <returns></returns>
        //public virtual UploadImage UploadImage(string fileName, string groupId, byte[] fileBytes)
        //{
        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);//用户授权令牌
        //    dic.Add("fileName", fileName);
        //    dic.Add("groupId", groupId);

        //    var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_uploadImage, isFile: true, stream: fileBytes, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    //return api_uploadImage(fileName, groupId);
        //    return this.DeserializeObject<UploadImage>(c);
        //}
        #endregion
        #region 产品
        /// <summary>
        /// 商品列表查询接口。主账号可查询所有商品，子账号只可查询自身所属商品。
        /// </summary>
        /// <param name="productStatusType">商品业务状态，目前提供4种，输入参数分别是：上架:onSelling ；下架:offline ；审核中:auditing ；审核不通过:editingRequired。</param>
        /// <param name="subject">商品标题模糊搜索字段。只支持半角英数字，长度不超过128。</param>
        /// <param name="groupId">商品分组搜索字段。输入商品分组id(groupId).</param>
        /// <param name="wsDisplay">商品下架原因：expire_offline：过期下架，user_offline：用户下架,violate_offline：违规下架,punish_offline：交易违规下架，degrade_offline：降级下架</param>
        /// <param name="offLineTime">到期时间搜索字段。商品到期时间，输入值小于等于30，单</param>
        /// <param name="productId">商品id搜索字段。输入所需查询的商品id，查询条数限制最大20。</param>
        /// <param name="exceptedProductIds">待排除的产品ID列表。</param>
        /// <param name="pageSize">每页查询商品数量。输入值小于100，默认20。</param>
        /// <param name="currentPage">需要商品的当前页数。默认第一页。</param>
        /// <returns></returns>
        public virtual ApiFindProductInfoListQueryResponse FindProductInfoListQuery(ProductStatusType productStatusType, string subject = null, int? groupId = null, WsDisplay? wsDisplay = null, int? offLineTime = null, long? productId = null, long[] exceptedProductIds = null, int? pageSize = null, int? currentPage = null)
        {
            return ModelClient.FindProductInfoListQuery(Convert.ToString(productStatusType), subject, groupId, Convert.ToString(wsDisplay), offLineTime, productId, exceptedProductIds, pageSize, currentPage);
        }
        /// <summary>
        /// 根据商品id查询单个商品的详细信息
        /// </summary>
        /// <param name="productId">商品ID</param>
        /// <returns></returns>
        public virtual ApiFindAeProductByIdResponse FindAeProductById(long productId)
        {
            return ModelClient.FindAeProductById(productId);
        }
        /// <summary>
        /// 编辑商品的单个字段(目前使用api.editSimpleProductFiled这个接口 暂不支持商品分组、商品属性、SKU、服务模板的修改。请注意！) 
        /// </summary>
        /// <param name="productId">指定编辑产品的id</param>
        /// 
        /// <returns></returns>
        public virtual ApiEditSimpleProductFiledResponse EditSimpleProductFiled(long productId, string fiedName, string fiedvalue)
        {
            return ModelClient.EditSimpleProductFiled(productId, fiedName, fiedvalue);
        }
        public virtual AliAttributesType GetChildAttributesResultByPostCateIdAndPath(int cateId, string parentAttrValueList = null)
        {
            //return ModelClient.GetChildAttributesResultByPostCateIdAndPath(cateId, parentAttrValueList);
            var c = StrClient.getChildAttributesResultByPostCateIdAndPath(cateId, parentAttrValueList);
            return this.DeserializeObject<AliAttributesType>(c); ;
        }
        /// <summary>
        ///  发布产品
        /// </summary>
        /// <returns></returns>
        public virtual ApiPostAeProductResponse PostAeProduct(string detail, string aeopAeProductSKUs, int deliveryTime, long? promiseTemplateId, int categoryId, string subject, string productPrice, int freightTemplateId, string imageURLs, int productUnit, bool? packageType, int? lotNum, int packageLength, int packageWidth, int packageHeight, string grossWeight, bool? isPackSell, int? baseUnit, int? addUnit, string addWeight, int? wsValidNum, string aeopAeProductPropertys, int? bulkOrder, int? bulkDiscount, long? sizechartId, string reduceStrategy, long? groupId, string currencyCode, string mobileDetail, System.DateTimeOffset? couponStartDate, System.DateTimeOffset? couponEndDate)
        {
            return this.ModelClient.PostAeProduct(
                detail: detail,
                aeopAeProductSKUs: System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(aeopAeProductSKUs)),//非必需
                deliveryTime: deliveryTime,
                categoryId: categoryId,
                subject: subject,
                freightTemplateId: freightTemplateId,
                imageURLs: imageURLs,
                productUnit: productUnit,
                packageLength: packageLength,
                packageWidth: packageWidth,
                packageHeight: packageHeight,
                grossWeight: grossWeight,
                aeopAeProductPropertys: Newtonsoft.Json.JsonConvert.SerializeObject(aeopAeProductPropertys),
                promiseTemplateId: promiseTemplateId,
                productPrice: productPrice,
                packageType: packageType,
                lotNum: lotNum,
                isPackSell: isPackSell,
                baseUnit: baseUnit,
                addUnit: addUnit,
                addWeight: addWeight,
                wsValidNum: wsValidNum,
                bulkOrder: bulkOrder,
                bulkDiscount: bulkDiscount,
                sizechartId: sizechartId,
                reduceStrategy: reduceStrategy,
                groupId: groupId,
                currencyCode: currencyCode,
                mobileDetail: mobileDetail,
                couponStartDate: couponStartDate,
                couponEndDate: couponEndDate
              );
        }

        /// <summary>
        ///  发布产品
        /// </summary>
        /// <returns></returns>
        public ApiPostAeProductResponse PostAeProduct(AliProductType product)
        {
            //product.AeopAeProductPropertys[1].AttrValue = "Scales";
            //product.AeopAeProductSKUs[0].SkuPrice = product.ProductPrice;
            //product.AeopAeProductSKUs[0].AeopSKUProperty = null;

            //product.CategoryId = 200001197;
            //string aeopAeProductSKUs="[ {\"skuPrice\": \"12.5\", \"skuStock\": \"true\", \"ipmSkuStock\":\"1\", \"skuCode\": \"1104\", \"aeopSKUProperty\": [] }]";
            //string aeopAeProductSKUs = Newtonsoft.Json.JsonConvert.SerializeObject(product.AeopAeProductSKUs);
            //aeopAeProductSKUs = "[{\"id\":\"200007566:361383;200007567:200007901;200007568:100016942\",\"ipmSkuStock\":1,\"skuPrice\":\"1.00\",\"skuStock\":true,\"aeopSKUProperty\":[{\"propertyValueId\":361383,\"skuPropertyId\":200007566},{\"propertyValueId\":200007901,\"skuPropertyId\":200007567},{\"propertyValueId\":100016942,\"skuPropertyId\":200007568}],\"skuCode\":\"896663\"},{\"id\":\"200007566:361383;200007567:200007901;200007568:100016943\",\"ipmSkuStock\":1,\"skuPrice\":\"1.00\",\"skuStock\":true,\"aeopSKUProperty\":[{\"propertyValueId\":361383,\"skuPropertyId\":200007566},{\"propertyValueId\":200007901,\"skuPropertyId\":200007567},{\"propertyValueId\":100016943,\"skuPropertyId\":200007568}],\"skuCode\":\"89664\"},{\"id\":\"200007566:361383;200007567:200007901;200007568:166\",\"ipmSkuStock\":1,\"skuPrice\":\"1.00\",\"skuStock\":true,\"aeopSKUProperty\":[{\"propertyValueId\":361383,\"skuPropertyId\":200007566},{\"propertyValueId\":200007901,\"skuPropertyId\":200007567},{\"propertyValueId\":166,\"skuPropertyId\":200007568}],\"skuCode\":\"89665\"},{\"id\":\"200007566:361383;200007567:200007901;200007568:100016944\",\"ipmSkuStock\":1,\"skuPrice\":\"1.00\",\"skuStock\":true,\"aeopSKUProperty\":[{\"propertyValueId\":361383,\"skuPropertyId\":200007566},{\"propertyValueId\":200007901,\"skuPropertyId\":200007567},{\"propertyValueId\":100016944,\"skuPropertyId\":200007568}],\"skuCode\":\"89667\"},{\"id\":\"200007566:361383;200007567:200007901;200007568:100016945\",\"ipmSkuStock\":1,\"skuPrice\":\"1.00\",\"skuStock\":true,\"aeopSKUProperty\":[{\"propertyValueId\":361383,\"skuPropertyId\":200007566},{\"propertyValueId\":200007901,\"skuPropertyId\":200007567},{\"propertyValueId\":100016945,\"skuPropertyId\":200007568}],\"skuCode\":\"89668\"},{\"id\":\"200007566:361383;200007567:200007901;200007568:100016946\",\"ipmSkuStock\":1,\"skuPrice\":\"1.00\",\"skuStock\":true,\"aeopSKUProperty\":[{\"propertyValueId\":361383,\"skuPropertyId\":200007566},{\"propertyValueId\":200007901,\"skuPropertyId\":200007567},{\"propertyValueId\":100016946,\"skuPropertyId\":200007568}],\"skuCode\":\"89669\"},{\"id\":\"200007566:361383;200007567:200007901;200007568:167\",\"ipmSkuStock\":1,\"skuPrice\":\"1.00\",\"skuStock\":true,\"aeopSKUProperty\":[{\"propertyValueId\":361383,\"skuPropertyId\":200007566},{\"propertyValueId\":200007901,\"skuPropertyId\":200007567},{\"propertyValueId\":167,\"skuPropertyId\":200007568}],\"skuCode\":\"89770\"}]";
            //  string testskus = "[{\"ipmSkuStock\":30,\"skuPrice\":\"3.41\",\"skuStock\":true,\"aeopSKUProperty\":[],\"skuCode\":\"6545\"}]";
            var dic = new Dictionary<string, object>();
            dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);/////用户授权令牌
            dic.Add("detail", product.Detail);/////详情
            dic.Add("aeopAeProductSKUs", System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(product.AeopAeProductSKUs.ToList().Select(f => { if (f.AeopSKUProperty == null || f.AeopSKUProperty.Count() == 0) return f; f.AeopSKUProperty = f.AeopSKUProperty.OrderBy(ff => ff.skuPropertyId).ToArray(); return f; }))));///非必需
            dic.Add("deliveryTime", product.DeliveryTime.ToString());/////备货期
                                                                     //dic.Add("promiseTemplateId","");////服务模板
            dic.Add("categoryId", product.CategoryId.ToString());////商品所属栏目id
            dic.Add("subject", System.Web.HttpUtility.UrlEncode(product.Subject));/////标题
            dic.Add("keyword", System.Web.HttpUtility.UrlEncode(product.Keyword));////搜索关键词
            dic.Add("productMoreKeywords1", product.ProductMoreKeywords1);///更多关键词 非必需
            dic.Add("productMoreKeywords2", product.ProductMoreKeywords2);///非必需
            dic.Add("groupId", product.GroupId.ToString());//////产品组ID
            dic.Add("productPrice", product.ProductPrice);///产品价格
            dic.Add("freightTemplateId", product.FreightTemplateId);/////运费模板id
            dic.Add("isImageDynamic", product.IsImageDynamic.ToString());////是否多动态图片
            dic.Add("imageURLs", product.ImageURLs);////图片url
            dic.Add("productUnit", product.ProductUnit.ToString());///商品单位编号
            dic.Add("packageType", product.PackageType.ToString());///是否打包销售   非必需
            dic.Add("lotNum", product.LotNum.ToString());////每包件数 打包销售lotNum>1  非大包lotNum=1 非必需
            dic.Add("packageLength", product.PackageLength);////商品包装长度
            dic.Add("packageHeight", product.PackageHeight);///商品包装高度
            dic.Add("packageWidth", product.PackageWidth);    ///商品包装宽度
            dic.Add("grossWeight", product.GrossWeight);////公斤
            dic.Add("isPackSell", false);////是否自定义计重   非必需

            // dic.Add("baseUnit", "");/////isPackSell为true时,此项必填。购买几件以内不增加运费。取值范围1-1000 非必需
            // dic.Add("addUnit", "");////isPackSell为true时,此项必填。 每增加件数.取值范围1-1000 非必需
            // dic.Add("addWeight", "");////////isPackSell为true时,此项必填。 对应增加的重量.取值范围:0.001-500.000,保留三位小数,采用进位制,单位:公斤 非必需
            dic.Add("wsValidNum", product.WsValidNum.ToString());///商品有效天数。取值范围:1-30,单位:天。
            //dic.Add("src", "isv");
            string AeopAeProductPropertys = Newtonsoft.Json.JsonConvert.SerializeObject(product.AeopAeProductPropertys);
            AeopAeProductPropertys = AeopAeProductPropertys.Replace("\"attrNameId\":0,", "").Replace("\"attrValueId\":0,", "").Replace("\"attrValue\":null,", "").Replace("\"attrName\":null", "");
            AeopAeProductPropertys = AeopAeProductPropertys.Replace(",}", "}");
            if (AeopAeProductPropertys.EndsWith(",}]"))
            {
                AeopAeProductPropertys = AeopAeProductPropertys.Replace(",}]", "}]");
            }
            //AeopAeProductPropertys.
            dic.Add("aeopAeProductPropertys", System.Web.HttpUtility.UrlEncode(AeopAeProductPropertys));///产品属性 是否必填根据getAttributesResultByCateId
            dic.Add("reduceStrategy", product.ReduceStrategy);
            if (product.BulkDiscount > 0 && product.BulkOrder > 0)
            {
                dic.Add("bulkOrder", product.BulkOrder);/////批发最小数量 。取值范围2-100000
                dic.Add("bulkDiscount", product.BulkDiscount);/////批发折扣
            }

            //dic.Add("sizechartId","");////尺码表模版Id

            var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_postAeProduct, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
            //var c = api_postAeProduct(cateId, parentAttrValueList);
            if (c.Contains("sku attribute order error"))
            {
                dic.Remove("aeopAeProductSKUs");
                dic.Add("aeopAeProductSKUs", System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(product.AeopAeProductSKUs.ToList().Select(f => { if (f.AeopSKUProperty == null || f.AeopSKUProperty.Count() == 0) return f; f.AeopSKUProperty = f.AeopSKUProperty.OrderByDescending(ff => ff.skuPropertyId).ToArray(); return f; }))));///非必需
                c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_postAeProduct, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
            }
            return this.DeserializeObject<ApiPostAeProductResponse>(c);
        }

        /// <summary>
        ///  发布产品
        /// </summary>
        /// <returns></returns>
        public ApiPostAeProductResponse PostAeProduct(NewpostAeProduct product)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);//用户授权令牌
            dic.Add("aeopAeProductSKUs", System.Web.HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(product.aeopAeProductSKUs)));///非必需
            dic.Add("imageURLs", product.imageURLs);//图片url
            dic.Add("aeopAeProductPropertys", Newtonsoft.Json.JsonConvert.SerializeObject(product.aeopAeProductPropertys));/////备货期
            dic.Add("detail", product.detail);//详情
            dic.Add("categoryId", product.categoryId.ToString());////商品所属栏目id
            dic.Add("deliveryTime", product.deliveryTime.ToString());/////备货期
            dic.Add("subject", System.Web.HttpUtility.UrlEncode(product.subject));/////标题
            dic.Add("productPrice", product.productPrice);//产品价格
            dic.Add("productUnit", product.productUnit);//
            dic.Add("packageLength", product.packageLength);//商品包装长度
            dic.Add("packageHeight", product.packageHeight);//商品包装高度
            dic.Add("packageWidth", product.packageWidth);//商品包装宽度
            dic.Add("grossWeight", product.grossWeight);//公斤
            dic.Add("reduceStrategy", product.reduceStrategy);//付款方式
            dic.Add("currencyCode", product.currencyCode);//货币类型
            dic.Add("freightTemplateId", product.freightTemplateId);//运费模板id
            dic.Add("groupId", product.groupId);//产品分组
            #region 非必须参数
            if (product.packageType)
            {
                dic.Add("packageType", product.packageType);//打包销售
                dic.Add("lotNum", product.lotNum);//每包件数
            }
            if (product.isPackSell)
            {
                dic.Add("isPackSell", product.isPackSell);//是否自定义重量
                dic.Add("baseUnit", product.baseUnit);//购买几件不增加运费
                dic.Add("addUnit", product.addUnit);//增加件数
                dic.Add("addWeight", product.addWeight);//对应增加的重量
            }
            if (product.wsValidNum > 0)
            {
                dic.Add("wsValidNum", product.wsValidNum);//产品有效期
            }
            if (product.bulkOrder > 0)
            {
                dic.Add("bulkOrder", product.bulkOrder);//批发最小数量
                dic.Add("bulkDiscount", product.bulkDiscount);//批发折扣 如,打68折,则存32。
            }
            #endregion 非必须参数
                var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_postAeProduct, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
            //var c = api_postAeProduct(cateId, parentAttrValueList);
            return this.DeserializeObject<ApiPostAeProductResponse>(c);
        }

        public virtual ApiGetProductGroupListResponse getProductGroupList()
        {
            return ModelClient.GetProductGroupList();
        }
        /// <summary>
        /// 编辑产品类目、属性、sku
        /// </summary>
        /// <param name="productId">商品id，一次只能上传一个</param>
        /// <param name="categoryId">产品类目ID，如果不想调整类目，则不要填写。</param>
        /// <param name="productSkus">该产品新的类目SKU属性。如果没有指定categoryId, 则该SKU属性则为当前产品所属类目下的SKU属性，如果指定了categoryId, 则该SKU属性则为新类目下的SKU属性。 特别提示：新增SKU实际可售库存属性ipmSkuStock，该属性值的合理取值范围为0~999999，如该商品有SKU时，请确保至少有一个SKU是有货状态，也就是ipmSkuStock取值是1~999999，在整个商品纬度库存值的取值范围是1~999999。</param>
        /// <param name="productProperties">该产品新的类目属性。如果没有指定categoryId, 则该类目属性则为当前产品所属类目下的类目属性，如果指定了categoryId, 则该类目属性则为新类目下的类目属性。</param>
        /// <returns></returns>
        public virtual ApiEditProductCidAttIdSkuResponse EditProductCidAttIdSku(long productId, long? categoryId = null, Model.AeopAeProductSKUs[] productSkus = null, Model.AeopAeProductPropertys[] productProperties = null)
        {
            return ModelClient.EditProductCidAttIdSku(productId, categoryId, Newtonsoft.Json.JsonConvert.SerializeObject(productSkus), Newtonsoft.Json.JsonConvert.SerializeObject(productProperties));
        }

        /// <summary>
        /// 根据skuid更改库价格信息
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="skuid"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public ApiEditSingleSkuPriceResponse EditSingleSkuPrice(string productid, string skuid, string price)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);/////用户授权令牌
            dic.Add("productId", productid);//产品id
            dic.Add("skuId", skuid); //aeopAeproductSKUs获取单个产品信息中"id"。
            dic.Add("skuPrice", price);//要修改的库存信息

            var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_editSingleSkuPrice, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
            return this.DeserializeObject<ApiEditSingleSkuPriceResponse>(c);
        }
        /// <summary>
        /// 根据skuid更改库存信息
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="skuid"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public ApiEditSingleSkuStockResponse EditSingleSkuStock(string productid, string skuid, string quantity)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);/////用户授权令牌
            dic.Add("productId", productid);//产品id
            dic.Add("skuId", skuid); //aeopAeproductSKUs获取单个产品信息中"id"。
            dic.Add("ipmSkuStock", quantity);///要修改的库存信息

            var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_editSingleSkuStock, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
            return this.DeserializeObject<ApiEditSingleSkuStockResponse>(c);
        }


        public ApiEditAeProductResponse EditProductCidAttIdSku(ApiFindAeProductByIdResponse product)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);/////用户授权令牌
            dic.Add("productId", product.ProductId);
            string skujson = Json.Encode(product.AeopAeProductSKUs);
            dic.Add("productSkus", skujson);///非必需
            var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_editProductCidAttIdSku, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
            return this.DeserializeObject<ApiEditAeProductResponse>(c);
        }
        #endregion 产品
        #region 运费
        /// <summary>
        /// 列出用户的运费模板
        /// </summary>
        public virtual ApiListFreightTemplateResponse ListFreightTemplate()
        {
            return ModelClient.ListFreightTemplate();
        }
        /// <summary>
        /// 运费计算
        /// </summary>
        /// <param name="length">长</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <param name="weight">毛重</param>
        /// <param name="count">数量</param>
        /// <param name="country">country</param>
        /// <param name="isCustomPackWeight">是否为自定义打包计重,Y/N	Y</param>
        /// <param name="packBaseUnit">打包计重几件以内按单个产品计重,当isCustomPackWeight=Y时必选</param>
        /// <param name="packAddUnit">打包计重超过部分每增加件数,当isCustomPackWeight=Y时必选</param>
        /// <param name="packAddWeight">打包计重超过部分续重,当isCustomPackWeight=Y时必选</param>
        /// <param name="freightTemplateId">运费模板ID 必选</param>
        /// <returns></returns>
        public virtual ApiCalculateFreightResponse CalculateFreight(int? length = null, int? width = null, int? height = null, int? weight = null, int? count = null, string country = null,
            bool? isCustomPackWeight = null, int? packBaseUnit = null, int? packAddUnit = null, double? packAddWeight = null, int? freightTemplateId = null, decimal? productPrice = null)
        {
            return ModelClient.CalculateFreight(length, width, height, weight, count, country, isCustomPackWeight, packBaseUnit, packAddUnit, packAddWeight, freightTemplateId, productPrice);
        }
        /// <summary>
        /// 通过模板ID获取运费模板的详细信息
        /// </summary>
        /// <param name="templateId">运费模板</param>
        /// <returns></returns>
        public virtual ApiGetFreightSettingByTemplateQueryResponse GetFreightSettingByTemplateQuery(int templateId)
        {
            return ModelClient.GetFreightSettingByTemplateQuery(templateId);
        }
        #endregion
        #region 类目
        /// <summary>
        /// 获取下级类目信息,同获取单个类目信息内容相同（cateId=0获得一级类目列表） 
        /// </summary>
        /// <param name="cateId"></param>
        /// <returns></returns>
        public virtual AliPostCategoryList GetChildrenPostCategoryById(int? cateId)
        {
            //return ModelClient.GetChildrenPostCategoryById(cateId);
            var c = StrClient.api_getChildrenPostCategoryById(cateId);
            return this.DeserializeObject<AliPostCategoryList>(c);
        }

        //public virtual string  GetChildAttributesResultByPostCateIdAndPath(int? cateId)
        //{
    
        //    var dic = new Dictionary<string, object>();
        //    dic.Add(AliExpressClient.fieldAccessToken, this.AccessToken);/////用户授权令牌
        //    dic.Add("cateId", cateId);
        //    var c = PostWebRequest(AliExpressClient.openapiIP, this.AppKey, AliExpressClient.Url + AliExpressClient.Api_getChildAttributesResultByPostCateIdAndPath, isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIsAppKey: true);
        //    return c;
        //}


        #endregion
        #region 其他
        /// <summary>
        /// 获取阿里巴巴服务器时间
        /// </summary>
        /// <returns></returns>
        public virtual System.DateTimeOffset GetCurrentTime()
        {
            //http://gw.api.alibaba.com/openapi/param2/1/system/currentTime/{AppKey}
            string c = this.GetAsync<System.String>(AliExpressClient.openapiIP + AliExpressClient.UrlCurrentTime + "/" + this.AppKey);
            return this.DeserializeObject<System.DateTimeOffset>(c);
        }
        #endregion 其他
    }
}