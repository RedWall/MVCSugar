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
        /// <summary>
        /// Generates a base64 encoded inline image from a byte[]
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML Helper</param>
        /// <param name="expression">The expression that returns the byte[] property from your model</param>
        /// <param name="htmlAttributes">Additional HTML attributes to include in the rendered img tag</param>
        /// <returns>img tag with base64 encoded inline image</returns>
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

        /// <summary>
        /// Generates string from the display name for the enum or the enum name if the display attribute does not exist
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="html">The HTML Helper</param>
        /// <param name="enumValue">The enum value</param>
        /// <returns>Display name for enum or enum name if display attribute does not exist</returns>
        public static MvcHtmlString EnumDisplay<TEnum>(this HtmlHelper html, TEnum enumValue) where TEnum : struct
        {
            var enumType = typeof(TEnum);

            var name = GetDisplayValue(enumType, enumValue);

            if (!string.IsNullOrWhiteSpace(name))
                return MvcHtmlString.Create(name);

            return MvcHtmlString.Create(enumValue.ToString());
        }

        /// <summary>
        /// Generates string from the display name for the enum or the the enum name if the display attribute does not exist
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML Helpter</param>
        /// <param name="expression">The expression that returns the enum property from your model</param>
        /// <returns>Display name for enum or enum name if display attribute does not exist</returns>
        public static MvcHtmlString EnumDisplayFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return EnumDisplayNameFor(html, expression);
        }

        /// <summary>
        /// Generates string from the display name for the enum or the the enum name if the display attribute does not exist
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML Helpter</param>
        /// <param name="expression">The expression that returns the enum property from your model</param>
        /// <returns>Display name for enum or enum name if display attribute does not exist</returns>
        public static MvcHtmlString EnumDisplayNameFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            if (html.ViewData == null || html.ViewData.Model == null)
                return MvcHtmlString.Empty;

            var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            var enumType = metaData.ModelType;

            var name = GetDisplayValue(enumType, metaData.Model);

            if (!string.IsNullOrWhiteSpace(name))
                return MvcHtmlString.Create(name);

            return DisplayExtensions.DisplayFor(html, expression);
        }

        private static string GetDisplayValue(Type enumType, object enumValue)
        {
            if (EnumHelper.IsValidForEnumHelper(enumType))
            {
                var enumName = Enum.GetName(enumType, enumValue);

                var enumField = enumType.GetField(enumName, BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public);

                if (enumField != null)
                {
                    var attr = enumField.GetCustomAttribute<DisplayAttribute>(inherit: false);

                    if (attr != null)
                    {
                        string name = attr.GetName();

                        if (!string.IsNullOrWhiteSpace(name))
                            return name;
                    }
                }
            }

            return null;
        }
    }
}
