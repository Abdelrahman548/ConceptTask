open System
open App.Data

[<EntryPoint>]
let main argv =
    // Initializing New Tasks
    let task1 = Task(1, "Finish F# project", DateTime(2024, 12, 10), Priority.High, Status.Pending)
    let task2 = Task(2, "Prepare for exam", DateTime(2024, 12, 5), Priority.Normal, Status.Pending)
    let task3 = Task(3, "Buy groceries", DateTime(2024, 12, 3), Priority.Low, Status.Pending)

    // Initializing a Task Manager
    let manager = TaskManager([task1; task2; task3], 3)

    // Getting All Tasks
    printfn "Initial Tasks:"
    manager.GetAllTasks() |> List.iter (fun t -> t.Describe())

    // Adding New Task
    let manager = manager.AddTask("Read a book", DateTime(2024, 12, 15), Priority.Low)
    printfn "\nAfter Adding a Task:"
    manager.GetAllTasks() |> List.iter (fun t -> t.Describe())

    // Completing a Task
    let manager = manager.CompleteTask(2)
    printfn "\nAfter Completing Task 2:"
    manager.GetAllTasks() |> List.iter (fun t -> t.Describe())

    // Updating Priority for a Task
    let manager = manager.UpdateTaskPriority(1, Priority.Low)
    printfn "\nAfter Updating Priority of Task 1:"
    manager.GetAllTasks() |> List.iter (fun t -> t.Describe())

    // Filter Based on Completed Tasks
    let completedTasks = manager.FilterTasks(fun t -> t.status = Status.Completed)
    printfn "\nCompleted Tasks:"
    completedTasks |> List.iter (fun t -> t.Describe())

    // Sort Based on High Prioirity
    let sortedTasks = manager.SortTasks(fun t -> t.priority)
    printfn "\nTasks Sorted by Priority:"
    sortedTasks |> List.iter (fun t -> t.Describe())

    // Checking Overdue Tasks
    let manager = manager.OverdueTasks()
    printfn "\nAfter Checking Overdue Tasks:"
    manager.GetAllTasks() |> List.iter (fun t -> t.Describe())

    0 // Return code
