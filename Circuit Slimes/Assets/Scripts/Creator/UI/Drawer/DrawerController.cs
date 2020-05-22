using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using Level;
using Creator.Editor;



namespace Creator.UI.Drawer
{
    public class DrawerController : MonoBehaviour
    {
        #region /* Other Controllers */
        
        private PuzzleEditor Editor { get; set; }
        private ModeUI.ModeUI Mode { get; set; }

        #endregion


        #region /* Drawer Attributes */

        private GameObject OpenButton { get; set; }
        private GameObject CloseButton { get; set; }

        private Transform Inside { get; set; }
        private Transform QuickSelect { get; set; }

        private Object OptionPrefab { get; set; }

        #endregion


        #region /* Drawer Animation Attributes */

        public bool DrawerOpen { get; set; }
        private Animator Animator { get; set; }

        #endregion



        #region === Init Methods ===

        public void Initialize(PuzzleEditor Editor, Transform DrawerSystem, List<string> options, ModeUI.ModeUI mode)
        {
            this.Editor = Editor;
            this.Mode = mode;

            Transform drawer = DrawerSystem.Find("Drawer");

            this.OpenButton  = drawer.Find("OpenDrawer").gameObject;
            this.CloseButton = drawer.Find("CloseDrawer").gameObject;

            this.Inside = drawer.Find("Inside");
            this.QuickSelect = drawer.Find("QuickSelect");

            this.OptionPrefab = Resources.Load(FileHelper.OPTION_PATH);

            this.DrawerOpen = false;
            this.Animator = drawer.GetComponent<Animator>();

            this.Populate(options);
        }

        #endregion


        #region === List Manipulation Methods ===
        
        private void Populate(List<string> options)
        {
            Transform parent;

            if (options.Count <= 4)
            {
                parent = this.QuickSelect;
                this.OpenButton.SetActive(false);
            }
            else
            {
                parent = this.Inside;
                this.OpenButton.SetActive(true);
            }
            
            foreach (string opt in options)
            {
                Option.CreateOption(this.Editor, this, this.OptionPrefab, parent, opt, this.Mode.AbleToEditOptions());
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


        public void UpdateOptions(List<string> newOptions)
        {
            this.Clear();
            this.Populate(newOptions);
        }

        #endregion


        #region === Drawer Animation ===
        
        public void Close()
        {
            if (this.DrawerOpen)
            {
                this.Animator.Play("CloseDrawer");
                this.CloseButton.SetActive(false);
                this.OpenButton.SetActive(true);
                this.DrawerOpen = false;
            }
        }
        
        #endregion
    }
}
