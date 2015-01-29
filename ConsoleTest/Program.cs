using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching;
using Enyim.Caching.Memcached;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var objs = ConfigurationManager.GetSection("enyim.com/memcached");
            
            MemcachedClient DistCache = new MemcachedClient(@"enyim.com/memcached");

            int runs = 100;
            int start = 200;
            if (args.Length > 1)
            {
                runs = int.Parse(args[0]);
                start = int.Parse(args[1]);
            }

            string keyBase = "testKey";
            string obj = "This is a test of an object blah blah es, serialization does not seem to slow things down so much.  The gzip compression is horrible horrible performance, so we only use it for very large objects.  I have not done any heavy benchmarking recently";

           

            //循环记时往服务器缓存上插入数据  等会我们要观察一下数据都存到哪个服务器上的Memcached server上了
            long begin = DateTime.Now.Ticks;
            for (int i = start; i < start + runs; i++)
            {
                DistCache.Store(StoreMode.Set, keyBase + i, obj);
            }
            long end = DateTime.Now.Ticks;
            long time = end - begin;

            //计算存储这些数据花了多长时间
            Console.WriteLine(runs + " sets: " + new TimeSpan(time).ToString() + "ms");

            //开始取数据，并记时
            begin = DateTime.Now.Ticks;
            int hits = 0;
            int misses = 0;
            for (int i = start; i < start + runs; i++)
            {
                string str = (string)DistCache.Get(keyBase + i);
                if (str != null)
                    ++hits;    //成功取到数据
                else
                    ++misses;  //丢失次数
            }
            end = DateTime.Now.Ticks;
            time = end - begin;

            //获取这些数据花了多长时间
            Console.WriteLine(runs + " gets: " + new TimeSpan(time).ToString() + "ms");
            Console.WriteLine("Cache hits: " + hits.ToString());
            Console.WriteLine("Cache misses: " + misses.ToString());
            Console.WriteLine("--------------------------------------------------------\r\n");

            Console.ReadLine();
        }
    }
}
