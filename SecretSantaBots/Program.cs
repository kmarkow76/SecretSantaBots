// See https://aka.ms/new-console-template for more information


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecretSantaBots.DataBase;


Console.WriteLine("Hello, World!");
var host = ApplicationDbContext.CreateHostBuilder(args).Build();




