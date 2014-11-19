using System;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar button, which can be clicked.
    /// </summary>
    public class Button : Variable
    {
        /// <summary>
        /// Occurs when this button is clicked by the user.
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Raises the Clicked event.
        /// </summary>
        public void OnClicked(EventArgs e)
        {
            if (Clicked != null)
                Clicked(this, e);
        }

        /// <summary>
        /// Initialization delegate, which creates the button.
        /// </summary>
        private static void InitButton(Variable var, String id, bool readOnly)
        {
            Tw.AddButton(var.ParentBar.Pointer, id,
                         ((Button)var).Callback,
                         IntPtr.Zero, null);
        }

        /// <summary>
        /// Creates a new button in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the button in.</param>
        /// <param name="clicked">A handler to call when the button is clicked.</param>
        /// <param name="def">An optional definition string for the new button.</param>
        public Button(Bar bar, EventHandler clicked = null, String def = null)
            : base(bar, InitButton, def)
        {
            callback = Callback;
            Clicked += clicked;
        }

        /// <summary>
        /// Called by AntTweakBar when the user clicks on the button.
        /// </summary>
        private readonly Tw.ButtonCallback callback;
        private void Callback(IntPtr clientData)
        {
            OnClicked(EventArgs.Empty);
        }

        public override String ToString()
        {
            return String.Format("[Button: Label={0}]", Label);
        }
    }
}
