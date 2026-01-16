# Dietcode.Core Lib

Bilioteca de functins e métodos que ajudfam a melhorar a automatização e trabalho dos codigos em .NET.

---

## 🚀 Instalação

Via **.NET CLI**:

```bash
dotnet add package Dietcode.Core.Lib --version 4.0.0

## 📚 Documentação
- How to set up in [Startup.cs]:

#csharp
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

## 📚 Documentação

- How to use it in your controller:

#csharp

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

## 📚 Documentação

- How to use it in your view (.cshtml):

#csharp

@inject Helpers.Localization loc
<html>
     <span>@loc["Hello World!"]</span>
</html>