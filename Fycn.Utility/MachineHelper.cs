using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Utility
{
    public class MachineHelper
    {
        private static RedisHelper _redisHelper0;

        private static RedisHelper redisHelper0
        {
            get
            {
                if(_redisHelper0==null)
                {
                    _redisHelper0 = new RedisHelper(0);
                }
                return _redisHelper0;
            }
            set
            {
                _redisHelper0 = value;
            }
        }

        private static RedisHelper _redisHelper1;

        private static RedisHelper redisHelper1
        {
            get
            {
                if (_redisHelper1 == null)
                {
                    _redisHelper1 = new RedisHelper(1);
                }
                return _redisHelper1;
            }
            set
            {
                _redisHelper1 = value;
            }
        }
        //判断机器是否在线
        public static bool IsOnline(string machineId)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                return false;
            }
            return redisHelper0.KeyExists(machineId);
        }

        //获取机器ip
        public static string GetIp(string machineId)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                return "";
            }
            var retVal = redisHelper0.StringGet(machineId);
            if(retVal==null)
            {
                return "";
            }

            return retVal;
        }

        //签到
        public static void Signature(string machineId, string ip)
        {
            redisHelper0.StringSet(machineId, ip, new TimeSpan(0,17,2));
        }

        //生成验证码
        public static string GenerateCode(string machineId, string code)
        {
            Random ran = new Random();
            int RandKey = ran.Next(100000, 999999);
            redisHelper1.StringSet(machineId+"-"+code, RandKey.ToString(), new TimeSpan(0, 0, 20));
            return RandKey.ToString();
        }

        //判断验证码是否合法
        public static bool IsLegal(string machineId, string signCode,string code)
        {
            var val = redisHelper1.StringGet(machineId + "-" + code);
            if(val==null)
            {
                return false;
            }
            return val == signCode;
        }

        //清除验证码
        public static void ClearCode(string machineId, string code)
        {
            redisHelper1.KeyDelete(machineId + "-" + code);
        }

        //验证订单是否合法
        public static bool IsLegalOrder(string orderNum)
        {
            try{
                if (redisHelper1.KeyExists(orderNum))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch{
                return redisHelper1.KeyExists(orderNum);
            }
            
        }

        //清除缓存订单
        public static void ClearCacheOrder(string orderNum)
        {
            redisHelper1.KeyDelete(orderNum);
        }

        //缓存订单
        public static void CacheOrder(string orderNum, string content)
        {
            redisHelper1.StringSet(orderNum, content, new TimeSpan(0, 5, 1));
        }

        //验证是否存在需要下推的指令
        public static bool IsExistPush(string machineId, string key)
        {
            if (redisHelper1.KeyExists(machineId + "-" + key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //清除下推缓存
        public static void ClearCachePush(string machineId, string key)
        {
           redisHelper1.KeyDelete(machineId + "-" + key);
            
        }

        //缓存下推
        public static void CachePush(string machineId, string key,string content)
        {
            redisHelper1.StringSet(machineId + "-" + key, content);
        }
    }
}
