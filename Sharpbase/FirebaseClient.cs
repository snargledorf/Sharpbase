using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using Sharpbase.EventStreaming;

namespace Sharpbase
{
    internal partial class FirebaseClient : IFirebaseClient
    {
        private HttpClient httpClient;

        private Dictionary<IEventContract, EventListenerRequest> eventListenerRequests = new Dictionary<IEventContract, EventListenerRequest>();

        public FirebaseClient(Uri baseUrl, IContext context)
        {
            Context = context;
            httpClient = CreateHttpClient(baseUrl);
        }

        public IContext Context { get; }

        public Task<Result> Remove(Firebase reference)
        {
            CheckDisposed();
            ArgUtils.CheckForNull(reference, nameof(reference));

            var request = new Request(reference, HttpMethod.Delete);
            return PerformRequest(request);
        }
        
        public Task<Result> Set(Firebase reference, object content)
        {
            CheckDisposed();

            ArgUtils.CheckForNull(reference, nameof(reference));
            ArgUtils.CheckForNull(content, nameof(content));

            var request = new Request(reference, HttpMethod.Put, content);
            return PerformRequest(request);
        }

        public async Task<Firebase> Push(Firebase reference, object content = null)
        {
            CheckDisposed();

            ArgUtils.CheckForNull(reference, nameof(reference));

            var request = new PushRequest(reference, content);

            Result result = await PerformRequest(request);
            if (!result.Success)
                throw result.Error;

            return result.Reference;
        }

        public async void AddEventListener(IEventContract contract)
        {
            CheckDisposed();

            ArgUtils.CheckForNull(contract, nameof(contract));

            if (eventListenerRequests.ContainsKey(contract))
                return;

            var eventListenerRequest = new EventListenerRequest(contract, new Cache());
            eventListenerRequests.Add(contract, eventListenerRequest);

            HttpRequestMessage requestMessage = eventListenerRequest.CreateRequestMessage(Context);
            HttpResponseMessage httpResponseMessage =
                await SendRequestMessage(requestMessage, HttpCompletionOption.ResponseHeadersRead);
            eventListenerRequest.CreateResult(httpResponseMessage, Context);
        }

        public void RemoveEventListener(IEventContract contract)
        {
            CheckDisposed();

            ArgUtils.CheckForNull(contract, nameof(contract));

            EventListenerRequest eventListenerRequest;
            if (!eventListenerRequests.TryGetValue(contract, out eventListenerRequest))
                return;

            eventListenerRequests.Remove(contract);

            eventListenerRequest.Stop();
        }

        private async Task<Result> PerformRequest(Request request)
        {
            try
            {
                HttpRequestMessage requestMessage = request.CreateRequestMessage(Context);
                HttpResponseMessage httpResponseMessage = await SendRequestMessage(requestMessage);
                return request.CreateResult(httpResponseMessage, Context);
            }
            catch (Exception ex)
            {
                return new Result(new FirebaseException(ex.Message, ex));
            }
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

        private Task<HttpResponseMessage> SendRequestMessage(HttpRequestMessage request, HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead)
        {
            return httpClient.SendAsync(request, completionOption);
        }
    }

    // IDisposable implementaion
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
                client?.Dispose();
            }

            disposed = true;
        }
    }
}