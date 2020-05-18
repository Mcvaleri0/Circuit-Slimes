using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Slimes;
namespace Puzzle.Actions
{
    public class SmartElectricMovement : ElectricMovement
    {
        private bool Crossing = false;

        public SmartElectricMovement() : base() { }

        public SmartElectricMovement(LevelBoard.Directions dir, Vector2Int coords, bool crossing) :
            base(dir, coords) { this.Crossing = crossing; }


        #region Action Methods
        override public Action Available(Agent agent)
        {
            var adjacents = CheckAdjacentSolderTiles(agent);

            if (adjacents.Count == 0) return null;

            var start = ((int)agent.Orientation) / 2;

            // If there's only one way to go
            if(adjacents.Count == 1)
            {
                var dir = adjacents[0];

                return new SmartElectricMovement(dir, LevelBoard.GetAdjacentCoords(agent.Coords, dir), false);
            }
            else if(agent is SmartElectricSlime slime)
            {
                LevelBoard.Directions choice = LevelBoard.Directions.None;

                var crossing = adjacents.Count > 2;

                for (var i = 0; i < 4; i++)
                {
                    var dir = (LevelBoard.Directions)((start + i + 3) % 4 * 2);

                    if (adjacents.Contains(dir))
                    {
                        var explored = slime.GetExploredPaths(agent.Coords);

                        if (crossing && explored != null && explored.Contains(dir))
                        {
                            choice = dir;
                        }
                        else
                        {
                            return new SmartElectricMovement(dir, LevelBoard.GetAdjacentCoords(slime.Coords, dir), crossing);
                        }
                    }
                }

                return new SmartElectricMovement(choice, LevelBoard.GetAdjacentCoords(slime.Coords, choice), crossing);
            }

            return null;
        }

        public override bool Confirm(Agent agent)
        {
            if (agent is SmartElectricSlime slime) {
                var origCoords = slime.Coords;
                var tile = slime.TileAt(this.MoveCoords);

                if (tile != null && tile.Type == Tile.Types.Solder)
                {
                    if (base.Confirm(slime))
                    {
                        if(this.Crossing) slime.RegisterExploredPath(origCoords, this.Direction);

                        return true;
                    }
                }
            }

            return false;
        }

        public override bool Undo(Agent agent)
        {
            if(agent is SmartElectricSlime slime)
            {
                var oppositeDir = (LevelBoard.Directions) ((((int) this.Direction) + 4) % 8);
                var origCoords  = LevelBoard.GetAdjacentCoords(slime.Coords, oppositeDir);

                slime.UnregisterExploredPath(origCoords, this.Direction);
            }

            return base.Undo(agent);
        }
        #endregion


        #region Aux
        protected new List<LevelBoard.Directions> CheckAdjacentSolderTiles(Agent agent)
        {
            var adjacents = new List<LevelBoard.Directions>();

            for(var i = 0; i < 4; i++)
            {
                var coords = agent.Coords + LevelBoard.DirectionalVectors[i * 2];

                // If that Space is free
                if (agent.IsFree(coords))
                {
                    var tile = agent.TileAt(coords);

                    // If it has a Solder Tile
                    if (tile != null && tile.Type == Tile.Types.Solder)
                        adjacents.Add((LevelBoard.Directions)(i * 2));
                }
            }

            return adjacents;
        }
        #endregion
    }
}