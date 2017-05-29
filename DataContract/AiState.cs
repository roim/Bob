using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace AiProtocol {
    public class AiState {
        public string type;
        public AiStateVector2 position;
        public AiStateVector2 size;
        public int id;
    }
}
