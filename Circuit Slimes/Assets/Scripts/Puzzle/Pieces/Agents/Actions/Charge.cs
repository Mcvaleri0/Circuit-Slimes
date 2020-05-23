using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Slimes;
using Puzzle.Pieces.Components;

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
                if(this.Crossing && agent is SmartElectricSlime slime)
                {
                    this.CameFrom = (LevelBoard.Directions)((((int) slime.Orientation) + 4) % 8);

                    slime.RegisterExploredPath(slime.Coords, this.CameFrom);

                    this.EnteredBy = LevelBoard.GetDirection(slime.Coords, this.ComponentCoords);
                    slime.RegisterExploredPath(slime.Coords, this.EnteredBy);

                    if (slime is SmarterElectricSlime smarter)
                        smarter.CrossingLog.Push(new KeyValuePair<Vector2Int, LevelBoard.Directions>(slime.Coords, this.EnteredBy));
                }

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

                if(this.Crossing && slime is SmartElectricSlime smart)
                {
                    smart.UnregisterExploredPath(this.ChargeCoords, this.CameFrom);
                    smart.UnregisterExploredPath(slime.Coords, this.EnteredBy);

                    if (smart is SmarterElectricSlime smarter) smarter.CrossingLog.Pop();
                }

                return true;
            }

            return false;
        }
        #endregion
    }
}