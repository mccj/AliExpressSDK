//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace AliExpressSDK
//{

//    //public class AliMessage
//    //{
//    //    /// <summary>
//    //    /// 订单url链接
//    //    /// </summary>
//    //    [JsonProperty("orderUrl")]
//    //    public string OrderUrl { get; set; }
//    //    /// <summary>
//    //    /// 创建时间
//    //    /// </summary>
//    //    [JsonProperty("gmtCreate")]
//    //    public System.DateTimeOffset? GmtCreate { get; set; }
//    //    /// <summary>
//    //    /// 接收器登录ID
//    //    /// </summary>
//    //    [JsonProperty("receiverLoginId")]
//    //    public string ReceiverLoginId { get; set; }
//    //    /// <summary>
//    //    /// 消息类型
//    //    /// </summary>
//    //    [JsonProperty("messageType")]
//    //    public string MessageType { get; set; }
//    //    /// <summary>
//    //    /// 是否有文件
//    //    /// </summary>
//    //    [JsonProperty("haveFile")]
//    //    public bool HaveFile { get; set; }
//    //    /// <summary>
//    //    /// 文件url地址
//    //    /// </summary>
//    //    [JsonProperty("fileUrl")]
//    //    public string FileUrl { get; set; }
//    //    /// <summary>
//    //    /// 产品编号
//    //    /// </summary>
//    //    [JsonProperty("productId")]
//    //    public string ProductId { get; set; }
//    //    /// <summary>
//    //    /// 内部信编号
//    //    /// </summary>
//    //    [JsonProperty("id")]
//    //    public string Id { get; set; }
//    //    /// <summary>
//    //    /// 内容
//    //    /// </summary>
//    //    [JsonProperty("content")]
//    //    public string Content { get; set; }
//    //    /// <summary>
//    //    /// 发件人名称
//    //    /// </summary>
//    //    [JsonProperty("senderName")]
//    //    public string SenderName { get; set; }
//    //    /// <summary>
//    //    /// 
//    //    /// </summary>
//    //    [JsonProperty("senderLoginId")]
//    //    public string SenderLoginId { get; set; }
//    //    /// <summary>
//    //    /// 产品地址
//    //    /// </summary>
//    //    [JsonProperty("productUrl")]
//    //    public string ProductUrl { get; set; }
//    //    /// <summary>
//    //    /// 是否读取
//    //    /// </summary>
//    //    [JsonProperty("read")]
//    //    public bool Read { get; set; }
//    //    /// <summary>
//    //    /// 收货人姓名
//    //    /// </summary>
//    //    [JsonProperty("receiverName")]
//    //    public string ReceiverName { get; set; }
//    //    /// <summary>
//    //    /// 产品名称
//    //    /// </summary>
//    //    [JsonProperty("productName")]
//    //    public string ProductName { get; set; }
//    //    /// <summary>
//    //    /// 类型编号
//    //    /// </summary>
//    //    [JsonProperty("typeId")]
//    //    public string TypeId { get; set; }
//    //    /// <summary>
//    //    /// 订单编号
//    //    /// </summary>
//    //    [JsonProperty("orderId")]
//    //    public string OrderId { get; set; }
//    //    /// <summary>
//    //    /// 关联编号
//    //    /// </summary>
//    //    [JsonProperty("relationId")]
//    //    public string RelationId { get; set; }
//    //}
//    //public class OrderMsgType
//    //{
//    //    [JsonProperty("imageFilePath")]
//    //    public string ImageFilePath { get; set; }

//    //    [JsonProperty("receiverEmail")]
//    //    public string ReceiverEmail { get; set; }

//    //    [JsonProperty("senderMemberSeq")]
//    //    public int SenderMemberSeq { get; set; }

//    //    [JsonProperty("receiverMemberSeq")]
//    //    public int ReceiverMemberSeq { get; set; }

//    //    [JsonProperty("msgSources")]
//    //    public string MsgSources { get; set; }

//    //    [JsonProperty("senderEmail")]
//    //    public string SenderEmail { get; set; }

//    //    [JsonProperty("haveFile")]
//    //    public string HaveFile { get; set; }

//    //    [JsonProperty("gmtCreate")]
//    //    public System.DateTimeOffset? GmtCreate { get; set; }

//    //    [JsonProperty("receiverLoginId")]
//    //    public string ReceiverLoginId { get; set; }

//    //    [JsonProperty("messageType")]
//    //    public string MessageType { get; set; }

//    //    [JsonProperty("id")]
//    //    public string Id { get; set; }

//    //    [JsonProperty("content")]
//    //    public string Content { get; set; }

//    //    [JsonProperty("senderName")]
//    //    public string SenderName { get; set; }

//    //    [JsonProperty("senderLoginId")]
//    //    public string SenderLoginId { get; set; }

//    //    [JsonProperty("receiverName")]
//    //    public string ReceiverName { get; set; }

//    //    [JsonProperty("typeId")]
//    //    public string TypeId { get; set; }

//    //    [JsonProperty("relationId")]
//    //    public string RelationId { get; set; }
//    //}

//    //public class AliOrderMsgList : AliSuccessType
//    //{

//    //    [JsonProperty("total")]
//    //    public int Total { get; set; }

//    //    [JsonProperty("msgList")]
//    //    public AliMessage[] MsgList { get; set; }

//    //}
//    //public class AliMessageList : AliSuccessType
//    //{

//    //    [JsonProperty("total")]
//    //    public int Total { get; set; }

//    //    [JsonProperty("msgList")]
//    //    public AliMessage[] MsgList { get; set; }

//    //}
//}
