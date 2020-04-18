using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;

namespace Puzzle.Actions
{
    public class ElectricMovement : Move
    {
        public ElectricMovement() : base() { }

        public ElectricMovement(LevelBoard.Directions dir, Vector2Int coords, Vector3 position) :
            base(dir, coords, position) { }


        #region Action Methods
        public override Action Available(Agent agent)
        {
            ArrayList adjacents = CheckAdjacentSolderTiles(agent);
                       
            var start = ((int) agent.Orientation) / 2;

            for (var i = 0; i < 4; i++)
            {
                var ind = (start + i + 3) % 4;

                if((bool) adjacents[ind] == true)
                {
                    var dir = (LevelBoard.Directions) (ind * 2);

                    var tcoords = agent.Board.GetAdjacentCoords(agent.Coords, dir);

                    if (tcoords.sqrMagnitude > 1000f) continue;
                        
                    var tpos = LevelBoard.WorldCoords(tcoords);

                    return new ElectricMovement(dir, tcoords, tpos);
                }
            }

            return null;
        }
        #endregion


        #region Aux
        protected ArrayList CheckAdjacentSolderTiles(Agent agent)
        {
            ArrayList adjacents = CheckCrossAdjacents(agent);
            
            for(var i = 0; i < 4; i++)
            {
                if((bool) adjacents[i])
                {
                    var coords = agent.Coords + LevelBoard.DirectionalVectors[i * 2];

                    var tile = agent.Board.GetTile(coords);

                    if (tile == null || tile.Type != Tile.Types.Solder) adjacents[i] = false;
                }
            }

            return adjacents;
        }
        #endregion
    }
}