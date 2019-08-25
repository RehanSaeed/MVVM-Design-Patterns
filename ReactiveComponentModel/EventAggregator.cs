using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace ReactiveComponentModel
{
    /// <inheritdoc />
    public class EventAggregator : Disposable, IEventAggregator
    {
        private readonly ConcurrentDictionary<Type, object> subjects = new ConcurrentDictionary<Type, object>();

        /// <inheritdoc />
        public IObservable<TEvent> GetEvent<TEvent>() =>
            (ISubject<TEvent>)subjects.GetOrAdd(typeof(TEvent), x => new Subject<TEvent>());

        /// <inheritdoc />
        public void Publish<TEvent>(TEvent eventValue)
        {
            if (subjects.TryGetValue(typeof(TEvent), out var subject))
            {
                ((ISubject<TEvent>)subject).OnNext(eventValue);
            }
        }

        /// <inheritdoc />
        protected override void DisposeManaged()
        {
            foreach (var item in subjects)
            {
                if (item.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
