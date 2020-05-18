using UnityEngine;
using System.Collections.Generic;
using Puzzle.Pieces.Slimes;
using Puzzle.Board;

namespace Puzzle {
    public class ElectricHivemind : MonoBehaviour
    {
        public Puzzle Puzzle { get; private set; }
    
        [SerializeField]
        public List<SmartElectricSlime> Society { get; private set; }

        [SerializeField]
        public Dictionary<Vector2Int, List<LevelBoard.Directions>> ExploredPaths { get; private set; }


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
        public void RegisterExplored(Vector2Int crossingCoords, LevelBoard.Directions exploredPath)
        {
            this.ExploredPaths.TryGetValue(crossingCoords, out var explored);

            if (explored == null)
            {
                explored = new List<LevelBoard.Directions>();

                this.ExploredPaths.Add(crossingCoords, explored);
            }

            if (!explored.Contains(exploredPath)) explored.Add(exploredPath);
        }

        public List<LevelBoard.Directions> GetExplored(Vector2Int crossingCoords)
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