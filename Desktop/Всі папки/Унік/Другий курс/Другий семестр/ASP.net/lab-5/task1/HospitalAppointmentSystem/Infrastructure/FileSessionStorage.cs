using System.Text.Json;

namespace HospitalAppointmentSystem.Infrastructure
{
    public class FileSessionStorage
    {
        private readonly string _storagePath;

        public FileSessionStorage(IWebHostEnvironment environment)
        {
            _storagePath = Path.Combine(environment.ContentRootPath, "App_Data", "Sessions");
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public void Save<T>(string key, T value)
        {
            var filePath = Path.Combine(_storagePath, $"{key}.json");
            var json = JsonSerializer.Serialize(value);
            File.WriteAllText(filePath, json);
        }

        public T? Load<T>(string key)
        {
            var filePath = Path.Combine(_storagePath, $"{key}.json");
            if (!File.Exists(filePath))
                return default;

            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<T>(json);
        }

        public void Delete(string key)
        {
            var filePath = Path.Combine(_storagePath, $"{key}.json");
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}