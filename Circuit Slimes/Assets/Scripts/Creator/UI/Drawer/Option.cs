using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Level;
using Creator.Editor;



namespace Creator.UI.Drawer
{
    public class Option : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }
        private string Item { get; set; }

        #endregion


        #region /* Moviment Attirbutes */
        
        private Vector3 StartPosition { get; set; }

        #endregion



        #region === Init Methods ===

        public static GameObject CreateOption(PuzzleEditor editor, Object optionPrefab,
                        Transform inside, string name, bool isChoice)
        {
            GameObject newObj = (GameObject) GameObject.Instantiate(optionPrefab, inside);
            newObj.name = name;

            Transform sprite = GetSprite(newObj, isChoice);

            string spritePath = Path.Combine(FileHelper.ITEMS_SPRITES_PATH, name);
            sprite.GetComponent<Image>().sprite = Resources.Load<Sprite>(spritePath);

            return newObj;
        }


        private static Transform GetSprite(GameObject newObj, bool isChoice)
        {
            if (isChoice)
            {
                return newObj.transform.Find("Sprite");
            }
            else
            {
                return newObj.transform.Find("Resource").Find("Sprite");
            }
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


        #region === Movement Methods ===

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            this.StartPosition = this.transform.position;
        }
        

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }


        void IEndDragHandler.OnEndDrag(PointerEventData data)
        {
            this.transform.position = this.StartPosition;
            this.StartPosition = Vector3.zero;
        }

        #endregion
    }
}
