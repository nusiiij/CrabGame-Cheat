using JNNJMods.Render;
using JNNJMods.UI.Elements;
using System.Collections.Generic;
using UnityEngine;

namespace JNNJMods.UI
{
    /// <summary>
    /// A Class for creating a ClickGUI.
    /// </summary>
    public class ClickGUI
    {
        #region Variables
        /// <summary>
        /// Current Instance(only 1 Instance is allowed)
        /// </summary>
        public static ClickGUI Instance { get; private set; }

        private bool shown;

        /// <summary>
        /// Defines if Windows are shown.
        /// </summary>
        public bool Shown
        {
            get
            {
                return shown;
            }

            set 
            {
                if (value)
                    Show();
                else
                    Hide();
            }
        }

        /// <summary>
        /// Defines if a Black transparent Background is drawn when shown
        /// </summary>
        public bool BlackOut;

        /// <summary>
        /// <see cref="KeyBindSelection"/> Handler for Key Binding
        /// </summary>
        public readonly KeyBindSelection keyBindSelection;

        private readonly float margin,
            controlHeight,
            controlDist;

        private readonly SortedDictionary<int, ElementManager> elementMap;

        public ElementInfo[] AllElements
        {
            get
            {
                List<ElementInfo> infos = new List<ElementInfo>();

                foreach(ElementManager em in elementMap.Values)
                {
                    infos.AddRange(em.Elements);
                }

                return infos.ToArray();
            }
        }

        #endregion

        /// <summary>
        /// Create a ClickGUI with preconfigured Values.
        /// </summary>
        public ClickGUI() : this(10, 40, 10) { }

        /// <summary>
        /// ClickGUI with custom Values.
        /// </summary>
        /// <param name="margin"></param>
        /// <param name="controlHeight">Element Height</param>
        /// <param name="controlDist">Element Distance</param>
        public ClickGUI(float margin, float controlHeight, float controlDist)
        {
            keyBindSelection = new KeyBindSelection();
            Instance = this;

            this.margin = margin;
            this.controlHeight = controlHeight;
            this.controlDist = controlDist;
            elementMap = new SortedDictionary<int, ElementManager>();
        }

        private CursorLockMode prevLockState;

        /// <summary>
        /// Show the ClickGUI.
        /// </summary>
        public void Show()
        {
            shown = true;
            prevLockState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        /// <summary>
        /// Hide the ClickGUI.
        /// </summary>
        public void Hide()
        {
            shown = false;
            Cursor.lockState = prevLockState;
            Cursor.visible = true;
        }

        /// <summary>
        /// Adds an Element.
        /// </summary>
        public void AddElement(ElementInfo info)
        {
            if (!elementMap.ContainsKey(info.WindowId)) return;
            elementMap[info.WindowId].Elements.Add(info);
            info.manager = elementMap[info.WindowId];
        }

        #region DrawWindows
        /// <summary>
        /// Adds an Window.
        /// </summary>
        /// <param name="id">ID of the Window.</param>
        /// <param name="text">Title of the Window.</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void AddWindow(int id, string text, float x, float y, float width, float height)
        {
            elementMap.Add(id, new ElementManager(id, text, x, y, width, height, margin, controlHeight, controlDist));
        }

        /// <summary>
        /// Draws added Windows, needs to be called from OnGUI().
        /// </summary>
        public void DrawWindows()
        {
            keyBindSelection.DrawSelection();
            if (Shown)
            {
                if(BlackOut)
                {
                    DrawingUtil.DrawFullScreenColor(DrawingUtil.TransparentBlack);
                }
            }
            else
                return;

            foreach (ElementManager manager in elementMap.Values)
            {
                if (manager.windowStyle == null || manager.windowStyle == GUIStyle.none)
                {
                    manager.windowStyle = GUI.skin.window;
                }

                manager.windowRect = GUI.Window(manager.WindowId, manager.windowRect, (GUI.WindowFunction)DrawWindow, manager.text, manager.windowStyle);

                if (!manager.allowOffscreen)
                {
                    manager.windowRect.x = Mathf.Clamp(manager.windowRect.x, 0, Screen.currentResolution.width - manager.windowRect.width);
                    manager.windowRect.y = Mathf.Clamp(manager.windowRect.y, 0, Screen.currentResolution.height - manager.windowRect.height);
                }
            }
        }

        /// <summary>
        /// Update function hook, needs to be called from Update().
        /// </summary>
        public void Update()
        {
            foreach(ElementInfo info in AllElements)
            {
                if(info.KeyBindable && info.KeyBind != KeyCode.None)
                {
                    if(Input.GetKeyDown(info.KeyBind))
                    {
                        info.Activate();
                    }
                }
            }
        }

        /// <summary>
        /// Draws the Elements of the Window.
        /// </summary>
        /// <param name="id"></param>
        void DrawWindow(int id)
        {
            ElementManager manager = elementMap[id];

            foreach (ElementInfo info in manager.Elements)
            {
                info.Render();
            }

            if (manager.draggable)
            {
                GUI.DragWindow(new Rect(0, 0, 10000, 20));
            }
        }
        #endregion
    }
}