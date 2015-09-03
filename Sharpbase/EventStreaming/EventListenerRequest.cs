using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Sharpbase.JsonSerilization;

namespace Sharpbase.EventStreaming
{
    internal partial class EventListenerRequest : Request
    {
        private readonly IEventContract contract;

        private readonly Cache cache;

        private CancellationTokenSource cts;
        
        public EventListenerRequest(IEventContract contract, Cache cache)
            : base(contract.Reference, HttpMethod.Get)
        {
            this.contract = contract;
            this.cache = cache;
            cts = new CancellationTokenSource();
        }

        public override HttpRequestMessage CreateRequestMessage(IContext context)
        {
            HttpRequestMessage httpRequestMessage = base.CreateRequestMessage(context);
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
            return httpRequestMessage;
        }

        public override Result CreateResult(HttpResponseMessage responseMessage, IContext context)
        {
            responseMessage.EnsureSuccessStatusCode();
            
            ProcessEventsUntilStopped(responseMessage, context);
            
            return null;
        }

        private async void ProcessEventsUntilStopped(HttpResponseMessage responseMessage, IContext context)
        {
            using (Stream contentStream = await responseMessage.Content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(contentStream))
                {
                    await ReadEventsFromReader(reader, context);
                }
            }
        }

        private async Task ReadEventsFromReader(TextReader reader, IContext context)
        {
            try
            {
                string eventLine;
                while ((eventLine = await reader.ReadLineAsync()) != null)
                {
                    CheckIfCancelled();

                    if (eventLine.StartsWith("event: "))
                    {
                        string dataLine = await reader.ReadLineAsync();

                        cts.Token.ThrowIfCancellationRequested();

                        string eventName = ParseEventLine(eventLine);
                        string json = ParseEventLine(dataLine);
                        IJsonNode tree = context.Serializer.Deserialize(json);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Expected event but received: {eventLine}");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (!disposed)
                    throw;
            }
        }

        private void CheckIfCancelled()
        {
            CheckDisposed();

            cts.Token.ThrowIfCancellationRequested();
        }

        private static string ParseEventLine(string eventLine)
        {
            int indexOfColon = eventLine.IndexOf(": ", StringComparison.Ordinal);
            int startOfData = indexOfColon + 2;
            return eventLine.Substring(startOfData);
        }

        public void Stop()
        {
            Dispose();
        }
    }

    internal partial class EventListenerRequest : IDisposable
    {
        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                CancellationTokenSource c = cts;
                cts = null;

                c?.Cancel(true);
                c?.Dispose();
            }

            disposed = true;
        }

        private void CheckDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(EventListenerRequest));
        }
    }
}