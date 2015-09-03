using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Sharpbase.Exceptions;

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

        public Task Remove(Path path, AuthToken token)
        {
            CheckDisposed();
            return SendRequest(HttpMethod.Delete, path, token);
        }

        public Task<Result> Push(Path path, object obj, AuthToken token)
        {
            CheckDisposed();

            // Default the value to false, otherwise the request will fail.
            return SendRequest(HttpMethod.Post, path, token, obj ?? false);
        }

        public Task<Result> Set(Path path, object obj, AuthToken token)
        {
            CheckDisposed();

            return SendRequest(HttpMethod.Put, path, token, obj);
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

        private static string CreateRequestUri(Path path, AuthToken token)
        {
            return token != AuthToken.Empty
                       ? string.Format("{0}.json?auth={1}", path, token)
                       : string.Format("{0}.json", path);
        }

        /// <summary>
        /// Throws an exception if the requeat was not successfull
        /// </summary>
        /// <param name="responseMessage">The response message to check</param>
        private static void CheckResponse(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
                return;

            Exception exception = GetFailedRequestException(responseMessage);

            // If there isn't a defined exception then throw the default one.
            if (exception == null)
                responseMessage.EnsureSuccessStatusCode();

            Debug.Assert(exception != null, "exception != null");

            throw exception;
        }

        private static Exception GetFailedRequestException(HttpResponseMessage responseMessage)
        {
            switch (responseMessage.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return new AuthDeniedException();
            }
            return null;
        }

        private async Task<Result> SendRequest(HttpMethod method, Path path, AuthToken token, object content = null)
        {
            try
            {
                HttpRequestMessage request = CreateRequest(method, path, token, content);

                HttpResponseMessage responseMessage = await DoSendRequest(request);

                CheckResponse(responseMessage);

                Snapshot snapshot = await CreateSnapshot(responseMessage);

                return new Result { Snapshot = snapshot };
            }
            catch (Exception ex)
            {
                return new Result { Error = new Error { Exception = ex } };
            }
        }

        private Task<HttpResponseMessage> DoSendRequest(HttpRequestMessage request)
        {
            return httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, Path path, AuthToken token, object content)
        {
            string requestUri = CreateRequestUri(path, token);
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