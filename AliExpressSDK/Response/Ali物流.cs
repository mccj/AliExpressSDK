using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SDK.Platform.AliExpressApi.RModel
{
    public class AliPrintInfoResponse : Model.AliErrorType
    {

        [JsonProperty("StatusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("Content-Disposition")]
        public string ContentDisposition { get; set; }

        [JsonProperty("Content-Type")]
        public string ContentType { get; set; }
        [JsonIgnore]
        public byte[] PdfFile { get { return string.IsNullOrWhiteSpace(this.Body) ? null : System.Convert.FromBase64String(this.Body); } }
    }
}

//namespace AliExpressSDK
//{

//    ///// <summary>
//    ///// 描述运单号面单PDF打印查询参数
//    ///// </summary>
//    //public class AeopWarehouseOrderQueryPdfRequest
//    //{
//    //    [JsonProperty("internationalLogisticsId")]
//    //    public string InternationalLogisticsId { get; set; }
//    //}
//    //public class AliLogisticsSellerAddresse: AliSuccessType
//    //{
//    //    [JsonProperty("senderSellerAddressesList")]
//    //    public Model.AlibabaAeLogisticsSellerAddressDTO[] SenderSellerAddressesList { get; set; }

//    //    [JsonProperty("pickupSellerAddressesList")]
//    //    public Model.AlibabaAeLogisticsSellerAddressDTO[] PickupSellerAddressesList { get; set; }
//    //    [JsonProperty("isSuccess")]
//    //    public override bool Success { get; set; }
//    //    [JsonProperty("errorCode")]
//    //    public override string ErrorCode { get; set; }
//    //}


//    //public class AliWarehouseOrderResultType : AliSuccessType
//    //{

//    //    [JsonProperty("result")]
//    //    public WarehouseOrderResult Result { get; set; }
//    //}
//    //public class WarehouseOrderResult : AliSuccessType
//    //{
//    //    /// <summary>
//    //    /// 物流订单号
//    //    /// </summary>
//    //    [JsonProperty("warehouseOrderId")]
//    //    public long WarehouseOrderId { get; set; }
//    //    /// <summary>
//    //    /// 关联的交易订单号
//    //    /// </summary>
//    //    [JsonProperty("tradeOrderId")]
//    //    public long TradeOrderId { get; set; }

//    //    /// <summary>
//    //    /// 交易订单来源(ESCROW)
//    //    /// </summary>
//    //    [JsonProperty("tradeOrderFrom")]
//    //    public string TradeOrderFrom { get; set; }
//    //    ///// <summary>
//    //    ///// 创建是否成功(成功为true,失败为false)
//    //    ///// </summary>
//    //    //[JsonProperty("success")]
//    //    //public bool Success { get; set; }

//    //    [JsonProperty("outOrderId")]
//    //    public long OutOrderId { get; set; }

//    //    [JsonProperty("intlTrackingNo")]
//    //    public string IntlTrackingNo { get; set; }


//    //    //[JsonProperty("errorDesc")]
//    //    //public string ErrorDesc { get; set; }
//    //    ///// <summary>
//    //    ///// 创建时错误码(1表示无错误)
//    //    ///// </summary>
//    //    //[JsonProperty("errorCode")]
//    //    //public int ErrorCode { get; set; }


//    //}




//    ///// <summary>
//    ///// 申报产品信息
//    ///// </summary>
//    //public class DeclareProductDTO
//    //{
//    //    /// <summary>
//    //    /// 申报中文名称(必填,长度1-20)
//    //    /// </summary>
//    //    [JsonProperty("categoryCnDesc")]
//    //    public string CategoryCnDesc { get; set; }
//    //    /// <summary>
//    //    /// 申报英文名称(必填,长度1-60)
//    //    /// </summary>
//    //    [JsonProperty("categoryEnDesc")]
//    //    public string CategoryEnDesc { get; set; }
//    //    /// <summary>
//    //    /// 是否包含锂电池(必填0/1)
//    //    /// </summary>
//    //    [JsonProperty("isContainsBattery")]
//    //    public int IsContainsBattery { get; set; }
//    //    /// <summary>
//    //    /// 产品申报金额(必填,0.01-10000.00)
//    //    /// </summary>
//    //    [JsonProperty("productDeclareAmount")]
//    //    public decimal ProductDeclareAmount { get; set; }
//    //    /// <summary>
//    //    /// 产品ID(必填,如为礼品,则设置为0)
//    //    /// </summary>
//    //    [JsonProperty("productId")]
//    //    public long ProductId { get; set; }
//    //    /// <summary>
//    //    /// 产品件数(必填1-999)
//    //    /// </summary>
//    //    [JsonProperty("productNum")]
//    //    public int ProductNum { get; set; }
//    //    /// <summary>
//    //    /// 产品申报重量(必填0.001-2.000)
//    //    /// </summary>
//    //    [JsonProperty("productWeight")]
//    //    public int ProductWeight { get; set; }
//    //    /// <summary>
//    //    /// 为仓储发货属性代码（团购订单，仓储发货必填，物流服务为RUSTON 哈尔滨备货仓 HRB_WLB_RUSTONHEB，属性代码对应AE商品的sku属性一级，暂时没有提供接口查询属性代码，可以在仓储管理--库存管理页面查看，例如： 团购产品的sku属性White对应属性代码 40414943126）
//    //    /// </summary>
//    //    [JsonProperty("scItemId")]
//    //    public int ScItemId { get; set; }
//    //    /// <summary>
//    //    /// 属性名称（团购订单，仓储发货必填，例如：White）
//    //    /// </summary>
//    //    [JsonProperty("skuValue")]
//    //    public string SkuValue { get; set; }
//    //}
//    ///// <summary>
//    ///// 地址信息
//    ///// </summary>
//    //public class AliAddressDTO
//    //{
//    //    /// <summary>
//    //    /// 收货人地址
//    //    /// </summary>
//    //    [JsonProperty("receiver")]
//    //    public Address Receiver { get; set; }
//    //    /// <summary>
//    //    /// 发货人地址
//    //    /// </summary>
//    //    [JsonProperty("sender")]
//    //    public Address Sender { get; set; }
//    //    /// <summary>
//    //    /// 揽收地址,如果是中俄航空Ruston需要揽收的订单，则再添加揽收地址信息，key值是pickup,字段同上，内容必须是中文（如无需揽收，则不必传pickup的值）
//    //    /// </summary>
//    //    [JsonProperty("pickup")]
//    //    public Address Pickup { get; set; }
//    //}
//    ///// <summary>
//    ///// 地址信息
//    ///// </summary>
//    //public class Address
//    //{
//    //    /// <summary>
//    //    /// 旺旺（必填，长度限制1-32字节）
//    //    /// </summary>
//    //    [JsonProperty("trademanageId")]
//    //    public string trademanageId { get; set; }
//    //    /// <summary>
//    //    /// 城市,（必填，长度限制1-48，可以直接填写城市信息）
//    //    /// </summary>
//    //    [JsonProperty("city")]
//    //    public string City { get; set; }
//    //    /// <summary>
//    //    /// 国家简称
//    //    /// </summary>
//    //    [JsonProperty("country")]
//    //    public string Country { get; set; }
//    //    /// <summary>
//    //    /// 区县，（必填，长度限制1-20字节）
//    //    /// </summary>
//    //    [JsonProperty("county")]
//    //    public string County { get; set; }
//    //    /// <summary>
//    //    /// 传真
//    //    /// </summary>
//    //    [JsonProperty("fax")]
//    //    public string Fax { get; set; }
//    //    /// <summary>
//    //    /// 移动电话（长度限制1- 30字节）
//    //    /// </summary>
//    //    [JsonProperty("mobile")]
//    //    public string Mobile { get; set; }
//    //    /// <summary>
//    //    /// 姓名,（必填，长度限制1-90字节）
//    //    /// </summary>
//    //    [JsonProperty("name")]
//    //    public string Name { get; set; }
//    //    /// <summary>
//    //    /// 手机（长度限制1- 54字节）
//    //    /// </summary>
//    //    [JsonProperty("phone")]
//    //    public string Phone { get; set; }
//    //    /// <summary>
//    //    /// 邮编
//    //    /// </summary>
//    //    [JsonProperty("postcode")]
//    //    public string Postcode { get; set; }
//    //    /// <summary>
//    //    /// 省/州（必填，长度限制1-48字节）
//    //    /// </summary>
//    //    [JsonProperty("province")]
//    //    public string Province { get; set; }
//    //    /// <summary>
//    //    /// 街道 ,（必填，长度限制1-90字节）
//    //    /// </summary>
//    //    [JsonProperty("streetAddress")]
//    //    public string StreetAddress { get; set; }
//    //    /// <summary>
//    //    /// 邮箱必填（长度限制1-64字节）
//    //    /// </summary>
//    //    [JsonProperty("email")]
//    //    public string Email { get; set; }

//    //}

//    ///// <summary>
//    ///// 查询物流追踪信息
//    ///// </summary>
//    //public class AliTrackingResultType : AliSuccessType
//    //{
//    //    /// <summary>
//    //    /// 物流公司官网地址
//    //    /// </summary>
//    //    [JsonProperty("officialWebsite")]
//    //    public string OfficialWebsite { get; set; }

//    //    [JsonProperty("details")]
//    //    public TrackingDetail[] Details { get; set; }
//    //}
//    ///// <summary>
//    ///// 查询物流追踪信息
//    ///// </summary>
//    //public class TrackingDetail
//    //{
//    //    /// <summary>
//    //    /// 事件描述
//    //    /// </summary>
//    //    [JsonProperty("eventDesc")]
//    //    public string EventDesc { get; set; }
//    //    /// <summary>
//    //    /// 事件时间
//    //    /// </summary>
//    //    [JsonProperty("eventDate")]
//    //    public string EventDate { get; set; }
//    //    /// <summary>
//    //    /// 地址
//    //    /// </summary>
//    //    [JsonProperty("address")]
//    //    public string Address { get; set; }
//    //    /// <summary>
//    //    /// 当前事件状态
//    //    /// </summary>
//    //    [JsonProperty("status")]
//    //    public string Status { get; set; }
//    //    /// <summary>
//    //    /// 签收人
//    //    /// </summary>
//    //    [JsonProperty("signedName")]
//    //    public string signedName { get; set; }
//    //}


//    ///// <summary>
//    ///// 列出平台所支持的物流服务
//    ///// </summary>
//    //public class AliLogisticsServiceResultType : AliSuccessType
//    //{

//    //    [JsonProperty("result")]
//    //    //public LogisticsServiceResult[] Result { get; set; }
//    //    public Model.AeopLogisticsServiceResult[] Result { get; set; }
//    //}
//    ///// <summary>
//    ///// 列出平台所支持的物流服务
//    ///// </summary>
//    //public class LogisticsServiceResult
//    //{
//    //    /// <summary>
//    //    /// 推荐显示排序
//    //    /// </summary>
//    //    [JsonProperty("recommendOrder")]
//    //    public int RecommendOrder { get; set; }
//    //    /// <summary>
//    //    /// 物流追踪号码校验规则，采用正则表达式
//    //    /// </summary>
//    //    [JsonProperty("trackingNoRegex")]
//    //    public string TrackingNoRegex { get; set; }
//    //    /// <summary>
//    //    /// 物流公司名称
//    //    /// </summary>
//    //    [JsonProperty("logisticsCompany")]
//    //    public string LogisticsCompany { get; set; }
//    //    /// <summary>
//    //    /// 最小处理时间
//    //    /// </summary>
//    //    [JsonProperty("minProcessDay")]
//    //    public int MinProcessDay { get; set; }
//    //    /// <summary>
//    //    /// 最大处理时间
//    //    /// </summary>
//    //    [JsonProperty("maxProcessDay")]
//    //    public int MaxProcessDay { get; set; }
//    //    /// <summary>
//    //    /// 物流服务显示名称
//    //    /// </summary>
//    //    [JsonProperty("displayName")]
//    //    public string DisplayName { get; set; }
//    //    /// <summary>
//    //    /// 物流服务key（用户选择的实际发货物流服务名称）
//    //    /// </summary>
//    //    [JsonProperty("serviceName")]
//    //    public string ServiceName { get; set; }
//    //}

//    ///// <summary>
//    ///// 获取中邮小包支持的国内快递公司信息
//    ///// </summary>
//    //public class AliWlbDomesticLogisticsCompanyResultType : AliSuccessType
//    //{

//    //    [JsonProperty("result")]
//    //    public WlbDomesticLogisticsCompanyResult[] Result { get; set; }
//    //}
//    ///// <summary>
//    ///// 获取中邮小包支持的国内快递公司信息
//    ///// </summary>
//    //public class WlbDomesticLogisticsCompanyResult
//    //{
//    //    /// <summary>
//    //    /// 国内快递公司名称
//    //    /// </summary>
//    //    [JsonProperty("name")]
//    //    public string Name { get; set; }
//    //    /// <summary>
//    //    /// 国内快递公司Id
//    //    /// </summary>
//    //    [JsonProperty("companyId")]
//    //    public long CompanyId { get; set; }
//    //    /// <summary>
//    //    /// 国内快递公司Code
//    //    /// </summary>
//    //    [JsonProperty("companyCode")]
//    //    public string CompanyCode { get; set; }
//    //}


//    //public class AliOnlineLogisticsResultType : AliSuccessType
//    //{

//    //    [JsonProperty("result")]
//    //    //public OnlineLogisticsResult[] Result { get; set; }
//    //    public Model.AeopLogisticsWarehouseOrderResult[] Result { get; set; }
//    //    /// <summary>
//    //    /// 当前页面
//    //    /// </summary>
//    //    [JsonProperty("currentPage")]
//    //    public int CurrentPage { get; set; }
//    //    /// <summary>
//    //    /// 总页数
//    //    /// </summary>
//    //    [JsonProperty("totalPage")]
//    //    public int TotalPage { get; set; }
//    //}


//    //public class OnlineLogisticsResult
//    //{
//    //    /// <summary>
//    //    /// 物流订单状态（wait_warehouse_receive_goods 等待仓库收货； warehouse_reject_goods 入库失败； wait_warehouse_send_goods 等待仓库发货； send_goods_success 已发货； closed 订单关闭；
//    //    /// </summary>
//    //    [JsonProperty("logisticsStatus")]
//    //    public string LogisticsStatus { get; set; }
//    //    /// <summary>
//    //    /// 国际物流订单类型 （CPAM_WLB_FPXSZ 小包-物流宝仓库-深圳市递四方速递 FPXSZ； CPAM_WLB_ZTOBJ 小包-物流宝仓库-中通海外北京仓 ZTOBJ； CPAM_WLB_CPHSH 小包-物流宝仓库-上海市邮政 CPHSH；）
//    //    /// </summary>
//    //    [JsonProperty("internationalLogisticsType")]
//    //    public string InternationalLogisticsType { get; set; }
//    //    /// <summary>
//    //    /// 物流订单id
//    //    /// </summary>
//    //    [JsonProperty("internationallogisticsId")]
//    //    public string InternationallogisticsId { get; set; }
//    //    /// <summary>
//    //    /// 交易订单ID
//    //    /// </summary>
//    //    [JsonProperty("orderId")]
//    //    public string OrderId { get; set; }
//    //    /// <summary>
//    //    /// 物流订单ID
//    //    /// </summary>
//    //    [JsonProperty("onlineLogisticsId")]
//    //    public long OnlineLogisticsId { get; set; }
//    //}


//    ///// <summary>
//    ///// 支持的中邮小包服务信息
//    ///// </summary>
//    //public class AliOnlineLogisticsServiceListResultType : AliSuccessType
//    //{
//    //    /// <summary>
//    //    /// 支持的中邮小包服务信息
//    //    /// </summary>
//    //    [JsonProperty("result")]
//    //    //public OnlineLogisticsServiceListResult[] Result { get; set; }
//    //    public Model.AeopOnlineLogisticsServiceResult[] Result { get; set; }
//    //}
//    ///// <summary>
//    ///// 支持的中邮小包服务信息
//    ///// </summary>
//    //public class OnlineLogisticsServiceListResult
//    //{
//    //    /// <summary>
//    //    /// 物流服务
//    //    /// </summary>

//    //    [JsonProperty("logisticsServiceName")]
//    //    public string LogisticsServiceName { get; set; }
//    //    /// <summary>
//    //    /// 人民币
//    //    /// </summary>
//    //    [JsonProperty("trialResult")]
//    //    public string TrialResult { get; set; }
//    //    /// <summary>
//    //    /// 时效
//    //    /// </summary>
//    //    [JsonProperty("logisticsTimeliness")]
//    //    public string LogisticsTimeliness { get; set; }
//    //    /// <summary>
//    //    /// 物流服务编码
//    //    /// </summary>
//    //    [JsonProperty("logisticsServiceId")]
//    //    public string LogisticsServiceId { get; set; }
//    //    /// <summary>
//    //    /// 仓库地址
//    //    /// </summary>
//    //    [JsonProperty("deliveryAddress")]
//    //    public string DeliveryAddress { get; set; }
//    //    public override string ToString()
//    //    {
//    //        return this.LogisticsServiceName + " " + this.TrialResult + " " + this.DeliveryAddress;
//    //    }
//    //}
//}
