using System;
using CommandLine;
using ConsoleTables;
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

            Parser.Default.ParseArguments<AddOptions, ListOptions, GetOptions, MarkDoneOptions, DeleteOptions>(args)
                .MapResult(
                    (AddOptions opts) => AddTask(taskRepository, opts),
                    (ListOptions opts) => ListTasks(taskRepository, opts),
                    (GetOptions opts) => GetTask(taskRepository, opts),
                    (MarkDoneOptions opts) => MarkDoneTask(taskRepository, opts),
                    (DeleteOptions opts) => DeleteTask(taskRepository, opts),
                    errs => 1
                );
        }

        private static int AddTask(ITaskRepository taskRepository, AddOptions options)
        {
            var task = new Task(options.Name, options.Description);
            taskRepository.Save(task);
            Console.WriteLine("Task added successfully.");
            return 0;
        }

        private static int ListTasks(ITaskRepository taskRepository, ListOptions options)
        {
            var table = new ConsoleTable("ID", "name", "description", "done?");

            foreach (Task task in taskRepository.FindAll())
            {
                table.AddRow(task.Id, task.Name, task.Description, task.Done);
            }

            table.Write();

            return 0;
        }

        private static int GetTask(ITaskRepository taskRepository, GetOptions options)
        {
            Task? task = taskRepository.FindById(options.Id);

            if (task != null)
            {
                // Print task information
                Console.WriteLine($"Task ID={task.Id}");
                Console.WriteLine($"Name: {task.Name}");
                Console.WriteLine($"Description: {task.Description}");
                Console.WriteLine($"Done: {task.Done}");
                return 0;
            }
            else
            {
                Console.WriteLine("Task not found");
                return 1;
            }
        }

        private static int MarkDoneTask(ITaskRepository taskRepository, MarkDoneOptions options)
        {
            Task? task = taskRepository.FindById(options.Id);

            if (task != null)
            {
                if (!task.Done)
                {
                    task.Done = true;
                    taskRepository.Save(task);
                    Console.WriteLine($"Task {task.Id} marked as done!");
                    return 0;
                }
                else
                {
                    Console.WriteLine($"Task already marked as done");
                    return 1;
                }
            }
            else
            {
                Console.WriteLine("Task not found");
                return 1;
            }
        }

        private static int DeleteTask(ITaskRepository taskRepository, DeleteOptions options)
        {
            bool deleted = taskRepository.DeleteById(options.Id);
            if (deleted)
            {
                Console.Write($"Deleted entity with id {options.Id}");
                return 0;
            }
            else
            {
                Console.WriteLine($"Task not found");
                return 1;
            }
        }
    }

    [Verb("add", HelpText = "Add a new task.")]
    internal class AddOptions
    {
        [Option('n', "name", Required = true, HelpText = "Name of the task.")]
        public string Name { get; set; } = string.Empty;

        [Option('d', "description", Required = true, HelpText = "Description of the task.")]
        public string Description { get; set; } = string.Empty;
    }

    [Verb("list")]
    internal class ListOptions
    {
    }

    [Verb("get", HelpText = "Get a task by ID.")]
    internal class GetOptions
    {
        [Option('i', "id", Required = true, HelpText = "ID of the task.")]
        public int Id { get; set; }
    }

    [Verb("mark-done", HelpText = "Mark task as done.")]
    internal class MarkDoneOptions
    {
        [Option('i', "id", Required = true, HelpText = "Id of the task.")]
        public int Id { get; set; }
    }

    [Verb("delete", HelpText = "Delete a task by ID.")]
    internal class DeleteOptions
    {
        [Option('i', "id", Required = true, HelpText = "ID of the task.")]
        public int Id { get; set; }
    }
}