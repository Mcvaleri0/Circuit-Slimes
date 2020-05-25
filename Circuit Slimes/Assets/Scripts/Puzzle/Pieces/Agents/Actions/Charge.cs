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

        private LevelBoard.Directions OrigOrientation;

        private LevelBoard.Directions WentTo;

        private bool RegisteredConnection = false;

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
                this.ChargeCoords = agent.Coords;

                this.OrigOrientation = agent.Orientation;

                this.SaveSmartInfo(agent);

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

                slime.RotateInBoard(this.OrigOrientation);
                slime.Rotate(this.OrigOrientation, 1f);

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
                this.OrigOrientation = smart.Orientation;

                // Save Direction Slime entered component by/left crossing from
                this.WentTo = LevelBoard.GetDirection(smart.Coords, this.ComponentCoords);

                this.RegisteredConnection = smart.CommunicateInfo(this.ChargeCoords, this.OrigOrientation, this.WentTo);
            }
        }

        private void DeleteSmartInfo(Agent agent)
        {
            // If this is a Crossing
            if (this.Crossing && agent is SmartElectricSlime smart)
            {
                smart.RollBackInfo(this.ChargeCoords, this.OrigOrientation, this.WentTo, this.RegisteredConnection);
            }
        }
        #endregion
    }
}