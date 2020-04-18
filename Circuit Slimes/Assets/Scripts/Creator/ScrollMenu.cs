using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Puzzle;



namespace Creator
{
    public class ScrollMenu
    {
        #region /* Constants */

        private const string ITEMS_PATH  = "Prefabs/Board Items";
        private const string BUTTON_PATH = "Prefabs/Creator/Button";

        #endregion

        #region /* UI Elements */

        private Transform Menu { get; set; }
        private Transform MenuContent { get; set; }

        #endregion

        #region /* Puzzle Attibutes */

        private Dictionary<string, Object> BoardItens { get; set; }
        private Puzzle.Puzzle Puzzle { get; set; }
        private Transform PuzzleObj { get; set; }

        #endregion


        public ScrollMenu(Transform menu, Transform content, Transform puzzleObj, Puzzle.Puzzle puzzle)
        {
            this.Menu = menu;
            this.MenuContent = content;

            this.Puzzle    = puzzle;
            this.PuzzleObj = puzzleObj;

            this.BoardItens  = new Dictionary<string, Object>();

            this.Initialize();
        }


        #region === Initialization Methods === 

        private void Initialize()
        {
            this.ResizeMenu();
            this.PopulateContent();
        }

        private void ResizeMenu()
        {
            RectTransform rect = this.Menu.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(Screen.width, Screen.height / 4);
            rect.anchoredPosition = new Vector2(0, -3 * Screen.height / 8);
        }

        private void PopulateContent()
        {
            Object[] icons = Resources.LoadAll(ITEMS_PATH);
            Object button  = Resources.Load(BUTTON_PATH);

            foreach (Object icon in icons)
            {
                this.InstantiateOption(icon.name, button);

                this.BoardItens.Add(icon.name, icon);
            }

        }

        #endregion

        #region === Instantiate Methods ===

        private void InstantiateOption(string text, Object button)
        {
            GameObject newObj = (GameObject) GameObject.Instantiate(button, this.MenuContent);
            newObj.GetComponentInChildren<Text>().text = text;

            newObj.GetComponent<Button>().onClick.AddListener(delegate { this.InstantiateBoardItem(text); });
        }

        private void InstantiateBoardItem(string name)
        {
            Debug.Log("Instantiating " + name);

            // TODO: make the player choose where he wants the item
            Vector2Int coords = new Vector2Int(1, 0);

            Transform parent;
            if (name.Contains("Tile"))
            {
                parent = this.PuzzleObj.Find("Tiles");
                Tile newTile = Tile.CreateTile(parent, this.Puzzle.Board, coords, name);
                this.Puzzle.AddTile(newTile);
            }
            else
            {
                parent = this.PuzzleObj.Find("Pieces");
                Piece newPiece = Piece.CreatePiece(parent, this.Puzzle.Board, coords, name);
                this.Puzzle.AddPiece(newPiece);
            }

        }

        #endregion

    }
}