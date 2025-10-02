using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace API_Lens
{
    internal class UrlValidator
    {
        public static bool IsUrlValid(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            url = url.Trim();
            if (url.Contains(" "))
                return false;

            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return false;

            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch
            {
                return false;
            }

            string hostPattern = @"^([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$";
            if (!Regex.IsMatch(uri.Host, hostPattern))
                return false;

            if (!uri.IsDefaultPort && (uri.Port < 1 || uri.Port > 65535))
                return false;

            // RFC3986!!
            string path = uri.AbsolutePath;
            if (!Regex.IsMatch(path, @"^\/[\w\-._~!$&'()*+,;=:@%/]*$"))
                return false;

            string query = uri.Query;
            if (!string.IsNullOrEmpty(query))
            {
                // ?key=value&key2=value2...
                if (!Regex.IsMatch(query, @"^\?([a-zA-Z0-9\-._~!$&'()*+,;=:@%]+=[a-zA-Z0-9\-._~!$&'()*+,;=:@%]*(&[a-zA-Z0-9\-._~!$&'()*+,;=:@%]+=[a-zA-Z0-9\-._~!$&'()*+,;=:@%]*)*)?$"))
                    return false;
            }

            // Fragment
            string fragment = uri.Fragment;
            if (!string.IsNullOrEmpty(fragment))
            {
                if (!Regex.IsMatch(fragment, @"^#[\w\-._~!$&'()*+,;=:@%]*$"))
                    return false;
            }

            if (Regex.IsMatch(path + query + fragment, @"(\?\?|\=\=|\&\&|\-\-|\.\.)"))
                return false;

            if (url.Length > 2083)
                return false;

            return true;
        }
    }
}