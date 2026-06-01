import matplotlib.pyplot as plt


def create_gantt_chart(title, schedule, filename):
    """
    Creates a clearer Gantt chart with each process on its own row.

    schedule format:
    [
        ("A", 0, 3),
        ("B", 3, 9)
    ]
    """

    processes = sorted(set(process for process, _, _ in schedule))
    process_positions = {process: i for i, process in enumerate(processes)}

    fig, ax = plt.subplots(figsize=(12, 5))

    for process, start, end in schedule:
        duration = end - start
        y_position = process_positions[process]

        ax.barh(
            y_position,
            duration,
            left=start,
            height=0.6,
            edgecolor="black"
        )

        ax.text(
            start + duration / 2,
            y_position,
            f"{process}\n{start}-{end}",
            ha="center",
            va="center",
            fontsize=9,
            fontweight="bold"
        )

    ax.set_yticks(list(process_positions.values()))
    ax.set_yticklabels(list(process_positions.keys()))
    ax.set_xlabel("Time")
    ax.set_ylabel("Process")
    ax.set_title(title, fontsize=14, fontweight="bold")

    max_time = max(end for _, _, end in schedule)
    ax.set_xlim(0, max_time + 1)
    ax.set_xticks(range(0, max_time + 1, 1))

    ax.grid(True, axis="x", linestyle="--", alpha=0.5)

    plt.tight_layout()
    plt.savefig(filename, dpi=250)
    plt.close()


fcfs_schedule = [
    ("A", 0, 3),
    ("B", 3, 9),
    ("C", 9, 14),
    ("D", 14, 17),
    ("E", 17, 23),
    ("F", 23, 25),
    ("G", 25, 31),
]

rr_tq1_schedule = [
    ("A", 0, 1),
    ("A", 1, 2),
    ("B", 2, 3),
    ("A", 3, 4),
    ("B", 4, 5),
    ("C", 5, 6),
    ("B", 6, 7),
    ("D", 7, 8),
    ("C", 8, 9),
    ("B", 9, 10),
    ("E", 10, 11),
    ("D", 11, 12),
    ("F", 12, 13),
    ("C", 13, 14),
    ("G", 14, 15),
    ("B", 15, 16),
    ("E", 16, 17),
    ("D", 17, 18),
    ("F", 18, 19),
    ("C", 19, 20),
    ("G", 20, 21),
    ("B", 21, 22),
    ("E", 22, 23),
    ("C", 23, 24),
    ("G", 24, 25),
    ("E", 25, 26),
    ("G", 26, 27),
    ("E", 27, 28),
    ("G", 28, 29),
    ("E", 29, 30),
    ("G", 30, 31),
]

rr_tq3_schedule = [
    ("A", 0, 3),
    ("B", 3, 6),
    ("C", 6, 9),
    ("D", 9, 12),
    ("B", 12, 15),
    ("E", 15, 18),
    ("F", 18, 20),
    ("C", 20, 22),
    ("G", 22, 25),
    ("E", 25, 28),
    ("G", 28, 31),
]

rr_tq4_schedule = [
    ("A", 0, 3),
    ("B", 3, 7),
    ("C", 7, 11),
    ("D", 11, 14),
    ("B", 14, 16),
    ("E", 16, 20),
    ("F", 20, 22),
    ("G", 22, 26),
    ("C", 26, 27),
    ("E", 27, 29),
    ("G", 29, 31),
]

rr_tq6_schedule = [
    ("A", 0, 3),
    ("B", 3, 9),
    ("C", 9, 14),
    ("D", 14, 17),
    ("E", 17, 23),
    ("F", 23, 25),
    ("G", 25, 31),
]

priority_rr_tq1_schedule = [
    ("A", 0, 1),
    ("A", 1, 2),
    ("A", 2, 3),
    ("B", 3, 4),
    ("B", 4, 5),
    ("C", 5, 6),
    ("C", 6, 7),
    ("C", 7, 8),
    ("E", 8, 9),
    ("E", 9, 10),
    ("E", 10, 11),
    ("E", 11, 12),
    ("C", 12, 13),
    ("C", 13, 14),
    ("G", 14, 15),
    ("G", 15, 16),
    ("G", 16, 17),
    ("D", 17, 18),
    ("D", 18, 19),
    ("D", 19, 20),
    ("B", 20, 21),
    ("B", 21, 22),
    ("B", 22, 23),
    ("B", 23, 24),
    ("B", 24, 25),
    ("B", 25, 26),
    ("F", 26, 27),
    ("F", 27, 28),
    ("E", 28, 29),
    ("E", 29, 30),
    ("G", 30, 31),
]

priority_rr_tq6_schedule = [
    ("A", 0, 3),
    ("B", 3, 9),
    ("E", 9, 15),
    ("C", 15, 20),
    ("G", 20, 26),
    ("D", 26, 29),
    ("F", 29, 31),
]


create_gantt_chart(
    "FCFS Scheduling Gantt Chart",
    fcfs_schedule,
    "../Pictures/fcfs-gantt.png"
)

create_gantt_chart(
    "Round Robin Gantt Chart - Time Quantum 1",
    rr_tq1_schedule,
    "../Pictures/rr-tq1-gantt.png"
)

create_gantt_chart(
    "Round Robin Gantt Chart - Time Quantum 3",
    rr_tq3_schedule,
    "../Pictures/rr-tq3-gantt.png"
)

create_gantt_chart(
    "Round Robin Gantt Chart - Time Quantum 4",
    rr_tq4_schedule,
    "../Pictures/rr-tq4-gantt.png"
)

create_gantt_chart(
    "Round Robin Gantt Chart - Time Quantum 6",
    rr_tq6_schedule,
    "../Pictures/rr-tq6-gantt.png"
)

create_gantt_chart(
    "Priority-Based Round Robin Gantt Chart - Time Quantum 1",
    priority_rr_tq1_schedule,
    "../Pictures/priority-rr-tq1-gantt.png"
)

create_gantt_chart(
    "Priority-Based Round Robin Gantt Chart - Time Quantum 6",
    priority_rr_tq6_schedule,
    "../Pictures/priority-rr-tq6-gantt.png"
)

print("Clearer Gantt charts created successfully.")