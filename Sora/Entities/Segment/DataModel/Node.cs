using System.Collections.Generic;
using Newtonsoft.Json;
using Sora.OnebotModel.ApiParams;

namespace Sora.Entities.Segment.DataModel;

/// <summary>
/// 自定义合并转发节点
/// </summary>
public sealed record Node
{
#region 属性

    /// <summary>
    /// 发送者昵称
    /// </summary>
    [JsonProperty(PropertyName = "sender")]
    public NodeSender Sender { get; internal set; }

    /// <summary>
    /// 发送事件戳
    /// </summary>
    [JsonProperty(PropertyName = "time")]
    public long Time { get; internal set; }

    /// <summary>
    /// 原始消息内容
    /// </summary>
    [JsonProperty(PropertyName = "content")]
    internal List<OnebotSegment> MessageList { get; set; }

    /// <summary>
    /// 消息来源群
    /// </summary>
    [JsonProperty(PropertyName = "group_id", NullValueHandling = NullValueHandling.Ignore)]
    public long GroupId { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    [JsonIgnore]
    public MessageBody MessageBody { get; internal set; }

#endregion

#region 发送者结构体

    /// <summary>
    /// 节点消息发送者
    /// </summary>
    public struct NodeSender
    {
        /// <summary>
        /// 发送者昵称
        /// </summary>
        [JsonProperty(PropertyName = "nickname")]
        public string Nick { get; internal set; }

        /// <summary>
        /// UID
        /// </summary>
        [JsonProperty(PropertyName = "user_id")]
        public long Uid { get; internal set; }
    }

#endregion
}