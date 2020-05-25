using UnityEngine;
using System.Collections.Generic;
using Puzzle.Board;

namespace Puzzle {
    public class Crossing
    {
        public Vector2Int Coords { get; private set; }

        public Dictionary<LevelBoard.Directions, int> Explorations { get; private set; }

        public Dictionary<LevelBoard.Directions, Crossing> Connections { get; private set; }

        public List<LevelBoard.Directions> DeadEnds { get; private set; }

        
        public Crossing(Vector2Int coords, List<LevelBoard.Directions> availableDirs)
        {
            this.Coords = coords;

            this.Explorations = new Dictionary<LevelBoard.Directions, int>();

            foreach(var dir in availableDirs)
            {
                this.Explorations.Add(dir, 0);
            }

            this.Connections = new Dictionary<LevelBoard.Directions, Crossing>();

            this.DeadEnds = new List<LevelBoard.Directions>();
        }


        public void Update(List<LevelBoard.Directions> available)
        {
            var toRemove = new List<LevelBoard.Directions>();

            foreach(var dir in this.Explorations.Keys)
            {
                if (!available.Contains(dir)) toRemove.Add(dir);
            }

            foreach(var dir in toRemove)
            {
                this.Explorations.Remove(dir);
                this.Connections.Remove(dir);
                if(this.DeadEnds.Contains(dir)) this.DeadEnds.Remove(dir);
            }

            foreach(var dir in available)
            {
                if (!this.Explorations.ContainsKey(dir)) this.Explorations.Add(dir, 0);
            }
        }


        #region Exploration
        public void MarkExplored(LevelBoard.Directions dir)
        {
            if (!this.Explorations.ContainsKey(dir)) this.Explorations.Add(dir, 0);

            this.Explorations[dir] = this.Explorations[dir] + 1;
        }

        public void MarkDeadEnd(LevelBoard.Directions dir)
        {
            if (!this.Explorations.ContainsKey(dir)) this.Explorations.Add(dir, 0);

            if(!this.DeadEnds.Contains(dir)) this.DeadEnds.Add(dir);
        }

        public void UnmarkExplored(LevelBoard.Directions dir)
        {
            if (!this.Explorations.ContainsKey(dir)) this.Explorations.Add(dir, 1);

            this.Explorations[dir] = this.Explorations[dir] - 1;
        }

        public void UnmarkDeadEnd(LevelBoard.Directions dir)
        {
            if (!this.Explorations.ContainsKey(dir)) this.Explorations.Add(dir, 1);

            if(this.DeadEnds.Contains(dir)) this.DeadEnds.Remove(dir);
        }


        public List<LevelBoard.Directions> GetUnexplored()
        {
            var unexplored = new List<LevelBoard.Directions>();

            foreach (var pair in this.Explorations)
            {
                if (pair.Value == 0) unexplored.Add(pair.Key);
            }

            return unexplored;
        }

        public int GetTotalUnexplored(Vector2Int origin)
        {
            var total = 0;

            foreach (var pair in this.Explorations)
            {
                var dir = pair.Key;

                // How many explorations have there been
                var explorations = pair.Value;

                // If it has been explored and leads to a crossing
                if (explorations > 0 && this.Connections.ContainsKey(dir))
                {
                    var connectedCrossing = this.Connections[dir];

                    if (origin == connectedCrossing.Coords) continue;

                    // Get the to explore total of that crossing
                    total += connectedCrossing.GetTotalUnexplored(this.Coords);
                }

                // Unexplored
                else if (explorations == 0) total += 1;
            }

            return total;
        }
        #endregion


        #region Connection
        public bool AddConnection(LevelBoard.Directions dir, Crossing other)
        {
            if (this.Explorations.ContainsKey(dir) && !this.Connections.ContainsKey(dir))
            {
                this.Connections.Add(dir, other);
                return true;
            }

            return false;
        }

        public void RemoveConnection(LevelBoard.Directions dir)
        {
            if (this.Connections.ContainsKey(dir)) this.Connections.Remove(dir);
        }
        #endregion


        #region Utility
        public int GetUtility(LevelBoard.Directions dir)
        {
            var utility = int.MinValue;

            if (!this.Explorations.ContainsKey(dir)) return utility;

            if (this.DeadEnds.Contains(dir)) return int.MinValue;

            utility = 1 - this.Explorations[dir]; // How many have gone in that direction

            if (this.Connections.ContainsKey(dir))
                utility += this.Connections[dir].GetTotalUnexplored(this.Coords); // Paths unexplored

            return utility;
        }

        public Dictionary<LevelBoard.Directions, int> GetUtilities()
        {
            var utilities = new Dictionary<LevelBoard.Directions, int>();

            foreach(var pair in this.Explorations)
            {
                var dir = pair.Key;

                utilities.Add(dir, this.GetUtility(dir));
            }

            return utilities;
        }
        #endregion


        public string ExplorationsToString()
        {
            var text = "{";

            foreach(var pair in this.Explorations)
            {
                text += " (" + pair.Key + ", " + pair.Value + ")";
            }

            return text + "}";
        }
    }
}
