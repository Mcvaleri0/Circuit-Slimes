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

            this.ExploredPaths = new Dictionary<Vector2Int, List<LevelBoard.Directions>>();
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
            this.Crossings[crossingCoords].Update(available);
        }

        public void RegisterExplored(Vector2Int crossingCoords, LevelBoard.Directions exploredPath)
        {
            this.Crossings.TryGetValue(crossingCoords, out var crossing);

            if (crossing == null)
            {
                crossing = new Crossing();

                this.ExploredPaths.Add(crossingCoords, explored);
            }

            if (!explored.Contains(exploredPath)) explored.Add(exploredPath);

            if (explored.Count == 4) explored.Clear();
        }

        public List<LevelBoard.Directions> GetUnexplored(Vector2Int crossingCoords)
        {
            this.ExploredPaths.TryGetValue(crossingCoords, out var explored);

            return explored;
        }

        public void UnregisterExplored(Vector2Int crossingCoords, LevelBoard.Directions exploredPath)
        {
            this.ExploredPaths.TryGetValue(crossingCoords, out var explored);

            if (explored != null && explored.Contains(exploredPath))
            {
                explored.Remove(exploredPath);
            }
        }
        #endregion
    }
}