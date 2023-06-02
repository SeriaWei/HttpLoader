﻿/* http://www.zkea.net/ 
 * Copyright (c) ZKEASOFT. All rights reserved. 
 * http://www.zkea.net/licenses */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using HttpLoader.HttpParser.Helps;

namespace HttpLoader.HttpParser
{
    public partial class HttpRequestContent
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public string HttpVersion { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> Cookies { get; set; }
        public string RequestBody { get; set; }
        public static HttpRequestContent Parse(string httpDefinition)
        {
            var lines = SplitToLines(httpDefinition);
            var requestHttp = new HttpBaseInfo(lines[0]);

            var parsed = new HttpRequestContent()
            {
                Url = requestHttp.Url,
                Method = requestHttp.Method,
                HttpVersion = requestHttp.HttpVersion,
                Headers = new HttpHeadersParser(lines).Headers,
                Cookies = new HttpCookiesParser(lines).Cookies,
                RequestBody = new HttpBodyParser(lines).Body
            };
            return parsed;

        }

        public static IEnumerable<HttpRequestContent> ParseMultiple(string httpDefinition)
        {
            using var reader = new StringReader(httpDefinition);
            string line;
            StringBuilder definition = new StringBuilder();
            var requests = new List<HttpRequestContent>();
            while ((line = reader.ReadLine()) != null)
            {
                if (IsRequest(line))
                {
                    if (definition.Length > 0)
                    {
                        requests.Add(Parse(definition.ToString()));
                    }
                    definition.Clear();
                }
                definition.AppendLine(line);                
            }
            if (definition.Length > 0)
            {
                requests.Add(Parse(definition.ToString()));
            }
            return requests;
        }

        private static bool IsRequest(string line)
        {
            foreach (var item in HttpBaseInfo.HttpVerbs)
            {
                if (line.StartsWith(item + " ", StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        public override string ToString()
        {
            var sb = new StringBuilder($"{Method} {Url} {HttpVersion}{Environment.NewLine}");

            var headersToIgnore = new List<string> { "Cookie" };

            foreach (var header in Headers)
            {
                if (headersToIgnore.Contains(header.Key)) continue;
                sb.Append($"{header.Key}: {header.Value}{Environment.NewLine}");
            }

            if (Cookies?.Count > 0)
            {
                var cookies = string.Join("; ", Cookies.Select(cookie => $"{cookie.Key}={cookie.Value}"));

                sb.Append($"Cookie:{cookies}{Environment.NewLine}");
            }

            if (!string.IsNullOrEmpty(RequestBody))
            {
                sb.Append(Environment.NewLine);
                sb.Append(RequestBody);
            }

            return sb.ToString().Trim();
        }
        private static string[] SplitToLines(string raw)
        {
            return raw.Trim().Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
        }
    }
}
