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

        protected bool On = false;

        public List<ElectricSlime> Charges { get; protected set; }

        public Dictionary<LevelBoard.Directions, Vector2Int> Connections { get; protected set; }

        // Init Method
        public override void Initialize(Puzzle puzzle, Vector2Int coords, Characteristics Characterization,
            LevelBoard.Directions ori = 0, int turn = 0)
        {
            base.Initialize(puzzle, coords, Characterization, ori, turn);

            this.Charges = new List<ElectricSlime>();

            this.Connections = new Dictionary<LevelBoard.Directions, Vector2Int>();
        }


        #region === Unity Events ===
        // Start is called before the first frame update
        new protected virtual void Start()
        {
            base.Start();

            this.Stats = new Statistics(10, 0, this.MaxCharges)
            {
                Food = this.StartingCharges
            };
        }

        new protected virtual void Update()
        {
            base.Update();

            if (!this.On && this.Stats.Food >= this.Stats.MaxFood) TurnOn();
            if (this.On  && this.Stats.Food <  this.Stats.MaxFood) TurnOff();
        }
        #endregion


        #region === Agent Methods ===
        override public Action Think()
        {
            this.UpdateConnections();

            return base.Think();
        }
        #endregion


        #region === Component Methods ===
        protected virtual void TurnOn()
        {
            this.On = true;
        }

        protected virtual void TurnOff()
        {
            this.On = false;
        }

        virtual protected void UpdateConnections()
        {
            var tile = this.Puzzle.GetTile(this.Coords);

            if (tile == null || tile.Type != Tile.Types.Solder) return;

            this.Connections.Clear();

            for(var i = 0; i < 4; i++)
            {
                int dirId = i * 2;

                tile = this.Puzzle.GetTile(this.Coords + LevelBoard.DirectionalVectors[dirId]);

                if(tile != null && tile.Type == Tile.Types.Solder)
                {
                    this.Connections.Add((LevelBoard.Directions) dirId, tile.Coords);
                }
            }
        }
 
        public Vector2Int RouteEnergy(LevelBoard.Directions entryDir)
        {
            int dirId = ((int) entryDir + 6) % 8;

            for(int i = 0; i < 4; i++)
            {
                LevelBoard.Directions checkDir = (LevelBoard.Directions) dirId;

                if(this.Connections.ContainsKey(checkDir) && this.IsFree(this.Connections[checkDir]))
                {
                    return this.Connections[checkDir];
                }

                dirId += 2;
                dirId %= 8;
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
            if(charge.Active) charge.Deactivate();

            this.Charges.Add(charge);
        }

        public ElectricSlime ReleaseCharge(ElectricSlime charge, Vector2Int coords)
        {
            if (charge != null && this.Charges.Contains(charge))
            {
                this.Charges.Remove(charge);

                charge.Reactivate(coords);

                return charge;
            }

            return null;
        }

        public ElectricSlime ReleaseCharge(Vector2Int coords)
        {
            ElectricSlime charge = this.Charges[0];

            return ReleaseCharge(charge, coords);
        }
        #endregion
    }
}

