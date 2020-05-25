using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game;
using Puzzle;
using Creator.UI.Drawer;
using Creator.Editor;
using Creator.Selection;



namespace Creator.UI.ModeUI
{

    public abstract class ModeUI
    {
        #region /* Creator Sub-Components */

        private SelectionSystem Selection { get; set; }
        private Mode.Mode Mode { get; set; }

        #endregion


        #region /* Puzzle Attributes */

        protected GameController Controller { get; private set; }
        protected PuzzleEditor Editor { get; set; }

        #endregion


        #region /* Save Attributes */

        public Transform SaveButton { get; private set; }

        #endregion


        #region /* Options Menu Attributes */
        
        private DrawerController Menu { get; set; }
        
        #endregion



        #region === Init Methods ===

        public ModeUI(GameController controller, PuzzleEditor editor, SelectionSystem selection, 
            Mode.Mode mode, Transform canvas)
        {
            this.Controller = controller;
            this.Editor     = editor;
            this.Selection  = selection;
            this.Mode       = mode;

            this.Initialize(canvas);
        }


        private void Initialize(Transform canvas)
        {
            this.InitializeButtons(canvas);
            this.InitializeMenu(canvas);
        }

        #endregion


        #region === Update Info Methods ===

        public void UpdateInfo()
        {
            this.Menu.UpdateOptions(this.MenuOptions());
        }

        #endregion


        #region === Buttons Methods ===

        private void InitializeButtons(Transform canvas)
        {
            Transform buttons = canvas.Find("Buttons");
            RectTransform rectTransform = buttons.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            this.SaveButton = buttons.Find("Save Button");
            this.InitializeSave();
        }


        public abstract void InitializeSave();

        #endregion


        #region === Options Methods ===
        
        private void InitializeMenu(Transform canvas)
        {
            Transform drawerSystem = canvas.Find("DrawerSystem");
            this.Menu = drawerSystem.GetComponent<DrawerController>();
                
            this.Menu.Initialize(this.Editor, drawerSystem, this.MenuOptions(), this);
        }


        public abstract List<string> MenuOptions();

        public bool AbleToEditOptions()
        {
            return this.Mode.AbleToEditOptions();
        }

        #endregion
    }
}
