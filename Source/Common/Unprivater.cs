using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
namespace rjw.Source.Common
{

    public static class Unprivater
    {
        internal const BindingFlags flags = BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        /// <summary>
        /// T is for returned type. For instance-class fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetProtectedValue<T>(string fieldName, object obj)
        {
            var fieldinfo = obj.GetType().GetField(fieldName, flags);
            var readData = fieldinfo.GetValue(obj);
            if (readData is T)
            {
                return (T)readData;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(readData, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
        }


        /// <summary>
        /// T is for returned type. For instance-class properties ( [ get; thingies ] )
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetProtectedProperty<T>(string propertyName, object obj)
        {
            var propinfo = obj.GetType().GetProperty(propertyName, flags);
            
            var getter = propinfo.GetGetMethod(nonPublic: true);
            var readData = getter.Invoke(obj, null);
            if (readData is T)
            {
                return (T)readData;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(readData, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// T is for returned type. For static-class fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldName"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T GetProtectedValue<T>(string fieldName, Type t)
        {
            var fieldinfo = t.GetField(fieldName, flags);
            var readData = fieldinfo.GetValue(null);
            if (readData is T)
            {
                return (T)readData;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(readData, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
        }


        // needs testing, dunno if it works for statics
        public static T GetProtectedProperty<T>(string propertyName, Type t)
        {
            var propinfo = t.GetProperty(propertyName, flags);
            
            var getter = propinfo.GetGetMethod(nonPublic: true);
            var readData = getter.Invoke(null, null);
            if (readData is T)
            {
                return (T)readData;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(readData, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
        }
        
    }

}
