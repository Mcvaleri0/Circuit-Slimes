using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Slimes
{
    public class SmartElectricSlime : ElectricSlime
    {
        public List<SmartElectricSlime> Society { get; private set; }

        public Dictionary<Vector2Int, List<LevelBoard.Directions>> ExploredPaths { get; private set; }

        #region === Unity Events ===

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Add(new Charge());
            this.KnownActions.Add(new SmartElectricMovement());

            this.Stats = new Statistics(10, 10, 10);

            this.InitializeSociety();
        }
        #endregion


        #region === Agent Methods ===
        override public Action Think()
        {
            return base.Think();
        }
        #endregion


        #region === Personal Methods ===
        private void InitializeSociety()
        {
            foreach(var agent in this.Puzzle.Agents)
            {
                if(agent != this && agent is SmartElectricSlime slime)
                {
                    if(slime.Society != null)
                    {
                        this.Society = slime.Society;
                    }

                    if(slime.ExploredPaths != null)
                    {
                        this.ExploredPaths = slime.ExploredPaths;
                    }
                }
            }

            if (this.Society == null) this.Society = new List<SmartElectricSlime>();

            this.Society.Add(this);

            if (this.ExploredPaths == null) this.ExploredPaths = new Dictionary<Vector2Int, List<LevelBoard.Directions>>();
        }

        public void RegisterExploredPath(Vector2Int crossingCoords, LevelBoard.Directions chosenDirection)
        {
            this.ExploredPaths.TryGetValue(crossingCoords, out var explored);

            if (explored == null)
            {
                explored = new List<LevelBoard.Directions>();

                this.ExploredPaths.Add(crossingCoords, explored);
            }

            if (!explored.Contains(chosenDirection)) explored.Add(chosenDirection);
        }
        #endregion
    }
}