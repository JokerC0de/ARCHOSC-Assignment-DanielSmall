using System;
using System.Collections.Generic;
using System.Linq;

class Process
{
    public string Name { get; set; }
    public int ArrivalTime { get; set; }
    public int ExecutionTime { get; set; }
    public int Priority { get; set; }

    public int RemainingTime { get; set; }
    public int StartTime { get; set; } = -1;
    public int FinishTime { get; set; }
    public int TurnaroundTime { get; set; }
    public double NormalisedTurnaroundTime { get; set; }
}

class GanttBlock
{
    public string ProcessName { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
}

class Program
{
    static void Main()
    {
        List<Process> processes = new List<Process>
        {
            new Process { Name = "A", ArrivalTime = 0, ExecutionTime = 3, Priority = 5 },
            new Process { Name = "B", ArrivalTime = 2, ExecutionTime = 6, Priority = 4 },
            new Process { Name = "C", ArrivalTime = 5, ExecutionTime = 5, Priority = 8 },
            new Process { Name = "D", ArrivalTime = 6, ExecutionTime = 3, Priority = 6 },
            new Process { Name = "E", ArrivalTime = 8, ExecutionTime = 6, Priority = 10 },
            new Process { Name = "F", ArrivalTime = 9, ExecutionTime = 2, Priority = 3 },
            new Process { Name = "G", ArrivalTime = 10, ExecutionTime = 6, Priority = 7 }
        };

        Console.WriteLine("===== LONG-TERM SCHEDULING =====");
        LongTermScheduling(processes);

        Console.WriteLine("\n===== FCFS SCHEDULING =====");
        FCFS(processes);

        Console.WriteLine("\n===== ROUND ROBIN: TIME QUANTUM 1 =====");
        RoundRobin(processes, 1);

        Console.WriteLine("\n===== ROUND ROBIN: TIME QUANTUM 3 =====");
        RoundRobin(processes, 3);

        Console.WriteLine("\n===== ROUND ROBIN: TIME QUANTUM 4 =====");
        RoundRobin(processes, 4);

        Console.WriteLine("\n===== ROUND ROBIN: TIME QUANTUM 6 =====");
        RoundRobin(processes, 6);

        Console.WriteLine("\n===== PRIORITY-BASED ROUND ROBIN: TIME QUANTUM 1 =====");
        PriorityRoundRobin(processes, 1);

        Console.WriteLine("\n===== PRIORITY-BASED ROUND ROBIN: TIME QUANTUM 6 =====");
        PriorityRoundRobin(processes, 6);
    }

    static List<Process> CloneProcesses(List<Process> original)
    {
        return original.Select(p => new Process
        {
            Name = p.Name,
            ArrivalTime = p.ArrivalTime,
            ExecutionTime = p.ExecutionTime,
            Priority = p.Priority,
            RemainingTime = p.ExecutionTime
        }).ToList();
    }

    static void LongTermScheduling(List<Process> original)
    {
        var admittedJobs = original.Where(p => p.Priority > 5).ToList();

        Console.WriteLine("Jobs admitted where Priority > 5:");

        foreach (var job in admittedJobs)
        {
            Console.WriteLine($"Job {job.Name}, Priority {job.Priority}, Arrival Time {job.ArrivalTime}, Execution Time {job.ExecutionTime}");
        }

        Console.WriteLine("\nExplanation:");
        Console.WriteLine("Long-term scheduling decides which jobs are admitted into the system.");
        Console.WriteLine("In this example, only jobs with priority greater than 5 are admitted.");
    }

    static void FCFS(List<Process> original)
    {
        var processes = CloneProcesses(original)
            .OrderBy(p => p.ArrivalTime)
            .ToList();

        List<GanttBlock> gantt = new List<GanttBlock>();
        int currentTime = 0;

        foreach (var process in processes)
        {
            if (currentTime < process.ArrivalTime)
            {
                currentTime = process.ArrivalTime;
            }

            process.StartTime = currentTime;
            process.FinishTime = currentTime + process.ExecutionTime;
            process.TurnaroundTime = process.FinishTime - process.ArrivalTime;
            process.NormalisedTurnaroundTime = (double)process.TurnaroundTime / process.ExecutionTime;

            gantt.Add(new GanttBlock
            {
                ProcessName = process.Name,
                Start = process.StartTime,
                End = process.FinishTime
            });

            currentTime = process.FinishTime;
        }

        PrintResults(processes);
        PrintGanttChart(gantt);
    }

    static void RoundRobin(List<Process> original, int timeQuantum)
    {
        var processes = CloneProcesses(original)
            .OrderBy(p => p.ArrivalTime)
            .ToList();

        Queue<Process> readyQueue = new Queue<Process>();
        List<GanttBlock> gantt = new List<GanttBlock>();

        int currentTime = 0;
        int completed = 0;
        int index = 0;

        while (completed < processes.Count)
        {
            while (index < processes.Count && processes[index].ArrivalTime <= currentTime)
            {
                readyQueue.Enqueue(processes[index]);
                index++;
            }

            if (readyQueue.Count == 0)
            {
                currentTime++;
                continue;
            }

            Process current = readyQueue.Dequeue();

            if (current.StartTime == -1)
            {
                current.StartTime = currentTime;
            }

            int runTime = Math.Min(timeQuantum, current.RemainingTime);

            gantt.Add(new GanttBlock
            {
                ProcessName = current.Name,
                Start = currentTime,
                End = currentTime + runTime
            });

            currentTime += runTime;
            current.RemainingTime -= runTime;

            while (index < processes.Count && processes[index].ArrivalTime <= currentTime)
            {
                readyQueue.Enqueue(processes[index]);
                index++;
            }

            if (current.RemainingTime > 0)
            {
                readyQueue.Enqueue(current);
            }
            else
            {
                current.FinishTime = currentTime;
                current.TurnaroundTime = current.FinishTime - current.ArrivalTime;
                current.NormalisedTurnaroundTime = (double)current.TurnaroundTime / current.ExecutionTime;
                completed++;
            }
        }

        PrintResults(processes);
        PrintGanttChart(gantt);
    }

    static void PriorityRoundRobin(List<Process> original, int timeQuantum)
    {
        var processes = CloneProcesses(original)
            .OrderBy(p => p.ArrivalTime)
            .ToList();

        List<Process> readyList = new List<Process>();
        List<GanttBlock> gantt = new List<GanttBlock>();

        int currentTime = 0;
        int completed = 0;
        int index = 0;

        while (completed < processes.Count)
        {
            while (index < processes.Count && processes[index].ArrivalTime <= currentTime)
            {
                readyList.Add(processes[index]);
                index++;
            }

            if (readyList.Count == 0)
            {
                currentTime++;
                continue;
            }

            Process current = readyList
                .OrderByDescending(p => p.Priority)
                .ThenBy(p => p.ArrivalTime)
                .First();

            readyList.Remove(current);

            if (current.StartTime == -1)
            {
                current.StartTime = currentTime;
            }

            int runTime = Math.Min(timeQuantum, current.RemainingTime);

            gantt.Add(new GanttBlock
            {
                ProcessName = current.Name,
                Start = currentTime,
                End = currentTime + runTime
            });

            currentTime += runTime;
            current.RemainingTime -= runTime;

            while (index < processes.Count && processes[index].ArrivalTime <= currentTime)
            {
                readyList.Add(processes[index]);
                index++;
            }

            if (current.RemainingTime > 0)
            {
                readyList.Add(current);
            }
            else
            {
                current.FinishTime = currentTime;
                current.TurnaroundTime = current.FinishTime - current.ArrivalTime;
                current.NormalisedTurnaroundTime = (double)current.TurnaroundTime / current.ExecutionTime;
                completed++;
            }
        }

        PrintResults(processes);
        PrintGanttChart(gantt);
    }

    static void PrintResults(List<Process> processes)
    {
        Console.WriteLine("\nProcess | Arrival | Exec | Priority | Finish | Turnaround | Normalised Turnaround");
        Console.WriteLine("--------|---------|------|----------|--------|------------|----------------------");

        foreach (var p in processes.OrderBy(p => p.Name))
        {
            Console.WriteLine($"{p.Name,-7} | {p.ArrivalTime,-7} | {p.ExecutionTime,-4} | {p.Priority,-8} | {p.FinishTime,-6} | {p.TurnaroundTime,-10} | {p.NormalisedTurnaroundTime:F2}");
        }
    }

    static void PrintGanttChart(List<GanttBlock> gantt)
    {
        Console.WriteLine("\nGantt Chart:");
        foreach (var block in gantt)
        {
            Console.Write($"| {block.ProcessName} ");
        }
        Console.WriteLine("|");

        foreach (var block in gantt)
        {
            Console.Write($"{block.Start}    ");
        }

        Console.WriteLine(gantt.Last().End);
    }
}