using System;
using System.Collections.Generic;
using AiProtocol;

namespace Bob.Entity
{
    class Player
    {
        readonly List<string> arrows = new List<string>();

        public AiStateVector2 AimDirection { get; private set; }
        public List<string> Arrows { get { return this.arrows; } }
        public bool CanHurt { get; private set; }
        public bool Dead { get; private set; }
        public bool DodgeCooldown { get; private set; }
        public int Facing { get; private set; }
        public int Idx { get; private set; }
        public bool OnGround { get; private set; }
        public bool OnWall { get; private set; }
        public AiStateVector2 Pos { get; set; }
        public bool Shield { get; private set; }
        public string Team { get; private set; }
        public bool Wings { get; private set; }

        public Player(AiStateInit player)
        {
            if (player == null) { throw new ArgumentNullException(nameof(player)); }
            this.Idx = player.index;
            this.Team = player.team;
        }

        public Player(AiStatePlayer player)
        {
            if (player == null) { throw new ArgumentNullException(nameof(player)); }
            this.Idx = player.playerIndex;
            this.Team = player.team;
        }

        public void Update(AiStatePlayer player)
        {
            if (player.playerIndex != this.Idx || player.team != this.Team)
            {
                throw new ArgumentException(nameof(player));
            }

            this.Arrows.Clear();

            this.AimDirection = player.aimDirection;
            this.Arrows.AddRange(player.arrows);
            this.CanHurt = player.canHurt;
            this.Dead = player.dead;
            this.DodgeCooldown = player.dodgeCooldown;
            this.Facing = player.facing;
            this.Idx = player.playerIndex;
            this.OnGround = player.onGround;
            this.OnWall = player.onWall;
            this.Pos = player.position;
            this.Shield = player.shield;
            this.Wings = player.wings;
        }

        public double Distance2(Player other)
        {
            if (other == null) { throw new ArgumentNullException(nameof(other)); }
            double x = other.Pos.x - this.Pos.x;
            double y = other.Pos.y - this.Pos.y;
            return x * x + y * y;
        }

        public bool IsAlly(Player other)
        {
            if (other == null) { throw new ArgumentNullException(nameof(other)); }
            return this.Team == other.Team;
        }
    }
}
