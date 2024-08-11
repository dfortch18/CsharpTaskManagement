using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using CsharpTaskManagement.Models;

namespace CsharpTaskManagement.Repositories
{
    public interface ITaskRepository : ICrudRepository<Task, int>
    {
    }

    public class JsonFileTaskRepository : ITaskRepository
    {
        private List<Task> _tasks = [];

        private readonly string _jsonFilePath;

        private int _currentId;

        private readonly JsonSerializerOptions _serializerOptions;

        private static readonly JsonSerializerOptions _defaultSerializerOptions = new() { WriteIndented = true };

        public JsonFileTaskRepository(string jsonFilePath, JsonSerializerOptions serializerOptions)
        {
            _jsonFilePath = jsonFilePath;
            _serializerOptions = serializerOptions;
            LoadJson();
            _currentId = _tasks.Count != 0 ? _tasks.Max(t => t.Id) : 0;
        }

        public JsonFileTaskRepository(string jsonFilePath) : this(jsonFilePath, _defaultSerializerOptions)
        {
        }

        public bool Delete(Task entity)
        {
            bool result = _tasks.Remove(entity);
            if (result)
            {
                SaveJson();
            }
            return result;
        }

        public bool DeleteById(int id)
        {
            int removedElements = _tasks.RemoveAll(t => t.Id == id);
            if (removedElements >= 1)
            {
                SaveJson();
                return true;
            }
            return false;
        }

        public IEnumerable<Task> FindAll()
        {
            return _tasks;
        }

        public Task? FindById(int id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public Task? Save(Task entity)
        {
            if (entity.Id == 0)
            {
                _currentId++;
                entity.Id = _currentId;
                _tasks.Add(entity);
            }
            else
            {
                var existingTask = _tasks.FirstOrDefault(t => t.Id == entity.Id);

                if (existingTask != null)
                {
                    existingTask.Name = entity.Name;
                    existingTask.Description = entity.Description;
                    existingTask.Done = entity.Done;
                }
                else
                {
                    _tasks.Add(entity);
                }
            }

            SaveJson();
            return entity;
        }

        private void LoadJson()
        {
            if (File.Exists(_jsonFilePath))
            {
                var json = File.ReadAllText(_jsonFilePath);
                try
                {
                    var tasks = JsonSerializer.Deserialize<List<Task>>(json);
                    _tasks = tasks ?? [];
                }
                catch (JsonException)
                {
                    _tasks = [];
                }
            }
            else
            {
                _tasks = [];
            }
        }

        private void SaveJson()
        {
            var json = JsonSerializer.Serialize(_tasks, _serializerOptions);
            File.WriteAllText(_jsonFilePath, json);
        }
    }
}