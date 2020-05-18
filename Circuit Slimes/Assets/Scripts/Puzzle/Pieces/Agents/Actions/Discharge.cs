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

        private bool NewCharge = false;

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
                if (component.Connections.Count > 0 && component.Stats.Food > 0)
                {
                    ElectricSlime charge = null;

                    // If there are Slimes stored
                    if (component.Charges.Count > 0)
                    {
                        charge = component.Charges[component.Charges.Count - 1]; // Get the next Slime to be discharged

                        // Find the approapriate connection to discharge it to
                        var componentExitCoords = component.Coords + component.Footprint[component.Footprint.Length - 1];
                        LevelBoard.Directions inDir = LevelBoard.GetDirection(charge.Coords, component.Coords);
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
                        var coords = component.RouteEnergy(LevelBoard.Directions.North);

                        if(coords.x != -1)
                        {
                            return new Discharge(charge, coords);
                        }
                    }
                }
            }

            return null;
        }

        public override bool Confirm(Agent agent)
        {
            if (agent is CircuitComponent component)
            {
                if (component.IsFree(this.TargetCoords))
                {
                    if (component.Charges.Count == 0)
                    {
                        var componentFootprint = component.GetFootprint();
                        LevelBoard.Directions ori = LevelBoard.GetDirection(componentFootprint[componentFootprint.Length - 1], this.TargetCoords);

                        var piece = component.CreatePiece(new Piece.Caracteristics("ElectricSlime"),
                            this.TargetCoords, ori, component.Turn + 1);

                        this.Charge = (ElectricSlime) piece;

                        this.NewCharge = true;
                    }
                    else
                    {
                        component.ReleaseCharge(this.Charge, this.TargetCoords);

                        var outDir = LevelBoard.GetDirection(agent.Coords, this.TargetCoords);

                        this.Charge.RotateInBoard(outDir);
                        this.Charge.Rotate(outDir, 1f);
                    }

                    this.Charge.Hide();

                    return true;
                }
            }

            return false;
        }

        override public bool Execute(Agent agent)
        {
            if (agent is CircuitComponent component) {
                this.Charge.Reveal();
                
                component.Stats.Food--;
            }

            return true;
        }

        override public bool Undo(Agent agent)
        {
            if (agent is CircuitComponent component)
            {
                if (this.NewCharge)
                {
                    component.DestroyPiece(this.Charge);
                }
                else
                {
                    component.ReceiveCharge(this.Charge);
                }

                component.Stats.Food++;
            }

            return true;
        }
        #endregion
    }
}