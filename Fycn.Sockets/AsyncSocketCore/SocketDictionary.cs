using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Sockets.AsyncSocketCore
{
    public class SocketDictionary
    {
        private static Dictionary<string, AsyncSocketUserToken> _dicSocket;
        public static Dictionary<string, AsyncSocketUserToken> DicSocket
        {
            get
            {
                if(_dicSocket==null)
                {
                    _dicSocket = new Dictionary<string, AsyncSocketUserToken>();
                }
                return _dicSocket;
            }
            set
            {
                _dicSocket = value;
            }
        }

        public static void Add(string machineId, AsyncSocketUserToken userToken)
        {
            if (DicSocket.ContainsKey(machineId))
            {
                DicSocket.Remove(machineId);
                DicSocket.Add(machineId, userToken);
            }
            else
            {
                DicSocket.Add(machineId, userToken);
            }
        }

        public static void Remove(string machineId)
        {
            if(DicSocket.ContainsKey(machineId))
            {
                DicSocket.Remove(machineId);
            }
        }

        public static AsyncSocketUserToken Get(string machineId)
        {
            if (DicSocket.ContainsKey(machineId))
            {
                return  DicSocket[machineId];
            }

            return null;
        }
    }
}
