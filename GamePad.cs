using System;
using System.Text;

namespace Bob
{
    public class GamePad
    {
        public Direction StickDirection { get; private set; }

        public bool Jump { get; set; }

        public bool Dash { get; set; }

        public bool Shoot { get; set; }

        public void Right()
        {
            this.StickDirection &= ~Direction.Left;
            this.StickDirection |= Direction.Right;
        }

        public void Left()
        {
            this.StickDirection &= ~Direction.Right;
            this.StickDirection |= Direction.Left;
        }

        public void Up()
        {
            this.StickDirection &= ~Direction.Down;
            this.StickDirection |= Direction.Up;
        }

        public void Down()
        {
            this.StickDirection &= ~Direction.Up;
            this.StickDirection |= Direction.Down;
        }

        StringBuilder sb = new StringBuilder();
        public void IssueCommand()
        {
            sb.Clear();
            if (this.Dash) { sb.Append('z'); }
            if (this.Jump) { sb.Append('j'); }
            if (this.Shoot) { sb.Append('s'); }
            if (this.StickDirection.HasFlag(Direction.Right)) { sb.Append('r'); }
            if (this.StickDirection.HasFlag(Direction.Left)) { sb.Append('l'); }
            if (this.StickDirection.HasFlag(Direction.Up)) { sb.Append('u'); }
            if (this.StickDirection.HasFlag(Direction.Down)) { sb.Append('d'); }
            Console.WriteLine(sb.ToString());

            this.Dash = false;
            this.Jump = false;
            this.Shoot = false;
            this.StickDirection = Direction.Center;
        }
    }

    [Flags]
    public enum Direction
    {
        Center = 0,
        Right = 1,
        Left = 2,
        Up = 4,
        Down = 8
    }
}
