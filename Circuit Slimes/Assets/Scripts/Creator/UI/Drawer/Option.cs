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
    public class Option : MonoBehaviour
    {
        #region /* Puzzle Attributes */

        private PuzzleEditor Editor { get; set; }

        #endregion


        #region /* Resource Attributes */
        
        private Resource Resource { get; set; }
        private bool CanEdit { get; set; }

        #endregion



        #region === Init Methods ===

        public static GameObject CreateOption(PuzzleEditor editor, DrawerController controller,
                                    Object prefab, Transform parent, string name, bool ableToEdit)
        {
            GameObject newObj = (GameObject) GameObject.Instantiate(prefab, parent);
            newObj.name = name;

            Initialize(editor, controller, newObj.transform, ableToEdit, name);

            return newObj;
        }


        private static void Initialize(PuzzleEditor editor, DrawerController controller, Transform newObj, bool ableToEdit, string name)
        {
            GameObject buttons = newObj.Find("PlusMinus").gameObject;

            Transform resource = newObj.Find("Resource");
            Transform sprite = resource.Find("Sprite");

            Text amountText = resource.Find("Amount").GetComponent<Text>();
            Option optionScp = newObj.GetComponent<Option>();

            Draggable draggable = sprite.GetComponent<Draggable>();

            InitializeButtons(buttons, ableToEdit);
            InitiliazeSprite(sprite, name);
            draggable.Initialize(controller, optionScp);
            optionScp.Initialize(editor, name, amountText, draggable, ableToEdit);
        }


        private static void InitializeButtons(GameObject buttons, bool ableToEdit)
        {
            buttons.SetActive(ableToEdit);
        }


        public static void InitiliazeSprite(Transform sprite, string name)
        {
            string spritePath = Path.Combine(FileHelper.ITEMS_SPRITES_PATH, name);
            sprite.GetComponent<Image>().sprite = Resources.Load<Sprite>(spritePath);
        }


        public void Initialize(PuzzleEditor editor, string item, Text text, Draggable draggable, bool ableToEdit)
        {
            this.Editor = editor;
            this.CanEdit = ableToEdit;

            this.Resource = this.Editor.GetResource(item);
            if (this.Resource != null)
            {
                this.Resource.DefineUI(text, draggable, ableToEdit);
            }
        }

        #endregion


        #region === Resource / Permisson Methods ===
        
        public void Increase()
        {
            this.Resource.Increase(true);
        }


        public void Decrease()
        {
            this.Resource.Decrease(true);
        }


        public void PlaceItem()
        {
            this.Editor.PlaceItem(this.Resource);
        }


        public string Name()
        {
            return this.Resource.Name;
        }

        #endregion
    }
}
