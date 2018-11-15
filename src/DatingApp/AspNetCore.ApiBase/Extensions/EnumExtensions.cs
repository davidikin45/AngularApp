using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ApiBase.Extensions
{
    public static class EnumExtensions
    {
        public static string Description(this Enum value)
        {
            return
                value
                    .GetType()
                    .GetMember(value.ToString())
                    .FirstOrDefault()
                    ?.GetCustomAttribute<DisplayAttribute>()
                    ?.Name;
        }

        public static IEnumerable<SelectListItem> ToSelectList(Type t, string selectedValue = null)
        {
            var dictionary = ToDictionary(t);
            return dictionary.Select(kvp => new SelectListItem() { Text = kvp.Value, Value = kvp.Key, Selected = selectedValue != null && kvp.Key == selectedValue });
        }

        public static Dictionary<string, string> ToDictionary(Type t)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (FieldInfo field in t.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public))
            {
                string description = field.Name;
                string id = field.Name;

                foreach (DisplayAttribute displayAttribute in field.GetCustomAttributes(true).OfType<DisplayAttribute>())
                {
                    description = displayAttribute.Name;
                }

                dictionary.Add(id, description);
            }

            return dictionary;
        }

        public static IEnumerable<SelectListItem> ToSelectList<T>(string selectedValue = null)
        {
            return ToSelectList(typeof(T), selectedValue);
        }

        public static Dictionary<string, string> ToDictionary<T>()
        {
            return ToDictionary(typeof(T));
        }

    }
}
