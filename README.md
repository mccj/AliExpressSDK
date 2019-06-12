# AliExpressSDK
---
           由于速卖通api已经更新至 https://developers.aliexpress.com/doc.htm?docId=107343&docType=1
           当前sdk已经不适应
---


阿里速卖通开放平台SDK C#版本
http://open.aliexpress.com/


## 使用例子
```C#
           var client = new SDK.Platform.AliExpressApi.AliExpressClient("appKey", "appSecret", "accessToken");
           var order = client.FindOrderById(11111);
```
## 扩展重写
```C#
       public class CustomAliExpressClient : SDK.Platform.AliExpressApi.AliExpressClient
       {
           public CustomAliExpressClient(string appKey, string appSecret, string accessToken = null, bool _throw = true)
               : base(appKey, appSecret, accessToken, _throw)
           {
               //this.StrClient
           }
           //重写 或 自定义方法，属性
           public override ApiFindOrderByIdResponse FindOrderById(long orderId)
           {
               return base.FindOrderById(orderId);

               //解析方法1，使用SDK现有的解析方法
               //SDK解析结果正确时，推荐使用
               var model = this.ModelClient.FindOrderById(orderId);
               return model;

               //解析方法2，使用SDK现有的 json 获取，自定义对象解析
               // SDK 获取 json 结果正确，但是解析对象错误，或不能满足需求时，推荐使用
               var json = this.StrClient.api_findOrderById(orderId);
               var customModel = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiFindOrderByIdResponse>(json);
               return customModel;

               //解析方法2，使用SDK现有的签名及请求，其他自定义
               var dic = new Dictionary<string, object>();
               dic.Add(SDK.Platform.AliExpressApi.AliExpressClient.fieldAccessToken, this.AccessToken);//用户授权令牌
               dic.Add("orderId", orderId);
               dic.Add("fieldList", null);
               dic.Add("extInfoBitFlag", null);
               var json2 = this.PostWebRequest(SDK.Platform.AliExpressApi.AliExpressClient.openapiIP, this.AppKey, SDK.Platform.AliExpressApi.AliExpressClient.Url + "api.findOrderById", isFile: false, stream: null, paramDic: dic, paramIsSign: true, paramIscon: true);
               var customModel2 = Newtonsoft.Json.JsonConvert.DeserializeObject<ApiFindOrderByIdResponse>(json2);
               return customModel2;

               //使用 System.Net.Http.HttpClient 获取数据
               var str1 = this.PostAsync<string, ApiFindOrderByIdResponse>("http://www.qq.com", "test");
           }
       }
```
