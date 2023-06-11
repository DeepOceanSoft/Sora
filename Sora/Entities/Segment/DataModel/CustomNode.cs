using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sora.Converter;
using Sora.Enumeration;
using Sora.OnebotModel.ApiParams;

namespace Sora.Entities.Segment.DataModel;

/// <summary>
/// <para>自定义转发节点</para>
/// <para>仅用于发送</para>
/// </summary>
public sealed record CustomNode
{
    /// <summary>
    /// 转发消息Id
    /// </summary>
    [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
    public string MessageId { get; internal set; }

    /// <summary>
    /// 发送者显示名字
    /// </summary>
    [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
    public string Name { get; internal set; }

    /// <summary>
    /// 发送者QQ号
    /// </summary>
    [JsonProperty(PropertyName = "uin", NullValueHandling = NullValueHandling.Ignore)]
    public string UserId { get; internal set; }

    /// <summary>
    /// 具体消息
    /// </summary>
    [JsonProperty(PropertyName = "content", NullValueHandling = NullValueHandling.Ignore)]
    internal dynamic Messages { get; set; }

    /// <summary>
    /// 转发时间
    /// </summary>
    [JsonProperty(PropertyName = "time", NullValueHandling = NullValueHandling.Ignore)]
    internal string Time { get; set; }

    /// <summary>
    /// 构造自定义节点
    /// </summary>
    /// <param name="messageId">消息ID</param>
    public CustomNode(int messageId)
    {
        MessageId = messageId.ToString();
        Name      = null;
        UserId    = null;
        Messages  = null;
        Time      = null;
    }

    /// <summary>
    /// 构造自定义节点q
    /// </summary>
    /// <param name="name">发送者名</param>
    /// <param name="userId">发送者ID</param>
    /// <param name="customMessage">消息段</param>
    /// <param name="time">消息段转发时间</param>
    public CustomNode(string name, long userId, MessageBody customMessage, DateTimeOffset? time = null)
    {
        MessageId = null;
        Name      = name;
        UserId    = userId.ToString();
        Messages = customMessage.Where(msg => msg.MessageType != SegmentType.Ignore)
                                .Select(msg => msg.ToOnebotSegment()).ToList();
        Time = (time?.ToUnixTimeSeconds() ?? DateTimeOffset.Now.ToUnixTimeSeconds()).ToString();
    }

    /// <summary>
    /// 构造自定义节点
    /// </summary>
    /// <param name="name">发送者名</param>
    /// <param name="userId">发送者ID</param>
    /// <param name="message">纯文本消息</param>
    /// <param name="time">消息段转发时间</param>
    public CustomNode(string name, long userId, string message, DateTimeOffset? time = null)
    {
        MessageId = null;
        Name      = name;
        UserId    = userId.ToString();
        Messages  = message;
        Time      = (time?.ToUnixTimeSeconds() ?? DateTimeOffset.Now.ToUnixTimeSeconds()).ToString();
    }

    /// <summary>
    /// 从CustomNode获取消息内容
    /// </summary>
    public MessageBody GetMessageBody()
    {
        return Messages switch
               {
                   string msg                  => new MessageBody { msg },
                   List<OnebotSegment> msgBody => msgBody.ToMessageBody(),
                   _                           => null
               };
    }
}