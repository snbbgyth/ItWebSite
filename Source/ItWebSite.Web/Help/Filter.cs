using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ItWebSite.Web.Help
{
    public class FilterHelper
    {
        ///
        /// 需要过滤的字符（多个以|相隔）
        ///
        public static string _keyWord = "";
        ///
        /// 需要过滤的字符（多个以|相隔）
        ///
        public static string KeyWord
        {
            get { return _keyWord; }
            set { _keyWord = value; }
        }
        ///
        /// 过滤 Javascript
        ///
        /// "content">
        ///
        public static string FilterScript(string content)
        {
            string commentPattern = @"(?'comment')";
            string embeddedScriptComments =
                @"(\/\*.*?\*\/|\/\/.*?[\n\r])";
            string scriptPattern =
                String.Format(@"(?'script']*>(.*?{0}?)*]*>)",
                embeddedScriptComments);
            string pattern = String.Format(@"(?s)({0}|{1})",
                commentPattern, scriptPattern);
            return StripScriptAttributesFromTags(Regex.Replace(content,
                pattern, string.Empty, RegexOptions.IgnoreCase));
        }
        ///
        /// 过滤javascript属性值（如onclick等）
        ///
        /// "content">
        ///
        private static string StripScriptAttributesFromTags(string content)
        {
            string eventAttribs =
                @"on(blur|c(hange|lick)|dblclick|focus|keypress|" +
                "(key|mouse)(down|up)|(un)?load" +
                "|mouse(move|o(ut|ver))|reset|s(elect|ubmit))";

            string pattern = String.Format(@"(?inx)" +
                @"\<(\w+)\s+((?'attribute'(?'attributeName'{0})\s*=\s*" +
                @"(?'delim'['""]?)(?'attributeValue'[^'"">]+)(\3)" +
                @")|(?'attribute'(?'attributeName'href)\s*=\s*" +
                @"(?'delim'['""]?)(?'attributeValue'javascript[^'"">]+)" +
                @"(\3))|[^>])*\>", eventAttribs);
            Regex re = new Regex(pattern);
            // 使用MatchEvaluator的委托
            return re.Replace(content, new MatchEvaluator(StripAttributesHandler));
        }
        ///
        /// 取得属性值
        ///
        /// "m">
        ///
        private static string StripAttributesHandler(Match m)
        {
            if (m.Groups["attribute"].Success)
            {
                return m.Value.Replace(m.Groups["attribute"].Value, "");
            }
            else
            {
                return m.Value;
            }
        }
        ///
        /// 去掉javascript（scr链接方式）
        ///
        /// "content">
        ///
        public static string FilterAHrefScript(string content)
        {
            string newstr = FilterScript(content);
            string regexstr = @" href[ ^=]*= *[\s\S]*script *:";
            return Regex.Replace(newstr, regexstr, string.Empty,
                RegexOptions.IgnoreCase);
        }
        ///
        /// 去掉链接文件
        ///
        /// "content">
        ///
        public static string FilterSrc(string content)
        {
            string newstr = FilterScript(content);
            string regexstr =
                @" src *= *['""]?[^\.]+\.(js|vbs|asp|aspx|php|jsp)['""]";
            return Regex.Replace(newstr, regexstr, @"",
                RegexOptions.IgnoreCase);
        }
        ///
        /// 过滤HTML
        ///
        /// "content">
        ///
        public static string FilterHtml(string content)
        {
            string newstr = FilterScript(content);
            string regexstr = @"]*>";
            return Regex.Replace(newstr, regexstr,
                string.Empty, RegexOptions.IgnoreCase);
        }
        ///
        /// 过滤 OBJECT
        ///
        /// "content">
        ///
        public static string FilterObject(string content)
        {
            try
            {
                string regexstr = @"(?i)])*>(\w|\W)*])*>";
                return Regex.Replace(content, regexstr,
                    string.Empty, RegexOptions.IgnoreCase);
            }
            catch (Exception)
            {
                return content;
            }
           
        }
        ///
        /// 过滤iframe
        ///
        /// "content">
        ///
        public static string FilterIframe(string content)
        {
            string regexstr = @"(?i)])*>(\w|\W)*])*>";
            return Regex.Replace(content, regexstr,
                string.Empty, RegexOptions.IgnoreCase);
        }
        ///
        /// 过滤frameset
        ///
        /// "content">
        ///
        public static string FilterFrameset(string content)
        {
            string regexstr = @"(?i)])*>(\w|\W)*])*>";
            return Regex.Replace(content, regexstr,
                string.Empty, RegexOptions.IgnoreCase);
        }
        ///
        /// 移除非法或不友好字符
        ///
        /// "chkStr">
        ///
        public static string FilterBadWords(string chkStr)
        {
            //这里的非法和不友好字符由你任意加，用“|”分隔，
            //支持正则表达式,由于本Blog禁止贴非法和不友好字符，
            //所以这里无法加上。
            if (chkStr == "")
            {
                return "";
            }
            string[] bwords = _keyWord.Split('|');
            int i, j;
            string str;
            StringBuilder sb = new StringBuilder();
            for (i = 0; i < bwords.Length; i++)
            {
                str = bwords[i].ToString().Trim();
                string regStr, toStr;
                regStr = str;
                Regex r = new Regex(regStr,
                    RegexOptions.IgnoreCase | RegexOptions.Singleline |
                    RegexOptions.Multiline);
                Match m = r.Match(chkStr);
                if (m.Success)
                {
                    j = m.Value.Length;
                    sb.Insert(0, "*", j);
                    toStr = sb.ToString();
                    chkStr = Regex.Replace(chkStr, regStr,
                        toStr, RegexOptions.IgnoreCase |
                        RegexOptions.Singleline | RegexOptions.Multiline);
                }
                sb.Remove(0, sb.Length);
            }
            return chkStr;
        }
        ///
        /// 过滤以上所有
        ///
        /// "content">
        ///
        public static string FilterAll(string content)
        {
            content = FilterHtml(content);
            content = FilterScript(content);
            content = FilterAHrefScript(content);
            content = FilterObject(content);
            content = FilterIframe(content);
            content = FilterFrameset(content);
            content = FilterSrc(content);
            content = FilterBadWords(content);
            return content;
        }
    }
}