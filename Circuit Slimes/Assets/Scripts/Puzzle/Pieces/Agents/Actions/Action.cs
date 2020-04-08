using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class Action
    {
        public virtual Action Available(Agent agent)
        {
            return null;
        }

        public virtual bool Execute(Agent agent)
        {
            return true;
        }


        #region === AUX Methods ===
        protected static ArrayList CheckAdjacents(Agent agent)
        {
            ArrayList adjacents = new ArrayList();

            var x = (int)agent.Coords.x;
            var y = (int)agent.Coords.y;

            // Right
            if (agent.IsFree(x + 1, y))
            {
                adjacents.Add(true);
            }
            else adjacents.Add(false);

            // Up
            if (agent.IsFree(x, y - 1))
            {
                adjacents.Add(true);
            }
            else adjacents.Add(false);

            // Down
            if (agent.IsFree(x, y + 1))
            {
                adjacents.Add(true);
            }
            else adjacents.Add(false);

            // Left
            if (agent.IsFree(x - 1, y))
            {
                adjacents.Add(true);
            }
            else adjacents.Add(false);

            if (adjacents.Count == 0) return null;

            return adjacents;
        }
        #endregion
    }
}