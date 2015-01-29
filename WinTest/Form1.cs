using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

namespace WinTest
{
    public partial class Form1 : Form
    {
        private MemcachedClient mc;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var config = new MemcachedClientConfiguration();
            config.Servers.Add(new IPEndPoint(IPAddress.Parse("192.168.1.116"), 11211));
            config.Protocol = MemcachedProtocol.Text;
            //config.Authentication.Type = typeof(PlainTextAuthenticator);
            //config.Authentication.Parameters["userName"] = "demo";
            //config.Authentication.Parameters["password"] = "demo";

            mc = new MemcachedClient(config);
            var flag = mc.Store(StoreMode.Set, "Hello", "World");
            MessageBox.Show(flag.ToString());

            //for (var i = 0; i < 100; i++)
            //{
            //    mc.Store(StoreMode.Set, "Hello", "World", DateTime.Now.AddSeconds(10));
            //}

            //var config = new MemcachedClientConfiguration();//创建配置参数
            //for (int i = 0; i < serverList.Count; i++)
            //{
            //    config.Servers.Add(new System.Net.IPEndPoint(IPAddress.Parse(serverList[i].Address.ToString()), serverList[i].Port));//增加服务节点
            //}
            //config.Protocol = MemcachedProtocol.Text;
            //config.Authentication.Type = typeof(PlainTextAuthenticator);//设置验证模式
            //config.Authentication.Parameters["userName"] = "uid";//用户名参数
            //config.Authentication.Parameters["password"] = "pwd";//密码参数
            //var mac = new MemcachedClient(config);//创建客户端

            //var list = new List<IPEndPoint>
            //{
            //    new IPEndPoint(IPAddress.Loopback, 11211)
            //};
            //MemberHelper.AddCache(list, "Carl", "dai");
        }

        private void button1_Click(object sender, EventArgs e)
        {

            MessageBox.Show(mc.Get("Hello").ToString());
        }
    }
}
