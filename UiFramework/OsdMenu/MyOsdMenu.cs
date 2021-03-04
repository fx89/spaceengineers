using IngameScript.drawing_framework;
using IngameScript.drawing_framework.sprites;
using IngameScript.ui_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript {
    class MyOsdMenu : MyOnScreenObject {
        private const int TITLE_ROW_HEIGHT  = 14;
        private const int PAGE_PADDING      =  3;
        private const int BOTTOM_ROW_HEIGHT = 17;
        private const int TITLE_LABEL_POS_Y =  4;

        private int resX, resY;

      /**
        * The title label will be displayed on top of the menu
        */
        private MyTextLabel TitleLabel;

      /**
        * Turns TRUE when the End() method is called.
        * Ensures that the CurrentMenu is set to the TopLevelMenu and that
        * the CurrentOption is set to the first option of te TopLevelMenu.
        */
        private bool isConstructed = false;

      /**
        * This is the list that the OSD menu will display initially
        */
        private MyList TopLevelMenu;

      /**
        * This is the list currently being displayed. Upon clicking options with mapped sub-menus,
        * this property will point to the list representing the submenu mapped to the clicked option.
        */
        private MyList CurrentMenu;

       /**
         * Required for construction purposes (i.e. knowing to which key to map a sub-menu)
         */
         private MyOnScreenObject CurrentOption;

       /**
         * Required for construction (i.e. when switching back from the sub-menu to the parent menu)
         */
         private Dictionary<MyList, MyOnScreenObject> MenuCurrentOptionsDictionary = new Dictionary<MyList, MyOnScreenObject>();

      /**
        * Required for navigation (i.e. when the back button is clicked, the previous list must be displayed)
        */
        private Dictionary<MyList, MyList> ParentMenusDictionary = new Dictionary<MyList, MyList>();

      /**
        * Required for navigation (i.e. when an option is clicked, the list mapped to it appears)
        */
        private Dictionary<MyOnScreenObject, MyList> SubmenusDictionary = new Dictionary<MyOnScreenObject, MyList>();

      /**
        * When an option is clicked, if there's no submenu assigned to it in the SubmenusDictionary,
        * then the assigned action is executed (if any).
        */
        private Dictionary<MyOnScreenObject, Action> ActionsDictionary = new Dictionary<MyOnScreenObject, Action>();

        private Dictionary<MyList, string> MenuTitlesDictionary = new Dictionary<MyList, string>();

        private MyBlinkingIcon UpArrow, DownArrow, InArrow, OutArrow;

        public MyOsdMenu(string title, int resX, int resY) : base(null, 0, 0, true) {
            if (title == null) {
                throw new ArgumentException("The title must not be null");
            }

            if (resX <=0 || resY <= 0) {
                throw new ArgumentException("Invalid resolution");
            }

            this.resX = resX;
            this.resY = resY;

            TopLevelMenu = CreateNewInvisibleList();
            MenuTitlesDictionary[TopLevelMenu] = title;
            TopLevelMenu.isVisible = true;
            AddChild(TopLevelMenu);
            CurrentMenu = TopLevelMenu;

            TitleLabel = new MyTextLabel(title, 3, TITLE_LABEL_POS_Y);
            AddChild(TitleLabel);

            int bottomIconsLeftMargin = resX / 2 - (StockSprites.SPRITE_UP.width * 2);
            int bottomIconsY = resY - BOTTOM_ROW_HEIGHT;

            DownArrow = new MyBlinkingIcon(bottomIconsLeftMargin , bottomIconsY, StockSprites.SPRITE_DOWN);
            UpArrow   = new MyBlinkingIcon(bottomIconsLeftMargin + StockSprites.SPRITE_UP.width , bottomIconsY, StockSprites.SPRITE_UP);
            InArrow   = new MyBlinkingIcon(bottomIconsLeftMargin + (StockSprites.SPRITE_UP.width * 2), bottomIconsY, StockSprites.SPRITE_UPDOWN);
            OutArrow  = new MyBlinkingIcon(bottomIconsLeftMargin + (StockSprites.SPRITE_UP.width * 3), bottomIconsY, StockSprites.SPRITE_REVERSE);

            AddChild(UpArrow);
            AddChild(DownArrow);
            AddChild(InArrow);
            AddChild(OutArrow);
        }

      /**
        * Tells the current menu or sub-menu to display just one item per page
        */
        public MyOsdMenu WithOneItemPerPage() {
            CurrentMenu.WithOneItemPerPage();
            return this;
        }

      /**
        * Sets the alignment for the current menu (top menu or sub-menu)
        * Expected values:
        *     0 = LEFT
        *     1 = CENTER
        *     2 = RIGHT
        * Any other value will default to LEFT
        * Also see the contsants:
        *     HORIZONTAL_ALIGN_LEFT
        *     HORIZONTAL_ALIGN_CENTER
        *     HORIZONTAL_ALIGN_RIGHT
        */
        public MyOsdMenu WithCustomHorizontalAlignment(int horizontalAlignment) {
            CurrentMenu.WithHorizontalAlignment(horizontalAlignment);
            return this;
        }

        public MyOsdMenu WithPictureOption(MySprite[] AnimationFrames) {
            CurrentOption = new MyStatefulAnimatedSprite(0, 0)
                .WithState("Default", new MyStatefulAnimatedSpriteState(AnimationFrames));

            CurrentMenu.AddChild(CurrentOption);

            return this;
        }

        public MyOsdMenu WithTextOption(string text) {
            CurrentOption = new MyTextLabel(text, 0, 0);
            CurrentMenu.AddChild(CurrentOption);
            return this;
        }

        public MyOsdMenu WithIconOption(string text, MySprite[] Frames, int floatingIconPosition) {
            MyIconLabel IconLabel = new MyIconLabel(0, 0, text, Frames);
            IconLabel.WithFloatingIconPosition(floatingIconPosition);
            CurrentOption = IconLabel;
            CurrentMenu.AddChild(CurrentOption);
            return this;
        }

        public MyOsdMenu WithAggregatedOptions(Action<MyOsdMenu> Act) {
            if (Act != null) {
                Act(this);
            }

            return this;
        }

        public MyOsdMenu WithAction(Action action) {
            ActionsDictionary[CurrentOption] = action;
            return this;
        }

        public MyOsdMenu WithSubMenu(string title) {
            if (title == null) {
                throw new ArgumentException("The title must not be null");
            }

            MenuCurrentOptionsDictionary[CurrentMenu] = CurrentOption;
            MyList ParentMenu = CurrentMenu;
            CurrentMenu = CreateNewInvisibleList();
            AddChild(CurrentMenu);
            SubmenusDictionary[CurrentOption] = CurrentMenu;
            ParentMenusDictionary[CurrentMenu] = ParentMenu;
            CurrentOption = null;
            MenuTitlesDictionary[CurrentMenu] = title;

            return this;
        }

        public MyOsdMenu EndSubMenu() {
            CurrentMenu = ParentMenusDictionary.GetValueOrDefault(CurrentMenu, null);
            CurrentOption = MenuCurrentOptionsDictionary.GetValueOrDefault(CurrentMenu, null);
            return this;
        }

        public MyOsdMenu End() {
            if (TopLevelMenu.GetItems().Count() == 0) {
                throw new InvalidOperationException("The top level menu must have at least one item");
            }

            CurrentMenu = TopLevelMenu;
            CurrentOption = TopLevelMenu.GetItems()[0];

            SetTitle(MenuTitlesDictionary[CurrentMenu]);

            isConstructed = true;

            return this;
        }

        public void CursorUp() {
            if (isConstructed) {
                if (CurrentMenu != null) {
                    UpArrow.Blink(3);
                    CurrentOption = CurrentMenu.SelectPreviousItem();
                }
            } else {
                throw new InvalidOperationException("Please call the End() method before operating the OSD menu");
            }
        }

        public void CursorDown() {
            if (isConstructed) {
                if (CurrentMenu != null) {
                    DownArrow.Blink(3);
                    CurrentOption = CurrentMenu.SelectNextItem();
                }
            } else {
                throw new InvalidOperationException("Please call the End() method before operating the OSD menu");
            }
        }

        public void CursorIn() {
            if (isConstructed) {
                InArrow.Blink(3);
                MyList Submenu = SubmenusDictionary.GetValueOrDefault(CurrentOption, null);
                if (Submenu == null) {
                    Action AssignedAction = ActionsDictionary.GetValueOrDefault(CurrentOption, null);
                    if (AssignedAction != null) {
                        AssignedAction();
                    }
                } else {
                    MenuCurrentOptionsDictionary[CurrentMenu] = CurrentOption;
                    CurrentMenu.isVisible = false;
                    CurrentMenu = Submenu;
                    CurrentMenu.isVisible = true;
                    CurrentOption = CurrentMenu.SelectFirstItem();
                    SetTitle(MenuTitlesDictionary[CurrentMenu]);
                }
            } else {
                throw new InvalidOperationException("Please call the End() method before operating the OSD menu");
            }
        }

        public void CursorOut() {
            if (isConstructed) {
                OutArrow.Blink(3);
                MyList ParentMenu = ParentMenusDictionary.GetValueOrDefault(CurrentMenu, null);
                if (ParentMenu != null) {
                    CurrentMenu.isVisible = false;
                    CurrentMenu = ParentMenu;
                    CurrentMenu.isVisible = true;
                    CurrentOption = CurrentMenu.GetSelectedItem();
                    MenuCurrentOptionsDictionary[CurrentMenu] = CurrentOption;
                    SetTitle(MenuTitlesDictionary[CurrentMenu]);
                }
            } else {
                throw new InvalidOperationException("Please call the End() method before operating the OSD menu");
            }
        }

        protected override void Compute(MyCanvas TargetCanvas) {
            // Nothing to do here
        }

        protected override void Draw(MyCanvas TargetCanvas) {
            // Nothing to do here
        }

        protected override void Init() {
         // This is the first stage where the Canvas and its default font are available,
         // making it possible to compute the text width so that it may be properly centered.
            SetTitle(MenuTitlesDictionary[CurrentMenu]);
        }

        public override int GetWidth() {
            return resX;
        }

        public override int GetHeight() {
            return resY;
        }

        private MyList CreateNewInvisibleList() {
            MyList ret = new MyList(
                PAGE_PADDING,
                TITLE_ROW_HEIGHT,
                resX - (PAGE_PADDING * 2),
                resY - PAGE_PADDING - TITLE_ROW_HEIGHT - BOTTOM_ROW_HEIGHT + 2
            );

            ret.isVisible = false;

            return ret;
        }

        private void SetTitle(string title) {
            TitleLabel.SetText(title);
            TitleLabel.x = (resX - TitleLabel.GetWidth()) / 2;
        }
    }
}
