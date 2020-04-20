﻿using System.Collections;
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

        public virtual bool Execute(Agent agent)
        {
            return true;
        }

        public virtual void Undo(Agent agent)
        {
            return;
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