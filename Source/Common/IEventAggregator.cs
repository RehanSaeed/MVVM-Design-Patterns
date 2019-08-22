namespace Common
{
    using System;

    /// <summary>
    /// Channels events from multiple objects into a single object to simplify registration for clients.
    /// </summary>
    /// <remarks>See https://www.martinfowler.com/eaaDev/EventAggregator.html.</remarks>
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets the event with the specified type.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <returns>An <see cref="IObservable{TEvent}"/>.</returns>
        IObservable<TEvent> GetEvent<TEvent>();

        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventValue">The event to publish.</param>
        void Publish<TEvent>(TEvent eventValue);
    }
}
