using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Slimes;
using Puzzle.Pieces.Components;

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

            var moveCoords = new Vector2Int(-1, -1);
            var chosenDir  = LevelBoard.Directions.None;
            var crossing   = adjacents.Count > 2;

            var start = ((int)agent.Orientation) / 2;

            // If there's only one way to go
            if(adjacents.Count == 1)
            {
                chosenDir = adjacents[0];

                moveCoords = LevelBoard.GetAdjacentCoords(agent.Coords, chosenDir);
            }
            else if(agent is SmartElectricSlime slime)
            {
                if(crossing) adjacents = this.CrosscheckUnexplored(slime, adjacents);

                for (var i = 0; i < 4; i++)
                {
                    var dir = (LevelBoard.Directions)((start + i + 3) % 4 * 2);

                    if (adjacents.Contains(dir))
                    {
                        moveCoords = LevelBoard.GetAdjacentCoords(slime.Coords, dir);

                        //DO STUFF
                    }
                }

                return new SmartElectricMovement(chosenDir, moveCoords, crossing);
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

                var piece = agent.PieceAt(coords);

                // If that Space is free or has a Component
                if (piece == null || piece is CircuitComponent)
                {
                    var tile = agent.TileAt(coords);

                    // If it has a Solder Tile
                    if (tile != null && tile.Type == Tile.Types.Solder)
                        adjacents.Add((LevelBoard.Directions)(i * 2));
                }
            }

            return adjacents;
        }


        protected List<LevelBoard.Directions> CrosscheckUnexplored(SmartElectricSlime slime, List<LevelBoard.Directions> adjacents)
        {
            slime.UpdateCrossing(slime.Coords, adjacents);

            return slime.GetUnexploredPaths(slime.Coords);
        }
        #endregion
    }
}