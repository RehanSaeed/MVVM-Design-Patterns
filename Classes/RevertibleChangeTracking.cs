using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReactiveComponentModel
{
    /// <summary>
    /// Provides support for rolling back changes made to this objects properties. This object
    /// automatically saves its state before it is changed. Also provides errors for the object if
    /// it is in an invalid state.
    /// </summary>
    /// <typeparam name="T">The type of this instance.</typeparam>
    public abstract class RevertibleChangeTracking<T> : EditableObject<T>, IRevertibleChangeTracking, IEquatable<T>
        where T : RevertibleChangeTracking<T>, new()
    {
        private bool isChanged;
        private bool isChangeTrackingEnabled;
        private bool isNew = true;

        /// <summary>
        /// Gets or sets a value indicating whether the change tracking of the object's status is enabled.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if change tracking ir enabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsChangeTrackingEnabled
        {
            get => isChangeTrackingEnabled;
            set
            {
                base.OnPropertyChanging("IsChangeTrackingEnabled");
                isChangeTrackingEnabled = value;
                base.OnPropertyChanged("IsChangeTrackingEnabled");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object's status changed.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if the object’s content has changed since the last call to <see cref="M:System.ComponentModel.IChangeTracking.AcceptChanges"/>; otherwise, <c>false</c>.
        /// </returns>
        public bool IsChanged
        {
            get => isChanged;

            private set
            {
                base.OnPropertyChanging("IsChanged");
                isChanged = value;
                base.OnPropertyChanged("IsChanged");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew
        {
            get => isNew;
            private set => SetProperty(ref isNew, value);
        }

        /// <summary>
        /// Resets the object’s state to unchanged by accepting the modifications.
        /// </summary>
        public virtual void AcceptChanges()
        {
            if (IsNew)
            {
                IsNew = false;
                IsChangeTrackingEnabled = true;
            }
            else if (IsChanged)
            {
                EndEdit();

                IsChanged = false;
            }
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
        /// </summary>
        public override void CancelEdit()
        {
            base.CancelEdit();

            IsChanged = false;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public virtual bool Equals(T other) => object.Equals(this, other);

        /// <summary>
        /// Resets the object’s state to unchanged by rejecting the modifications.
        /// </summary>
        public virtual void RejectChanges() => CancelEdit();

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (IsChangeTrackingEnabled)
            {
                if (Equals(Original))
                {
                    IsChanged = false;
                }
                else
                {
                    IsChanged = true;
                }
            }
        }

        /// <summary>
        /// Raises the PropertyChanging event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected override void OnPropertyChanging([CallerMemberName] string propertyName = null)
        {
            if (IsChangeTrackingEnabled)
            {
                BeginEdit();
            }

            base.OnPropertyChanging(propertyName);
        }
    }
}
