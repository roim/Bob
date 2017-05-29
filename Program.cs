using System;
using System.Text;
using AiProtocol;
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

        static void Main(string[] args)
        {
            // The communication is done over stdin. Every line is a json.

            // First, read the information about your player. This is sent only once.
            stateInit = Read<AiStateInit>();

            // The seed is based on the bot index so bots act differently.
            rand = new Random((int)DateTime.UtcNow.Ticks + stateInit.index * 100000);

            while (true)
            {
                // Your bot will read the state of the game in a loop and send the actions back. 
                // It can be either the state of the scenario (the grid) or the state of the 
                // entities (players, arrows, items, etc).
                // Check the 'type' and act accordingly.
                var aiState = Read<JToken>();
                string type = aiState.SelectToken("type").Value<string>();
                if (type == "scenario")
                {
                    // There is the 'scenario' state, update that informs your bot about the 
                    // current state of the ground.
                    // DON'T ISSUE ANY COMMAND ATER READING A SCENARIO STATE.
                    stateScenario = aiState.ToObject<AiStateScenario>();
                    continue;
                }
                else if (type == "update")
                {
                    // When reading an 'update' state, your bot is expected to issue a command 
                    // back for that particular loop.
                    stateUpdate = aiState.ToObject<AiStateUpdate>();
                }

                // This bot acts based on the position of the other player only. It
                // has a very random playstyle:
                //  - Runs to the enemy when it is below.
                //  - Runs away from the enemy when it is above.
                //  - Shoots when in the same horizontal line.
                //  - Dashes randomly.
                //  - Jumps randomly.

                // Your bot is expected to output string with the 'pressed buttons'. 
                // The order of the characters are irrelevant. Any other character is ignored.
                // r = right
                // l = left
                // u = up
                // d = down
                // j = jump
                // z = dash
                // s = shoot

                var sb = new StringBuilder();

                AiStatePlayer myState = null;
                AiStatePlayer otherState = null;

                foreach (JToken state in stateUpdate.entities)
                {
                    if (state.SelectToken("type").Value<string>() == "player")
                    {
                        AiStatePlayer statePlayer = state.ToObject<AiStatePlayer>();
                        if (statePlayer.playerIndex == stateInit.index)
                        {
                            myState = statePlayer;
                        }
                        else if (statePlayer.team != stateInit.team)
                        {
                            otherState = statePlayer;
                        }
                    }
                }

                // Our bot only do anything if players are in game.
                if (myState == null || otherState == null)
                {
                    Console.WriteLine();
                    continue;
                }

                if (otherState.position.y >= myState.position.y ||
                    otherState.position.y < myState.position.y - 15)
                {
                    // Runs away if enemy is above
                    if (otherState.position.x > myState.position.x)
                    {
                        sb.Append('r');
                    }
                    else
                    {
                        sb.Append('l');
                    }
                }
                else
                {
                    // Runs to enemy if it is below
                    if (otherState.position.x > myState.position.x)
                    {
                        sb.Append('l');
                    }
                    else
                    {
                        sb.Append('r');
                    }

                    // If in the same line shoots, 
                    if (Math.Abs(myState.position.y - otherState.position.y) < 5)
                    {
                        sb.Append('s');
                    }
                }

                // Presses dash in 20% of the loops.
                if (rand.Next(5) == 0)
                {
                    sb.Append('z');
                }

                // Presses jump in 10% of the loops.
                if (rand.Next(10) == 0)
                {
                    sb.Append('j');
                }

                // Issue the command back to the game.
                Console.WriteLine(sb.ToString());
            }
        }

        private static T Read<T>()
        {
            string line = Console.ReadLine();
            T obj = JsonConvert.DeserializeObject<T>(line);
            return obj;
        }
    }
}
