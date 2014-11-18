using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AntTweakBar
{
    /// <summary>
    /// An event-driven button.
    /// </summary>
    public class Button : Variable
    {
        private readonly TW.ButtonCallback callback;

        /// <summary>
        /// Called by AntTweakBar when the user clicks on the button.
        /// </summary>
        private void Callback(IntPtr clientData)
        {
            OnClicked(EventArgs.Empty);
        }

        /// <summary>
        /// Occurs when the button is clicked.
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Raises the Clicked event.
        /// </summary>
        private void OnClicked(EventArgs e)
        {
            if (Clicked != null)
                Clicked(this, e);
        }

        private static void InitVariable(Variable var, String id)
        {
            TW.AddButton(var.ParentBar.Pointer, id,
                         ((Button)var).Callback,
                         IntPtr.Zero);
        }

        public Button(Bar bar, EventHandler clicked = null, String def = null)
            : base(bar, InitVariable, def)
        {
            callback = Callback;
            Clicked += clicked;
        }
    }

    /// <summary>
    /// A separator to delimitate variables.
    /// </summary>
    public class Separator : Variable
    {
        private static void InitVariable(Variable var, String id)
        {
            TW.AddSeparator(var.ParentBar.Pointer, id);
        }

        public Separator(Bar bar, String def = null)
            : base(bar, InitVariable, def)
        {

        }
    }

    /// <summary>
    /// A variable holding a string value.
    /// </summary>
    public class StringVariable : Variable
    {
        #region Fields

        private readonly TW.GetVarCallback getCallback;
        private readonly TW.SetVarCallback setCallback;

        /// <summary>
        /// Occurs when the user changes the variable.
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        private void OnChanged(EventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        private String value;

        #endregion

        private static void InitVariable(Variable var, String id)
        {
            TW.AddVarCB(var.ParentBar.Pointer, id,
                        TW.VariableType.TW_TYPE_CSSTRING,
                        ((StringVariable)var).SetCallback,
                        ((StringVariable)var).GetCallback,
                        IntPtr.Zero);
        }

        public StringVariable(Bar bar, String initialValue = "", String def = null)
            : base(bar, InitVariable, def)
        {
            setCallback = SetCallback;
            getCallback = GetCallback;
            Value = initialValue;
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

        protected virtual bool Validate(String value)
        {
            return true; /* to be overridden */
        }

        private unsafe void SetCallback(IntPtr pointer, IntPtr clientData)
        {
            string tmp = Marshal.PtrToStringAnsi(pointer);
            bool changed = tmp != Value;
            Value = tmp;

            if (changed)
                OnChanged(EventArgs.Empty);
        }

        private unsafe void GetCallback(IntPtr pointer, IntPtr clientData)
        {
            var bytes = Encoding.UTF8.GetBytes(Value);
            Marshal.Copy(bytes, 0, pointer, bytes.Length);
            ((byte*)pointer)[bytes.Length] = 0;
        }
    }
}

