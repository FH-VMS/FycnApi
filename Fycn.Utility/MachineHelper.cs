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

        //签到
        public static void Signature(string machineId, string ip)
        {
            RedisHelper redisHelper = new RedisHelper(0);
            redisHelper.StringSet(machineId, ip, new TimeSpan(0,15,2));
        }
    }
}
