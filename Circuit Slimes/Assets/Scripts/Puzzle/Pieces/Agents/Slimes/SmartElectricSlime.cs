using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Slimes
{
    public class SmartElectricSlime : ElectricSlime
    {
        public ElectricHivemind Hivemind { get; private set; }

        #region === Unity Events ===

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Clear();
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
            var parentObj = transform.parent.gameObject;

            parentObj.TryGetComponent<ElectricHivemind>(out var hivemind);

            if(hivemind == null)
            {
                this.Hivemind = (ElectricHivemind) parentObj.AddComponent(typeof(ElectricHivemind));
            }
            else
            {
                this.Hivemind = hivemind;
            }
        }

        public void UpdateCrossing(Vector2Int crossingCoords, List<LevelBoard.Directions> available)
        {
            this.Hivemind.UpdateCrossing(crossingCoords, available);
        }


        public void RegisterExploredPath(Vector2Int crossingCoords, LevelBoard.Directions chosenDirection)
        {
            this.Hivemind.RegisterExplored(crossingCoords, chosenDirection);
        }

        public void UnregisterExploredPath(Vector2Int crossingCoords, LevelBoard.Directions chosenDirection)
        {
            this.Hivemind.UnregisterExplored(crossingCoords, chosenDirection);
        }


        public List<LevelBoard.Directions> GetUnexploredPaths(Vector2Int crossingCoords)
        {
            return this.Hivemind.GetUnexplored(crossingCoords);
        }
        #endregion
    }
}