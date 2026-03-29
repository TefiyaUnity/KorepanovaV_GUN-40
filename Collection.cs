namespace HomeWork
{
    internal class Program
    {
        private class ListTask
        {
            private readonly List<string> _listOfStrings = new List<string>();

            public void TaskLoop()
            {
                Console.WriteLine("Задание 1: Работа со списком строк");
                Console.WriteLine("Для выхода введите 'exit'");

                // Добавляем начальные элементы
                _listOfStrings.Add("Первый элемент");
                _listOfStrings.Add("Второй элемент");
                _listOfStrings.Add("Третий элемент");

                while (true)
                {
                    Console.WriteLine("\nТекущее содержимое списка:");
                    for (int i = 0; i < _listOfStrings.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {_listOfStrings[i]}");
                    }

                    Console.Write("\nВведите новую строку для добавления в конец списка (или 'exit' для выхода): ");
                    string input = Console.ReadLine();

                    if (input == "exit")
                    {
                        Console.WriteLine("Завершение задания 1.");
                        break;
                    }

                    _listOfStrings.Add(input);

                    Console.Write("Введите строку для добавления в середину списка: ");
                    input = Console.ReadLine();

                    if (input == "exit")
                    {
                        Console.WriteLine("Завершение задания 1.");
                        break;
                    }

                    int middleIndex = _listOfStrings.Count / 2;
                    _listOfStrings.Insert(middleIndex, input);
                }
            }
        }

        private class DictionaryTask
        {
            private readonly Dictionary<string, int> _studentGrades = new Dictionary<string, int>();

            public void TaskLoop()
            {
                Console.WriteLine("Задание 2: Словарь оценок студентов");
                Console.WriteLine("Для выхода введите 'exit'");

                while (true)
                {
                    Console.Write("\nВведите имя студента (или 'exit' для выхода): ");
                    string name = Console.ReadLine();

                    if (name == "--exit")
                    {
                        Console.WriteLine("Завершение задания 2.");
                        break;
                    }

                    Console.Write("Введите оценку студента (2-5): ");
                    string gradeInput = Console.ReadLine();

                    if (gradeInput == "--exit")
                    {
                        Console.WriteLine("Завершение задания 2.");
                        break;
                    }

                    if (!int.TryParse(gradeInput, out int grade) || grade < 2 || grade > 5)
                    {
                        Console.WriteLine("Ошибка: оценка должна быть целым числом от 2 до 5.");
                        continue;
                    }

                    _studentGrades[name] = grade;
                    Console.WriteLine($"Студент {name} с оценкой {grade} добавлен.");

                    Console.Write("\nВведите имя студента для просмотра оценки: ");
                    name = Console.ReadLine();

                    if (name == "--exit")
                    {
                        Console.WriteLine("Завершение задания 2.");
                        break;
                    }

                    if (_studentGrades.ContainsKey(name))
                    {
                        Console.WriteLine($"Оценка студента {name}: {_studentGrades[name]}");
                    }
                    else
                    {
                        Console.WriteLine($"Студент с именем {name} не найден.");
                    }
                }
            }
        }

        private class DoublyLinkedListTask
        {
            private class Node
            {
                public string Data { get; set; }
                public Node Next { get; set; }
                public Node Prev { get; set; }

                public Node(string data)
                {
                    Data = data;
                }
            }

            private Node _head;
            private Node _tail;
            private int _count;

            public void TaskLoop()
            {
                Console.WriteLine("Задание 3: Двусвязный список");
                Console.WriteLine("Для выхода введите 'exit'");

                Console.WriteLine("\nВведите от 3 до 6 элементов для списка:");

                while (_count < 6)
                {
                    Console.Write($"Элемент {_count + 1} (или ''exit' для завершения ввода): ");
                    string input = Console.ReadLine();

                    if (input == "exit")
                    {
                        if (_count >= 3)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Нужно ввести минимум 3 элемента. Продолжайте.");
                            continue;
                        }
                    }

                    Add(input);

                    if (_count == 6)
                    {
                        Console.WriteLine("Максимальное количество элементов (6) достигнуто.");
                        break;
                    }

                    if (_count >= 3)
                    {
                        Console.Write("Продолжить ввод? (y/n или 'exit'): ");
                        string choice = Console.ReadLine();
                        if (choice.ToLower() == "n" || choice == "exit")
                        {
                            break;
                        }
                    }
                }

                Console.WriteLine("\nСписок в прямом порядке:");
                PrintForward();

                Console.WriteLine("\nСписок в обратном порядке:");
                PrintBackward();

                Console.WriteLine("Завершение задания 3.");
            }

            private void Add(string data)
            {
                Node newNode = new Node(data);

                if (_head == null)
                {
                    _head = newNode;
                    _tail = newNode;
                }
                else
                {
                    newNode.Prev = _tail;
                    _tail.Next = newNode;
                    _tail = newNode;
                }

                _count++;
            }

            private void PrintForward()
            {
                Node current = _head;
                int index = 1;
                while (current != null)
                {
                    Console.WriteLine($"{index}. {current.Data}");
                    current = current.Next;
                    index++;
                }
            }

            private void PrintBackward()
            {
                Node current = _tail;
                int index = _count;
                while (current != null)
                {
                    Console.WriteLine($"{index}. {current.Data}");
                    current = current.Prev;
                    index--;
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Выберите задание:");
            Console.WriteLine("1 - Работа со списком строк");
            Console.WriteLine("2 - Словарь оценок студентов");
            Console.WriteLine("3 - Двусвязный список");
            Console.Write("Введите номер задания (1-3) или 'exit' для выхода: ");

            string input = Console.ReadLine();

            if (input.ToLower() == "exit")
            {
                return;
            }

            if (int.TryParse(input, out int task) && task >= 1 && task <= 3)
            {
                switch (task)
                {
                    case 1:
                        CheckTaskFirst();
                        break;
                    case 2:
                        CheckTaskSecond();
                        break;
                    case 3:
                        CheckTaskThird();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод. Пожалуйста, введите число от 1 до 3.");
            }
        }

        private static void CheckTaskFirst()
        {
            var listTask = new ListTask();
            listTask.TaskLoop();
        }

        private static void CheckTaskSecond()
        {
            var dictionaryTask = new DictionaryTask();
            dictionaryTask.TaskLoop();
        }

        private static void CheckTaskThird()
        {
            var doublyLinkedListTask = new DoublyLinkedListTask();
            doublyLinkedListTask.TaskLoop();
        }
    }
}