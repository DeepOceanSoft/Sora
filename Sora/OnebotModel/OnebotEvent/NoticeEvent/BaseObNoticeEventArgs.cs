using Newtonsoft.Json;

namespace Sora.OnebotModel.OnebotEvent.NoticeEvent;

/// <summary>
/// 消息事件基类
/// </summary>
internal abstract class BaseObNoticeEventArgs : BaseObApiEventArgs
{
    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonProperty(PropertyName = "notice_type", NullValueHandling = NullValueHandling.Ignore)]
    internal string NoticeType { get; set; }

    /// <summary>
    /// 操作对象UID
    /// </summary>
    [JsonProperty(PropertyName = "user_id", NullValueHandling = NullValueHandling.Ignore)]
    internal long UserId { get; set; }
}