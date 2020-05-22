using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Level;
using Puzzle;
using Creator.Editor;



namespace Creator.UI.Drawer
{
    public class Option : MonoBehaviour/*, IDragHandler, IBeginDragHandler, IEndDragHandler*/
    {
        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }
        private string Item { get; set; }

        #endregion


        #region /* Resource Attributes */
        
        private Resource Resource { get; set; }
        
        #endregion



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

            if (!ableToEdit)
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

            this.Resource = this.Editor.GetResource(this.Item);
        }

        #endregion


        #region === Resource / Permisson Methods ===
        
        public void Increase()
        {
            this.Resource.Increase();
        }


        public void Decrease()
        {
            this.Resource.Decrease();
        }


        public void PlaceItem()
        {
            this.Editor.PlaceItem(this.Item);
        }

        #endregion
    }
}
