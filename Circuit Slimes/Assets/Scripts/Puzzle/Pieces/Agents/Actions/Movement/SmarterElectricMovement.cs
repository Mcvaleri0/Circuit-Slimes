using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Slimes;
using Puzzle.Pieces.Components;

namespace Puzzle.Actions
{
    public class SmarterElectricMovement : SmartElectricMovement
    {
        public SmarterElectricMovement() : base() { }

        public SmarterElectricMovement(LevelBoard.Directions dir, Vector2Int coords, bool crossing) :
            base(dir, coords, crossing) { }


        #region Action Methods
        override public Action Available(Agent agent)
        {
            var adjacents = CheckAdjacentSolderTiles(agent);

            #region Crossing Attributes
            var crossing = false;

            Dictionary<LevelBoard.Directions, int> utilities = null;

            if(adjacents.Count > 2)
            {
                crossing = true;

                var slime = (SmarterElectricSlime)agent;

                slime.UpdateCrossing(slime.Coords, new List<LevelBoard.Directions>(adjacents.Keys));

                utilities = slime.GetUtilities(slime.Coords);
            }
            #endregion

            var start = ((int)(agent.Orientation)) / 2;

            var maxUtility = int.MinValue;
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
                    if (crossing && utilities.ContainsKey(dir)) utility += utilities[dir];

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
                var origCoords = slime.Coords;
                var tile = slime.TileAt(this.MoveCoords);

                // If there still is a Solder Tile at that Space
                if (tile != null && tile.Type == Tile.Types.Solder)
                {
                    // If the Slime can still Move there
                    if (base.Confirm(slime))
                    {
                        // If it's departing a Crossing
                        if (this.Crossing)
                        {
                            // Calculate direction from which the Slime came
                            var invDir = (LevelBoard.Directions)((((int) this.OrigOrientation) + 4) % 8);

                            slime.RegisterExploredPath(origCoords, invDir);         // Register it as explored
                            slime.RegisterExploredPath(origCoords, this.Direction); // Register the chosen direction as explored

                            // If the Slime has come from a Crossing
                            if (slime.CrossingLog.Count > 0)
                            {
                                var last = slime.CrossingLog.Peek(); // Get last visited Crossing

                                // If the last visited Crossing is not the present Crossing
                                if (last.Key != origCoords)
                                {
                                    // Add this Crossing as the last one's child
                                    slime.AddChildToCrossing(last.Key, last.Value, origCoords);
                                }
                            }

                            // Save this as the last visited Crossing
                            slime.CrossingLog.Push(new KeyValuePair<Vector2Int, LevelBoard.Directions>(origCoords, this.Direction));
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public override bool Undo(Agent agent)
        {
            if(agent is SmarterElectricSlime slime)
            {
                // Calculate direction from which the Slime originally came from
                var invDir = (LevelBoard.Directions)((((int)this.OrigOrientation) + 4) % 8);

                // Calculate inverse movement direction
                var oppositeDir = (LevelBoard.Directions) ((((int) this.Direction) + 4) % 8);

                // Calculate Slime's original coordinates
                var origCoords  = LevelBoard.GetAdjacentCoords(slime.Coords, oppositeDir);

                // Unregister explorations
                slime.UnregisterExploredPath(origCoords, invDir);
                slime.UnregisterExploredPath(origCoords, this.Direction);

                // Revert to previous last visited Crossing
                slime.CrossingLog.Pop();
            }

            // Undo Move
            return base.Undo(agent);
        }
        #endregion
    }
}