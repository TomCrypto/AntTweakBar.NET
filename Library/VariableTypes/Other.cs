using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace AntTweakBar
{
    /// <summary>
    /// An event-driven button.
    /// </summary>
    public sealed class Button : Variable
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

        public Button(Bar bar, EventHandler clicked = null, String def = null) : base(bar)
        {
            Clicked += clicked;
            callback = Callback;

            TW.AddButton(Owner, ID, callback, IntPtr.Zero);

            Owner.Add(this);
            Label = "undef";

            if (def != null)
                SetDefinition(def);
        }
    }

    /// <summary>
    /// A separator to delimitate variables.
    /// </summary>
    public sealed class Separator : Variable
    {
        public Separator(Bar bar, String def = null) : base(bar)
        {
            TW.AddSeparator(Owner, ID);

            Owner.Add(this);
            Label = "undef";

            if (def != null)
                SetDefinition(def);
        }
    }

    /// <summary>
    /// A variable holding a string value.
    /// </summary>
    public sealed class StringVariable : Variable<String>
    {
        public StringVariable(Bar bar, String initialValue = "", String def = null)
            : base(bar, initialValue, def)
        {

        }

        protected override bool Validate(String newValue)
        {
            return (newValue != null);
        }
    }
}

