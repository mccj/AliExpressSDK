using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDK.Platform.AliExpressApi.Model
{
    public class NewpostAeProduct
    {
        /// <summary>
        ///(必须) Detail详情
        /// </summary>
        public string detail { get; set; }
        /// <summary>
        ///(必须) 列表类型
        /// </summary>
        public NewAeopAeProductSKUs[] aeopAeProductSKUs { get; set; }
        /// <summary>
        /// 备货期。取值范围:1-60;单位:天
        /// </summary>
        public int deliveryTime { get; set; }
        /// <summary>
        /// 服务模板设置。（需和服务模板查询接口api.queryPromiseTemplateById进行关联使用
        /// </summary>
        public long promiseTemplateId { get; set; }
        /// <summary>
        /// (必须)商品所属类目ID。必须是叶子类目，通过类目接口获取。
        /// </summary>
        public int categoryId { get; set; }
        /// <summary>
        /// (必须)商品标题 长度在1-128之间英文。
        /// </summary>
        public string subject { get; set; }
        /// <summary>
        /// 商品一口价。取值范围:0-100000,保留两位小数;单位:美元。如:200.07，
        /// 上传多属性产品的时候，有好几个SKU和价格，productprice无需填写。
        /// </summary>
        public string productPrice { get; set; }
        /// <summary>
        /// 运费模版ID。通过运费接口listFreightTemplate获取。
        /// </summary>
        public int freightTemplateId { get; set; }
        /// <summary>
        /// 产品的主图URL列表。如果这个产品有多张主图，那么这些URL之间使用英文分号(";")隔开。
        /// 一个产品最多只能有6张主图。图片格式JPEG，文件大小5M以内；图片像素建议大于800*800
        /// </summary>
        public string imageURLs { get; set; }
        /// <summary>
        /// (必须)商品单位 (存储单位编号)
        /// </summary>
        public int productUnit { get; set; }
        /// <summary>
        /// 打包销售: true 非打包销售:false
        /// </summary>
        public bool packageType { get; set; }
        /// <summary>
        /// 每包件数。 打包销售情况，lotNum>1,非打包销售情况,lotNum=1
        /// </summary>
        public int lotNum { get; set; }
        /// <summary>
        /// 商品包装长度。取值范围:1-700,单位:厘米。产品包装尺寸的最大值+2×（第二大值+第三大值）不能超过2700厘米
        /// </summary>
        public int packageLength { get; set; }
        /// <summary>
        /// 商品包装宽度。取值范围:1-700,单位:厘米。        /// </summary>
        public int packageWidth { get; set; }
        /// <summary>
        /// 商品包装高度。取值范围:1-700,单位:厘米。
        /// </summary>
        public int packageHeight { get; set; }
        /// <summary>
        /// 商品毛重,取值范围:0.001-500.000,保留三位小数,采用进位制,单位:公斤。
        /// </summary>
        public string grossWeight { get; set; }
        /// <summary>
        /// 是否自定义计重.true为自定义计重,false反之.
        /// </summary>
        public bool isPackSell { get; set; }
        /// <summary>
        /// isPackSell为true时,此项必填。购买几件以内不增加运费。取值范围1-1000
        /// </summary>
        public int baseUnit { get; set; }
        /// <summary>
        /// isPackSell为true时,此项必填。 每增加件数.取值范围1-1000。
        /// </summary>
        public int addUnit { get; set; }
        /// <summary>
        /// 	isPackSell为true时,此项必填。 对应增加的重量.取值范围:0.001-500.000,保留三位小数,采用进位制,单位:公斤。
        /// </summary>
        public string addWeight { get; set; }
        /// <summary>
        /// 商品有效天数。取值范围:1-30,单位:天。
        /// </summary>
        public int wsValidNum { get; set; }
        /// <summary>
        /// （必须）产品属性，以json格式进行封装后提交
        /// </summary>
        public NewAeopAeProductPropertys[] aeopAeProductPropertys { get; set; }
        /// <summary>
        /// 批发最小数量 。取值范围2-100000。批发最小数量和批发折扣需同时有值或无值。
        /// </summary>
        public int bulkOrder { get; set; }
        /// <summary>
        /// 批发折扣。扩大100倍，存整数。取值范围:1-99。注意：这是折扣，不是打折率。 
        /// 如,打68折,则存32。批发最小数量和批发折扣需同时有值或无值。
        /// </summary>
        public int bulkDiscount { get; set; }
        /// <summary>
        /// 	尺码表模版ID。必须选择当前类目下的尺码模版
        /// </summary>
        public long sizechartId { get; set; }
        /// <summary>
        /// 库存扣减策略，总共有2种：下单减库存(place_order_withhold)和支付减库存(payment_success_deduct)。
        /// </summary>
        public string reduceStrategy { get; set; }
        /// <summary>
        /// 这个产品需要关联的产品分组ID. 只能关联一个产品分组，如果想关联多个产品分组，请使用api.setGroups接口。
        /// </summary>
        public long groupId { get; set; }
        /// <summary>
        /// 货币单位。如果不提供该值信息，则默认为"USD"；非俄罗斯卖家这个属性值可以不提供。
        /// 对于俄罗斯海外卖家，该单位值必须提供，如: "RUB"
        /// </summary>
        public string currencyCode { get; set; }
        /// <summary>
        /// mobile Detail详情。以下内容会被过滤，但不影响产品提交:
        /// </summary>
        public string mobileDetail { get; set; }
        /// <summary>
        /// 卡券商品开始有效期
        /// </summary>

        public DateTimeOffset couponStartDate { get; set; }
        /// <summary>
        /// 卡券商品结束有效期
        /// </summary>
        public DateTimeOffset couponEndDate { get; set; }

        public string productId { get; set; }
        public bool success { get; set; }



    }
}
