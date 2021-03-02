using IngameScript.drawing_framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript.ui_framework {
  /**
    * This class defines the split between the two stages of the drawing
    * process for on-screen objects, as well as basic properties of such
    * objects. It also provides some basic handling of child objects.
    */
    public abstract class MyOnScreenObject {
        public int x;
        public int y;
        public bool isVisible = true;
        public bool invertColors = false;
        public MyOnScreenObject ParentObject;
        public List<MyOnScreenObject> ChildObjects = new List<MyOnScreenObject>();
        private Func<MyOnScreenObject, int> ClientCycleMethod;
        private Func<MyCanvas, int> ClientDrawMethod;
        private bool isObjectNotInitialized = true;

        public MyOnScreenObject(MyOnScreenObject ParentObject, int x, int y, bool isVisible) {
            this.SetParent(ParentObject);
            this.x = x;
            this.y = y;
            this.isVisible = isVisible;

            if (ParentObject != null) {
                ParentObject.AddChild(this);
            }
        }

      /**
        * This method should be used in case the caller needs to implement custom functionality
        * which is lite enough to fit inside a lambda expression instead of a new class.
        */
        public MyOnScreenObject WithClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) {
            SetClientCycleMethod(ClientCycleMethod);
            return this;
        }

        public void SetClientCycleMethod(Func<MyOnScreenObject, int> ClientCycleMethod) {
            this.ClientCycleMethod = ClientCycleMethod;
        }

        public MyOnScreenObject WithClientDrawMethod(Func<MyCanvas, int> ClientDrawMethod) {
            this.ClientDrawMethod = ClientDrawMethod;
            return this;
        }


      /**
        * Adds the referenced object to the list of child objects while
        * also setting its parent object reference to this object
        */
        public virtual void AddChild(MyOnScreenObject ChildObject) {
            if (ChildObject != null) {
                ChildObjects.Add(ChildObject);
                ChildObject.SetParent(this);
            }
        }

        public void AddChildAtLocation(MyOnScreenObject ChildObject, int x, int y) {
            if (ChildObject != null) {
                ChildObject.SetPosition(x, y);
                AddChild(ChildObject);
            }
        }

        public void SetPosition(int x, int y) {
            this.x = x;
            this.y = y;
        }

      /**
        * Removes the referenced child from the list of child objects and
        * sets its parent reference to null
        */
        public virtual void RemoveChild(MyOnScreenObject ChildObject) {
            if (ChildObject != null) {
                ChildObjects.Remove(ChildObject);
                ChildObject.RemoveParent();
            }
        }

      /**
        * Sets the object's parent. Makes sure there are no null.
        * To unset the parent, use the RemoveParent() method.
        */
        public virtual void SetParent(MyOnScreenObject ParentObject) {
            if (ParentObject != null) {
                this.ParentObject = ParentObject;
            }
        }

      /**
        * Removes the object's parent
        */
        public void RemoveParent() {
            this.ParentObject = null;
        }

        public MyOnScreenObject GetTopLevelParent() { 
            if (ParentObject == null) {
                return this;
            }

            return ParentObject.GetTopLevelParent();
        }

      /**
        * Recursive call through all the parents to check if the object should be drawn
        */
        public bool IsObjectVisible() {
            return isVisible && (ParentObject == null || ParentObject.IsObjectVisible());
        }

      /**
        * This should be called from the drawing loop. It establishes the order
        * of the operations.
        */
        public virtual void Cycle(MyCanvas TargetCanvas) {
         // Compute position, frame index and other state-related properties
         // which might have to be computed even if the object is not visible
         // on screen
            Compute(TargetCanvas);

         // Run the custom functionality (if any)
            if (ClientCycleMethod != null) {
                ClientCycleMethod(this);
            }

         // Cycle child objects (if any)
            foreach (MyOnScreenObject ChildObject in ChildObjects) {
                ChildObject.Cycle(TargetCanvas);
            }

         // Initialize the object, if it is not initialized.
         // This will help in computations which require the entire
         // parent tree to be available, which might not always be
         // true during the construction phase.
            if (isObjectNotInitialized) {
                Init();
                isObjectNotInitialized = true;
            }

         // If the object is visible on screen, then draw it
            if (IsObjectVisible()) {
                // Run the client draw method, if it's set
                if (ClientDrawMethod != null) {
                    ClientDrawMethod(TargetCanvas);
                }

                // Call the object's draw method
                Draw(TargetCanvas);
            }
        }

      /**
        * Run through all parents recursively to get the absolute X position of
        * the object on screen
        */
        public int GetAbsoluteX() {
            return x + (ParentObject == null ? 0 : ParentObject.GetAbsoluteX());
        }

      /**
        * Run through all parents recursively to get the absolute Y position of
        * the object on screen
        */
        public int GetAbsoluteY() {
            return y + (ParentObject == null ? 0 : ParentObject.GetAbsoluteY());
        }

      /**
        * This method handles some drawing-related logic, such as computing
        * the position of certain sprites on the TargetScreen
        */
        protected abstract void Compute(MyCanvas TargetCanvas);

      /**
        * This method handles the drawing of the object onto the TargetScreen
        */
        protected abstract void Draw(MyCanvas TargetCanvas);

      /**
        * All on-screen objects must implement this method as this information
        * is vital to the proper operation of container objects
        */
        public abstract int GetWidth();

      /**
        * All on-screen objects must implement this method as this information
        * is vital to the proper operation of container objects
        */
        public abstract int GetHeight();

      /**
        * Called before the first farme is drawn
        */
        protected abstract void Init();
    }
}
