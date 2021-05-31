using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Conduit.Extensions
{
    public class Errors
    {
        public string Message { get; set; }
    }

    public class ErrorDetails
    {
        public bool Success { get; set; }
        public Errors Errors { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });
        }
    }
}
