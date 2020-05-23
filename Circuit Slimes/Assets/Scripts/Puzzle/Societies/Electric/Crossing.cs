using UnityEngine;
using System.Collections.Generic;
using Puzzle.Board;

namespace Puzzle {
    public class Crossing
    {
        public Dictionary<LevelBoard.Directions, int> Available { get; private set; }

        public Dictionary<LevelBoard.Directions, Crossing> Connections { get; private set; }

        
        public Crossing(List<LevelBoard.Directions> availableDirs)
        {
            this.Available = new Dictionary<LevelBoard.Directions, int>();

            foreach(var dir in availableDirs)
            {
                this.Available.Add(dir, 1);
            }

            this.Connections = new Dictionary<LevelBoard.Directions, Crossing>();
        }


        public void Update(List<LevelBoard.Directions> available)
        {
            var toRemove = new List<LevelBoard.Directions>();

            foreach(var dir in this.Available.Keys)
            {
                if (!available.Contains(dir)) toRemove.Add(dir);
            }

            foreach(var dir in toRemove)
            {
                this.Available.Remove(dir);
                this.Connections.Remove(dir);
            }

            foreach(var dir in available)
            {
                if (!this.Available.ContainsKey(dir)) this.Available.Add(dir, 1);
            }
        }


        #region Exploration
        public void MarkExplored(LevelBoard.Directions dir)
        {
            if (!this.Available.ContainsKey(dir)) this.Available.Add(dir, 1);

            this.Available[dir] = this.Available[dir] - 1;
        }

        public void UnmarkExplored(LevelBoard.Directions dir)
        {
            if (!this.Available.ContainsKey(dir)) this.Available.Add(dir, 0);

            this.Available[dir] = this.Available[dir] + 1;
        }


        public List<LevelBoard.Directions> GetUnexplored()
        {
            var unexplored = new List<LevelBoard.Directions>();

            foreach (var pair in this.Available)
            {
                if (pair.Value > 0) unexplored.Add(pair.Key);
            }

            return unexplored;
        }

        public bool FullyExplored()
        {
            foreach (var pair in this.Available)
            {
                if (pair.Value >= 0) return false;
            }

            return true;
        }


        public void ClearExplored()
        {
            this.Available.Clear();
        }

        #endregion


        #region Connection
        public void AddConnection(LevelBoard.Directions dir, Crossing other)
        {
            if (this.Available.ContainsKey(dir) && !this.Connections.ContainsKey(dir))
                this.Connections.Add(dir, other);
        }

        public void RemoveConnection(LevelBoard.Directions dir)
        {
            if (this.Connections.ContainsKey(dir)) this.Connections.Remove(dir);
        }
        #endregion


        #region Utility
        public int GetTotalUtility(LevelBoard.Directions reachedFrom)
        {
            var utility = 0;

            var invDir = LevelBoard.InvertDirection(reachedFrom);

            foreach(var pair in this.Available)
            {
                var dir = pair.Key;

                if (dir == invDir) continue;

                utility += this.GetUtility(dir);
            }

            return utility;
        }

        public int GetUtility(LevelBoard.Directions dir)
        {
            var utility = 0;

            if (!this.Available.ContainsKey(dir)) return utility;

            utility = this.Available[dir];

            if (this.Connections.ContainsKey(dir))
                utility = this.Connections[dir].GetTotalUtility(dir);

            return utility;
        }

        public Dictionary<LevelBoard.Directions, int> GetUtilities()
        {
            var utilities = new Dictionary<LevelBoard.Directions, int>();

            foreach(var pair in this.Available)
            {
                var dir = pair.Key;

                utilities.Add(dir, this.GetUtility(dir));
            }

            return utilities;
        }
        #endregion


        public string AvailableToString()
        {
            var text = "{";

            foreach(var pair in this.Available)
            {
                text += " (" + pair.Key + ", " + pair.Value + ")";
            }

            return text + "}";
        }
    }
}
