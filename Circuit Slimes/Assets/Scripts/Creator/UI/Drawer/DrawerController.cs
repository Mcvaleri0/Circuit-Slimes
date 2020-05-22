using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using Level;
using Creator.Editor;



namespace Creator.UI.Drawer
{
    public class DrawerController
    {
        #region /* Puzzle Editor */
        
        private PuzzleEditor Editor { get; set; }

        #endregion


        #region /* Open Button Attributes */

        private GameObject OpenButton { get; set; }

        #endregion


        #region /* Inside Attributes */

        private Transform Inside { get; set; }

        private Object Choice { get; set; }

        #endregion


        #region /* Quick Selection Attributes */

        private Transform QuickSelect { get; set; }

        private Object Resource { get; set; }

        #endregion



        #region === Init Methods ===

        public DrawerController(PuzzleEditor Editor, Transform canvas, List<string> options)
        {
            this.Editor = Editor;

            Transform drawer = canvas.Find("DrawerSystem").Find("Drawer");

            this.OpenButton = drawer.Find("OpenDrawer").gameObject;

            this.Inside = drawer.Find("Inside");
            this.Choice = Resources.Load(FileHelper.CHOICE_PATH);

            this.QuickSelect = drawer.Find("QuickSelect");
            this.Resource = Resources.Load(FileHelper.RESOURCE_PATH);

            this.Populate(options);
        }

        #endregion


        #region === List Manipulation Methods ===
        
        private void Populate(List<string> options)
        {
            Object option;
            Transform parent;

            bool isChoice = (options.Count < 4); 

            if (isChoice)
            {
                option = this.Resource;
                parent = this.QuickSelect;
                this.OpenButton.SetActive(false);
            }
            else
            {
                option = this.Choice;
                parent = this.Inside;
                this.OpenButton.SetActive(true);
            }
            
            foreach (string opt in options)
            {
                GameObject newObj = Option.CreateOption(this.Editor, option, parent, opt, isChoice);
            }
        }


        private void Clear()
        {
            this.Clear(this.QuickSelect);
            this.Clear(this.Inside);
        }


        private void Clear(Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }


        public void Update(List<string> newOptions)
        {
            this.Clear();
            this.Populate(newOptions);
        }

        #endregion
    }
}
