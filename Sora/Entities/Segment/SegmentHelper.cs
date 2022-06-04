﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sora.Enumeration;

namespace Sora.Entities.Segment;

/// <summary>
/// 消息段扩展
/// </summary>
public static class SegmentHelper
{
    #region 扩展构建方法

    /// <summary>
    /// 生成AT 消息段
    /// </summary>
    /// <param name="uid">uid</param>
    public static SoraSegment ToAt(this long uid)
    {
        return SoraSegment.At(uid);
    }

    /// <summary>
    /// 生成AT 消息段
    /// </summary>
    /// <param name="uid">uid</param>
    public static SoraSegment ToAt(this int uid)
    {
        return SoraSegment.At(uid);
    }

    /// <summary>
    /// 转换方法
    /// </summary>
    public static MessageBody ToMessageBody(this IEnumerable<SoraSegment> message)
    {
        return new MessageBody(message.ToList());
    }

    #endregion

    #region 消息字符串处理

    /// <summary>
    /// 处理传入数据
    /// </summary>
    /// <param name="dataStr">数据字符串</param>
    /// <returns>
    /// <para><see langword="retStr"/>处理后数据字符串</para>
    /// <para><see langword="isMatch"/>是否为合法数据字符串</para>
    /// </returns>
    internal static (string retStr, bool isMatch) ParseDataStr(string dataStr)
    {
        if (string.IsNullOrEmpty(dataStr)) return (null, false);
        dataStr = dataStr.Replace('\\', '/');
        //当字符串太长时跳过正则检查
        if (dataStr.Length > 1000) return (dataStr, true);

        FileType type = StaticVariable.FileRegices.Single(i => i.Value.IsMatch(dataStr)).Key;

        switch (type)
        {
            case FileType.UnixFile: //linux/osx
                if (Environment.OSVersion.Platform != PlatformID.Unix   &&
                    Environment.OSVersion.Platform != PlatformID.MacOSX &&
                    !File.Exists(dataStr))
                    return (dataStr, false);
                else
                    return ($"file:///{dataStr}", true);
            case FileType.WinFile: //win
                if (Environment.OSVersion.Platform == PlatformID.Win32NT && File.Exists(dataStr))
                    return ($"file:///{dataStr}", true);
                else
                    return (dataStr, false);
            default:
                return (dataStr, true);
        }
    }

    /// <summary>
    /// 图片流转Base64字符串
    /// </summary>
    /// <param name="stream">图片流</param>
    public static string StreamToBase64(this Stream stream)
    {
        if (stream.Length > int.MaxValue) throw new ArgumentOutOfRangeException(nameof(stream), "流长度超出范围限制");
        byte[] arr = new byte[stream.Length];
        stream.Position = 0;
        stream.Read(arr, 0, (int)stream.Length);
        return "base64://" + Convert.ToBase64String(arr);
    }

    #endregion
}