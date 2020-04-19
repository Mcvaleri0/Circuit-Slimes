using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Board;
using Puzzle.Pieces.Slimes;
using CircuitComponent = Puzzle.Pieces.Components.CircuitComponent;

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

        public Action Available(CircuitComponent agent)
        {
            if(agent.Connections.Count > 0 && agent.Stats.Food > agent.Stats.MaxFood)
            {
                ElectricSlime charge = agent.Charges.Peek();

                if(charge == null)
                {
                    charge = (ElectricSlime) Piece.CreatePiece(agent.transform.parent, agent.Puzzle, agent.Coords + Vector2Int.down, "ElectricSlime");

                    agent.Puzzle.AddPiece(charge);

                    agent.ReceiveCharge(charge);
                }

                return new Discharge(charge);
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