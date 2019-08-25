using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveComponentModel
{
    /// <summary>
    /// Provides functionality to commit or rollback changes to an object that is used as a data
    /// source and provide errors for the object if it is in an invalid state.
    /// </summary>
    /// <typeparam name="T">The type of this instance.</typeparam>
    public abstract class EditableObject<T> : NotifyDataErrorInfo<T>, ICloneable<T>, IEditableObject
        where T : EditableObject<T>, new()
    {
        private readonly Subject<Unit> beginEditingSubject = new Subject<Unit>();
        private readonly Subject<Unit> cancelEditingSubject = new Subject<Unit>();
        private readonly Subject<Unit> endEditingSubject = new Subject<Unit>();

        /// <summary>
        /// Gets the when begin editing observable event. Occurs when beginning editing.
        /// </summary>
        /// <value>
        /// The when begin editing observable event.
        /// </value>
        public IObservable<Unit> WhenBeginEditing => beginEditingSubject.AsObservable();

        /// <summary>
        /// Gets the when cancel editing observable event. Occurs when cancelling editing.
        /// </summary>
        /// <value>
        /// The when begin cancel observable event.
        /// </value>
        public IObservable<Unit> WhenCancelEditing => cancelEditingSubject.AsObservable();

        /// <summary>
        /// Gets the when end editing observable event. Occurs when ending editing.
        /// </summary>
        /// <value>
        /// The when begin end observable event.
        /// </value>
        public IObservable<Unit> WhenEndEditing => endEditingSubject.AsObservable();

        /// <summary>
        /// Gets the original version of this object before <see cref="BeginEdit"/> was called.
        /// </summary>
        /// <value>The original version of this object before <see cref="BeginEdit"/> was called.</value>
        public T Original { get; private set; }

        /// <summary>
        /// Clones the clonable object of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>
        /// The cloned object of type <typeparamref name="T"/>.
        /// </returns>
        public T Clone()
        {
            var clone = new T();
            clone.Load((T)this);
            return clone;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Begins an edit on an object.
        /// </summary>
        public virtual void BeginEdit()
        {
            if (Original == null)
            {
                Original = Clone();

                OnBeginEditing();
            }
        }

        /// <summary>
        /// Discards changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> call.
        /// </summary>
        public virtual void CancelEdit()
        {
            if (Original != null)
            {
                Load(Original);

                Original = null;

                OnCancelEditing();
            }
        }

        /// <summary>
        /// Pushes changes since the last <see cref="M:System.ComponentModel.IEditableObject.BeginEdit"/> or <see cref="M:System.ComponentModel.IBindingList.AddNew"/> call into the underlying object.
        /// </summary>
        public virtual void EndEdit()
        {
            Original = null;

            OnEndEditing();
        }

        /// <summary>
        /// Loads the specified item.
        /// </summary>
        /// <param name="item">The item to load this instance from.</param>
        public abstract void Load(T item);

        /// <summary>
        /// Disposes the managed resources implementing <see cref="IDisposable" />.
        /// </summary>
        protected override void DisposeManaged()
        {
            beginEditingSubject.Dispose();
            cancelEditingSubject.Dispose();
            endEditingSubject.Dispose();
        }

        /// <summary>
        /// Called when editing has began.
        /// </summary>
        protected virtual void OnBeginEditing() => beginEditingSubject.OnNext(Unit.Default);

        /// <summary>
        /// Called when editing is cancelled.
        /// </summary>
        protected virtual void OnCancelEditing() => cancelEditingSubject.OnNext(Unit.Default);

        /// <summary>
        /// Called when editing has ended.
        /// </summary>
        protected virtual void OnEndEditing() => endEditingSubject.OnNext(Unit.Default);
    }
}
