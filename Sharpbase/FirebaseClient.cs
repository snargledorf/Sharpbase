using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sharpbase
{
    internal partial class FirebaseClient : IFirebaseClient
    {
        private readonly IContext context;

        private HttpClient httpClient;

        public FirebaseClient(string baseUrl, IContext context)
        {
            this.context = context;

            var baseUri = new Uri(baseUrl);
            httpClient = CreateHttpClient(baseUri);
        }

        public Task Remove(Path path)
        {
            CheckDisposed();
            return SendRequest(HttpMethod.Delete, path);
        }

        public Task<Result> Push(Path path, object obj)
        {
            CheckDisposed();

            // Default the value to false, otherwise the request will fail.
            return SendRequest(HttpMethod.Post, path, obj ?? false);
        }

        public Task<Result> Set(Path path, object obj)
        {
            CheckDisposed();

            return SendRequest(HttpMethod.Put, path, obj);
        }

        private static HttpClient CreateHttpClient(Uri baseUri)
        {
            var messageHandler = new HttpClientHandler
                                     {
                                         AllowAutoRedirect = true
                                     };
            
            var client = new HttpClient(messageHandler, true) { BaseAddress = baseUri };

            return client;
        }

        private static string CreateRequestUri(Path path)
        {
            string requestUri = path.ToString();
            if (!requestUri.EndsWith(".json"))
                requestUri += ".json";
            return requestUri;
        }

        private async Task<Result> SendRequest(HttpMethod method, Path path, object content = null)
        {
            try
            {
                HttpRequestMessage request = CreateRequest(method, path, content);

                HttpResponseMessage responseMessage = await DoSendRequest(request);
                responseMessage.EnsureSuccessStatusCode();

                Snapshot snapshot = await CreateSnapshot(responseMessage);

                return new Result { Snapshot = snapshot };
            }
            catch (Exception ex)
            {
                return new Result { Error = new Error { Exception = ex } };
            }
        }

        private ConfiguredTaskAwaitable<HttpResponseMessage> DoSendRequest(HttpRequestMessage request)
        {
            return httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, Path path, object content)
        {
            string requestUri = CreateRequestUri(path);
            var request = new HttpRequestMessage(method, requestUri);

            if (content != null)
            {
                string json = context.Serializer.Serialize(content);
                request.Content = new StringContent(json);
            }

            return request;
        }

        private async Task<Snapshot> CreateSnapshot(HttpResponseMessage responseMessage)
        {
            string json = await responseMessage.Content.ReadAsStringAsync();
            return new Snapshot(json, context);
        }
    }

    internal partial class FirebaseClient : IDisposable
    {
        private bool disposed;

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException("FirebaseApi");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                HttpClient client = httpClient;
                httpClient = null;
                if (client != null) client.Dispose();
            }

            disposed = true;
        }
    }
}