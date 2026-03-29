using System;
using System.Text;

namespace StringTasks
{
    class Program
    {
        // Задание 1: Конкатенация двух строк
        public static string ConcatenateStrings(string str1, string str2)
        {
            return str1 + str2;
        }

        // Задание 2: Приветствие пользователя с именем и возрастом
        public static string GreetUser(string name, int age)
        {
            return $"Hello, {name}!\nYou are {age} years old.";
        }

        // Задание 3: Анализ строки (длина, верхний/нижний регистр)
        public static string AnalyzeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "Строка пуста или null.";

            return $"Количество символов: {input.Length}\n" +
                  $"В верхнем регистре: {input.ToUpper()}\n" +
                  $"В нижнем регистре: {input.ToLower()}";
        }

        // Задание 4: Получение первых 5 символов строки
        public static string GetFirstFiveChars(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            if (input.Length <= 5)
                return input;

            return input.Substring(0, 5);
        }

        // Задание 5: Объединение массива строк в StringBuilder
        public static StringBuilder CombineStrings(string[] strings)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < strings.Length; i++)
            {
                sb.Append(strings[i]);
                if (i < strings.Length - 1)
                    sb.Append(" ");
            }

            return sb;
        }

        // Задание 6: Замена слов в строке
        public static string ReplaceWords(string inputString, string wordToReplace, string replacementWord)
        {
            if (string.IsNullOrEmpty(inputString) ||
                string.IsNullOrEmpty(wordToReplace))
                return inputString;

            return inputString.Replace(wordToReplace, replacementWord);
        }

        static void Main(string[] args)
        {
            // Примеры проверки методов

            // 1. ConcatenateStrings
            Console.WriteLine("1. ConcatenateStrings:");
            Console.WriteLine(ConcatenateStrings("Привет, ", "мир!"));
            Console.WriteLine();

            // 2. GreetUser
            Console.WriteLine("2. GreetUser:");
            Console.WriteLine(GreetUser("Алексей", 25));
            Console.WriteLine();

            // 3. AnalyzeString
            Console.WriteLine("3. AnalyzeString:");
            Console.WriteLine(AnalyzeString("Привет, Мир!"));
            Console.WriteLine();

            // 4. GetFirstFiveChars
            Console.WriteLine("4. GetFirstFiveChars:");
            Console.WriteLine(GetFirstFiveChars("Программирование"));
            Console.WriteLine(GetFirstFiveChars("Тест"));
            Console.WriteLine();

            // 5. CombineStrings
            Console.WriteLine("5. CombineStrings:");
            string[] words = { "Hello", "world", "from", "C#" };
            var resultSb = CombineStrings(words);
            Console.WriteLine(resultSb.ToString());
            Console.WriteLine();

            // 6. ReplaceWords
            Console.WriteLine("6. ReplaceWords:");
            string result = ReplaceWords("Hello world", "world", "universe");
            Console.WriteLine(result);
            // Проверка условия из примера
            Console.WriteLine($"Проверка: {(result == "Hello universe" ? "Успешно" : "Ошибка")}");
        }
    }
}
