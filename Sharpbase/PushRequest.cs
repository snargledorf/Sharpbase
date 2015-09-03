using System.Net.Http;

namespace Sharpbase
{
    internal class PushRequest : Request
    {
        public PushRequest(Firebase reference, object content = null)
            : base(reference, HttpMethod.Post, content ?? false)
        {
        }

        public override Result CreateResult(HttpResponseMessage httpResponseMessage, IContext context)
        {
            Result result = base.CreateResult(httpResponseMessage, context);
            if (!result.Success)
                return result;

            string json = httpResponseMessage.Content.ReadAsStringAsync().Result;
            var pushResponse = context.Serializer.Deserialize<PushResponse>(json);
            Firebase newRef = result.Reference.Child(pushResponse.Name);
            return new Result(newRef);
        }
    }
}