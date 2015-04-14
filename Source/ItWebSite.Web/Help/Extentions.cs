using System;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace ItWebSite.Web.Help
{
    public static class Extentions
    {
        public static string ToSummary(this string content, int count)
        {
            if (string.IsNullOrEmpty(content) || content.Trim().Length < count)
                return content;
            return content.Trim().Substring(0, count) + "...";
        }

       

        public static string StripTagsRegex(this string source)
        {
            if (string.IsNullOrEmpty(source)) return string.Empty;
            //return Regex.Replace(source, "<.*?>", string.Empty);

            string stroutput = source;
            //Regex regex = new Regex(@"<.*?>");


            stroutput = StripTagsCharArray(stroutput);

            var scriptRegex = new Regex(@"(?s)<\s?script.*?(/\s?>|<\s?/\s?script\s?>)");
            stroutput = scriptRegex.Replace(stroutput, "");
            //stroutput = FilterScript(stroutput);
            return stroutput;

        }

      
        /// <summary>
        /// Compiled regular expression for performance.
        /// </summary>
        static Regex _htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);

        /// <summary>
        /// Remove HTML from string with compiled Regex.
        /// </summary>
        public static string StripTagsRegexCompiled(this string source)
        {
            return _htmlRegex.Replace(source, string.Empty);
        }

        /// <summary>
        /// Remove HTML tags from string using char array.
        /// </summary>
        public static string StripTagsCharArray(this  string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;
            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static string FieldIdFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            var id = html.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            // because "[" and "]" aren't replaced with "_" in GetFullHtmlFieldId
            return id.Replace('[', '_').Replace(']', '_');
        }

        public static string FieldNameFor<T>(this HtmlHelper<T> html, string filedName)
        {
            var id = html.ViewData.TemplateInfo.GetFullHtmlFieldName(filedName);
            // because "[" and "]" aren't replaced with "_" in GetFullHtmlFieldId
            return id.Replace('[', '_').Replace(']', '_');
        }

        public static int GenerateRandomInteger(int min = 0, int max = int.MaxValue)
        {
            var randomNumberBuffer = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(randomNumberBuffer);
            return new Random(BitConverter.ToInt32(randomNumberBuffer, 0)).Next(min, max);
        }

    }
}