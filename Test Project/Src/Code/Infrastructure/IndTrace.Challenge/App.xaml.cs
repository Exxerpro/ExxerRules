namespace IndTrace.Challenge
{
    /// <summary>
    /// Represents the main application class for the IndTrace Challenge MAUI application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates the main application window when the app is activated.
        /// </summary>
        /// <param name="activationState">The activation state information.</param>
        /// <returns>A new <see cref="Window"/> instance containing the main page.</returns>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage());
        }
    }
}
