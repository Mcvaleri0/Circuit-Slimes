using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Puzzle.Actions;
using Puzzle.Board;

namespace Puzzle.Pieces.Slimes
{
    public class SmarterElectricSlime : SmartElectricSlime
    {
        public Stack<KeyValuePair<Vector2Int, LevelBoard.Directions>> CrossingLog;


        #region === Unity Events ===

        // Start is called before the first frame update
        override protected void Start()
        {
            base.Start();

            this.KnownActions.Clear();
            this.KnownActions.Add(new SmarterElectricMovement());

            this.CrossingLog = new Stack<KeyValuePair<Vector2Int, LevelBoard.Directions>>();
        }
        #endregion


        #region === Agent Methods ===
        override public Action Think()
        {
            return base.Think();
        }
        #endregion


        #region === Personal Methods ===
        public Dictionary<LevelBoard.Directions, int> GetUtilities(Vector2Int crossingCoords)
        {
            return this.Hivemind.GetUtilities(crossingCoords);
        }

        public void RegisterCrossingConnection(Vector2Int start, LevelBoard.Directions dir, Vector2Int end)
        {
            this.Hivemind.AddCrossingConnection(start, dir, end);
        }

        public void UnregisterCrossingConnection(Vector2Int start, LevelBoard.Directions dir, Vector2Int end)
        {
            this.Hivemind.RemoveCrossingConnection(start, dir, end);
        }
        #endregion
    }
}