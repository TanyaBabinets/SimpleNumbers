using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
//Создайте приложение, использующее механизм мьютексов. 
//    Создайте в коде приложения несколько потоков.
//Первый поток генерирует набор случайных чисел и записывает их в файл. 
//    Второй поток ожидает, когда первый закончит своё исполнение, 
//    после чего анализирует содержимое файла и создаёт новый файл, в котором должны
//быть собраны только простые числа из первого файла.
//Третий поток ожидает, когда закончится второй поток, после чего создаёт новый файл, 
//в котором должны быть собраны все простые числа из второго файла у которых
//последняя цифра равна 7. 
namespace SimpleNumbers
{
    internal class Program
    {

        static void GeneratorOfNumbers()
        {
            Random rnd = new Random();
            try
            {
                FileStream file = new FileStream("NewNumbers.txt", FileMode.Create, FileAccess.Write);
              
               StreamWriter writer = new StreamWriter(file);
                int range = rnd.Next(1000);
                for (int i = 0; i < 1000; i++)
                {
                    int n = rnd.Next(range);
                    writer.WriteLine(n);
                }
                writer.Close();
                file.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка чтения файла: {e.Message}");
            }
        }

        static public bool IsSimple(int number)
        {
            for (int i = 2; i < number; i++)
            {
                if (number % i == 0)
                    return false;
            }
            return true;
        }
        static public void SimpleNumbers()
        {
            try
            {
                string file1 = "NewNumbers.txt";
                string file2 = "OnlySimple.txt";               
                string[] lines = System.IO.File.ReadAllLines(file1); // Читаем содержимое исходного файла               
                using (StreamWriter writer = new StreamWriter(file2))
                {
                    foreach (string l in lines)
                    {
                        if (IsSimple(Int32.Parse(l)))
                        {
                            writer.WriteLine(l);
                        }
                    }
                               }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка чтения файла: {e.Message}");
            }
        }

        public static void Seven()
        {

            string file2 = "OnlySimple.txt";
            string file3 = "Only7.txt";
            
               string[] lines = System.IO.File.ReadAllLines(file2);   // Читаем содержимое исходного файла       
                string sorting = @"\b\d*7\b"; //поиск чисел с последней цифрой 7
            
            using (StreamWriter writer = new StreamWriter(file3))
            {
                foreach (string line in lines)
                {                  
                    MatchCollection sevens = Regex.Matches(line, sorting);// Находим все числа с последней цифрой 7                    
                    foreach (Match match in sevens)// Записываем найденные числа в новый файл
                    {
                        writer.WriteLine(match.Value);
                    }
                }
            }

            Console.WriteLine("числа с последней цифрой 7 записаны в файл " + file3);
        }
        /// <summary>
        /// ////////////////////////////////////////////////////// 4 zadanie
        /// </summary>
        public static void Report4()
        {
            string[] filePaths = { "NewNumbers.txt", "OnlySimple.txt", "Only7.txt" };
            
            List<string> reportLines = new List<string>();// Создаем список для хранения результатов

            foreach (string f in filePaths)
            {
                try
                {                  
                    string[] lines = System.IO.File.ReadAllLines(f); // cчитываем содержимое файла                    
                
                    long fileSize = new FileInfo(f).Length;

                    /                  
                    reportLines.Add($"Файл: {f}");
                    reportLines.Add($"Количество чисел: {lines.Count()}");
                    reportLines.Add($"Размер файла (байты): {fileSize}");
                    reportLines.Add("Содержимое файла:");
                    reportLines.AddRange(lines);
                    reportLines.Add(new string('-',10));
                }
                catch (Exception ex)
                {
                    reportLines.Add($"Ошибка {f}: {ex.Message}");
                }
            }

            // Записываем отчет в итоговый файл
            System.IO.File.WriteAllLines("report.txt", reportLines);

            Console.WriteLine("Отчет в файле 'report.txt'.");
        }
           

static void ThreadFunction()
        {
            try
            {
                bool CreatedNew;
               
                Mutex mutex = new Mutex(false, "36F9FCCA-343D-4D04-9883-9A75A21DA5F0", out CreatedNew);
                mutex.WaitOne();
                GeneratorOfNumbers();
             
               SimpleNumbers();
            
               Seven();
            
               Report4();
                mutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка {e.Message}");
            }
        }
        static void Main(string[] args)
        {
            Task.Factory.StartNew(ThreadFunction);
            Thread.Sleep(5000);
            
        }
    }
}
