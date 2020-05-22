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
    public class Option : MonoBehaviour/*, IDragHandler, IBeginDragHandler, IEndDragHandler*/
    {
        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }
        private string Item { get; set; }

        #endregion


        //#region /* Moviment Attirbutes */
        
        //private Vector3 StartPosition { get; set; }

        //#endregion



        #region === Init Methods ===

        public static GameObject CreateOption(PuzzleEditor editor, DrawerController controller,
                                    Object prefab, Transform parent, string name, bool ableToEdit)
        {
            GameObject newObj = (GameObject) GameObject.Instantiate(prefab, parent);
            newObj.name = name;

            InitializeUI(newObj.transform, name, ableToEdit);
            InitializeFunctionality(newObj, editor, controller, name);

            return newObj;
        }


        private static void InitializeUI(Transform option, string name, bool ableToEdit)
        {
            Transform resource = option.Find("Resource");
            Transform sprite   = resource.Find("Sprite");

            string spritePath = Path.Combine(FileHelper.ITEMS_SPRITES_PATH, name);
            sprite.GetComponent<Image>().sprite = Resources.Load<Sprite>(spritePath);

            if (ableToEdit)
            {
                resource.Find("Amount").gameObject.SetActive(true);
            }
            else
            {
                option.Find("PlusMinus").gameObject.SetActive(false);
            }
        }


        private static void InitializeFunctionality(GameObject obj, PuzzleEditor editor,
                                DrawerController controller, string name)
        {
            Option option = obj.GetComponent<Option>();
            option.InitializeOption(editor, name);
            
            obj.GetComponentInChildren<Draggable>().Initialize(controller, option);
        }


        public void InitializeOption(PuzzleEditor editor, string item)
        {
            this.Editor = editor;
            this.Item   = item;
        }

        #endregion


        #region === Resource / Permisson Methods ===
        
        public void Increase()
        {
            this.Editor.AddPermission(this.Item);
        }


        public void Decrease()
        {
            this.Editor.RemovePermission(this.Item);
        }


        public void PlaceItem()
        {
            this.Editor.PlaceItem(this.Item);
        }

        #endregion


        #region === Movement Methods ===

        //void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        //{
        //    this.StartPosition = this.transform.position;
        //}
        

        //void IDragHandler.OnDrag(PointerEventData eventData)
        //{
        //    transform.position = Input.mousePosition;
        //}


        //void IEndDragHandler.OnEndDrag(PointerEventData data)
        //{
        //    this.transform.position = this.StartPosition;
        //    this.StartPosition = Vector3.zero;
        //}

        #endregion
    }
}
