using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace TaskBase.Infrastructure
{
    public static class EnumExtenders
    {
        public static string ToStringX(this Enum enumerate)
        {
            var type = enumerate.GetType();
            var fieldInfo = type.GetField(enumerate.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (attributes.Length != 0) ? attributes[0].Description : enumerate.ToString();
        }       
    }
}