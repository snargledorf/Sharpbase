using System.Net.Http;

namespace Sharpbase
{
    internal class Request
    {
        public Request(Firebase reference, HttpMethod method, object content = null)
        {
            Reference = reference;
            Method = method;
            Content = content;
        }

        public Firebase Reference { get; }

        public HttpMethod Method { get; }

        public object Content { get; }

        public virtual HttpRequestMessage CreateRequestMessage(IContext context)
        {
            string requestUri = CreateRequestUri(context);
            var requestMessage = new HttpRequestMessage(Method, requestUri);

            if (Content != null)
            {
                string json = context.Serializer.Serialize(Content);
                requestMessage.Content = new StringContent(json);
            }

            return requestMessage;
        }

        private string CreateRequestUri(IContext context)
        {
            return context.AuthToken != AuthToken.Empty
                       ? $"{Reference.Path}.json?auth={context.AuthToken}"
                       : $"{Reference.Path}.json";
        }

        public virtual Result CreateResult(HttpResponseMessage httpResponseMessage, IContext context)
        {
            if (httpResponseMessage.IsSuccessStatusCode)
                return new Result(Reference);

            FirebaseException exception = FirebaseException.ForHttpStatusCode(httpResponseMessage.StatusCode);
            return new Result(exception);
        }
    }
}