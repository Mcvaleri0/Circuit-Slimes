using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Puzzle;



namespace Creator
{
    public class CreatorController : MonoBehaviour
    {
        #region /* UI Atributes */

        private ScrollMenu ScrollMenu { get; set; }

        #endregion

        #region /* Detection Atributes */

        private const float DOUBLE_CLICK_WINDOW = 0.5f;

        private bool SingleClick { get; set; }
        private float TimeFirstClick { get; set; }

        #endregion


        #region === Unity Events ===

        void Update()
        {
            if (this.DoubleClick())
            {
                Debug.Log("Double Click");
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

            this.SingleClick = false;
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

        #region === Detection Methods ===

        private bool DoubleClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!SingleClick)
                {
                    TimeFirstClick = Time.time;
                    SingleClick = true;
                }
                else
                {
                    if ((Time.time - TimeFirstClick) > DOUBLE_CLICK_WINDOW)
                    {
                        TimeFirstClick = Time.time;
                    }
                    else
                    {
                        SingleClick = false;

                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

    }
}
