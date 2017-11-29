using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TechSense
{
    public enum AccessLevel
    {
        Read = 1,
        Full = 2
    }
    public enum Priority
    {
        High = 1,
        Medium,
        Low
    }

    public enum Status
    {
        [EnumOrder(Order = 1)]
        Assess = 1,
        [EnumOrder(Order = 2)]
        Trial,
        [EnumOrder(Order = 3)]
        Adopt,
        [EnumOrder(Order = 5)]
        Depreciate,
        [EnumOrder(Order = 4)]
        Continue
    }

    public enum Visibility
    {
        False = 0,
        True
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumOrderAttribute : Attribute
    {
        public int Order { get; set; }
    }

    public static class EnumExtenstions
    {
        public static IEnumerable<SelectListItem> GetWithOrder(this Enum enumVal)
        {
            return enumVal.GetType().GetWithOrder();
        }

        public static IEnumerable<SelectListItem> GetWithOrder(this Type type)
        {
            //if (!type.IsEnum)
            //{
            //    throw new ArgumentException("Type must be an enum");
            //}
            // caching for result could be useful
            return type.GetFields()
                                   .Where(field => field.IsStatic)
                                   .Select(field => new
                                   {
                                       field,
                                       attribute = field.GetCustomAttribute<EnumOrderAttribute>()
                                   })
                                    .Select(fieldInfo => new
                                    {
                                        name = fieldInfo.field.Name,
                                        value = (int)Enum.Parse(typeof(Status), fieldInfo.field.Name, true),
                                        order = fieldInfo.attribute != null ? fieldInfo.attribute.Order : 0
                                    })
                                   .OrderBy(field => field.order)
                                   .Select(field => new SelectListItem() { Text = field.name, Value = field.value.ToString() } );
        }
    }
}
