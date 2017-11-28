using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Fycn.Utility
{
    public static class ReflectionHandler
    {
        public static void Execute(MethodInfo methodInfo, object instance, params object[] parameters)
        {
            Type typ = instance.GetType();
            MethodInfo mi1 = typ.GetMethod(methodInfo.Name);
            MethodInfo invoker = mi1.MakeGenericMethod(typ);
            //var invoker = FastReflectionCaches.MethodInvokerCache.Get(methodInfo);
       
            if (parameters == null)
            {
                parameters = new object[0];
            }
            invoker.Invoke(instance, parameters);
        }

        public static void PropertyFastSetValue(PropertyInfo pi, object instance, object value)
        {
            if (instance == null) throw new Exception("no instance!");
            pi.SetValue(instance, value);
        }
        public static object PropertyFastGetValue(PropertyInfo pi, object instance)
        {
            if (instance == null) throw new Exception("no instance!");
            return pi.GetValue(instance);
        }

        private static Dictionary<string, Type> _typeList = null;
        public static Dictionary<string, Type> GetTypeListByfolderPath(string classPath)
        {
            var assembly = Assembly.Load(classPath);
            return _typeList ?? (_typeList = assembly.GetTypes().ToDictionary(t => t.Name));
        }

        public static Object CreateInstance(string assemblyName, string typeName)
        {
            var assembly = Assembly.Load(assemblyName);
            return assembly.CreateInstance(typeName);
        }
        public static T GetClass<T>(string classPath)
        {
            var t = Type.GetType(classPath);
            if (t != null)
            {
                return (T)Activator.CreateInstance(t);
            }
            return default(T);
        }

        public static IDictionary<string, object> GetDictionaryFromObj<T>(T t)
        {
            var type = t.GetType();
            return type.GetProperties().ToDictionary(p => p.Name.ToLower(), p => p.GetValue(t, null));
        }


        #region 对象操作

        /// <summary>
        /// 用于对像间，同属性值赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TM"></typeparam>
        /// <param name="originObj"></param>
        /// <returns></returns>
        /*
        public static T GetCopyObj<T, TM>(TM originObj)
        {
            var tObj = Activator.CreateInstance<T>();
            var oPis = typeof(TM).GetProperties();
            var tPis = typeof(T).GetProperties();
            foreach (var propertyInfo in oPis)
            {
                var pName = propertyInfo.Name;
                if (!propertyInfo.GetType().IsValueType) continue;
                var tP = from t in tPis
                         where String.Equals(t.Name, pName, StringComparison.CurrentCultureIgnoreCase)
                         select t;
                
                // var tP = tPis.Find(t => String.Equals(t.Name, pName, StringComparison.CurrentCultureIgnoreCase), Predicate<T>(t));
          
                var oValue = propertyInfo.GetValue(originObj, null);
                if (tP != null)
                {
                    tP.SetValue(tObj, oValue, null);
                }
            }
            return tObj;
        }
        */
        /// <summary>
        /// 判断对象是否是值类型（包括字符串）
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsValueType(object o)
        {
            return o is string || o is DateTime || o.GetType().IsValueType;
        }

        public static string ObjName<T>(this T t)
        {
            return typeof(T).Name;
        }



        #endregion
    }
}
