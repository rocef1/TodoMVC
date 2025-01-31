using Microsoft.AspNetCore.Mvc;
using TodoMVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace TodoMVC.Controllers
{
    public class ZavdaniaController : Controller
    {
        private static List<Zavdania> _zavdaniaList = new List<Zavdania>();
        private const string DefaultFilePath = "zavdania.json"; // Шлях до файлу за замовчуванням
        private string _currentFilePath = DefaultFilePath;

        private void LoadData(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    var jsonData = System.IO.File.ReadAllText(filePath);
                    _zavdaniaList = JsonSerializer.Deserialize<List<Zavdania>>(jsonData) ?? new List<Zavdania>();
                    Console.WriteLine($"Дані завантажено з файлу: {filePath}");
                }
                else
                {
                    _zavdaniaList = new List<Zavdania>();
                    SaveData(filePath);
                    Console.WriteLine($"Файл {filePath} не знайдено. Створено новий файл.");
                }
                _currentFilePath = filePath; // Оновлюємо шлях до файлу
            }
            catch (Exception ex)
            {
                _zavdaniaList = new List<Zavdania>();
                Console.WriteLine($"Помилка завантаження даних: {ex.Message}");
            }
        }

        private void SaveData(string filePath)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(_zavdaniaList, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(filePath, jsonData);
                Console.WriteLine($"Дані збережено у файл: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка збереження даних: {ex.Message}");
            }
        }

        public IActionResult Index()
        {
            return RedirectToAction("TaskList");
        }

        public IActionResult TaskList()
        {
            Console.WriteLine($"Виконується завантаження даних з файлу: {_currentFilePath}");
            LoadData(_currentFilePath);
            return View(_zavdaniaList);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(Zavdania zavdania)
        {
            LoadData(_currentFilePath); // Завантаження даних з поточного файлу
            zavdania.Id = _zavdaniaList.Any() ? _zavdaniaList.Max(z => z.Id) + 1 : 1;
            zavdania.IsCompleted = false;
            _zavdaniaList.Add(zavdania); // Додавання нового завдання до списку
            SaveData(_currentFilePath); // Збереження у поточний файл
            return RedirectToAction("TaskList");
        }

        public IActionResult Edit(int id)
        {
            LoadData(_currentFilePath);
            var zavdania = _zavdaniaList.FirstOrDefault(z => z.Id == id);
            return View(zavdania);
        }

        [HttpPost]
        public IActionResult Edit(Zavdania zavdania)
        {
            LoadData(_currentFilePath);
            var existing = _zavdaniaList.FirstOrDefault(z => z.Id == zavdania.Id);
            if (existing != null)
            {
                existing.Title = zavdania.Title;
                existing.Description = zavdania.Description;
                existing.Priority = zavdania.Priority;
                existing.IsCompleted = zavdania.IsCompleted;
            }
            SaveData(_currentFilePath);
            return RedirectToAction("TaskList");
        }

        public IActionResult Delete(int id)
        {
            LoadData(_currentFilePath);
            var zavdania = _zavdaniaList.FirstOrDefault(z => z.Id == id);
            if (zavdania != null)
            {
                _zavdaniaList.Remove(zavdania);
            }
            SaveData(_currentFilePath);
            return RedirectToAction("TaskList");
        }

        public IActionResult MarkAsCompleted(int id)
        {
            LoadData(_currentFilePath);
            var zavdania = _zavdaniaList.FirstOrDefault(z => z.Id == id);
            if (zavdania != null)
            {
                zavdania.IsCompleted = true;
            }
            SaveData(_currentFilePath);
            return RedirectToAction("TaskList");
        }

        public IActionResult NewList()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmNewList(string newFilePath)
        {
            if (!string.IsNullOrWhiteSpace(newFilePath))
            {
                _zavdaniaList = new List<Zavdania>();
                SaveData(newFilePath);
                _currentFilePath = newFilePath; // Оновлення шляху до нового файлу
            }
            return RedirectToAction("TaskList");
        }

        public IActionResult OpenList()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmOpenList(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (System.IO.File.Exists(filePath))
                {
                    LoadData(filePath);
                    _currentFilePath = filePath; // Оновлення шляху до нового файлу
                    Console.WriteLine($"Відкрито файл: {filePath}");
                }
                else
                {
                    TempData["ErrorMessage"] = "Файл не знайдено. Створюємо новий файл.";
                    _zavdaniaList = new List<Zavdania>();
                    SaveData(filePath);
                    _currentFilePath = filePath; // Оновлення шляху до нового файлу
                    Console.WriteLine($"Новий файл створено: {filePath}");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Шлях до файлу не може бути порожнім.";
            }
            return RedirectToAction("TaskList");
        }
    }
}
