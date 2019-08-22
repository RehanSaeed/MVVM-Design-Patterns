namespace CommandPattern
{
    using System;
    using System.Threading.Tasks;
    using Common;
    using Common.Commands;
    using Common.Services;
    using DryIoc;

    public class Program
    {
        public static async Task Main()
        {
            var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());
            container.Register<IService1, Service1>();
            container.Register<IService2, Service2>();
            container.Register<IService3, Service3>();
            container.Register<SomeViewModel>();
            container.Register<GreetingCommand>();
            container.Register<OtherViewModel>();

            var someViewModel = container.Resolve<SomeViewModel>();

            if (someViewModel.NoParameterCommand.CanExecute())
            {
                someViewModel.NoParameterCommand.Execute();
            }

            if (someViewModel.HasParameterCommand.CanExecute("Hi"))
            {
                someViewModel.HasParameterCommand.Execute("Hi");
            }

            if (someViewModel.NoParameterAsyncCommand.CanExecute())
            {
                await someViewModel.NoParameterAsyncCommand.Execute();
            }

            if (someViewModel.HasParameterAsyncCommand.CanExecute("Hi"))
            {
                await someViewModel.HasParameterAsyncCommand.Execute("Hi");
            }

            var otherViewModel = container.Resolve<OtherViewModel>();
            if (otherViewModel.GreetingCommand.CanExecute("Foo"))
            {
                // This should never execute
            }

            if (otherViewModel.GreetingCommand.CanExecute("Hello"))
            {
                otherViewModel.GreetingCommand.Execute("Hello");
            }

            Console.Read();
        }
    }

    public class SomeViewModel : NotifyPropertyChanges
    {
        public SomeViewModel()
        {
            this.NoParameterCommand = new DelegateCommand(this.NoParameter);
            this.HasParameterCommand = new DelegateCommand<string>(this.HasParameter);
            this.NoParameterAsyncCommand = new AsyncDelegateCommand(this.NoParameterAsync);
            this.HasParameterAsyncCommand = new AsyncDelegateCommand<string>(this.HasParameterAsync);
        }

        public DelegateCommand NoParameterCommand { get; }

        public DelegateCommand<string> HasParameterCommand { get; }

        public AsyncDelegateCommand NoParameterAsyncCommand { get; }

        public AsyncDelegateCommand<string> HasParameterAsyncCommand { get; }

        private void NoParameter() => Console.WriteLine("Hello World");

        private void HasParameter(string greeting) => Console.WriteLine($"{greeting}");

        private Task NoParameterAsync()
        {
            Console.WriteLine("Async Hello World");
            return Task.CompletedTask;
        }

        private Task HasParameterAsync(string greeting)
        {
            Console.WriteLine($"Async {greeting}");
            return Task.CompletedTask;
        }
    }

    public class GreetingCommand : Command<string>
    {
        private readonly IService1 service1;

        public GreetingCommand(IService1 service1) => this.service1 = service1;

        // returning false from CanExecute can cause a control's IsEnabled property to be set to false.
        // You can use this behaviour to do other things like hide the control:
        // <Style.Triggers>
        //     <Trigger Property = "IsEnabled" Value="False">
        //         <Setter Property="Visibility" Value="Collapsed"/>
        //     </Trigger>
        // </Style.Triggers>
        public override bool CanExecute(string parameter) => parameter.Contains("H");

        public override void Execute(string greeting) => Console.WriteLine(greeting);
    }

    public class OtherViewModel : NotifyPropertyChanges
    {
        private readonly IService2 service2;

        public OtherViewModel(
            GreetingCommand greetingCommand,
            IService2 service2)
        {
            this.GreetingCommand = greetingCommand;
            this.service2 = service2;
        }

        public GreetingCommand GreetingCommand { get; }
    }
}
