- How to set up in [Startup.cs]:


public void ConfigureServices(IServiceCollection services)
{
     // - Register the localization service for use within the application.
     services.AddTransient<ILocalization, Localization>();
}


public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
     // - Adds a dictionary with a culture name and words.
     Localization.AddDictionary("pt-BR", 
         new Dictionary<string, string>
         {
             { "Hello World!", "Olá Mundo!" }
         }
     );
     
     // - Or use a static preset dictionary defined elsewhere in your project
     Localization.AddDictionary("en-US", EN_US.myDictionary);
}


- How to use it in your controller:


public class HomeController
{
     private Localization loc;
     // - Requests the registered service via injection in the constructor.
     public HomeController(Localization _loc)
     {
         loc = _loc;
     }
     
     public IActionResult Index()
     {
         // - Look up for the "Hello World!" string in the current dictionary, defined by the culture method below. If empty or word not found it'll return the requested string "Hello World!".
         string helloWorld = loc["Hello World!"];
         return View();
     }
     
     [HttpGet]
     public IActionResult SetCulture(string culture)
     {
         // - Sets the culture cookie value. Requests from this user/cookie will look for a dictionary with the same culture name.
         loc.SetCulture(culture);
         
         return Redirect(Request.Headers["Referer"].ToString());
     }
}


- How to use it in your view (.cshtml):


@inject Helpers.Localization loc
<html>
     <span>@loc["Hello World!"]</span>
</html>