namespace CommandPattern
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Common;
    using Common.Commands;
    using Common.Services;
    using DryIoc;

    /// <summary>
    /// See <see cref="ICommand"/>.
    /// </summary>
    /// <example>
    /// Command="{Binding Foo}"
    /// CommandParameter="{Binding Bar}"
    /// </example>
    public class Program
    {
        public static async Task Main()
        {
            var container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());
            container.Register<IService1, Service1>();
            container.Register<IService2, Service2>();
            container.Register<IService3, Service3>();

            container.Register<FirstViewModel>();
            await FirstUsage.Execute(container);

            container.Register<GreetingCommand>();
            container.Register<SecondViewModel>();
            SecondUsage.Execute(container);

            container.Register<DoStuffOnTheViewModelCommand>();
            container.Register<ThirdViewModel>();
            ThirdUsage.Execute(container);

            Console.Read();
        }
    }

    #region First

    public static class FirstUsage
    {
        public static async Task Execute(Container container)
        {
            var firstViewModel = container.Resolve<FirstViewModel>();

            if (firstViewModel.NoParameterCommand.CanExecute())
            {
                firstViewModel.NoParameterCommand.Execute();
            }

            if (firstViewModel.HasParameterCommand.CanExecute("Hi"))
            {
                firstViewModel.HasParameterCommand.Execute("Hi");
            }

            if (firstViewModel.NoParameterAsyncCommand.CanExecute())
            {
                await firstViewModel.NoParameterAsyncCommand.Execute();
            }

            if (firstViewModel.HasParameterAsyncCommand.CanExecute("Hi"))
            {
                await firstViewModel.HasParameterAsyncCommand.Execute("Hi");
            }
        }
    }

    public class FirstViewModel : NotifyPropertyChanges
    {
        public FirstViewModel()
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

    #endregion

    #region Second

    public static class SecondUsage
    {
        public static void Execute(Container container)
        {
            var secondViewModel = container.Resolve<SecondViewModel>();
            if (secondViewModel.GreetingCommand.CanExecute("Foo"))
            {
                // This should never execute
            }

            if (secondViewModel.GreetingCommand.CanExecute("Hello"))
            {
                secondViewModel.GreetingCommand.Execute("Hello");
            }
        }
    }

    public class SecondViewModel : NotifyPropertyChanges
    {
        private readonly IService2 service2;

        public SecondViewModel(
            GreetingCommand greetingCommand,
            IService2 service2)
        {
            this.GreetingCommand = greetingCommand;
            this.service2 = service2;
        }

        // returning false from CanExecute can cause a control's IsEnabled property to be set to false.
        // You can use this behaviour to do other things like hide the control:
        // <Button Command="{Binding GreetingCommand}">
        //     <Button.Style>
        //         <Style TargetType="Button">
        //             <Style.Triggers>
        //                 <Trigger Property="IsEnabled" Value="False">
        //                     <Setter Property="Visibility" Value="Collapsed"/>
        //                 </Trigger>
        //             </Style.Triggers>
        //         </Style>
        //     </Button.Style>
        // </Button>
        public GreetingCommand GreetingCommand { get; }
    }

    public class GreetingCommand : Command<string>
    {
        private readonly IService1 service1;

        public GreetingCommand(IService1 service1) => this.service1 = service1;

        public override bool CanExecute(string parameter) => parameter.Contains("H");

        public override void Execute(string greeting) => Console.WriteLine(greeting);
    }

    #endregion

    #region Third

    public static class ThirdUsage
    {
        public static void Execute(Container container)
        {
            var thirdViewModel = container.Resolve<ThirdViewModel>();
            if (thirdViewModel.DoStuffOnTheViewModelCommand.CanExecute(thirdViewModel))
            {
                thirdViewModel.DoStuffOnTheViewModelCommand.Execute(thirdViewModel);
            }
        }
    }

    public class ThirdViewModel : NotifyPropertyChanges
    {
        private readonly IService3 service3;
        private string text;

        public ThirdViewModel(
            DoStuffOnTheViewModelCommand doStuffOnTheViewModelCommand,
            IService3 service3)
        {
            this.DoStuffOnTheViewModelCommand = doStuffOnTheViewModelCommand;
            this.service3 = service3;
        }

        // <Button Command="{Binding DoStuffOnTheViewModelCommand}"
        //         CommandParameter="{Binding}"/>
        public DoStuffOnTheViewModelCommand DoStuffOnTheViewModelCommand { get; }

        public string Text
        {
            get => this.text;
            set => this.SetProperty(ref this.text, value);
        }
    }

    public class DoStuffOnTheViewModelCommand : Command<ThirdViewModel>
    {
        private readonly IService1 service1;

        public DoStuffOnTheViewModelCommand(IService1 service1) => this.service1 = service1;

        public override void Execute(ThirdViewModel otherViewModel) => otherViewModel.Text = "Hello World";
    }

    #endregion
}
