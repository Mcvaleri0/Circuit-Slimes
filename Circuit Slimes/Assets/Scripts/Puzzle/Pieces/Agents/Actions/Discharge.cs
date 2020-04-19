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
        public ElectricSlime Charge { get; private set; }

        public Discharge() { }

        public Discharge(ElectricSlime charge) 
        {
            this.Charge = charge;
        }

        #region Action Methods

        override public Action Available(Agent agent)
        {
            if (agent is CircuitComponent component)
            {
                if (component.Connections.Count > 0 && component.Stats.Food > component.Stats.MaxFood)
                {
                    ElectricSlime charge = component.Charges.Peek();

                    if (charge == null)
                    {
                        charge = (ElectricSlime)Piece.CreatePiece(component.transform.parent, component.Puzzle, component.Coords + Vector2Int.down, "ElectricSlime");

                        component.Puzzle.AddPiece(charge);

                        component.ReceiveCharge(charge);
                    }

                    return new Discharge(charge);
                }
            }

            return null;
        }

        public bool Execute(CircuitComponent agent)
        {
            LevelBoard.Directions inputDir = LevelBoard.GetDirection(this.Charge.Coords, agent.Coords);

            Vector2Int outputCoords = agent.RouteEnergy(inputDir);

            agent.ReleaseCharge(outputCoords);

            agent.Stats.Food--;

            return true;
        }
        #endregion
    }
}