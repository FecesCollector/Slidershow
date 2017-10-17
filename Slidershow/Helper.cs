using System;
using System.Text.RegularExpressions;

namespace Slidershow
{
    public static class Helper
    {
        public static string UnescapeTumblr(this string url)
        {
            int start = url.IndexOf("?z=") + 3;
            int end = url.IndexOf("&t=");

            string input = url.Substring(start, end - start);
            string decoded = Uri.UnescapeDataString(input);

            return decoded;
        }

        public static string DecodeHtml(this string url)
        {
            return Regex.Replace(url, "<.*?>", string.Empty);
        }
    }
}
