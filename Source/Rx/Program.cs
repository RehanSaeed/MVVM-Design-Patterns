namespace Rx
{
    using System;
    using System.Collections.Generic;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;
    using Common;
    using DryIoc;

    public class Program
    {
        public static void Main()
        {
            var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());
            container.Register<CSharpEventPublisherViewModel>(Reuse.Singleton);
            container.Register<CSharpEventSubscriberViewModel>(Reuse.Singleton);
            container.Register<RxPublisherViewModel>(Reuse.Singleton);
            container.Register<RxSubscriberViewModel>(Reuse.Singleton);

            CSharpEventUsage.Execute(container);
            ReactiveExtensionsUsage.Execute(container);

            Console.Read();
        }
    }

    #region C# Events

    public static class CSharpEventUsage
    {
        public static void Execute(Container container)
        {
            var subscriberViewModel = container.Resolve<CSharpEventSubscriberViewModel>();
            var publisherViewModel = container.Resolve<CSharpEventPublisherViewModel>();
            publisherViewModel.RaiseNewMessage("Hello");
        }
    }

    public class NewMessageEventArgs : EventArgs
    {
        public NewMessageEventArgs(string text) => this.Text = text;

        public string Text { get; }
    }

    public class CSharpEventPublisherViewModel
    {
        public event EventHandler<NewMessageEventArgs> NewMessage;

        public void RaiseNewMessage(string text) => this.NewMessage.Invoke(this, new NewMessageEventArgs(text));
    }

    public class CSharpEventSubscriberViewModel : Disposable
    {
        private readonly CSharpEventPublisherViewModel publisherViewModel;

        public CSharpEventSubscriberViewModel(CSharpEventPublisherViewModel publisherViewModel)
        {
            this.publisherViewModel = publisherViewModel;
            publisherViewModel.NewMessage += this.OnNewMessage;
        }

        protected override void DisposeManaged() => this.publisherViewModel.NewMessage -= this.OnNewMessage;

        private void OnNewMessage(object sender, NewMessageEventArgs e) => Console.WriteLine(e.Text);
    }

    #endregion

    #region IEnumerator is Dual of IObserver

    public class Dual
    {
        public Dual()
        {
#pragma warning disable CS0219 // Shhh...sleep C# compiler
            IEnumerable<int> enumerable = null;
            IEnumerator<int> enumerator = null;

            IObservable<int> observable = null;
            IObserver<int> observer = null;
#pragma warning restore CS0219
        }
    }

    #endregion

    #region Reactive Extensions

    public static class ReactiveExtensionsUsage
    {
        public static void Execute(Container container)
        {
            var subscriberViewModel = container.Resolve<RxSubscriberViewModel>();
            var publisherViewModel = container.Resolve<RxPublisherViewModel>();

            publisherViewModel.NewMessage("Hi");
            publisherViewModel.NewMessage("Hello");
        }
    }

    public class RxPublisherViewModel
    {
        private readonly Subject<string> newMessageSubject = new Subject<string>();

        public IObservable<string> WhenNewMessage => this.newMessageSubject.AsObservable();

        public void NewMessage(string text) => this.newMessageSubject.OnNext(text);
    }

    public class RxSubscriberViewModel : Disposable
    {
        private readonly IDisposable newMessageSubscription;

        public RxSubscriberViewModel(RxPublisherViewModel publisherViewModel) =>
            this.newMessageSubscription = publisherViewModel.WhenNewMessage
                .Where(x => x.Contains("Hello"))
                .Select(x => $"{x} ({x.Length})")
                .Throttle(TimeSpan.FromSeconds(1))
                // .ObserveOnDispatcher();
                .Subscribe(this.OnNewMessage);

        protected override void DisposeManaged() => this.newMessageSubscription.Dispose();

        private void OnNewMessage(string text) => Console.WriteLine(text);
    }

    #endregion


}
