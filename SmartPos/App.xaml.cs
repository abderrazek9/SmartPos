using SmartPos.Data;

namespace SmartPos
{
    public partial class App : Application
    {


       // private readonly DataBaseService _dataBaseService;

        public App(DataBaseService dataBaseService)
        {
            InitializeComponent();

            MainPage = new AppShell();


           // _dataBaseService = dataBaseService;

            Task.Run(async () => await dataBaseService.InitializeDataBaseAsync())
                .GetAwaiter()
                .GetResult();
        }

        //  protected override async void OnStart()
        // {
        //   base.OnStart();
        // initialisei and seed Data base
        //await _dataBaseService.InitializeDataBaseAsync();
        //  }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

           window.Height = window.MinimumHeight = 780;
           window.Width = window.MinimumWidth = 1280;



            return window;
        }

    }
}