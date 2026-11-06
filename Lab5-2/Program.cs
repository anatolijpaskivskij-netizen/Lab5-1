using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PoetDatabaseAbstractApp
{
    // Власне виключення для обробки помилок вводу даних
    public class InvalidPoetDataException : Exception
    {
        public InvalidPoetDataException(string message) : base(message) { }
    }

    // Абстрактний базовий клас (з урахуванням вимог лабораторної №5)
    public abstract class Poet
    {
        private string lastName;
        private string language;
        private int collectionsCount;

        public string LastName
        {
            get => lastName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidPoetDataException("Прізвище не може бути порожнім.");
                lastName = value.Trim();
            }
        }

        public string Language
        {
            get => language;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidPoetDataException("Мова не може бути порожньою.");
                language = value.Trim();
            }
        }

        public int CollectionsCount
        {
            get => collectionsCount;
            set
            {
                if (value < 0)
                    throw new InvalidPoetDataException("Кількість збірок не може бути від'ємною.");
                collectionsCount = value;
            }
        }

        protected Poet()
        {
            lastName = "Невідомо";
            language = "Українська";
            collectionsCount = 0;
        }

        protected Poet(string lastName, string language, int collectionsCount)
        {
            LastName = lastName;
            Language = language;
            CollectionsCount = collectionsCount;
        }

        // Абстрактні методи для реалізації в похідному класі (індивідуальне завдання)
        public abstract int GetLastNameLength();
        public abstract int GetTotalListeners();
        public abstract string GetDayWithMaxListeners();

        public override string ToString()
        {
            return $"{LastName};{Language};{CollectionsCount}";
        }
    }

    // Похідний клас (Сутність "Виступ")
    public class PoetPerformance : Poet
    {
        private DateTime performanceDate;
        private string venue;
        private int listenersCount;

        public DateTime PerformanceDate
        {
            get => performanceDate;
            set => performanceDate = value;
        }

        public string Venue
        {
            get => venue;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new InvalidPoetDataException("Місце виступу не може бути порожнім.");
                venue = value.Trim();
            }
        }

        public int ListenersCount
        {
            get => listenersCount;
            set
            {
                if (value < 0)
                    throw new InvalidPoetDataException("Кількість слухачів не може бути від'ємною.");
                listenersCount = value;
            }
        }

        public PoetPerformance() : base()
        {
            performanceDate = DateTime.Now;
            venue = "Невідомо";
            listenersCount = 0;
        }

        public PoetPerformance(string lastName, string language, int collectionsCount, DateTime performanceDate, string venue, int listenersCount)
            : base(lastName, language, collectionsCount)
        {
            PerformanceDate = performanceDate;
            Venue = venue;
            ListenersCount = listenersCount;
        }

        // Реалізація абстрактних методів базового класу
        public override int GetLastNameLength()
        {
            return LastName.Length;
        }

        public override int GetTotalListeners()
        {
            return ListenersCount;
        }

        public override string GetDayWithMaxListeners()
        {
            return PerformanceDate.ToString("yyyy-MM-dd");
        }

        public override string ToString()
        {
            return $"{base.ToString()};{PerformanceDate:yyyy-MM-dd};{Venue};{ListenersCount}";
        }

        public static PoetPerformance FromString(string line)
        {
            string[] parts = line.Split(';');
            if (parts.Length != 6) throw new FormatException("Невірний формат рядка у файлі.");

            return new PoetPerformance(
                parts[0],
                parts[1],
                int.Parse(parts[2]),
                DateTime.Parse(parts[3]),
                parts[4],
                int.Parse(parts[5])
            );
        }
    }

    // Менеджер бази даних для роботи з файлом та списком об'єктів
    public class DatabaseManager
    {
        private List<PoetPerformance> records;
        private string dbFilePath;

        public DatabaseManager(string dbFilePath)
        {
            this.dbFilePath = dbFilePath;
            records = new List<PoetPerformance>();
            LoadFromFile();
            if (records.Count == 0)
            {
                CreateDefaultRecords();
                SaveToFile();
            }
        }

        private void CreateDefaultRecords()
        {
            records.Add(new PoetPerformance("Шевченко", "Українська", 12, new DateTime(2025, 3, 9), "Національна філармонія", 450));
            records.Add(new PoetPerformance("Костенко", "Українська", 15, new DateTime(2025, 4, 12), "Театр імені Франка", 600));
            records.Add(new PoetPerformance("Франко", "Українська", 25, new DateTime(2025, 5, 28), "Оперний театр", 550));
            records.Add(new PoetPerformance("Стус", "Українська", 8, new DateTime(2025, 1, 15), "Культурний центр", 300));
            records.Add(new PoetPerformance("Симоненко", "Українська", 6, new DateTime(2025, 6, 1), "Будинок вчених", 400));
        }

        public void LoadFromFile()
        {
            if (!File.Exists(dbFilePath)) return;
            records.Clear();
            string[] lines = File.ReadAllLines(dbFilePath, Encoding.UTF8);
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        records.Add(PoetPerformance.FromString(line));
                    }
                    catch { }
                }
            }
        }

        public void SaveToFile()
        {
            List<string> lines = new List<string>();
            foreach (var record in records)
            {
                lines.Add(record.ToString());
            }
            File.WriteAllLines(dbFilePath, lines, Encoding.UTF8);
        }

        public void AddRecord(PoetPerformance record)
        {
            records.Add(record);
            SaveToFile();
        }

        public void EditRecord(int index, PoetPerformance record)
        {
            if (index < 0 || index >= records.Count)
                throw new ArgumentOutOfRangeException("Запису з таким індексом не існує.");
            records[index] = record;
            SaveToFile();
        }

        public void DeleteRecord(int index)
        {
            if (index < 0 || index >= records.Count)
                throw new ArgumentOutOfRangeException("Запису з таким індексом не існує.");
            records.RemoveAt(index);
            SaveToFile();
        }

        public List<PoetPerformance> GetAllRecords()
        {
            return records;
        }

        // Обчислення індивідуального завдання Варіанта 8 за допомогою поліморфних методів
        public void PrintIndividualTaskResults()
        {
            if (records.Count == 0)
            {
                Console.WriteLine("База даних порожня.");
                return;
            }

            int totalListeners = records.Sum(r => r.GetTotalListeners());
            var maxPerformance = records.OrderByDescending(r => r.ListenersCount).FirstOrDefault();

            Console.WriteLine("\n=== РЕЗУЛЬТАТИ ОБЧИСЛЕНЬ (ІНДИВІДУАЛЬНЕ ЗАВДАННЯ ВАРІАНТ 8) ===");
            Console.WriteLine($"1. Сумарна кількість слухачів на всіх виступах: {totalListeners}");

            if (maxPerformance != null)
            {
                Console.WriteLine($"2. День з найбільшою кількістю слухачів: {maxPerformance.GetDayWithMaxListeners()} ({maxPerformance.ListenersCount} слухачів, поет: {maxPerformance.LastName})");
            }

            Console.WriteLine("\n3. Довжина прізвища кожного поета:");
            foreach (var record in records)
            {
                Console.WriteLine($"   - {record.LastName}: {record.GetLastNameLength()} літер");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            string dbFile = "poets_abstract_db.txt";
            DatabaseManager db = new DatabaseManager(dbFile);

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== БАЗА ДАНИХ: ПОЕТИ ТА ВИСТУПИ (АБСТРАКТНИЙ КЛАС) ===");
                Console.WriteLine("[о] - Вивести всі записи на екран з файла");
                Console.WriteLine("[д] - Додати новий запис");
                Console.WriteLine("[р] - Редагувати запис");
                Console.WriteLine("[в] - Видалити запис");
                Console.WriteLine("[а] - Обчислити результати індивідуального завдання");
                Console.WriteLine("[Enter] - Вихід");
                Console.Write("\nОберіть дію (натисніть відповідну клавішу): ");

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                Console.WriteLine();

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    break;
                }

                char key = char.ToLower(keyInfo.KeyChar);

                try
                {
                    switch (key)
                    {
                        case 'о':
                            PrintRecords(db.GetAllRecords());
                            break;

                        case 'д':
                            PoetPerformance newRec = InputPoetPerformance();
                            db.AddRecord(newRec);
                            Console.WriteLine("\nЗапис успішно додано!");
                            break;

                        case 'р':
                            PrintRecords(db.GetAllRecords());
                            Console.Write("\nВведіть номер запису для редагування (індекс): ");
                            if (int.TryParse(Console.ReadLine(), out int editIdx))
                            {
                                PoetPerformance editedRec = InputPoetPerformance();
                                db.EditRecord(editIdx, editedRec);
                                Console.WriteLine("\nЗапис успішно оновлено!");
                            }
                            else
                            {
                                Console.WriteLine("Некоректний індекс.");
                            }
                            break;

                        case 'в':
                            PrintRecords(db.GetAllRecords());
                            Console.Write("\nВведіть номер запису для видалення (індекс): ");
                            if (int.TryParse(Console.ReadLine(), out int delIdx))
                            {
                                db.DeleteRecord(delIdx);
                                Console.WriteLine("\nЗапис успішно видалено!");
                            }
                            else
                            {
                                Console.WriteLine("Некоректний індекс.");
                            }
                            break;

                        case 'а':
                            db.PrintIndividualTaskResults();
                            break;

                        default:
                            Console.WriteLine("Невідома команда. Спробуйте ще раз.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nПомилка: {ex.Message}");
                }

                Console.WriteLine("\nНатисніть будь-яку клавішу, щоб продовжити...");
                Console.ReadKey();
            }
        }

        static void PrintRecords(List<PoetPerformance> list)
        {
            if (list.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            Console.WriteLine(new string('-', 100));
            Console.WriteLine($"| {"№",-3} | {"Прізвище",-15} | {"Мова",-12} | {"Збірок",-7} | {"Дата виступу",-12} | {"Місце виступу",-22} | {"Слухачів",-8} |");
            Console.WriteLine(new string('-', 100));

            for (int i = 0; i < list.Count; i++)
            {
                var r = list[i];
                Console.WriteLine($"| {i,-3} | {r.LastName,-15} | {r.Language,-12} | {r.CollectionsCount,-7} | {r.PerformanceDate,-12:yyyy-MM-dd} | {r.Venue,-22} | {r.ListenersCount,-8} |");
            }
            Console.WriteLine(new string('-', 100));
        }

        static PoetPerformance InputPoetPerformance()
        {
            Console.Write("Введіть прізвище поета: ");
            string lastName = Console.ReadLine();

            Console.Write("Введіть мову: ");
            string lang = Console.ReadLine();

            Console.Write("Введіть кількість збірок: ");
            if (!int.TryParse(Console.ReadLine(), out int collections))
            {
                throw new InvalidPoetDataException("Кількість збірок має бути цілим числом.");
            }

            Console.Write("Введіть дату виступу (РРРР-ММ-ДД): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
            {
                throw new InvalidPoetDataException("Невірний формат дати.");
            }

            Console.Write("Введіть місце виступу: ");
            string venue = Console.ReadLine();

            Console.Write("Введіть кількість слухачів: ");
            if (!int.TryParse(Console.ReadLine(), out int listeners))
            {
                throw new InvalidPoetDataException("Кількість слухачів має бути цілим числом.");
            }

            return new PoetPerformance(lastName, lang, collections, date, venue, listeners);
        }
    }
}