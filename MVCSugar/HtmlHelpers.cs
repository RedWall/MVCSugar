using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString ImageFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            if (html.ViewData == null || html.ViewData.Model == null)
                return MvcHtmlString.Empty;

            var deleg = expression.Compile();
            var val = deleg(html.ViewData.Model) as byte[];

            if (val == null || !val.Any())
                return MvcHtmlString.Empty;

            var img = new TagBuilder("img");
            img.MergeAttribute("src", string.Format("data:image/png;base64,{0}", Convert.ToBase64String(val)));

            if (htmlAttributes != null)
            {
                htmlAttributes.GetType().GetProperties().ToList().ForEach(prop =>
                {
                    img.MergeAttribute(prop.Name, prop.GetValue(htmlAttributes).ToString());
                });
            }

            return MvcHtmlString.Create(img.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString EnumDisplayFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return EnumDisplayNameFor(html, expression);
        }

        public static MvcHtmlString EnumDisplayNameFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            if (html.ViewData == null || html.ViewData.Model == null)
                return MvcHtmlString.Empty;

            var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            if (EnumHelper.IsValidForEnumHelper(metaData))
            {
                var enumType = metaData.ModelType;
                var enumName = Enum.GetName(enumType, metaData.Model);

                var enumField = enumType.GetField(enumName, BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);

                if (enumField != null)
                {
                    var attr = enumField.GetCustomAttribute<DisplayAttribute>(inherit: false);

                    if (attr != null)
                    {
                        string name = attr.GetName();

                        if (!string.IsNullOrWhiteSpace(name))
                            return MvcHtmlString.Create(name);
                    }
                }
            }

            return DisplayExtensions.DisplayFor(html, expression);
        }

    }
}
