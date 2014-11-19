using System;

namespace AntTweakBar
{
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
            TW.AddSeparator(var.ParentBar.Pointer, id, null);
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

        public override String ToString()
        {
            return String.Format("[Separator]");
        }
    }
}
