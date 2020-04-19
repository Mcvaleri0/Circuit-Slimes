using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces;
using Puzzle.Pieces.Slimes;
using Puzzle.Pieces.Components;

namespace Puzzle.Actions
{
    public class Discharge : Action
    {
        private Vector2Int TargetCoords;

        public ElectricSlime Charge { get; private set; }

        public Discharge() { }

        public Discharge(ElectricSlime charge, Vector2Int tCoords) 
        {
            this.Charge = charge;

            this.TargetCoords = tCoords;
        }


        #region Action Methods

        override public Action Available(Agent agent)
        {
            // If the Agent is a Component
            if (agent is CircuitComponent component)
            {
                // If the Component is connected and has surplus charge
                if (component.Connections.Count > 0 && 
                    component.Stats.Food > component.Stats.MaxFood &&
                    component.Stats.Food > 0)
                {
                    ElectricSlime charge = null;

                    // If there are Slimes stored
                    if (component.Charges.Count > 0)
                    {
                        charge = component.Charges.Peek(); // Get the next Slime to be discharged

                        // Find the approapriate connection to discharge it to
                        var inDir  = LevelBoard.GetDirection(charge.Coords, component.Coords);
                        var coords = component.RouteEnergy(inDir);

                        // If an approapriate connection was found
                        if(coords.x != -1)
                        {
                            return new Discharge(charge, coords); // Return the potential Action
                        }
                    }

                    // If there are no Slimes stored
                    else
                    {
                        var coords = component.RouteEnergy(LevelBoard.Directions.South);

                        return new Discharge(charge, coords);
                    }
                }
            }

            return null;
        }

        override public bool Execute(Agent agent)
        {
            if (agent is CircuitComponent component) {
                if (this.Charge == null)
                {
                    LevelBoard.Directions ori = LevelBoard.GetDirection(component.Coords, this.TargetCoords);

                    this.Charge = (ElectricSlime) Piece.
                        CreatePiece(component.transform.parent, component.Puzzle,
                        this.TargetCoords, "ElectricSlime", ori, component.Turn + 1);

                    component.Puzzle.AddPiece(this.Charge);
                }
                else
                {
                    component.ReleaseCharge(this.TargetCoords);
                }
                
                component.Stats.Food--;
            }

            return true;
        }
        #endregion


        #region === AUX Methods ===
        private static Vector2Int ConnectionAvailable(CircuitComponent component, LevelBoard.Directions inputDirection)
        {
            var start = ((int) inputDirection) / 2;

            for (var i = 0; i < 4; i++)
            {
                var ind = ((start + i + 3) % 4) * 2;
                var dir = (LevelBoard.Directions) ind;

                if(component.Connections.TryGetValue(dir, out var coords))
                {
                    return coords;
                }
            }

            return new Vector2Int(-1, -1);
        }
        #endregion
    }
}