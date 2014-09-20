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
        public static MvcHtmlString ImageFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object htmlAttributes = null)
        {
            if (htmlHelper.ViewData == null || htmlHelper.ViewData.Model == null)
                return MvcHtmlString.Empty;

            var deleg = expression.Compile();
            var val = deleg(htmlHelper.ViewData.Model) as byte[];

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

        public static MvcHtmlString DisplayEnumNameFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression)
        {
            if (htmlHelper.ViewData == null || htmlHelper.ViewData.Model == null)
                return MvcHtmlString.Empty;

            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var enumType = metaData.ModelType;
            var enumName = Enum.GetName(enumType, metaData.Model);

            var enumField = enumType.GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public).FirstOrDefault(field => field.Name == enumName);

            if (enumField == null)
                return MvcHtmlString.Create(enumName);

            var attr = enumField.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() as DisplayAttribute;

            if (attr == null)
                return MvcHtmlString.Create(enumField.Name);

            return MvcHtmlString.Create(attr.Name);
        }

    }
}
