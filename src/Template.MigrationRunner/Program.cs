using Microsoft.Extensions.DependencyInjection;
using Template.MigrationRunner.Configuration;
using Template.MigrationRunner.ConsoleMenu;

var host = Configure.Build(args);

var menuRunner = host.Services.GetService<MenuRunner>();

menuRunner?.Run(args);
