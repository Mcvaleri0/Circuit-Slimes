using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Pieces;
using Puzzle.Board;

namespace Puzzle.Actions
{
    public class Action
    {
        public virtual Action Available(Agent agent)
        {
            return null;
        }

        public virtual void Begin(Agent agent)
        {
            return;
        }

        public virtual bool Confirm(Agent agent)
        {
            return true;
        }

        public virtual bool Execute(Agent agent)
        {
            return true;
        }

        public virtual bool End(Agent agent)
        {
            return true;
        }

        public virtual bool Undo(Agent agent)
        {
            return true;
        }


        #region === AUX Methods ===
        protected static ArrayList CheckCrossAdjacents(Agent agent)
        {
            ArrayList adjacents = new ArrayList();

            for (var i = 0; i < 4; i++)
            {
                var ind = i * 2;

                var coords = agent.Coords + LevelBoard.DirectionalVectors[ind];

                if (agent.IsFree(coords))
                {
                    adjacents.Add(true);
                }
                else adjacents.Add(false);
            }

            return adjacents;
        }
        #endregion
    }
}