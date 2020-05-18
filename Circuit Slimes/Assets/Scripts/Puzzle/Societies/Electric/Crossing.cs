using UnityEngine;
using System.Collections.Generic;
using Puzzle.Board;

namespace Puzzle {
    public class Crossing
    {
        public List<LevelBoard.Directions> Available { get; private set; }

        public List<LevelBoard.Directions> Explored { get; private set; }

        
        public Crossing(List<LevelBoard.Directions> available)
        {
            this.Available = available;

            this.Explored = new List<LevelBoard.Directions>();
        }


        public void Update(List<LevelBoard.Directions> available)
        {
            this.Available = available;

            var newExplored = new List<LevelBoard.Directions>();

            foreach(var dir in this.Available)
            {
                if(this.Explored.Contains(dir))
                {
                    newExplored.Add(dir);
                }
            }

            if (newExplored.Count == this.Available.Count) newExplored.Clear();

            this.Explored = newExplored;
        }


        public void MarkExplored(LevelBoard.Directions dir)
        {
            if (!this.Explored.Contains(dir)) this.Explored.Add(dir);

            if (this.Explored.Count == this.Available.Count) this.Explored.Clear();
        }

        public List<LevelBoard.Directions> GetUnexplored()
        {
            var unexplored = new List<LevelBoard.Directions>();

            foreach(var dir in this.Available)
            {
                if (!this.Explored.Contains(dir)) unexplored.Add(dir);
            }

            return unexplored;
        }

        public void UnmarkExplored(LevelBoard.Directions dir)
        {
            if (this.Explored.Contains(dir)) this.Explored.Remove(dir);
        }
    }
}
