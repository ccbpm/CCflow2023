﻿
namespace BP.Port.WeiXin.Msg
{
    public class WeiXinGZHModel
    {
        public class AccessToken
        {
            /// <summary>
            /// 网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同
            /// </summary>
            public string access_token { get; set; }
            /// <summary>
            /// access_token接口调用凭证超时时间，单位（秒）
            /// </summary>
            public string expires_in { get; set; }
            /// <summary>
            /// 用户刷新access_token
            /// </summary>
            public string refresh_token { get; set; }
            /// <summary>
            /// 用户唯一标识，请注意，在未关注公众号时，用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 用户授权的作用域，使用逗号（,）分隔
            /// </summary>
            public string scope { get; set; }
            /// <summary>
            /// 返回信息
            /// </summary>
            public string errcode { get; set; }
        }
        public class GZHUser {
            /// <summary>
            /// 用户的唯一标识
            /// </summary>
            public string openid { get; set; }
            /// <summary>
            /// 用户昵称
            /// </summary>
            public string nickname { get; set; }
            /// <summary>
            /// 用户的性别，值为1时是男性，值为2时是女性，值为0时是未知
            /// </summary>
            public string sex { get; set; }
            /// <summary>
            /// 用户个人资料填写的省份
            /// </summary>
            public string province { get; set; }
            /// <summary>
            /// 普通用户个人资料填写的城市
            /// </summary>
            public string city { get; set; }
            /// <summary>
            /// 国家，如中国为CN
            /// </summary>
            public string country { get; set; }
            /// <summary>
            /// 用户头像，最后一个数值代表正方形头像大小（有0、46、64、96、132数值可选，0代表640*640正方形头像）
            /// 用户没有头像时该项为空。若用户更换头像，原有头像URL将失效
            /// </summary>
            public string headimgurl { get; set; }
            /// <summary>
            /// 用户特权信息，json 数组，如微信沃卡用户为（chinaunicom）
            /// </summary>
            public string privilege { get; set; }
            /// <summary>
            /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段。
            /// </summary>
            public string unionid { get; set; }
            /// <summary>
            /// 返回信息
            /// </summary>
            public string errcode { get; set; }
        }
        public class Ticket {
            /// <summary>
            /// 二维码ticket
            /// </summary>
            public string ticket { get; set; }
            /// <summary>
            /// 二维码有效时长
            /// </summary>
            public string expire_seconds { get; set; }
            /// <summary>
            /// 二维码查看地址
            /// </summary>
            public string url { get; set; }
        }
    }
}
