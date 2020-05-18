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

        public SmartElectricMovement(LevelBoard.Directions dir, Vector2Int coords) :
            base(dir, coords) { }


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

                return new SmartElectricMovement(dir, LevelBoard.GetAdjacentCoords(agent.Coords, dir));
            }
            else if(agent is SmartElectricSlime slime)
            {
                if (adjacents.Count > 2) this.Crossing = true;

                LevelBoard.Directions choice = LevelBoard.Directions.None;

                for (var i = 0; i < 4; i++)
                {
                    var dir = (LevelBoard.Directions)((start + i + 3) % 4 * 2);

                    if (adjacents.Contains(dir))
                    {
                        slime.ExploredPaths.TryGetValue(slime.Coords, out var explored);

                        if (this.Crossing && explored != null && explored.Contains(dir))
                        {
                            choice = dir;
                        }
                        else
                        {
                            return new SmartElectricMovement(dir, LevelBoard.GetAdjacentCoords(slime.Coords, dir));
                        }
                    }
                }

                return new SmartElectricMovement(choice, LevelBoard.GetAdjacentCoords(slime.Coords, choice));
            }

            return null;
        }

        public override bool Confirm(Agent agent)
        {
            if (agent is SmartElectricSlime slime) {
                var tile = slime.TileAt(this.MoveCoords);

                if (tile != null && tile.Type == Tile.Types.Solder)
                {
                    if (base.Confirm(slime))
                    {
                        slime.RegisterExploredPath(slime.Coords, LevelBoard.GetDirection(slime.Coords, this.MoveCoords));

                        return true;
                    }
                }
            }

            return false;
        }
        #endregion


        #region Aux
        protected List<LevelBoard.Directions> CheckAdjacentSolderTiles(Agent agent)
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
                    if (tile == null || tile.Type != Tile.Types.Solder)
                        adjacents.Add((LevelBoard.Directions)(i * 2));
                }
            }

            return adjacents;
        }
        #endregion
    }
}