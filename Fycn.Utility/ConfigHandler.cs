using System;
using System.Collections.Generic;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Fycn.Utility;
using System.IO;

namespace Fycn.Utility
{
    public class ConfigHandler
    {

        #region Server Config

        private static string _serverUrlBasePath;

        public static string ServerUrlBasePath
        {
            get
            {
                if (String.IsNullOrEmpty(_serverUrlBasePath))
                {
                    _serverUrlBasePath = ConfigurationManager.AppSettings["ServerUrlBasePath"] ?? "http://localhost:12345/api/";
                }
                return _serverUrlBasePath;
            }
        }

        private static string _ehrFilePath;

        public static string EhrFilePath
        {
            get
            {
                if (String.IsNullOrEmpty(_ehrFilePath))
                {
                    _ehrFilePath = ConfigurationManager.AppSettings["EhrFilePath"] ?? "~/Files/EHR/";
                }
                return _ehrFilePath;
            }
        }

        private static string _ehrFileRelativePath;

        public static string EhrFileRelativePath
        {
            get
            {
                if (String.IsNullOrEmpty(_ehrFileRelativePath))
                {
                    _ehrFileRelativePath = ConfigurationManager.AppSettings["EhrFilePath"] ?? "~/Files/EHR/";
                }
                return _ehrFileRelativePath;
            }
        }

        #endregion

        #region Auth Config

        private static string _customName;

        public static string CustomName
        {
            get
            {
                if (String.IsNullOrEmpty(_customName))
                {
                    _customName = ConfigurationManager.AppSettings["CustomName"] ?? "南京中大";
                }
                return _customName;
            }
        }

        private static bool _needToken;

        public static bool NeedToken
        {
            get
            {
                if (!_needToken)
                {
                    _needToken = bool.Parse(ConfigurationManager.AppSettings["NeedToken"] ?? "false");
                }
                return _needToken;
            }
        }

        #endregion

        #region DataBaseConfig

        private static string _connectionString;

        public static string ConnectionString
        {
            get
            {
                if (String.IsNullOrEmpty(_connectionString))
                {
                    if (ConfigurationManager.AppSettings["DataBase"] != null)
                        _connectionString = ConfigurationManager.AppSettings["DataBase"];
                }
                return _connectionString.Trim();
            }
        }

        private static string _dataBaseType;

        public static string DataBaseType
        {
            get
            {
                if (String.IsNullOrEmpty(_dataBaseType))
                {
                    if (ConfigurationManager.AppSettings["DataBaseType"] != null)
                        _dataBaseType = ConfigurationManager.AppSettings["DataBaseType"];
                    else
                    {
                        _dataBaseType = "MySql";
                    }
                }
                return _dataBaseType.Trim();
            }
        }

        private static string _dataBaseProvider;

        public static string DataBaseProvider
        {
            get
            {
                if (String.IsNullOrEmpty(_dataBaseProvider))
                {
                    if (ConfigurationManager.AppSettings["DataBaseProvider"] != null)
                        _dataBaseProvider = ConfigurationManager.AppSettings["DataBaseProvider"];
                    else
                    {
                        _dataBaseProvider = "Oracle.ManagedDataAccess.Client.Oracle";
                    }
                }
                return _dataBaseProvider.Trim();
            }
        }

        private static string _dataBaseAssemblyName;

        public static string DataBaseAssemblyName
        {
            get
            {
                if (String.IsNullOrEmpty(_dataBaseAssemblyName))
                {
                    if (ConfigurationManager.AppSettings["DataBaseAssemblyName"] != null)
                        _dataBaseAssemblyName = ConfigurationManager.AppSettings["DataBaseAssemblyName"];
                    else
                    {
                        _dataBaseAssemblyName = "Oracle.ManagedDataAccess";
                    }
                }
                return _dataBaseAssemblyName.Trim();
            }
        }

        private static string _mongoDbConnString;

        public static string MongoDbConnString
        {
            get
            {
                if (String.IsNullOrEmpty(_mongoDbConnString))
                {
                    _mongoDbConnString = ConfigurationManager.AppSettings["MongoDbConnString"] ?? "mongodb://127.0.0.1:27017";
                }
                return _mongoDbConnString;
            }
        }

        private static string _mongoDataBase;

        public static string MongoDataBase
        {
            get
            {
                if (String.IsNullOrEmpty(_mongoDataBase))
                {
                    _mongoDataBase = ConfigurationManager.AppSettings["MongoDataBase"] ?? "local";
                }
                return _mongoDataBase;
            }
        }

        private static string _frequency;

        public static string Frequency
        {
            get
            {
                if (String.IsNullOrEmpty(_frequency))
                {
                    _frequency = ConfigurationManager.AppSettings["Frequency"] ?? "0";
                }
                return _frequency;
            }
        }

        #endregion

        #region Cache Config

        private static string _cacheStrategy;

        public static string CacheStrategy
        {
            get
            {
                if (String.IsNullOrEmpty(_cacheStrategy))
                {
                    if (ConfigurationManager.AppSettings["CacheStrategy"] != null)
                        _cacheStrategy = ConfigurationManager.AppSettings["CacheStrategy"];
                    else
                        _cacheStrategy = "HttpCache";
                }
                return _cacheStrategy;
            }
        }

        private static TimeSpan _timeOutSpan = new TimeSpan(0);

        public static TimeSpan TimeOutSpan
        {
            get
            {
                if (_timeOutSpan.Ticks == 0)
                {
                    if (ConfigurationManager.AppSettings["TimeOutSpan"] != null)
                        _timeOutSpan.Add(new TimeSpan(Convert.ToInt32(ConfigurationManager.AppSettings["TimeOutSpan"])));
                }
                return _timeOutSpan;
            }
        }

        private static DateTime _absoluteTime = DateTime.MaxValue;

        public static DateTime AbsoluteTime
        {
            get
            {
                if (_absoluteTime == DateTime.MaxValue)
                {
                    if (ConfigurationManager.AppSettings["AbsoluteTime"] != null)
                    {
                        var dtFI = new DateTimeFormatInfo { ShortTimePattern = "yyyyMMdd" };
                        _absoluteTime = DateTime.Parse(ConfigurationManager.AppSettings["AbsoluteTime"], dtFI);
                    }
                }
                return _absoluteTime;
            }
        }

        #endregion

        #region LogConfig

        private static string _logType;

        public static string LogType
        {
            get
            {
                if (String.IsNullOrEmpty(_logType))
                {
                    if (ConfigurationManager.AppSettings["LogType"] != null)
                        _logType = ConfigurationManager.AppSettings["LogType"];
                    else
                        _logType = "TxtFileLog";
                }
                return _logType;
            }
        }

        private static string _logFile;

        public static string LogFile
        {
            get
            {
                if (String.IsNullOrEmpty(_logFile))
                {
                    if (ConfigurationManager.AppSettings["LogFile"] != null)
                        _logFile = ConfigurationManager.AppSettings["LogFile"];
                    else
                        _logFile = "DefaultLog";
                }
                return _logFile;
            }
        }

        public static int _logLevel;

        public static int LogLevel
        {
            get
            {
                if (_logLevel != 0)
                    return _logLevel;
                _logLevel = ConfigurationManager.AppSettings["LogLevel"] != null ? int.Parse(ConfigurationManager.AppSettings["LogLevel"]) : 1;
                return _logLevel;
            }
        }

        #endregion

        #region OnlineChatConfig

        private static int _chatServerPort;

        public static int ChatServerPort
        {
            get
            {
                if (_chatServerPort != 0)
                    return _chatServerPort;
                _chatServerPort = ConfigurationManager.AppSettings["ServerPort"] != null ? int.Parse(ConfigurationManager.AppSettings["ChatServerPort"]) : 8088;
                return _chatServerPort;
            }
        }

        private static int _tcpListenPort;

        public static int TcpListenPort
        {
            get
            {
                if (_tcpListenPort != 0)
                    return _tcpListenPort;
                _tcpListenPort = ConfigurationManager.AppSettings["TcpListenPort"] != null ? int.Parse(ConfigurationManager.AppSettings["TcpListenPort"]) : 6666;
                return _tcpListenPort;
            }
        }

        #endregion

        #region Language Config

        private static string _curLanguage;

        public static string CurrentLanguage
        {
            get
            {
                if (!String.IsNullOrEmpty(_curLanguage))
                    return _curLanguage;
                _curLanguage = ConfigurationManager.AppSettings["CurLang"] ?? "zh-cn";
                return _curLanguage;
            }
        }

        #endregion

        #region Out System Interface
        private static string _resourceUrl;

        public static string ResourceUrl
        {
            get
            {
                if (String.IsNullOrEmpty(_resourceUrl))
                {
                    _resourceUrl = ConfigurationManager.AppSettings["ResourceUrl"] ?? "fy-cn.top";
                }
                return _resourceUrl;
            }
        }
        #endregion


        #region 上传地址
        private static string _uploadUrl;

        public static string UploadUrl
        {
            get
            {
                if (String.IsNullOrEmpty(_uploadUrl))
                {
                    _uploadUrl = ConfigurationManager.AppSettings["UploadAddress"] ?? Directory.GetCurrentDirectory();
                }
                return _uploadUrl;
            }
        }
        #endregion
    }
}
