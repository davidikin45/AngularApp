using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetCore.ApiBase.Extensions
{
    public static class FormExtensions
    {

        public static MvcForm BeginForm<TController>(this IHtmlHelper helper, Expression<Action<TController>> action) where TController : Controller
        {
            return BeginForm(helper, action, FormMethod.Post, null);
        }

        public static MvcForm BeginForm<TController>(this IHtmlHelper helper, Expression<Action<TController>> action, FormMethod method) where TController : Controller
        {
            return BeginForm(helper, action, method, null);
        }

        public static MvcForm BeginForm<TController>(this IHtmlHelper helper, Expression<Action<TController>> action, FormMethod method, object htmlAttributes) where TController : Controller
        {
            return BeginForm(helper, action, method, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static ExpandoObject ToExpandoObject(this IFormCollection collection)
        {
            ExpandoObject expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;

            foreach (var item in collection.Keys.ToDictionary(key => key, value => collection[value]))
            {
                dictionary.Add(item.Key, item.Value);
            }

            return expando;
        }
    }
}
