using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace AiProtocol {
    public class AiStateUpdate {
        public string type = "update";
        public List<JToken> entities = new List<JToken>();

        public string input;
    }
}
