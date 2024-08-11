using System;
using CommandLine;
using ConsoleTables;
using CsharpTaskManagement.Cli;
using CsharpTaskManagement.Models;
using CsharpTaskManagement.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace CsharpTaskManagement
{
    internal class Program
    {
        private static readonly string _jsonFilePath = "tasks.json";

        public static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddTransient<ITaskRepository>(provider => new JsonFileTaskRepository(_jsonFilePath))
                .BuildServiceProvider();

            ITaskRepository taskRepository = services.GetService<ITaskRepository>()!;

            var cli = new CsharpTaskManagementCli(taskRepository);
            cli.Start(args);
        }
    }
}