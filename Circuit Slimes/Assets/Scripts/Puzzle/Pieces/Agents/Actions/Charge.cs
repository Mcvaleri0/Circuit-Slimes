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

        public Charge() { }

        public Charge(CircuitComponent component) 
        {
            this.Component = component;
        }

        #region Action Methods

        override public Action Available(Agent agent)
        {
            if (agent is ElectricSlime slime)
            {
                var start = ((int)slime.Orientation) / 2;

                for (var i = 0; i < 4; i++)
                {
                    var ind = ((start + i + 3) % 4) * 2;

                    var coords = slime.Coords + LevelBoard.DirectionalVectors[ind];

                    var piece = slime.Board.GetPiece(coords);
                    var tile  = slime.Board.GetTile(coords);

                    // If there's a Solder Tile
                    if(tile != null && tile.Type == Tile.Types.Solder)
                    {
                        // If there's a Circuit Component
                        if(piece != null && piece.Caracterization.Category == Piece.Categories.Component)
                        {
                            // If the component is not at full charge
                            if (piece is CircuitComponent component &&
                                component.Stats.Food < component.Stats.MaxFood + 1)
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

        override public bool Execute(Agent agent)
        {
            if (agent is ElectricSlime slime) {
                this.Component.ReceiveCharge(slime);

                this.Component.Stats.Food++;
            }

            return true;
        }

        override public bool Undo(Agent agent)
        {
            if (agent is ElectricSlime slime)
            {
                this.Component.ReleaseCharge(slime, slime.Coords);

                slime.Orientation = (LevelBoard.Directions) (((int) slime.Orientation + 4) % 8);

                this.Component.Stats.Food--;

                return true;
            }

            return false;
        }
        #endregion
    }
}