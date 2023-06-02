using HttpLoader.HttpParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpLoader.RequestClient
{
    public class HttpRequesetSender
    {
        private readonly HttpClient _httpClient;
        private readonly HttpRequestContent _request;
        public HttpRequesetSender(HttpRequestContent httpRequest)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/113.0.0.0 Safari/537.36 Edg/113.0.1774.42");
            _request = httpRequest;
        }
        public async Task<HttpResponseMessage> SendAsync()
        {
            return await _httpClient.SendAsync(_request.ConvertToHttpRequestMessage());
        }
        public string Action { get { return _request.Method + " " + _request.Url; } }
    }
}
