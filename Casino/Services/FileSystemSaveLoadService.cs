using System;
using System.IO;
using Services;

namespace Services
{
    public class FileSystemSaveLoadService : ISaveLoadService<string>
    {
        private readonly string _path;

        public FileSystemSaveLoadService(string path)
        {
            _path = path;
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
        }

        public void SaveData(string data, string id)
        {
            string filePath = Path.Combine(_path, $"{id}.txt");
            File.WriteAllText(filePath, data);
        }

        public string LoadData(string id)
        {
            string filePath = Path.Combine(_path, $"{id}.txt");
            if (!File.Exists(filePath))
                return null;
            return File.ReadAllText(filePath);
        }
    }
}
