using UnityEngine;
using System.Collections.Generic;
using Puzzle.Board;

namespace Puzzle {
    public class Crossing
    {
        public Dictionary<LevelBoard.Directions, int> Available { get; private set; }

        public Dictionary<LevelBoard.Directions, Crossing> Children { get; private set; }

        
        public Crossing(List<LevelBoard.Directions> availableDirs)
        {
            this.Available = new Dictionary<LevelBoard.Directions, int>();

            foreach(var dir in availableDirs)
            {
                this.Available.Add(dir, 1);
            }

            this.Children = new Dictionary<LevelBoard.Directions, Crossing>();
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
                this.Children.Remove(dir);
            }

            foreach(var dir in available)
            {
                if (!this.Available.ContainsKey(dir)) this.Available.Add(dir, 1);
            }
        }


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


        public void AddChild(LevelBoard.Directions dir, Crossing child)
        {
            if (this.Available.ContainsKey(dir) && !this.Children.ContainsKey(dir))
                this.Children.Add(dir, child);
        }

        public void RemoveChild(LevelBoard.Directions dir)
        {
            if (this.Children.ContainsKey(dir)) this.Children.Remove(dir);
        }


        public List<LevelBoard.Directions> GetUnexplored()
        {
            var unexplored = new List<LevelBoard.Directions>();

            foreach (var pair in this.Available)
            {
                if (pair.Value >= 0) unexplored.Add(pair.Key);
            }

            return unexplored;
        }

        public int GetTotalUtility()
        {
            var utility = 0;

            foreach(var pair in this.GetUtilities())
            {
                var dir = pair.Key;

                if (this.Children.ContainsKey(dir))
                {
                    utility += this.Children[dir].GetTotalUtility();
                }
                else
                {
                    utility += pair.Value;
                }
            }

            return utility;
        }

        public Dictionary<LevelBoard.Directions, int> GetUtilities()
        {
            var utilities = new Dictionary<LevelBoard.Directions, int>();

            foreach(var pair in this.Available)
            {
                var dir = pair.Key;

                var utility = pair.Value;

                if (this.Children.ContainsKey(dir))
                    utility = this.Children[dir].GetTotalUtility();

                utilities.Add(dir, utility);
            }

            return utilities;
        }


        public bool FullyExplored()
        {
            foreach(var pair in this.Available)
            {
                if (pair.Value >= 0) return false;
            }

            return true;
        }

        public void ClearExplored()
        {
            this.Available.Clear();
        }


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
