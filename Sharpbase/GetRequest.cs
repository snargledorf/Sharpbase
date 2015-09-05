using System.Net.Http;
using Sharpbase.JsonSerilization;

namespace Sharpbase
{
    internal class GetRequest : Request
    {
        public GetRequest(Firebase reference)
            :base(reference, HttpMethod.Get)
        {
        }

        public override Result CreateResult(HttpResponseMessage httpResponseMessage, IContext context)
        {
            Result result = base.CreateResult(httpResponseMessage, context);

            string json = httpResponseMessage.Content.ReadAsStringAsync().Result;

            IJsonObject jsonObject = context.Serializer.Deserialize(json);

            var snapshot = new Snapshot(jsonObject, result.Reference);

            return new SnapshotResult(snapshot, result.Reference);
        }
    }
}