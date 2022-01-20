using Newtonsoft.Json;

namespace Sora.OnebotModel.OnebotEvent.MetaEvent;

/// <summary>
/// 生命周期事件
/// </summary>
internal sealed class OnebotLifeCycleEventArgs : BaseObMetaEventArgs
{
    /// <summary>
    /// <para>事件子类型</para>
    /// <para>当前版本只可能为<see langword="connect"/></para>
    /// </summary>
    [JsonProperty(PropertyName = "sub_type")]
    internal string SubType { get; set; }
}