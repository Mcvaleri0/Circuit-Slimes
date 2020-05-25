using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

using Level;
using Creator.Editor;
using Puzzle;

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

        private Object OptionPrefab { get; set; }

        #endregion


        #region /* Quick List Attributes */

        private const int QUICK_MAX = 4;

        private Transform QuickSelect { get; set; }

        private Queue<string> QuickItems { get; set; }
        private Queue<GameObject> QuickObjs { get; set; }

        #endregion


        #region /* Drawer Animation Attributes */

        public bool DrawerOpen { get; set; }
        private Animator Animator { get; set; }

        #endregion



        #region === Init Methods ===

        public void Initialize(PuzzleEditor Editor, Transform DrawerSystem, List<string> options, ModeUI.ModeUI mode)
        {
            this.Editor = Editor;
            this.Mode   = mode;

            Transform drawer = DrawerSystem.Find("Drawer");

            this.OpenButton  = drawer.Find("OpenDrawer").gameObject;
            this.CloseButton = drawer.Find("CloseDrawer").gameObject;

            this.Inside = drawer.Find("Inside");

            this.OptionPrefab = Resources.Load(FileHelper.OPTION_PATH);

            this.QuickSelect = drawer.Find("QuickSelect");
            this.QuickItems  = new Queue<string>(QUICK_MAX);
            this.QuickObjs   = new Queue<GameObject>(QUICK_MAX);

            this.DrawerOpen = false;
            this.Animator = drawer.GetComponent<Animator>();

            this.Populate(options);
        }

        #endregion


        #region === Drawer Manipulation Methods ===
        
        private void Populate(List<string> options)
        {
            foreach (string opt in options)
            {
                Option.CreateOption(this.Editor, this, this.OptionPrefab, this.Inside, opt, this.Mode.AbleToEditOptions());
                this.AddToQuick(opt);
            }
        }


        private void Clear()
        {
            this.QuickItems.Clear();
            this.QuickObjs.Clear();

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


        #region === Drawer Animation Methods ===
        
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


        #region === Quick Selection Methods ===
        
        private void PopulateQuickSelection(List<string> options)
        {
            foreach (string option in options)
            {
                this.AddToQuick(option);
            }
        }


        public void AddToQuick(string name)
        {
            if (!this.QuickItems.Contains(name)) {
                GameObject objToQuick = Option.CreateOption(this.Editor, this, this.OptionPrefab,
                                            this.QuickSelect, name, this.Mode.AbleToEditOptions());

                if (this.QuickItems.Count >= QUICK_MAX)
                {
                    this.QuickItems.Dequeue();
                    
                    GameObject objToDestroy = this.QuickObjs.Dequeue();

                    Resource resource = this.Editor.GetResource(objToDestroy.name);
                    Draggable draggable = objToDestroy.GetComponentInChildren<Draggable>();
                    resource.Draggables.Remove(draggable);

                    GameObject.Destroy(objToDestroy);
                }

                this.QuickItems.Enqueue(name);
                this.QuickObjs.Enqueue(objToQuick);
            }
        }
        
        #endregion
    }
}
