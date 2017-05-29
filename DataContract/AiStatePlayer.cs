using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AiProtocol {
    public class AiStatePlayer : AiState {
        public int playerIndex;
        public bool shield;
        public bool wings;
        public string state;
        public List<string> arrows;
        public bool canHurt;
        public bool dead;
        public int facing;
        public bool onGround;
        public bool onWall;
        public AiStateVector2 aimDirection;
        public string team;
        public bool dodgeCooldown;
    }
}
