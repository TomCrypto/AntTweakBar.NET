using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AntTweakBar
{
    /// <summary>
    /// An AntTweakBar group, to hierarchically organize variables in bars.
    /// </summary>
    public sealed class Group : IEquatable<Group>
    {
        /// <summary>
        /// Gets this group's parent bar.
        /// </summary>
        public Bar ParentBar { get { return parentBar; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Bar parentBar;

        /// <summary>
        /// Gets this group's unique identifier.
        /// </summary>
        internal String ID { get { return id; } }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private String id;

        /// <summary>
        /// Internal constructor used for reconstructing a group from an identifier.
        /// </summary>
        /// <param name="bar">The parent bar of the new group.</param>
        /// <param name="id">The identifier of the new group.</param>
        internal Group(Bar bar, String id)
        {
            this.parentBar = bar;
            this.id = id;
        }

        /// <summary>
        /// Creates a new group in a given bar and puts variables in it.
        /// </summary>
        /// <param name="parentBar">The bar the new group should belong to.</param>
        /// <param name="label">A label to display for the new group.</param>
        /// <param name="variables">Variables to put in the new group.</param>
        public Group(Bar parentBar, String label, params Variable[] variables)
        {
            if (parentBar == null) {
                throw new ArgumentNullException("parentBar");
            } else if (label == null) {
                throw new ArgumentNullException("label");
            } else if (variables == null) {
                throw new ArgumentNullException("variables");
            }

            id = Guid.NewGuid().ToString();
            this.parentBar = parentBar;

            foreach (var variable in variables) {
                variable.Group = this;
            }

            Label = label;
        }

        /// <summary>
        /// Creates a new group in a given bar and puts variables in it.
        /// </summary>
        /// <param name="parentBar">The bar the new group should belong to.</param>
        /// <param name="label">A label to display for the new group.</param>
        /// <param name="variables">Variables to put in the new group.</param>
        public Group(Bar parentBar, String label, IEnumerable<Variable> variables)
            : this(parentBar, label, variables.ToArray())
        {

        }

        /// <summary>
        /// Gets or sets this group's label.
        /// </summary>
        public String Label
        {
            get { return Tw.GetStringParam(ParentBar.Pointer, ID, "label"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "label", value); }
        }

        /// <summary>
        /// Gets the sets the parent group this group is in.
        /// </summary>
        public Group Parent
        {
            get
            {
                var groupID = Tw.GetStringParam(ParentBar.Pointer, ID, "group");
                if ((groupID != null) && (groupID != "")) {
                    return new Group(ParentBar, groupID);
                } else {
                    return null;
                }
            }

            set
            {
                if ((value != null) && (value.ParentBar != ParentBar)) {
                    throw new ArgumentException("Cannot move groups across bars.");
                }

                if (value != null) {
                    Tw.SetParam(ParentBar.Pointer, ID, "group", value.ID);
                } else {
                    Tw.SetParam(ParentBar.Pointer, ID, "group", "");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether this group is open or closed.
        /// </summary>
        public Boolean Open
        {
            get { return Tw.GetBooleanParam(ParentBar.Pointer, ID, "opened"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "opened", value); }
        }

        /// <summary>
        /// Gets or sets whether this group is visible or not.
        /// </summary>
        public Boolean Visible
        {
            get { return Tw.GetBooleanParam(ParentBar.Pointer, ID, "visible"); }
            set { Tw.SetParam(ParentBar.Pointer, ID, "visible", value); }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) {
                return false;
            }

            if (!(obj is Group)) {
                return false;
            }

            return Equals(obj as Group);
        }

        public bool Equals(Group other)
        {
            return (ID == other.ID);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override String ToString()
        {
            return String.Format("[Group: Label={0}]", Label);
        }
    }
}
