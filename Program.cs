using System;
using System.Collections.Generic;
using System.Linq;
using AiProtocol;
using Bob.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bob
{
    class Program
    {
        static AiStateInit stateInit;
        static AiStateScenario stateScenario;
        static AiStateUpdate stateUpdate;
        static Random rand;

        static GamePad GamePad = new GamePad();
        static Player Self;

        static readonly List<Player> Allies = new List<Player>();
        static readonly List<Player> Enemies = new List<Player>();
        static IEnumerable<Player> Players { get { return PlayersById.Values; } }
        static readonly Dictionary<int, Player> PlayersById = new Dictionary<int, Player>();

        static void Main(string[] args)
        {
            // The communication is done over stdin. Every line is a json.
            // First, read the information about your player. This is sent only once.
            stateInit = Read<AiStateInit>();
            Self = new Player(stateInit);
            PlayersById[Self.Idx] = Self;

            // The seed is based on the bot index so bots act differently.
            rand = new Random((int)DateTimeOffset.UtcNow.Ticks + stateInit.index * 100000);

            while (true)
            {
                // Your bot will read the state of the game in a loop and send the actions back. 
                // It can be either the state of the scenario (the grid) or the state of the 
                // entities (players, arrows, items, etc).
                // Check the 'type' and act accordingly.
                dynamic aiState = ReadDynamic();
                if (aiState.type == Constants.AiStateTypeScenario)
                {
                    stateScenario = aiState.ToObject<AiStateScenario>();
                    continue;
                }
                else if (aiState.type == Constants.AiStateTypeUpdate)
                {
                    stateUpdate = aiState.ToObject<AiStateUpdate>();
                }

                foreach (dynamic state in stateUpdate.entities)
                {
                    if (state.type == "player")
                    {
                        AiStatePlayer statePlayer = state.ToObject<AiStatePlayer>();

                        Player p;
                        if (!PlayersById.TryGetValue(statePlayer.playerIndex, out p))
                        {
                            p = new Player(statePlayer);
                            PlayersById[p.Idx] = p;

                            if (p.IsAlly(Self))
                            {
                                Allies.Add(p);
                            }
                            else
                            {
                                Enemies.Add(p);
                            }
                        }

                        p.Update(statePlayer);
                    }
                }

                if (Self.Dead)
                {
                    Console.WriteLine();
                    continue;
                }

                // Run towards closest enemy
                Player closestEnemy = Enemies.Where(e => !e.Dead).OrderBy(e => Self.Distance2(e)).FirstOrDefault();
                if (closestEnemy != null)
                {
                    if (closestEnemy.Pos.x > Self.Pos.x)
                    {
                        GamePad.Right();
                    }
                    else
                    {
                        GamePad.Left();
                    }
                }

                bool hasEnemyAbove = false;
                foreach (Player enemy in Enemies)
                {
                    if (enemy.Dead) { continue; }
                    double distance = Math.Sqrt(enemy.Distance2(Self));

                    // Enemy might stomp us
                    if (distance < 100 && enemy.Pos.y > Self.Pos.y)
                    {
                        hasEnemyAbove = true;
                        if (enemy.Pos.x > Self.Pos.x)
                        {
                            GamePad.Right();
                        }
                        else
                        {
                            GamePad.Left();
                        }
                    }

                    // Same line
                    if (Math.Abs(Self.Pos.y - enemy.Pos.y) < 5)
                    {
                        if (rand.NextDouble() > 0.5)
                        {
                            GamePad.Shoot = true;
                        }
                        else if (rand.NextDouble() > 0.25)
                        {
                            GamePad.Dash = true;
                        }
                    }
                }

                if (!hasEnemyAbove)
                {
                    if (rand.NextDouble() > 0.1)
                    {
                        GamePad.Jump = true;
                    }
                }

                GamePad.IssueCommand();
            }
        }

        static dynamic ReadDynamic()
        {
            return JObject.Parse(Console.ReadLine());
        }

        static T Read<T>()
        {
            return JsonConvert.DeserializeObject<T>(Console.ReadLine());
        }
    }
}
