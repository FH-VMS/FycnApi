using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Utility
{
    public class MachineHelper
    {
        //判断机器是否在线
        public static bool IsOnline(string machineId)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                return false;
            }
            RedisHelper redisHelper = new RedisHelper(0);
            return redisHelper.KeyExists(machineId);
        }

        //获取机器ip
        public static string GetIp(string machineId)
        {
            if (string.IsNullOrEmpty(machineId))
            {
                return "";
            }
            RedisHelper redisHelper = new RedisHelper(0);
            if (redisHelper.KeyExists(machineId))
            {
                return redisHelper.StringGet(machineId);
            }
            else
            {
                return "";
            }
        }

        //签到
        public static void Signature(string machineId, string ip)
        {
            RedisHelper redisHelper = new RedisHelper(0);
            redisHelper.StringSet(machineId, ip, new TimeSpan(0,15,2));
        }

        //生成签到码
        public static string GenerateSignCode(string machineId)
        {
            RedisHelper redisHelper = new RedisHelper(1);
            Random ran = new Random();
            int RandKey = ran.Next(100000, 999999);
            redisHelper.StringSet(machineId+"-code", RandKey.ToString(), new TimeSpan(0, 0, 20));
            return RandKey.ToString();
        }

        //判断签到是否合法
        public static bool IsLegalSign(string machineId, string signCode)
        {
            RedisHelper redis4A = new RedisHelper(1);
            if (redis4A.KeyExists(machineId+"-code"))
            {
                return redis4A.StringGet(machineId + "-code") == signCode;
            }
            else
            {
                return false;
            }
        }

        //清除机器码
        public static void ClearSignCode(string machineId)
        {
            RedisHelper redisHelper = new RedisHelper(1);
            if (redisHelper.KeyExists(machineId + "-code"))
            {
                redisHelper.KeyDelete(machineId + "-code");
            }
        }

        //验证订单是否合法
        public static bool IsLegalOrder(string orderNum)
        {
            RedisHelper redis4A = new RedisHelper(1);
            if (redis4A.KeyExists(orderNum))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
