using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
    /**
     * This is the main class of the application. Its purpose is to store
     * references to a multitude of pages, allow callers to activate the
     * required page and cycle the current page.
     */
    public class MyOnScreenApplication {
	    private List<MyScreen> TargetScreens = new List<MyScreen>();
	    private List<MyPage> Pages = new List<MyPage>();
	    private MyPage CurrentPage;
	    private MyCanvas Canvas;

	    // When rendering on higher resolutions, the compilation of the string
	    // to be displayed on-screen involves more operations than the maximum
	    // allowed within one single loop. To work, it must be split into
	    // several iterations. Sadly, it also has to be split on multiple screens.
	    private int currIteration, nIterations;

	    public MyOnScreenApplication() {
		    currIteration = 0;
		    nIterations = 1;
	    }

	    public MyOnScreenApplication WithCanvas(MyCanvas Canvas) {
		    this.Canvas = Canvas;
		    return this;
	    }

	    public MyOnScreenApplication OnScreen(MyScreen TargetScreen) {
		    return OnScreen(TargetScreen, 0, 0, Canvas.GetResX(), Canvas.GetResY());
	    }

	    public MyOnScreenApplication OnScreen(MyScreen TargetScreen, int clipRectX1, int clipRectY1, int clipRectX2, int clipRectY2) {
		    if (Canvas == null) {
			    throw new InvalidOperationException("Invalid initialization of MyOnScreenApplication. Please call WithCanvas() before OnScreen().");
		    }

		    TargetScreen.WithCanvas(Canvas);
		    TargetScreen.WithClippedBuffer(clipRectX1, clipRectY1, clipRectX2, clipRectY2);
		    TargetScreens.Add(TargetScreen);
		    nIterations++;
		    return this;
	    }

	    /**
	     * The initializationMonitoringFunction is run once per frame by the Cycle method of the POST page.
	     * When it returns TRUE, the application switches to the first page that comes after the POST page.
	     * If There is no other page than the POST page, then nothing will happen.
	     */
	    public MyOnScreenApplication WithDefaultPostPage(Func<MyOnScreenApplication,bool> initializationMonitoringFunction) {
	     // Make sure the POST page is the first one added
		    if (Pages.Count() > 0) {
			    throw new InvalidOperationException("The POST page must be the first page ever added to the application");
		    }
		
	     // Enforce the order of builder operations to avoid null pointer exceptions
		    if (Canvas == null) {
			    throw new InvalidOperationException("Please call WithCanvas() before calling WithDefaultPostPage()");
		    }

	     // Make sure the initialization function is set
		    if (initializationMonitoringFunction == null) {
			    throw new ArgumentException("The initialization monitoring function must be a lambda taking in a MyOnScreenObject and returning a bool");
		    }

	     // Create the POST page and give it the functionality of switching to the next page once
	     // the initialization monitoring function returns true
		    MyPage POSTPage = (MyPage) new MyPage().WithClientCycleMethod((MyOnScreenObject obj) => {
			    if (initializationMonitoringFunction(this)) {
				    SwitchToPage(1); // will do nothing if the page does not exist
			    }
		    return 0; });

	     // Add the POST page to the application
		    this.AddPage(POSTPage);

	     // Create the panel and add it to the POST page
		    MyPanel Panel = new MyPanel(0, 0, Canvas.GetResX(), Canvas.GetResY()).WithOptionalParameters(true, true, false);
		    POSTPage.AddChild(Panel);

	     // Add another filled panel to the POST page, to serve as background for the INITIALIZING text
		    MyPanel TextBackgroundPanel = new MyPanel(0, 0, 2, 2).WithOptionalParameters(true, true, true);
		    POSTPage.AddChild(TextBackgroundPanel);

	     // Add the INIIALIZING text label to the POST page
		    MyTextLabel TextLabel = new MyTextLabel("INITIALIZING", 1, 1).WithOptionalParameters(true, false, false);
		    POSTPage.AddChild(TextLabel);

	     // Compute the location and dimensions of the text and that of its back panel based on
	     // the resolution of the Canvas
		    int textLabelWidth = TextLabel.GetWidth();
		    int textLabelHeight = TextLabel.GetHeight();

	     // Update the label coordinates
		    TextLabel.x = (Canvas.GetResX() - textLabelWidth) / 2;
		    TextLabel.y = (Canvas.GetResY() - textLabelHeight) / 2 - 3;

	     // Update the panel coordinates (the drawing framework handles overflows)
		    TextBackgroundPanel.x = TextLabel.x - 3;
		    TextBackgroundPanel.y = TextLabel.y - 2;
		    TextBackgroundPanel.SetWidth(textLabelWidth + 6);
		    TextBackgroundPanel.SetHeight(textLabelHeight + 3);

	     // Add the moving square (a panel with some simple animation logic)
		    POSTPage.AddChild(
			    new MyPanel(TextBackgroundPanel.x, TextBackgroundPanel.y + TextBackgroundPanel.GetHeight() + 2, 7, 4)
				    .WithOptionalParameters(true, true, true)
				    .WithClientCycleMethod((MyOnScreenObject obj) => {
					    obj.x++;
					    if (obj.x > TextBackgroundPanel.x + TextBackgroundPanel.GetWidth() - 7) {
						    obj.x = TextBackgroundPanel.x;
					    }
				    return 0; })
		    );

		    return this;
	    }

	    public void AddPage(MyPage Page) {
		    Pages.Add(Page);
		    Page.SetApplication(this);
		    if (CurrentPage == null) {
			    CurrentPage = Page;
		    }
	    }

	    public void SwitchToPage(int pageNumber) {
		    if (pageNumber < 0 || pageNumber >= Pages.Count) {
			    return;
		    }
		    CurrentPage = Pages[pageNumber];
	    }

	    public void Cycle() {
		    // Process the current iteration
		    if (currIteration == 0) {
			    Canvas.Clear();
			    CurrentPage.Cycle(Canvas);
		    } else {
			    TargetScreens[currIteration - 1].FlushBufferToScreen(CurrentPage.invertColors);
		    }

		    // Go to the next iteration
		    currIteration++;
		    if (currIteration >= nIterations) {
			    currIteration = 0;
		    }
	    }

	    public MyCanvas GetCanvas() { 
		    return Canvas;
	    }
    }
}
