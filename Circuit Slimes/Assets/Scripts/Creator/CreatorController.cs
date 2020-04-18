using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Puzzle;



namespace Creator
{
    public class CreatorController : MonoBehaviour, IPointerClickHandler
    {
        #region /* UI Atributes */

        private ScrollMenu ScrollMenu { get; set; }

        #endregion

        #region /* detection Atributes */

        private const int clickCount = 2;

        #endregion


        #region === Unity Events ===

        void Start()
        {
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.clickCount >= clickCount)
            {
                Debug.Log("Click " + eventData.clickCount);
            }
        }


        #endregion

        #region === Initialization Methods ===

        public void Initialize()
        {
            Transform canvas = this.transform.Find("Canvas");
            Transform menu = canvas.Find("Scroll Menu");
            Transform content = menu.Find("Viewport").Find("Content");

            Transform puzzleObj = GameObject.Find("Puzzle").transform;
            PuzzleController puzzleController = GameObject.Find("PuzzleController").GetComponent<PuzzleController>();

            this.ScrollMenu = new ScrollMenu(menu, content, puzzleObj, puzzleController.Puzzle);

            this.InitializeButtons(canvas, puzzleController);
        }

        private void InitializeButtons(Transform canvas, PuzzleController controller)
        {
            // Change SaveButton Location
            Transform save = canvas.Find("Save Button");
            RectTransform saveRect = save.GetComponent<RectTransform>();

            float x = (Screen.width  / 2) - (saveRect.sizeDelta.x / 2) - 5;
            float y = (Screen.height / 2) - (saveRect.sizeDelta.y / 2) - 5;
            saveRect.anchoredPosition = new Vector2(x, y);

            // add click listener
            int level = controller.CurrentLevel;
            save.GetComponent<Button>().onClick.AddListener(delegate { controller.SavePuzzle(level); });
        }

        #endregion

    }
}
