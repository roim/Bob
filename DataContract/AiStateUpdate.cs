using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AiProtocol
{
    public class AiStateUpdate
    {
        public string type = "update";
        public List<JToken> entities = new List<JToken>();

        public string input;
    }
}
