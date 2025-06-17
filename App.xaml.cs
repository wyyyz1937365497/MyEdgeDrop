namespace MyEdgeDrop
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window()
            {
                Width = 385,
                Height = DeviceDisplay.MainDisplayInfo.Height-200,
                Page = new AppShell(),
            };
        }

    }
}
