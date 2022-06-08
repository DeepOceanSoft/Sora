using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sora.Command;
using Sora.Entities.Base;
using Sora.Entities.Info.InternalDataInfo;
using Sora.Enumeration;
using Sora.Util;

namespace Sora.EventArgs.SoraEvent;

/// <summary>
/// 框架事件基类
/// </summary>
public abstract class BaseSoraEventArgs : System.EventArgs
{
    #region 属性

    /// <summary>
    /// 当前事件的API执行实例
    /// </summary>
    public SoraApi SoraApi { get; }

    /// <summary>
    /// 链接ID
    /// </summary>
    public Guid ConnId { get; }

    /// <summary>
    /// 服务ID
    /// </summary>
    internal Guid ServiceGuid { get; }

    /// <summary>
    /// 当前事件名
    /// </summary>
    public string EventName { get; }

    /// <summary>
    /// 事件产生时间
    /// </summary>
    public DateTime Time { get; }

    /// <summary>
    /// 接收当前事件的机器人UID
    /// </summary>
    public long LoginUid { get; }

    /// <summary>
    /// 事件产生时间戳
    /// </summary>
    internal long TimeStamp { get; set; }

    /// <summary>
    /// <para>是否在处理本次事件后再次触发其他事件，默认为触发[<see langword="true"/>]</para>
    /// <para>如:处理Command后可以将此值设置为<see langword="false"/>来阻止后续的事件触发，为<see langword="true"/>时则会触发其他相匹配的指令和事件</para>
    /// <para>如果出现了不同表达式同时被触发且优先级相同的情况，则这几个指令的执行顺序将是不确定的，请避免这种情况的发生</para>
    /// </summary>
    public bool IsContinueEventChain { get; set; }

    /// <summary>
    /// 消息来源类型
    /// </summary>
    public SourceFlag SourceType { get; }

    #endregion

    #region 构造函数

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="serviceId">当前服务ID</param>
    /// <param name="connectionId">服务器链接标识</param>
    /// <param name="eventName">事件名</param>
    /// <param name="loginUid">当前使用的QQ号</param>
    /// <param name="time">连接时间</param>
    /// <param name="sourceType">来源</param>
    internal BaseSoraEventArgs(Guid serviceId, Guid connectionId, string     eventName,
                               long loginUid,  long time,         SourceFlag sourceType)
    {
        SoraApi              = StaticVariable.ConnectionInfos[connectionId].ApiInstance;
        ServiceGuid          = serviceId;
        ConnId               = connectionId;
        EventName            = eventName;
        LoginUid             = loginUid;
        TimeStamp            = time;
        Time                 = time.ToDateTime();
        IsContinueEventChain = true;
        SourceType           = sourceType;
    }

    #endregion

    #region 连续指令

    /// <summary>
    /// <para>等待下一条消息触发正则表达式</para>
    /// <para>当所在的上下文被重复触发时则会直接返回<see langword="null"/></para>
    /// </summary>
    internal object WaitForNextRegexMessage(long            sourceUid,    string[]  commandExps, MatchType matchType,
                                            RegexOptions    regexOptions, TimeSpan? timeout,
                                            Func<ValueTask> timeoutTask,  long      sourceGroup = 0)
    {
        //生成指令上下文
        WaitingInfo waitInfo =
            CommandManager.GenerateWaitingCommandInfo(sourceUid, sourceGroup, commandExps, matchType, SourceType,
                regexOptions, ConnId, ServiceGuid);
        return WaitForNextMessage(waitInfo, timeout, timeoutTask);
    }

    /// <summary>
    /// 等待下一条消息触发自定义匹配方法
    /// </summary>
    internal object WaitForNextCustomMessage(long      sourceUid, Func<BaseMessageEventArgs, bool> matchFunc,
                                             TimeSpan? timeout,   Func<ValueTask> timeoutTask, long sourceGroup = 0)
    {
        //生成指令上下文
        WaitingInfo waitInfo =
            CommandManager.GenerateWaitingCommandInfo(
                sourceUid, sourceGroup, matchFunc, SourceType, ConnId, ServiceGuid);
        return WaitForNextMessage(waitInfo, timeout, timeoutTask);
    }

    /// <summary>
    /// <para>等待下一条消息触发</para>
    /// <para>当所在的上下文被重复触发时则会直接返回<see langword="false"/></para>
    /// </summary>
    internal object WaitForNextMessage(WaitingInfo waitInfo, TimeSpan? timeout, Func<ValueTask> timeoutTask)
    {
        //检查是否为初始指令重复触发
        if (StaticVariable.WaitingDict.Any(i => i.Value.IsSameSource(waitInfo)))
            return null;
        //连续指令不再触发后续事件
        IsContinueEventChain = false;
        Guid sessionId = Guid.NewGuid();
        //添加上下文并等待信号量
        StaticVariable.WaitingDict.TryAdd(sessionId, waitInfo);
        //是否正常接受到触发信号
        bool receiveSignal =
            //等待信号量
            StaticVariable.WaitingDict[sessionId].Semaphore.WaitOne(timeout ?? TimeSpan.FromMilliseconds(-1));
        //取出匹配指令的事件参数并删除上一次的上下文
        object retEventArgs = receiveSignal ? StaticVariable.WaitingDict[sessionId].EventArgs : null;
        StaticVariable.WaitingDict.TryRemove(sessionId, out _);
        //在超时时执行超时任务
        if (!receiveSignal && timeoutTask != null) Task.Run(timeoutTask.Invoke);
        return retEventArgs;
    }

    #endregion
}