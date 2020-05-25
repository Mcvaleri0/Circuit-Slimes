using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Slimes;
using Puzzle.Pieces.Components;
using System.CodeDom;

namespace Puzzle.Actions
{
    public class SmarterElectricMovement : Move
    {
        protected bool Crossing = false;

        protected Vector2Int OrigCoords;

        protected bool RegisteredConnection = false;

        public SmarterElectricMovement() : base() { }

        public SmarterElectricMovement(LevelBoard.Directions dir, Vector2Int coords, bool crossing) :
            base(dir, coords) { this.Crossing = crossing; }


        #region Action Methods
        public override Action Available(Agent agent)
        {
            var adjacents = this.CheckAdjacentSolderTiles(agent);

            #region Crossing Attributes
            var crossing = false;

            Dictionary<LevelBoard.Directions, int> utilities = null;

            var slime = (SmarterElectricSlime)agent;

            // If this is a Crossing
            if (adjacents.Count > 2)
            {
                crossing = true;

                slime.UpdateCrossing(slime.Coords, new List<LevelBoard.Directions>(adjacents.Keys));

                utilities = slime.GetUtilities(slime.Coords);
            }
            #endregion

            var start = ((int)(agent.Orientation)) / 2;

            var maxUtility = float.MinValue;
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
                    var utility = (4 - i) * 0.25f;

                    // If this is a Crossing get the Directions utility
                    if (crossing && utilities.ContainsKey(dir)) utility += utilities[dir];

                    if (dir == LevelBoard.InvertDirection(agent.Orientation))
                    {
                        if (utility > 0) utility *= 0.5f;
                        else             utility *= 2f;
                    }

                    // If there's a Piece that is not a Component
                    // Or a Component that is at capacity, check the next Direction
                    if ((piece != null && !(piece is CircuitComponent)) ||
                        (piece is CircuitComponent component && component.Stats.Food >= component.Stats.MaxFood))
                    {
                        if(utility > maxUtility) slime.PathBlocked = true;
                        continue;
                    }

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
                if (isMove) return new SmarterElectricMovement(bestChoice, LevelBoard.GetAdjacentCoords(agent.Coords, bestChoice), crossing);
                else return new Charge((CircuitComponent) adjacents[bestChoice], crossing);
            }

            return null;
        }

        public override bool Confirm(Agent agent)
        {
            if (agent is SmarterElectricSlime slime) {
                var tile = slime.TileAt(this.MoveCoords);

                // If there still is a Solder Tile at that Space
                if (tile != null && tile.Type == Tile.Types.Solder)
                {
                    this.OrigCoords = slime.Coords;

                    // If the Slime can still Move there
                    if (base.Confirm(slime))
                    {
                        // If it's departing a Crossing
                        if (this.Crossing)
                        {
                            this.RegisteredConnection = slime.CommunicateInfo(this.OrigCoords, this.OrigOrientation, this.Direction);
                        }

                        return true;
                    }
                }
                else
                {
                    slime.PathBlocked = true;
                }
            }

            return false;
        }

        public override bool Undo(Agent agent)
        {
            if(agent is SmarterElectricSlime slime)
            {
                if (this.Crossing)
                {
                    slime.RollBackInfo(this.OrigCoords, this.OrigOrientation, this.Direction, this.RegisteredConnection);
                }                
            }

            // Undo Move
            return base.Undo(agent);
        }
        #endregion


        #region AUX Methods
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