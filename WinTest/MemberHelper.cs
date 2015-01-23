using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

/// <summary>
/// MemberHelper 的摘要说明
/// </summary>
public class MemberHelper
{
    public MemberHelper()
    {
        //
        // TODO: 在此处添加构造函数逻辑
        //
    }

    #region 创建Memcache客户端
    /// <summary>
    /// 创建Memcache客户端
    /// </summary>
    /// <param name="serverList">服务列表</param>
    /// <returns></returns>
    private static MemcachedClient CreateServer(List<IPEndPoint> serverList)
    {
        var config = new MemcachedClientConfiguration();//创建配置参数
        for (int i = 0; i < serverList.Count; i++)
        {
            config.Servers.Add(new System.Net.IPEndPoint(IPAddress.Parse(serverList[i].Address.ToString()), serverList[i].Port));//增加服务节点
        }
        config.Protocol = MemcachedProtocol.Text;
        config.Authentication.Type = typeof(PlainTextAuthenticator);//设置验证模式
        config.Authentication.Parameters["userName"] = "uid";//用户名参数
        config.Authentication.Parameters["password"] = "pwd";//密码参数
        var mac = new MemcachedClient(config);//创建客户端
        return mac;
    }
    #endregion

    #region 添加缓存
    /// <summary>
    /// 添加缓存(键不存在则添加，存在则替换)
    /// </summary>
    /// <param name="serverList">服务器列表</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <returns></returns>
    public static bool AddCache(List<IPEndPoint> serverList, string key, object value)
    {
        using (MemcachedClient mc = CreateServer(serverList))
        {
            return mc.Store(StoreMode.Set, key, value);
        }
    }
    #endregion

    #region 添加缓存
    /// <summary>
    /// 添加缓存(键不存在则添加，存在则替换)
    /// </summary>
    /// <param name="serverList">服务器列表</param>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    /// <param name="minutes">缓存时间(分钟)</param>
    /// <returns></returns>
    public static bool AddCache(List<IPEndPoint> serverList, string key, object value, int minutes)
    {
        using (MemcachedClient mc = CreateServer(serverList))
        {
            return mc.Store(StoreMode.Set, key, value, DateTime.Now.AddMinutes(minutes));
        }
    }
    #endregion

    #region 获取缓存
    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="serverList">服务器列表</param>
    /// <param name="key">键</param>
    /// <returns>返回缓存，没有找到则返回null</returns>
    public static object GetCache(List<IPEndPoint> serverList, string key)
    {
        using (MemcachedClient mc = CreateServer(serverList))
        {
            return mc.Get(key);
        }
    }
    #endregion

    #region 是否存在该缓存
    /// <summary>
    /// 是否存在该缓存
    /// </summary>
    /// <param name="serverList">服务器列表</param>
    /// <param name="key">键</param>
    /// <returns></returns>
    public static bool IsExists(List<IPEndPoint> serverList, string key)
    {
        using (MemcachedClient mc = CreateServer(serverList))
        {
            return mc.Get(key) != null;
        }
    }
    #endregion

    #region 删除缓存(如果键不存在，则返回false)
    /// <summary>
    /// 删除缓存(如果键不存在，则返回false)
    /// </summary>
    /// <param name="serverList">服务器列表</param>
    /// <param name="key">键</param>
    /// <returns>成功:true失败:false</returns>
    public static bool DelCache(List<IPEndPoint> serverList, string key)
    {
        using (MemcachedClient mc = CreateServer(serverList))
        {
            return mc.Remove(key);
        }
    }
    #endregion

    #region 清空缓存
    /// <summary>
    /// 清空缓存
    /// </summary>
    /// <param name="serverList">服务器列表</param>
    public static void FlushCache(List<IPEndPoint> serverList)
    {
        using (MemcachedClient mc = CreateServer(serverList))
        {
            mc.FlushAll();
        }
    }
    #endregion

}