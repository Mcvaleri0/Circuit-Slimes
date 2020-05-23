using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Slimes;
using Puzzle.Pieces.Components;
using System.Net;

namespace Puzzle.Actions
{
    public class Charge : Action
    {
        public CircuitComponent Component { get; private set; }

        public Vector2Int ChargeCoords { get; private set; }

        public Vector2Int ComponentCoords { get; private set; }

        private bool Crossing = false;

        private LevelBoard.Directions CameFrom;

        private LevelBoard.Directions EnteredBy;

        public Charge() { }

        public Charge(CircuitComponent component) 
        {
            this.Component = component;

            this.ComponentCoords = component.Coords;
        }

        public Charge(CircuitComponent component, bool crossing)
        {
            this.Component = component;

            this.ComponentCoords = component.Coords;

            this.Crossing = crossing;
        }


        #region === Action Methods ===
        override public Action Available(Agent agent)
        {
            if (agent is ElectricSlime slime)
            {
                var start = ((int)slime.Orientation) / 2;

                for (var i = 0; i < 4; i++)
                {
                    var ind = ((start + i + 3) % 4) * 2;

                    var coords = slime.Coords + LevelBoard.DirectionalVectors[ind];

                    var piece = slime.PieceAt(coords);
                    var tile  = slime.TileAt(coords);

                    // If there's a Solder Tile
                    if(tile != null && tile.Type == Tile.Types.Solder)
                    {
                        // If there's a Circuit Component
                        if(piece != null && piece.Characterization.Category == Piece.Categories.Component)
                        {
                            // If the component is not at full charge
                            if (piece is CircuitComponent component &&
                                component.Stats.Food < component.Stats.MaxFood)
                            {
                                return new Charge(component);
                            }
                        }
                        break;
                    }
                }
            }

            return null;
        }

        public override bool Confirm(Agent agent)
        {
            // If Component is still in place
            if(agent.PieceAt(this.ComponentCoords) == this.Component &&
               this.Component.Stats.Food < this.Component.Stats.MaxFood)
            {
                this.SaveSmartInfo(agent);

                this.ChargeCoords = agent.Coords;

                this.Component.Stats.Food++;

                return true;
            }

            return false;
        }

        override public bool Execute(Agent agent)
        {
            if (agent is ElectricSlime slime) {
                this.Component.ReceiveCharge(slime);
            }

            return true;
        }

        override public bool Undo(Agent agent)
        {
            if (agent is ElectricSlime slime)
            {
                this.Component.Stats.Food--;

                this.Component.ReleaseCharge(slime, this.ChargeCoords);

                var outDir = LevelBoard.GetDirection(this.ChargeCoords, this.Component.Coords);

                slime.RotateInBoard(outDir);
                slime.Rotate(outDir, 1f);

                this.DeleteSmartInfo(agent);

                return true;
            }

            return false;
        }
        #endregion

        #region Smart Additions
        private void SaveSmartInfo(Agent agent)
        {
            // If this is a Crossing
            if (this.Crossing && agent is SmartElectricSlime smart)
            {
                // Save the Direction the Slime reached the Crossing from
                this.CameFrom = LevelBoard.InvertDirection(smart.Orientation);

                // Register it as explored for that crossing
                smart.RegisterExploredPath(smart.Coords, this.CameFrom);

                // Save Direction Slime entered component by/left crossing from
                this.EnteredBy = LevelBoard.GetDirection(smart.Coords, this.ComponentCoords);

                // Register it as explored for that crossing
                smart.RegisterExploredPath(smart.Coords, this.EnteredBy);

                // If Smart Slime is Smarter
                if (smart is SmarterElectricSlime smarter)
                {
                    // If the Slime has come from a Crossing
                    if (smarter.CrossingLog.Count > 0)
                    {
                        var last = smarter.CrossingLog.Peek(); // Get last visited Crossing

                        // If the last visited Crossing is not the present Crossing
                        if (last.Key != smart.Coords)
                        {
                            // Add this Crossing as the last one's child
                            smarter.RegisterCrossingConnection(last.Key, last.Value, smart.Coords);
                        }
                    }

                    // Save this Crossing as its most recently visited, and Direction from whic it left
                    smarter.CrossingLog.Push(new KeyValuePair<Vector2Int, LevelBoard.Directions>(smarter.Coords, this.EnteredBy));
                }
            }
        }

        private void DeleteSmartInfo(Agent agent)
        {
            // If this is a Crossing
            if (this.Crossing && agent is SmartElectricSlime smart)
            {
                // Unregister this exploration of the Crossing
                smart.UnregisterExploredPath(this.ChargeCoords, this.CameFrom);
                smart.UnregisterExploredPath(this.ChargeCoords, this.EnteredBy);

                if (smart is SmarterElectricSlime smarter && smarter.CrossingLog.Count > 0)
                {
                    smarter.CrossingLog.Pop();

                    if (smarter.CrossingLog.Count > 0)
                    {
                        var last = smarter.CrossingLog.Peek();

                        if (last.Key == this.ChargeCoords) return;

                        smarter.UnregisterCrossingConnection(last.Key, last.Value, this.ChargeCoords);
                    }
                }
            }
        }
        #endregion
    }
}