namespace ViewModelComposition
{
    using System;
    using System.Collections.ObjectModel;
    using Common;
    using Common.Services;
    using DryIoc;

    public class Program
    {
        public static void Main()
        {
            var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());
            container.Register<IService1, Service1>();
            container.Register<IService2, Service2>();
            container.Register<IService3, Service3>();
            container.Register<MessageViewModel>();
            container.Register<InputViewModel>();
            container.Register<ConversationViewModel>();

            var conversationViewModel = container.Resolve<ConversationViewModel>();
            conversationViewModel.AddMessage("Hello World");

            Console.Read();
        }
    }

    public class ConversationViewModel : NotifyPropertyChanges
    {
        private readonly Func<MessageViewModel> messageViewModelFactory;
        private readonly IService1 service1;

        public ConversationViewModel(
            InputViewModel inputViewModel,
            Func<MessageViewModel> messageViewModelFactory,
            IService1 service1)
        {
            this.Input = inputViewModel;
            this.messageViewModelFactory = messageViewModelFactory;
            this.service1 = service1;

            this.Messages = new ObservableCollection<MessageViewModel>();
        }

        public InputViewModel Input { get; }

        public ObservableCollection<MessageViewModel> Messages { get; }

        public void AddMessage(string text)
        {
            var messageViewModel = this.messageViewModelFactory();
            messageViewModel.Text = text;
            this.Messages.Add(messageViewModel);
        }
    }

    public class InputViewModel : NotifyPropertyChanges
    {
        private readonly IService2 service2;

        private string text;

        public InputViewModel(IService2 service2) => this.service2 = service2;

        public string Text
        {
            get => this.text;
            set => this.SetProperty(ref this.text, value);
        }
    }

    public class MessageViewModel : NotifyPropertyChanges
    {
        private readonly IService3 service3;

        private string text;

        public MessageViewModel(IService3 service3) => this.service3 = service3;

        public string Text
        {
            get => this.text;
            set => this.SetProperty(ref this.text, value);
        }
    }
}
