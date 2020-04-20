﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class CardinalMove : Move
    {
        public CardinalMove(LevelBoard.Directions dir, Vector2Int coords) : base(dir, coords) { }


        #region Action Methods
        public override Action Available(Agent agent)
        {
            ArrayList freeSpots = CheckCrossAdjacents(agent);

            if(freeSpots != null)
            {
                var start = ((int) agent.Orientation) / 2;

                for (var i = 0; i < 4; i++)
                {
                    var ind = (start + i + 3) % 4;

                    if((bool) freeSpots[ind] == true)
                    {
                        var dir = (LevelBoard.Directions) (ind * 2);

                        var tcoords = agent.Board.GetAdjacentCoords(agent.Coords, dir);

                        if (tcoords.sqrMagnitude > 1000f) continue;

                        return new CardinalMove(dir, tcoords);
                    }
                }
            }

            return null;
        }
        #endregion
    }
}