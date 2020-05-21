using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using Level;
using Creator.Editor;



namespace Creator.UI.Wheel
{
    public class WheelOption : MonoBehaviour
    {
        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }
        private string Item { get; set; }

        #endregion



        #region === Init Methods ===

        public static GameObject CreateOption(PuzzleEditor editor, ListPositionCtrl listController,
            Object optionPrefab, Transform circularList, string name)
        {
            GameObject newObj = (GameObject) GameObject.Instantiate(optionPrefab, circularList);
            newObj.name = name;

            WheelOption option = newObj.GetComponent<WheelOption>();
            option.Initialize(editor, name);

            Transform sprite = newObj.transform.Find("Sprite");
            ItemDragHandler dragHandler = sprite.GetComponent<ItemDragHandler>();
            dragHandler.selectionWheel = listController;
            dragHandler.Option = option;

            string spritePath = Path.Combine(FileHelper.ITEMS_SPRITES_PATH, name);
            sprite.GetComponent<Image>().sprite = Resources.Load<Sprite>(spritePath);

            return newObj;
        }

        public void Initialize(PuzzleEditor editor, string item)
        {
            this.Editor = editor;
            this.Item   = item;
        }

        #endregion


        #region === Placing Methods ===
        
        public void PlaceItem()
        {
            this.Editor.PlaceItem(this.Item);
        }

        #endregion

    }
}
