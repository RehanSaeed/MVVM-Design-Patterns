namespace EventAggregatorPattern
{
    using System;
    using Common;
    using DryIoc;

    public class Program
    {
        public static void Main()
        {
            var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());
            container.Register<IEventAggregator, EventAggregator>(Reuse.Singleton);
            container.Register<PublisherViewModel>();
            container.Register<SubscriberViewModel>();

            var subscriberViewModel = container.Resolve<SubscriberViewModel>();
            var publisherViewModel = container.Resolve<PublisherViewModel>();
            publisherViewModel.NewMessage("Hello");

            Console.Read();
        }
    }

    public class NewMessageEvent
    {
        public NewMessageEvent(string text) => this.Text = text;

        public string Text { get; }
    }

    public class PublisherViewModel
    {
        private readonly IEventAggregator eventAggregator;

        public PublisherViewModel(IEventAggregator eventAggregator) => this.eventAggregator = eventAggregator;

        public void NewMessage(string text) => this.eventAggregator.Publish(new NewMessageEvent(text));
    }

    public class SubscriberViewModel
    {
        public SubscriberViewModel(IEventAggregator eventAggregator) =>
            eventAggregator
                .GetEvent<NewMessageEvent>()
                // .ObserveOnDispatcher()
                .Subscribe(this.OnNewMessage);

        private void OnNewMessage(NewMessageEvent newMessageEvent) => Console.WriteLine(newMessageEvent.Text);
    }

    // Publisher and subscriber are completely decoupled.
    // There can be multiple publishers e.g. Clear conversation history from two totally different parts of the app.
    // There can be multiple subscribers e.g. New user message event kicks off multiple view models.
}
