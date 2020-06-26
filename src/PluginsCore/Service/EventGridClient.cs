using Microsoft.Crm.Sdk.Messages;
using PluginsCore.Comun;
using PluginsCore.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace PluginsCore.Service
{
    public partial class EventGridClient
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        internal string BaseUri { get; set; }

        /// <summary>
        /// Credentials needed for the client to connect to Azure.
        /// </summary>
        public string Credentials { get; private set; }

        /// <summary>
        /// Version of the API to be used with the client request.
        /// </summary>
        public string ApiVersion { get; private set; }

        /// <summary>
        /// The preferred language for the response.
        /// </summary>
        public string AcceptLanguage { get; set; }

        /// <summary>
        /// The retry timeout in seconds for Long Running Operations. Default value is
        /// 30.
        /// </summary>
        public int? LongRunningOperationRetryTimeout { get; set; }

        /// <summary>
        /// Whether a unique x-ms-client-request-id should be generated. When set to
        /// true a unique x-ms-client-request-id value is generated and included in
        /// each request. Default is true.
        /// </summary>
        public bool? GenerateClientRequestId { get; set; }


        public EventGridClient(string credentials)
        {
            if (credentials == null)
            {
                throw new System.ArgumentNullException("credentials");
            }
            Credentials = credentials;
            Initialize();
        }


        /// <summary>
        /// Initializes client properties.
        /// </summary>
        private void Initialize()
        {
            BaseUri = "https://{topicHostname}";
            ApiVersion = "2018-01-01";
            AcceptLanguage = "en-US";
            LongRunningOperationRetryTimeout = 30;
            GenerateClientRequestId = true;
        }


        public async Task<HttpResponseMessage> PublishEvents(string topicHostname, IList<EventGridEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (topicHostname == null)
            {
                throw new InvalidOperationException("Informe topicHostname");
            }
            if (events == null)
            {
                throw new InvalidOperationException("informe parâmetro events");
            }

            var _baseUrl = BaseUri;
            var _url = _baseUrl + (_baseUrl.EndsWith("/") ? "" : "/") + "api/events";
            _url = _url.Replace("{topicHostname}", topicHostname);
            List<string> _queryParameters = new List<string>();
            _queryParameters.Add(string.Format("api-version={0}", System.Uri.EscapeDataString(ApiVersion)));

            if (_queryParameters.Count > 0)
            {
                _url += (_url.Contains("?") ? "&" : "?") + string.Join("&", _queryParameters);
            }
            // Create HTTP transport objects
            var _httpRequest = new HttpRequestMessage();
            _httpRequest.Method = new HttpMethod("POST");
            _httpRequest.RequestUri = new System.Uri(_url);
            _httpRequest.Headers.TryAddWithoutValidation("aeg-sas-key", Credentials);
            // Set Headers
            if (GenerateClientRequestId != null && GenerateClientRequestId.Value)
            {
                _httpRequest.Headers.TryAddWithoutValidation("x-ms-client-request-id", System.Guid.NewGuid().ToString());
            }
            if (AcceptLanguage != null)
            {
                if (_httpRequest.Headers.Contains("accept-language"))
                {
                    _httpRequest.Headers.Remove("accept-language");
                }
                _httpRequest.Headers.TryAddWithoutValidation("accept-language", AcceptLanguage);
            }


            // Serialize Request
            string _requestContent = null;
            if (events != null)
            {
                var serialized = JSONSerializer<IList<EventGridEvent>>.Serialize(events);
                _requestContent = serialized;
                _httpRequest.Content = new StringContent(_requestContent, System.Text.Encoding.UTF8);
                _httpRequest.Content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json; charset=utf-8");
            }
            using (HttpClient client = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                client.Timeout = TimeSpan.FromMilliseconds(15000); //15 seconds
                client.DefaultRequestHeaders.ConnectionClose = true; //Set KeepAlive to false
                cancellationToken.ThrowIfCancellationRequested();
                var _httpResponse = await client.SendAsync(_httpRequest, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                return _httpResponse;
            }
        }

    }
}
