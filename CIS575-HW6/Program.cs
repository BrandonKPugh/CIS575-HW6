using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CIS575_HW6
{
    class Program
    {
        private delegate long SortingMethod(int[] list);

        private static Stopwatch stopwatch = new Stopwatch();

        const int CHART_WIDTH = 24;

        static void Main(string[] args)
        {
            List<SortingMethod> algorithms = new List<SortingMethod>();
            algorithms.Add(InsertionSort);
            algorithms.Add(HeapSort);
            algorithms.Add(SlowSort);
            Benchmark(algorithms);
        }

        static void Benchmark(List<SortingMethod> algorithms)
        {
            DrawChart(algorithms);

            List<List<TimeSpan>> timeSpans = new List<List<TimeSpan>>();

            foreach(SortingMethod method in algorithms)
            {
                timeSpans.Add(new List<TimeSpan>());
            }

            while(true)
            {
                AddTime(NextTest(algorithms, timeSpans), timeSpans);
            }
        }

        private static Tuple<Tuple<TimeSpan, long>, int, int> NextTest(List<SortingMethod> algorithms, List<List<TimeSpan>> timeSpans)
        {
            int x = 0;
            int y = 0;
            TimeSpan shortest = new TimeSpan(0);
            for(int i = 0; i < timeSpans.Count; i++)
            {
                List<TimeSpan> span = timeSpans[i];
                if (span.Count == 0)
                {
                    x = i;
                    y = 0;
                    break;
                }
                else
                {
                    if (shortest.TotalMilliseconds == 0)
                    {
                        shortest = span[span.Count - 1];
                        x = i;
                        y = span.Count - 1;
                    }
                    else if (span[span.Count - 1] < shortest)
                    {
                        shortest = span[span.Count - 1];
                        x = i;
                        y = span.Count - 1;
                    }
                }
            }
            long temp = (long)(Math.Pow(10, y));
            Console.SetCursorPosition(13 + ((CHART_WIDTH + 1) * x), y + 1);
            Console.Write("  Running...");
            Console.SetCursorPosition(0, CHART_WIDTH);
            return new Tuple<Tuple<TimeSpan, long>, int, int>(MeasureTime(algorithms[x], (long)(Math.Pow(10, y))), x, y);
        }

        private static void AddTime(Tuple<Tuple<TimeSpan, long>, int, int> tuple, List<List<TimeSpan>> timeSpans)
        {
            TimeSpan timeSpan = tuple.Item1.Item1;
            long compares = tuple.Item1.Item2;
            int x = 13 + ((CHART_WIDTH + 1) * tuple.Item2);
            int y = tuple.Item3 + 1;

            timeSpans[tuple.Item2].Add(tuple.Item1.Item1);

            Console.SetCursorPosition(x, y);

            string output;
            if (timeSpan.TotalSeconds > 60)
            {
                output = timeSpan.TotalMinutes.ToString("0.000") + "m ";
            }
            else if (timeSpan.TotalMilliseconds > 1000)
            {
                output = timeSpan.TotalSeconds.ToString("0.000") + "s ";
            }
            else
            {
                output = timeSpan.TotalMilliseconds.ToString("0.000") + "ms";
            }
            string compareOutput = "";
            if (compares < 1000)
            {
                string s = compares.ToString("0.000");
                string spaces = "";
                for(int i = 0; i < 9 - s.Length; i++)
                {
                    spaces += ' ';
                }
                compareOutput += " c: " + spaces + s;
            }
            else if (compares < 1000000)
            {
                string s = ((double)compares / 1000).ToString("0.000");
                string spaces = "";
                for (int i = 0; i < 8 - s.Length; i++)
                {
                    spaces += ' ';
                }
                compareOutput += " c: " + spaces + s + "k";
            }
            else if (compares < 1000000000)
            {
                string s = ((double)compares / 1000000).ToString("0.000");
                string spaces = "";
                for (int i = 0; i < 8 - s.Length; i++)
                {
                    spaces += ' ';
                }
                compareOutput += " c: " + spaces + s + "m";
            }
            else if (compares < 1000000000000)
            {
                string s = ((double)compares / 1000000000).ToString("0.000");
                string spaces = "";
                for (int i = 0; i < 8 - s.Length; i++)
                {
                    spaces += ' ';
                }
                compareOutput += " c: " + spaces + s + "b";
            }
            for (int i = 0; i < 12 - compareOutput.Length; i++)
            {
                output += ' ';
            }
            output += compareOutput;
            for (int i = 0; i < CHART_WIDTH - output.Length; i++)
            {
                Console.Write(' ');
            }
            Console.Write(output);
            Console.SetCursorPosition(0, CHART_WIDTH);
        }

        private static void DrawChart(List<SortingMethod> algorithms)
        {
            Console.Write(" Array Size ");
            for (int i = 0; i < algorithms.Count; i++)
            {
                Console.Write("|");
                for (int j = 0; j < (CHART_WIDTH - algorithms[i].Method.Name.Length) / 2; j++)
                {
                    Console.Write(' ');
                }
                Console.Write(algorithms[i].Method.Name);
                for (int j = 0; j < (CHART_WIDTH - algorithms[i].Method.Name.Length + 1) / 2; j++)
                {
                    Console.Write(' ');
                }
            }
            Console.WriteLine("|");

            for(int i = 0; i < 12; i++)
            {
                long num = (long) Math.Pow(10, i);
                Console.Write(num);
                for(int j = 0; j < 12 - num.ToString().Length; j++)
                {
                    Console.Write(' ');
                }
                for (int j = 0; j < algorithms.Count; j++)
                {
                    Console.Write("|");
                    for(int k = 0; k < CHART_WIDTH; k++)
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine("|");
            }
        }

        static Tuple<TimeSpan, long> MeasureTime(SortingMethod sort, long count)
        {

            int[] list = new int[count];
            GenerateList(list);

            stopwatch.Reset();
            stopwatch.Start();
            long compares = sort(list);
            stopwatch.Stop();
            return new Tuple<TimeSpan, long>(stopwatch.Elapsed, compares);
        }

        static void GenerateList(int[] list)
        {
            for(int i = 0; i < list.Length; i++)
            {
                list[i] = (17 * i) % list.Length;
            }
        }

        static long HeapSort(int[] list)
        {
            long compares = 0;

            int count = list.Length;

            for(int i = (count/2) - 1; i > -1; i--)
            {
                compares += Sift(list, count, i);
            }

            for(int i = count - 1; i > -1; i--)
            {
                int temp = list[i];
                list[i] = list[0];
                list[0] = temp;

                compares += Sift(list, i, 0);
            }
            return compares;
        }

        private static long Sift(int[] list, int count, int index)
        {
            long compares = 0;
            int root = index;
            int left = 2 * index + 1;
            int right = 2 * index + 2;

            if(left < count)
            {
                compares++;
                if (list[left] > list[root])
                {
                    root = left;
                }
            }

            if(right < count)
            {
                compares++;
                if (list[right] > list[root])
                {
                    root = right;
                }
            }

            if(root != index)
            {
                int temp = list[index];
                list[index] = list[root];
                list[root] = temp;

                compares += Sift(list, count, root);
            }
            return compares;
        }

        static long InsertionSort(int[] list)
        {
            long compares = 0;
            for(int i = 1; i < list.Length; i++)
            {
                for(int j = i - 1; j >= 0; j--)
                {
                    compares++;
                    if(list[j] > list[j+1])
                    {
                        int temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }
            return compares;
        }

        static long SlowSort(int[] list)
        {
            return SlowSortRecursive(list, 0, list.Length - 1);
        }
        
        private static long SlowSortRecursive(int[] list, int start, int end)
        {
            long compares = 0;
            int count = end - start + 1;
            if(count == 2)
            {
                compares++;
                if(list[start] > list[end])
                {
                    int temp = list[start];
                    list[start] = list[end];
                    list[end] = temp;
                }
            }
            else if(count > 2)
            {
                int a = start + count / 3;
                int b = start + (2 * count / 3) - 1;
                if (b == start + (2 * (count - 1) / 3) - 1)
                    b++;
                compares += SlowSortRecursive(list, start, b);
                compares += SlowSortRecursive(list, a, end);
                compares += SlowSortRecursive(list, start, b);
            }
            return compares;
        }

        static void Output(int[] list)
        {
            int spacing = 1;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].ToString().Length > spacing)
                    spacing = list[i].ToString().Length;
            }
            spacing++;
            for (int i = 0; i < list.Length; i++)
            {
                string num = list[i].ToString();
                int len = num.Length;
                for (int j = 0; j < spacing - len; j++)
                {
                    Console.Write(' ');
                }
                Console.Write(num);
            }
            Console.WriteLine();
        }
    }
}
