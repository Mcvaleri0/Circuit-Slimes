using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Slimes;
using Puzzle.Pieces.Components;

namespace Puzzle.Actions
{
    public class SmartElectricMovement : Move
    {
        private bool Crossing = false;

        public SmartElectricMovement() : base() { }

        public SmartElectricMovement(LevelBoard.Directions dir, Vector2Int coords, bool crossing) :
            base(dir, coords) { this.Crossing = crossing; }


        #region Action Methods
        override public Action Available(Agent agent)
        {
            var adjacents = CheckAdjacentSolderTiles(agent);

            #region Crossing Attributes
            var crossing = false;

            List<LevelBoard.Directions> unexplored = null;

            if(adjacents.Count > 2)
            {
                crossing = true;

                var slime = (SmartElectricSlime)agent;

                slime.UpdateCrossing(slime.Coords, new List<LevelBoard.Directions>(adjacents.Keys));

                unexplored = slime.GetUnexploredPaths(slime.Coords);
            }
            #endregion

            var start = ((int)(agent.Orientation)) / 2;

            var maxUtility = 0;
            var bestChoice = LevelBoard.Directions.None;
            var isMove     = true;

            // For each cardinal Direction
            for(var i = 0; i < 4; i++)
            {
                // Make the Direction
                var dir = (LevelBoard.Directions)(((start + i + 3) * 2) % 8);

                // If there is an adjacent Solder Tile
                if(adjacents.TryGetValue(dir, out var piece))
                {
                    var utility = 1;

                    // If there's a Piece that is not a Component
                    // Or a Component that is at capacity, check the next Direction
                    if ((piece != null && !(piece is CircuitComponent)) ||
                        (piece is CircuitComponent component && component.Stats.Food >= component.Stats.MaxFood)) continue;

                    // If this Direction has not been explored
                    if (crossing && unexplored.Contains(dir)) utility += 1;

                    // If the utility is better going this way
                    if(utility > maxUtility)
                    {
                        maxUtility = utility;

                        bestChoice = dir;

                        isMove = piece == null;
                    }
                }
            }

            // If the best choice is not to stay in place
            if(bestChoice != LevelBoard.Directions.None)
            {
                if (isMove) return new SmartElectricMovement(bestChoice, LevelBoard.GetAdjacentCoords(agent.Coords, bestChoice), crossing);
                else return new Charge((CircuitComponent) adjacents[bestChoice], crossing);
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
        protected Dictionary<LevelBoard.Directions, Piece> CheckAdjacentSolderTiles(Agent agent)
        {
            Dictionary<LevelBoard.Directions, Piece> adjacents = new Dictionary<LevelBoard.Directions, Piece>();

            // For each cardinal direction
            for (var i = 0; i < 4; i++)
            {
                var dir = (LevelBoard.Directions)(i * 2); // Create Direction

                // Get coordinates of adjacent Space in that Direction
                var coords = LevelBoard.GetAdjacentCoords(agent.Coords, dir);

                // Get Tile at that Space
                var tile = agent.TileAt(coords);

                // If there's a Solder Tile there
                if (tile != null && tile.Type == Tile.Types.Solder)
                {
                    // Get Piece at that Space
                    var piece = agent.PieceAt(coords);

                    // Add adjacency
                    adjacents.Add(dir, piece);
                }
            }

            return adjacents;
        }
        #endregion
    }
}