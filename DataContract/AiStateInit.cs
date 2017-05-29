using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiProtocol {
    public class AiStateInit {
        public string type = "init";

        // The index of the player
        public int index;

        // The team of the player (red/blue)
        public string team;
    }
}
