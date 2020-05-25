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

        public bool DeadEnds;

        public bool PathBlocked;

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

        public void RegisterDeadEnd(Vector2Int crossingCoords, LevelBoard.Directions dir)
        {
            this.Hivemind.RegisterDeadEnd(crossingCoords, dir);
        }

        public void UnregisterDeadEnd(Vector2Int crossingCoords, LevelBoard.Directions dir)
        {
            this.Hivemind.UnregisterDeadEnd(crossingCoords, dir);
        }

        public override bool CommunicateInfo(Vector2Int crossingCoords, LevelBoard.Directions dirIn, LevelBoard.Directions dirOut)
        {
            base.CommunicateInfo(crossingCoords, dirIn, dirOut);

            var connected = false;

            // If the Slime has come from a Crossing
            if (this.CrossingLog.Count > 0)
            {
                var last = this.CrossingLog.Peek(); // Get last visited Crossing

                // If the last visited Crossing is not the present Crossing
                if (last.Key != crossingCoords)
                {
                    // Add this Crossing as the last one's child
                    connected = this.RegisterCrossingConnection(last.Key, last.Value, crossingCoords, dirIn);
                }
                else if (this.DeadEnds && !this.PathBlocked)
                {
                    this.RegisterDeadEnd(crossingCoords, LevelBoard.InvertDirection(dirIn));

                    connected = true;
                }

                this.PathBlocked = false;
            }

            // Save this Crossing as its most recently visited, and Direction from which it left
            this.CrossingLog.Push(new KeyValuePair<Vector2Int, LevelBoard.Directions>(crossingCoords, dirOut));

            return connected;
        }

        public override void RollBackInfo(Vector2Int crossingCoords, LevelBoard.Directions dirIn,
            LevelBoard.Directions dirOut, bool connected)
        {
            // Pop last Log entry
            this.CrossingLog.Pop();

            // If the Slime had come from a Crossing and a connection was made
            if (this.CrossingLog.Count > 0 && connected)
            {
                var last = this.CrossingLog.Peek(); // Get last visited Crossing

                // If the last visited Crossing is not the present Crossing
                if (last.Key != crossingCoords)
                {
                    // Disconnect the Crossings
                    this.UnregisterCrossingConnection(last.Key, last.Value, crossingCoords, dirIn);
                }
                else if (this.DeadEnds)
                {
                    this.UnregisterDeadEnd(crossingCoords, LevelBoard.InvertDirection(dirIn));
                }
            }

            base.RollBackInfo(crossingCoords, dirIn, dirOut, connected);
        }


        protected bool RegisterCrossingConnection(Vector2Int start, LevelBoard.Directions dirOut,
            Vector2Int end, LevelBoard.Directions dirIn)
        {
            return this.Hivemind.AddCrossingConnection(start, dirOut, end, dirIn);
        }

        protected void UnregisterCrossingConnection(Vector2Int start, LevelBoard.Directions dirOut,
            Vector2Int end, LevelBoard.Directions dirIn)
        {
            this.Hivemind.RemoveCrossingConnection(start, dirOut, end, dirIn);
        }
        #endregion
    }
}