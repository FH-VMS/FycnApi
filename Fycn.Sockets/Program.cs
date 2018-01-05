using Fycn.Sockets.AsyncSocketCore;
using Fycn.Utility;
using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace Fycn.Sockets
{
    class Program
    {
        public static ILog Logger;
        public static AsyncSocketServer AsyncSocketSvr;
        public static string FileDirectory;
        static void Main(string[] args)
        {
            try
            {
                DateTime currentTime = DateTime.Now;
                //log4net.GlobalContext.Properties["LogDir"] = currentTime.ToString("yyyyMM");
                //log4net.GlobalContext.Properties["LogFileName"] = "_SocketAsyncServer" + currentTime.ToString("yyyyMMdd");
                ILoggerRepository repository = LogManager.CreateRepository("Fycn.Sockets");
                XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
                Logger = log4net.LogManager.GetLogger(repository.Name, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                FileDirectory = ConfigurationManager.AppSettings["FileDirectory"];
                if (FileDirectory == "")
                    FileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                if (!Directory.Exists(FileDirectory))
                    Directory.CreateDirectory(FileDirectory);
                int port = 0;
                if (!(int.TryParse(ConfigurationManager.AppSettings["Port"], out port)))
                    port = 9999;
                int parallelNum = 0;
                if (!(int.TryParse(ConfigurationManager.AppSettings["ParallelNum"], out parallelNum)))
                    parallelNum = 8000;
                int socketTimeOutMS = 0;
                if (!(int.TryParse(ConfigurationManager.AppSettings["SocketTimeOutMS"], out socketTimeOutMS)))
                    socketTimeOutMS = 5 * 60 * 1000;

                AsyncSocketSvr = new AsyncSocketServer(parallelNum);
                AsyncSocketSvr.SocketTimeOutMS = socketTimeOutMS;
                AsyncSocketSvr.Init();

                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                string ipV4 = "0.0.0.0";
                /*
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipV4 = IpEntry.AddressList[i].ToString();
                    }
                }
                */
                IPEndPoint listenPoint = new IPEndPoint(IPAddress.Parse(ipV4), port);
                AsyncSocketSvr.Start(listenPoint);
                InitTimer(socketTimeOutMS);
                Console.WriteLine("Press any key to terminate the server process....");
                Console.Read();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void InitTimer(int socketTimeOutMS)
        {
            Timer t = new Timer(socketTimeOutMS);//实例化Timer类，单位毫秒；
            t.Elapsed += new ElapsedEventHandler(TimeOut);//到达时间的时候执行事件；
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；

           
        }

        private static void TimeOut(object source, ElapsedEventArgs e)
        {
           AsyncSocketUserToken[] userTokenArray = null;
            AsyncSocketSvr.AsyncSocketUserTokenList.CopyList(ref userTokenArray);
            for (int i = 0; i < userTokenArray.Length; i++)
            {
                //Program.Logger.InfoFormat("clear machine id is {0}", userTokenArray[i].MachineId);
                
                if (MachineHelper.IsOnline(userTokenArray[i].MachineId))
                {
                    break;
                }
                   
                try
                {
                    lock (userTokenArray[i])
                    {
                        //清除缓存字典
                        SocketDictionary.Remove(userTokenArray[i].MachineId);
                        AsyncSocketSvr.CloseClientSocket(userTokenArray[i]);
                        Program.Logger.InfoFormat("clear ip is {0}", userTokenArray[i].ConnectSocket.RemoteEndPoint.ToString());
                    }
                }
                catch (Exception E)
                {
                    //Program.Logger.ErrorFormat("Daemon thread check timeout socket error, message: {0}", E.Message);
                    //Program.Logger.Error(E.StackTrace);
                }
            }

        }
    }
}
