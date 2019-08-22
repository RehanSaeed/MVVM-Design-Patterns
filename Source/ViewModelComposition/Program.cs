using Common;
using DryIoc;
using System;
using System.Collections.ObjectModel;

namespace ViewModelComposition
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());
            container.Register<IService1, Service1>();
            container.Register<IService2, Service2>();
            container.Register<IService3, Service3>();
            container.Register<MessageViewModel>();
            container.Register<TextViewModel>();
            container.Register<ConversationViewModel>();

            var conversationViewModel = container.Resolve<ConversationViewModel>();
            conversationViewModel.AddMessage();

            Console.Read();
        }
    }

    public class ConversationViewModel
    {
        private readonly Func<MessageViewModel> messageViewModelFactory;
        private readonly IService1 service1;

        public ConversationViewModel(
            TextViewModel textViewModel,
            Func<MessageViewModel> messageViewModelFactory,
            IService1 service1)
        {
            this.Text = textViewModel;
            this.messageViewModelFactory = messageViewModelFactory;
            this.service1 = service1;

            this.Messages = new ObservableCollection<MessageViewModel>();
        }

        public ObservableCollection<MessageViewModel> Messages { get; }

        public TextViewModel Text { get; }

        public void AddMessage()
        {
            var messageViewModel = this.messageViewModelFactory();
            messageViewModel.Text = "Hello World";
            this.Messages.Add(messageViewModel);
        }
    }

    public class TextViewModel : NotifyPropertyChanges
    {
        private readonly IService2 service2;

        private string text;

        public TextViewModel(IService2 service2)
        {
            this.service2 = service2;
        }

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

        public MessageViewModel(IService3 service3)
        {
            this.service3 = service3;
        }

        public string Text
        {
            get => this.text;
            set => this.SetProperty(ref this.text, value);
        }
    }
}
