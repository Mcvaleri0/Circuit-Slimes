using UnityEngine;
using System.Collections.Generic;
using Puzzle.Pieces.Slimes;
using Puzzle.Board;

namespace Puzzle {
    public class ElectricHivemind : MonoBehaviour
    {
        public Puzzle Puzzle { get; private set; }
    
        public List<SmartElectricSlime> Society { get; private set; }

        public Dictionary<Vector2Int, Crossing> Crossings { get; private set; }


        #region === Unity Methods ===
        // Use this for initialization
        void Start()
        {
            this.Puzzle = transform.parent.GetComponent<Puzzle>();

            this.Society = new List<SmartElectricSlime>();

            this.Crossings = new Dictionary<Vector2Int, Crossing>();
        }

        // Update is called once per frame
        void Update()
        {
            this.UpdateSociety();
        }
        #endregion


        #region === Society Methods ===
        public void UpdateSociety()
        {
            this.Society.Clear();

            foreach(var agent in this.Puzzle.Agents)
            {
                if (agent is SmartElectricSlime slime) this.Society.Add(slime);
            }
        }
        #endregion


        #region === Exploration Methods ===
        public void UpdateCrossing(Vector2Int crossingCoords, List<LevelBoard.Directions> available)
        {
            bool _new;

            if (!this.Crossings.ContainsKey(crossingCoords))
                this.Crossings.Add(crossingCoords, new Crossing(crossingCoords, available)); _new = true;

            var crossing = this.Crossings[crossingCoords];

            if(!_new) crossing.Update(available);
        }


        public void RegisterExplored(Vector2Int crossingCoords, LevelBoard.Directions exploredPath)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            // Should never happen
            if (crossing == null) return;

            crossing.MarkExplored(exploredPath);

            /*Debug.Log("Register Explored - " + crossingCoords + " -> " + exploredPath);

            var utilities = crossing.GetUtilities();

            var utils = "{";

            foreach (var pair in utilities)
            {
                utils += " (" + pair.Key + ", " + pair.Value + ")";
            }

            Debug.Log(crossingCoords + " Utils: " + utils);*/
        }

        public void RegisterDeadEnd(Vector2Int crossingCoords, LevelBoard.Directions deadEnd)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            // Should never happen
            if (crossing == null) return;

            crossing.MarkDeadEnd(deadEnd);

            /*Debug.Log("Register Dead End - " + crossingCoords + " -> " + deadEnd);

            var utilities = crossing.GetUtilities();

            var utils = "{";

            foreach (var pair in utilities)
            {
                utils += " (" + pair.Key + ", " + pair.Value + ")";
            }

            Debug.Log(crossingCoords + " Utils: " + utils);*/
        }

        public void UnregisterExplored(Vector2Int crossingCoords, LevelBoard.Directions exploredPath)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            //Should never happen
            if (crossing == null) return;

            crossing.UnmarkExplored(exploredPath);

            /*Debug.Log("Unregister Explored - " + crossingCoords + " -> " + exploredPath);

            var utilities = crossing.GetUtilities();

            var utils = "{";

            foreach (var pair in utilities)
            {
                utils += " (" + pair.Key + ", " + pair.Value + ")";
            }

            Debug.Log(crossingCoords + " Utils: " + utils);*/
        }

        public void UnregisterDeadEnd(Vector2Int crossingCoords, LevelBoard.Directions deadEnd)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            // Should never happen
            if (crossing == null) return;

            crossing.UnmarkDeadEnd(deadEnd);

            /*Debug.Log("Unregister Dead End - " + crossingCoords + " -> " + deadEnd);

            var utilities = crossing.GetUtilities();

            var utils = "{";

            foreach (var pair in utilities)
            {
                utils += " (" + pair.Key + ", " + pair.Value + ")";
            }

            Debug.Log(crossingCoords + " Utils: " + utils);*/
        }


        public List<LevelBoard.Directions> GetUnexplored(Vector2Int crossingCoords)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            if (crossing == null) return null;

            return crossing.GetUnexplored();
        }

        public Dictionary<LevelBoard.Directions, int> GetUtilities(Vector2Int crossingCoords)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            if (crossing == null) return null;

            var utilities = crossing.GetUtilities();
            /*
            var utils = "{";

            foreach(var pair in utilities)
            {
                utils += " (" + pair.Key + ", " + pair.Value + ")";
            }

            Debug.Log(crossingCoords + " Utils: " + utils);*/

            return utilities;
        }


        public bool AddCrossingConnection(Vector2Int startCoords, LevelBoard.Directions dirOut,
            Vector2Int endCoords, LevelBoard.Directions dirIn)
        {
            if(this.Crossings.TryGetValue(startCoords, out var start) &&
               this.Crossings.TryGetValue(endCoords, out var end))
            {
                var first  = start.AddConnection(dirOut, end);

                var invDir = LevelBoard.InvertDirection(dirIn);

                var second = end.AddConnection(invDir, start);

                //if(first)  Debug.Log("Added connection between " + startCoords + " through " + dirOut + " to " + endCoords);
                //if(second) Debug.Log("Added connection between " + endCoords   + " through " + invDir + " to " + startCoords);

                return first || second;                
            }

            return false;
        }

        public void RemoveCrossingConnection(Vector2Int startCoords, LevelBoard.Directions dirOut,
            Vector2Int endCoords, LevelBoard.Directions dirIn)
        {
            if (this.Crossings.TryGetValue(startCoords, out var start) &&
               this.Crossings.TryGetValue(endCoords, out var end))
            {
                start.RemoveConnection(dirOut);

                var invDir = LevelBoard.InvertDirection(dirIn);

                end.RemoveConnection(invDir);

                Debug.Log("Removed connection between " + startCoords + " through " + dirOut + " to " + endCoords);
                Debug.Log("Removed connection between " + endCoords   + " through " + invDir + " to " + startCoords);
            }
        }
        #endregion
    }
}