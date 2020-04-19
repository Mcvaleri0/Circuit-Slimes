using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Pieces.Slimes;
using Puzzle.Board;
using Puzzle.Actions;

namespace Puzzle.Pieces.Components
{
    public class CircuitComponent : Agent
    {
        public int MaxCharges = 0;

        public int StartingCharges = 1;

        public Queue<ElectricSlime> Charges { get; protected set; }

        public Dictionary<LevelBoard.Directions, Vector2Int> Connections { get; protected set; }


        // Init Method
        public void Initialize(Puzzle puzzle, Vector2Int coords, ComponentTypes type,
            LevelBoard.Directions ori = 0, int turn = 0)
        {
            base.Initialize(puzzle, coords, Categories.Component, ori, turn);

            this.ComponentType = type;

            this.Charges = new Queue<ElectricSlime>();

            this.Connections = new Dictionary<LevelBoard.Directions, Vector2Int>();
        }


        #region === Unity Events ===
        // Start is called before the first frame update
        new protected virtual void Start()
        {
            base.Start();
        }
        #endregion


        #region === Agent Methods ===
        public override Action Think()
        {
            this.UpdateConnections();

            return base.Think();
        }
        #endregion

        #region === Component Methods ===
        protected void UpdateConnections()
        {
            this.Connections.Clear();

            for(var i = 0; i < 4; i++)
            {
                int dirId = i * 2;

                Tile tile = this.Board.GetTile(this.Coords + LevelBoard.DirectionalVectors[dirId]);

                if(tile != null && tile.Type == Tile.Types.Solder)
                {
                    this.Connections.Add((LevelBoard.Directions) dirId, tile.Coords);
                }
            }
        }
 
        public Vector2Int RouteEnergy(LevelBoard.Directions entryDir)
        {
            int dirId = (int) entryDir;

            for(int i = 0; i < 4; i++)
            {
                LevelBoard.Directions checkDir = (LevelBoard.Directions) ((dirId + (i + 1) * 2) % 8);

                if(this.Connections.ContainsKey(checkDir))
                {
                    return this.Connections[checkDir];
                }
            }

            return new Vector2Int(-1,-1);
        }

        public Vector2Int RouteEnergy(Vector2Int entryPoint)
        {
            LevelBoard.Directions entryDir = LevelBoard.GetDirection(entryPoint, this.Coords);

            return this.RouteEnergy(entryDir);
        }

        public void ReceiveCharge(ElectricSlime charge)
        {
            if(charge.Active)
            {
                charge.Deactivate();
            }

            this.Charges.Enqueue(charge);
        }

        public ElectricSlime ReleaseCharge(Vector2Int coords)
        {
            ElectricSlime charge = this.Charges.Dequeue();

            if (charge != null)
            {
                charge.Reactivate(coords);
            }

            return charge;
        }
        #endregion
    }
}

