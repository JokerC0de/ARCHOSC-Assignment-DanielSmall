using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("===== OS RESOURCE ALLOCATION: BANKER'S ALGORITHM =====\n");

        int[,] maximum =
        {
            { 7, 5, 4 },
            { 3, 2, 2 },
            { 9, 0, 2 },
            { 2, 2, 2 },
            { 4, 4, 4 }
        };

        int[,] allocation =
        {
            { 0, 1, 0 },
            { 2, 0, 0 },
            { 3, 0, 2 },
            { 2, 1, 1 },
            { 0, 0, 2 }
        };

        int[] availableTask1 = { 10, 5, 7 };
        int[] availableTask2 = { 12, 5, 7 };
        int[] availableTask3 = { 12, 7, 7 };

        RunBankersAlgorithm("TASK 1: Initial State", allocation, maximum, availableTask1);
        RunBankersAlgorithm("TASK 2: Increased R1 to 12", allocation, maximum, availableTask2);
        RunBankersAlgorithm("TASK 3: Increased R1 to 12 and R2 to 7", allocation, maximum, availableTask3);
    }

    static void RunBankersAlgorithm(string title, int[,] allocation, int[,] maximum, int[] available)
    {
        int processes = allocation.GetLength(0);
        int resources = allocation.GetLength(1);

        int[,] need = new int[processes, resources];

        Console.WriteLine("===== " + title + " =====\n");

        Console.WriteLine("Available Resources:");
        Console.WriteLine($"R1 = {available[0]}, R2 = {available[1]}, R3 = {available[2]}\n");

        Console.WriteLine("Need Matrix = Maximum - Allocation");

        for (int i = 0; i < processes; i++)
        {
            Console.Write($"P{i + 1}: ");

            for (int j = 0; j < resources; j++)
            {
                need[i, j] = maximum[i, j] - allocation[i, j];
                Console.Write(need[i, j] + " ");
            }

            Console.WriteLine();
        }

        bool[] finished = new bool[processes];
        List<string> safeSequence = new List<string>();
        int[] work = (int[])available.Clone();

        bool progressMade;

        do
        {
            progressMade = false;

            for (int i = 0; i < processes; i++)
            {
                if (!finished[i] && CanProcessRun(i, need, work))
                {
                    Console.WriteLine($"\nP{i + 1} can run because its need is less than or equal to available resources.");

                    for (int j = 0; j < resources; j++)
                    {
                        work[j] += allocation[i, j];
                    }

                    finished[i] = true;
                    safeSequence.Add("P" + (i + 1));
                    progressMade = true;

                    Console.WriteLine($"P{i + 1} has completed and released its allocated resources.");
                    Console.WriteLine($"New Available: R1 = {work[0]}, R2 = {work[1]}, R3 = {work[2]}");
                }
            }

        } while (progressMade);

        bool safe = true;

        for (int i = 0; i < processes; i++)
        {
            if (!finished[i])
            {
                safe = false;
                break;
            }
        }

        Console.WriteLine("\nResult:");

        if (safe)
        {
            Console.WriteLine("System is in a SAFE state.");
            Console.WriteLine("Safe Sequence: " + string.Join(" -> ", safeSequence));
        }
        else
        {
            Console.WriteLine("System is in an UNSAFE state.");
            Console.WriteLine("Not all processes could finish.");
        }

        Console.WriteLine("\n--------------------------------------------------\n");
    }

    static bool CanProcessRun(int process, int[,] need, int[] available)
    {
        int resources = available.Length;

        for (int j = 0; j < resources; j++)
        {
            if (need[process, j] > available[j])
            {
                return false;
            }
        }

        return true;
    }
}