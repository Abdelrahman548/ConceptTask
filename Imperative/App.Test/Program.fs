open App.Data

open System

[<EntryPoint>]
let main argv =
    
    let filePath = "MyTasks.json"
    let numOfDays = 1
    let taskManager = TaskManager()
    let myTasks = FileOperations.loadTasksFromFile(filePath)
    if not myTasks.IsEmpty then
        taskManager.SetTasks(myTasks)

    ////Check Task's Due Date and Mark it Overdue
    taskManager.OverdueTasks()
    
    let printTask (task: Task) = 
        if(task.status = Status.Overdue) then
            Console.ForegroundColor <- ConsoleColor.DarkMagenta
        elif(task.status = Status.Completed) then
            Console.ForegroundColor <- ConsoleColor.Green
        elif((task.dueDate - DateTime.Now).TotalDays < numOfDays) then
            Console.ForegroundColor <- ConsoleColor.Red
        else
            Console.ForegroundColor <- ConsoleColor.Yellow
        printfn "\t{ %s }" (task.Describe())
        Console.ForegroundColor <- ConsoleColor.White

    let displayMenu () =
        printfn "========================================================"
        printfn "Task Manager"
        printfn "1. Add Task"
        printfn "2. Delete Task"
        printfn "3. Complete Task"
        printfn "4. Update Task Priority"
        printfn "5. Show  Task"
        printfn "6. Show Current Tasks"
        printfn "7. Show Overdue Tasks"
        printfn "8. Save & Exit"
        printfn "========================================================"

    let mutable running = true

    
    while running do
        displayMenu()
        printf "Enter your choice: "
        match Console.ReadLine() with
        | "1" -> 
            printf "Enter description: "
            let description = Console.ReadLine()
            printf "Enter due date (YYYY-MM-DD): "
            let dueDate = DateTime.Parse(Console.ReadLine())
            printf "Enter priority (High=0, Normal=1, Low=2): "
            let priority = Enum.Parse<Priority>(Console.ReadLine())
            taskManager.AddTask(description, dueDate, priority)
            printfn "Task added successfully."
        | "2" ->
            printf "Enter Task ID to delete: "
            let id = int(Console.ReadLine())
            let isExist = taskManager.searchTask(id);
            if isExist then
                taskManager.DeleteTask(id)
                printfn "Task deleted successfully."
            else
                printfn "Invalid ID."
        | "3" ->
            printf "Enter Task ID to complete: "
            let id = int(Console.ReadLine())
            let isExist = taskManager.searchTask(id);
            if isExist then
                taskManager.CompleteTask(id)
                printfn "Task completed successfully."
            else
                printfn "Invalid ID."
        | "4" ->
            printf "Enter Task ID to Update Priority: "
            let id = int(Console.ReadLine())
            let isExist = taskManager.searchTask(id);
            if isExist then
                printf "Enter priority (High=0, Normal=1, Low=2): "
                let priority = Enum.Parse<Priority>(Console.ReadLine())
                taskManager.UpdateTaskPriority(id, priority)
                printfn "Task completed successfully."
            else
                printfn "Invalid ID."
        | "5" ->
            printf "Enter Task ID : "
            let id = int(Console.ReadLine())
            let isExist = taskManager.searchTask(id);
            if isExist then
                let task = taskManager.getTask(id)
                printTask(task)
            else
                printfn "Invalid ID."
        | "6" ->
            printfn "All Tasks:"
            taskManager.OverdueTasks()
            for task in taskManager.GetAllTasks() do
                if(task.status <> Status.Overdue) then
                    printTask(task)
        | "7" ->
            printfn "Overdue Tasks:"
            taskManager.OverdueTasks()
            for task in taskManager.GetAllTasks() do
                if(task.status = Status.Overdue) then
                    printTask(task)
        | "8" ->
            FileOperations.saveTasksToFile (taskManager.GetAllTasks()) (filePath)
            printfn "Exiting... Goodbye!"
            running <- false
        | _ ->
            printfn "Invalid choice, please try again."

    0