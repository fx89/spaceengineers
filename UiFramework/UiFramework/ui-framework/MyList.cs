using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
    public class MyList : MyOnScreenObject {
	    private MyPanel		  Panel;
	    private MyScrollbar	  Scrollbar;
	    private MyPanel		  SelectionBackground;
	    private MyOnScreenObject SelectedItem;
	    private bool			 oneItemPerPage = false;
	    private int			  selectedItemIndex;
	    private int			  startPosY;
	    private int			  padding = 2;
	    private int			  horizontalAlignment = Constants.HORIZONTAL_ALIGN_LEFT;

	    /**
	     * Cannot use ChildObjects of the base class since it contains the panel
	     * and the scrollbar, which must be excluded from the calculations.
	     */
	    private List<MyOnScreenObject> Items = new List<MyOnScreenObject>();

	    public MyList(int x, int y, int width, int height)
	    : base(null, x, y, true) {
		    Panel = new MyPanel(0, 0, width, height); base.AddChild(Panel);
		    Scrollbar = new MyScrollbar(Panel);

		    SelectionBackground = new MyPanel(0, 0, width, 1).WithOptionalParameters(true, true, false);
		    base.AddChild(SelectionBackground);
	    }

	    public MyList WithCustomScrollbarWidth(int scrollbarWidth) {
		    Scrollbar.WithCustomWidth(scrollbarWidth);
		    return this;
	    }

	    public MyList WithOneItemPerPage() {
		    this.oneItemPerPage = true;
		    return this;
	    }

	    public MyList WithPadding(int padding) { 
		    this.padding = padding;
		    return this;
	    }

	    public MyList WithHorizontalAlignment(int horizontalAlignment) {
		    this.horizontalAlignment = horizontalAlignment;
		    return this;
	    }

	    public override void AddChild(MyOnScreenObject Item) {
		    base.AddChild(Item);

		    if (SelectedItem == null) {
			    SetInitialSelectedItem(Item);
			    UpdateScrollbarPosition();
		    }

		    Items.Add(Item);

		    UpdateItemPositions();
	    }

	    public override void RemoveChild(MyOnScreenObject Item) {
		    base.RemoveChild(Item);

		    if (Item == SelectedItem) {
			    SetInitialSelectedItem(ChildObjects[0]);
			    UpdateScrollbarPosition();
		    }

		    Items.Remove(Item);

		    UpdateItemPositions();
	    }

	    private void SetInitialSelectedItem(MyOnScreenObject Item) {
		    SelectedItem = Item;
		    selectedItemIndex = 0;
		    startPosY = padding;
	    }

	    private int ComputeItemHorizontalPosition(MyOnScreenObject Item) {
		    if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_RIGHT) {
			    return GetWidth() - Scrollbar.GetWidth() - padding - Item.GetWidth();
		    } else if (horizontalAlignment == Constants.HORIZONTAL_ALIGN_CENTER) {
			    return (GetWidth() - Scrollbar.GetWidth() - Item.GetWidth()) / 2;
		    }
		    else {
			    return padding;
		    }

	    }

	    private void UpdateItemPositions() {

	     // If the list is empty, don't do anything
		    if (Items.Count() == 0) {
			    return;
		    }

	     // If there's just one item per page, then the selected item has to be
	     // placed in the middle of the page, while all the other items are to
	     // be hidden
		    if (oneItemPerPage) {
			    foreach (MyOnScreenObject Item in Items) {
				    if (Item == SelectedItem) {
					    Item.isVisible = true;
					    Item.x = ComputeItemHorizontalPosition(Item);
					    Item.y = (Panel.GetHeight() - Item.GetHeight()) / 2;
				    } else {
					    Item.isVisible = false;
				    }

				    Item.invertColors = false;
			    }

			    SelectionBackground.isVisible = false;
		    }
	     // If there are many items per page
		    else {
		     // Compute the max list height, as it will be needed later
			    int listMaxHeight = GetHeight() - (padding * 2);

		     // Update the startPosY:
		     //	- if the selected item is above the top margin of the list box,
		     //	  then make it so the selected item is at the top of the list box
		     //	- if the selected item is below the bottom margin of the list box,
		     //	  then make it so the selected item is at the bottom of the list box

		     // Get the current y position of the selected item based on the
		     // previously set startPosY
			    int selItemY = startPosY;
			    for (int idx = 0 ; idx < selectedItemIndex ; idx++) {
				    selItemY += Items[idx].GetHeight();
			    }

		     // Update the startPosY, if required
			    if (selItemY < padding) {
				    startPosY += padding - selItemY;
			    } else if (selItemY + SelectedItem.GetHeight() > listMaxHeight) {
				    startPosY -= selItemY + SelectedItem.GetHeight() - listMaxHeight;
			    }

		     // Once the startPosY has been updated, the items may be layed out vertically

		     // Update item vertical positions
			    int currPosY = startPosY;
			    foreach (MyOnScreenObject Item in Items) {
				    Item.y = currPosY;
				    Item.x = ComputeItemHorizontalPosition(Item);
				    currPosY += Item.GetHeight();
				    Item.isVisible = Item.y >= padding && Item.y + Item.GetHeight() <= listMaxHeight;
				    Item.invertColors = Item == SelectedItem;
			    }

		     // Update the vertical position of the selection background
			    SelectionBackground.x = padding;
			    SelectionBackground.y = SelectedItem.y;
			    SelectionBackground.SetWidth(GetWidth() - Scrollbar.GetWidth() - (padding * 2));
			    SelectionBackground.SetHeight(SelectedItem.GetHeight());
			    SelectionBackground.isVisible = true;
		    }
	    }

	    private void UpdateScrollbarPosition() {
		    Scrollbar.SetPosPct(Items.Count() == 0 ? 0f : ((float)selectedItemIndex / ((float)Items.Count() - 1)));
	    }

	    public MyOnScreenObject SelectNextItem() {
		    if (SelectedItem != null) {
			    selectedItemIndex = Items.IndexOf(SelectedItem); // if it doesn't work, use this: int index = myList.FindIndex(a => a.Prop == oProp);
			    if (selectedItemIndex >= 0 && selectedItemIndex < Items.Count() - 1) {
				    selectedItemIndex++;
				    SelectedItem = Items[selectedItemIndex];
				    UpdateItemPositions();
				    UpdateScrollbarPosition();
			    }
		    }

		    return SelectedItem;
	    }

	    public MyOnScreenObject SelectPreviousItem() {
		    if (SelectedItem != null) {
			    selectedItemIndex = Items.IndexOf(SelectedItem); // if it doesn't work, use this: int index = myList.FindIndex(a => a.Prop == oProp);
			    if (selectedItemIndex > 0) {
				    selectedItemIndex--;
				    SelectedItem = Items[selectedItemIndex];
				    UpdateItemPositions();
				    UpdateScrollbarPosition();
			    }
		    }

		    return SelectedItem;
	    }

	    public MyOnScreenObject GetSelectedItem() {
		    return SelectedItem;
	    }

	    public MyOnScreenObject SelectFirstItem() {
		    SetInitialSelectedItem(Items[0]);
		    return SelectedItem;
	    }

	    protected override void Compute(MyCanvas TargetCanvas) {
		    // Nothing to do here
	    }

	    protected override void Draw(MyCanvas TargetCanvas) {
		    // Nothing to do here
	    }

	    public override int GetWidth() {
		    return Panel.GetWidth();
	    }

	    public override int GetHeight() {
		    return Panel.GetHeight();
	    }
	    public List<MyOnScreenObject> GetItems() {
		    return Items;
	    }

	    protected override void Init() {
		    UpdateItemPositions();
	    }
    }
}
