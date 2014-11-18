using System;
using System.Runtime.InteropServices;
using System.Text;

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
        private static void InitButton(Variable var, String id)
        {
            TW.AddButton(var.ParentBar.Pointer, id,
                         ((Button)var).Callback,
                         IntPtr.Zero);
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
        private readonly TW.ButtonCallback callback;
        private void Callback(IntPtr clientData)
        {
            OnClicked(EventArgs.Empty);
        }

        #region Misc.

        public override String ToString()
        {
            return String.Format("[Button: Label={0}]", Label);
        }

        #endregion
    }

    /// <summary>
    /// An AntTweakBar separator, which visually separates other variables.
    /// </summary>
    public class Separator : Variable
    {
        /// <summary>
        /// Initialization delegate, which creates the separator.
        /// </summary>
        private static void InitSeparator(Variable var, String id)
        {
            TW.AddSeparator(var.ParentBar.Pointer, id);
        }

        /// <summary>
        /// Creates a new separator in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the separator in.</param>
        /// <param name="def">An optional definition string for the new separator.</param>
        public Separator(Bar bar, String def = null)
            : base(bar, InitSeparator, def, false)
        {                                /* ^^^^^ */
            /* Special case: separators should have no labels. */
        }

        #region Misc.

        public override String ToString()
        {
            return String.Format("[Separator]");
        }

        #endregion
    }

    /// <summary>
    /// An AntTweakBar variable which can hold a string.
    /// </summary>
    public class StringVariable : Variable
    {
        /// <summary>
        /// Occurs when the user changes this variable's value.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        public void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Gets or sets the value of this variable.
        /// </summary>
        public String Value
        {
            get { return value; }
            set
            {
                if ((value == null) || !Validate (value))
                    throw new ArgumentOutOfRangeException("value", "Invalid variable value");
                else
                    this.value = value;
            }
        }

        private String value;

        /// <summary>
        /// Initialization delegate, which creates the string variable.
        /// </summary>
        private static void InitStringVariable(Variable var, String id)
        {
            TW.AddVarCB(var.ParentBar.Pointer, id,
                        TW.VariableType.TW_TYPE_CSSTRING,
                        ((StringVariable)var).SetCallback,
                        ((StringVariable)var).GetCallback,
                        IntPtr.Zero);
        }

        /// <summary>
        /// Creates a new string variable in a given bar.
        /// </summary>
        /// <param name="bar">The bar to create the string variable in.</param>
        /// <param name="initialValue">The initial value of the variable.</param>
        /// <param name="def">An optional definition string for the new variable.</param>
        public StringVariable(Bar bar, String initialValue = "", String def = null)
            : base(bar, InitStringVariable, def)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;
            Value = initialValue;
        }

        /// <summary>
        /// A validation method for derived classes. Override this to provide custom
        /// variables with arbitrary validation logic. If the user's input does not
        /// pass validation, the variable's value reverts to its previous contents.
        /// </summary>
        /// <remarks>
        /// The validation method is also invoked when setting the variable's value.
        /// </remarks>
        /// <param name="value">The value the variable will contain.</param>
        /// <returns>Whether the variable is allowed to have this value.</returns>
        protected virtual bool Validate(String value)
        {
            return true;
        }

        /// <summary>
        /// Called by AntTweakBar when the user changes the variable's value.
        /// </summary>
        private readonly TW.SetVarCallback setCallback;
        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            string tmp = Marshal.PtrToStringAnsi(pointer);
            bool changed = tmp != Value;
            Value = tmp;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Called by AntTweakBar when AntTweakBar needs the variable's value.
        /// </summary>
        private readonly TW.GetVarCallback getCallback;
        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            var bytes = Encoding.UTF8.GetBytes(Value);
            Marshal.Copy(bytes, 0, pointer, bytes.Length);
            ((byte*)pointer)[bytes.Length] = 0;
        }

        #region Misc.

        public override String ToString()
        {
            return String.Format("[StringVariable: Label={0}, Value={1}]", Label, Value);
        }

        #endregion
    }
}