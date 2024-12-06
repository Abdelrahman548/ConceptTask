open System
open App.Data
open ListExtensions


let displayMenu () =
    printfn "========================================================"
    printfn "Task Manager"
    printfn "1. Add Task"
    printfn "2. Delete Task"
    printfn "3. Complete Task"
    printfn "4. Update Task Priority"
    printfn "5. Show Task"
    printfn "6. Show Current Tasks"
    printfn "7. Show Overdue Tasks"
    printfn "8. Save & Exit"
    printfn "========================================================"


let numOfDays = 1

let printTask (task: Task) =
        if task.status = Status.Overdue then
            Console.ForegroundColor <- ConsoleColor.DarkMagenta
        elif task.status = Status.Completed then
            Console.ForegroundColor <- ConsoleColor.Green
        elif (task.dueDate - DateTime.Now).TotalDays < float numOfDays then
            Console.ForegroundColor <- ConsoleColor.Red
        else
            Console.ForegroundColor <- ConsoleColor.Yellow
        printfn "\t{ %s }" (task.Describe())
        Console.ForegroundColor <- ConsoleColor.White


let handleAddTask (taskManager: TaskManager) =
    printf "Enter description: "
    let description = Console.ReadLine()
    printf "Enter due date (YYYY-MM-DD): "
    let dueDate = DateTime.Parse(Console.ReadLine())
    printf "Enter priority (High=0, Normal=1, Low=2): "
    let priority = Enum.Parse<Priority>(Console.ReadLine())
    let updatedManager = taskManager.AddTask(description, dueDate, priority)
    printfn "Task added successfully."
    updatedManager

let handleDeleteTask (taskManager: TaskManager) =
    printf "Enter Task ID to delete: "
    let id = int(Console.ReadLine())
    if taskManager.SearchTaskExists(id) then
        let updatedManager = taskManager.DeleteTask(id)
        printfn "Task deleted successfully."
        updatedManager
    else
        printfn "Invalid ID."
        taskManager

let handleCompleteTask (taskManager: TaskManager) =
    printf "Enter Task ID to complete: "
    let id = int(Console.ReadLine())
    if taskManager.SearchTaskExists(id) then
        let updatedManager = taskManager.CompleteTask(id)
        printfn "Task completed successfully."
        updatedManager
    else
        printfn "Invalid ID."
        taskManager

let handleUpdatePriority (taskManager: TaskManager) =
    printf "Enter Task ID to Update Priority: "
    let id = int(Console.ReadLine())
    if taskManager.SearchTaskExists(id) then
        printf "Enter priority (High=0, Normal=1, Low=2): "
        let priority = Enum.Parse<Priority>(Console.ReadLine())
        let updatedManager = taskManager.UpdateTaskPriority(id, priority)
        printfn "Task priority updated successfully."
        updatedManager
    else
        printfn "Invalid ID."
        taskManager

let handleShowTask (taskManager: TaskManager) =
    printf "Enter Task ID : "
    let id = int(Console.ReadLine())
    if taskManager.SearchTaskExists(id) then
        match taskManager.SearchTask(id) with
        | [] -> printfn "Task not found"
        | H::T -> printfn "%s" (H.Describe())
    else
        printfn "Invalid ID."
    taskManager

let handleShowTasks (taskManager: TaskManager) =
    printfn "All Tasks:"
    let allTasks = taskManager.GetAllTasks()
    let filteredTasks = MyFilter (fun t -> t.status <> Status.Overdue) allTasks
    MyIter printTask filteredTasks
    taskManager

let handleShowOverdueTasks (taskManager: TaskManager) =
    printfn "Overdue Tasks:"
    let overdueTasks = MyFilter(fun t -> t.status = Status.Overdue) (taskManager.GetAllTasks())
    MyIter printTask overdueTasks
    taskManager

let handleSaveAndExit (taskManager: TaskManager) filePath =
    FileOperations.saveTasksToFile (taskManager.GetAllTasks()) filePath
    printfn "Exiting... Goodbye!"
    taskManager

let rec runApp taskManager =
    displayMenu()
    printf "Enter your choice: "
    match Console.ReadLine() with
    | "1" -> 
        let updatedManager = handleAddTask taskManager
        runApp updatedManager
    | "2" -> 
        let updatedManager = handleDeleteTask taskManager
        runApp updatedManager
    | "3" -> 
        let updatedManager = handleCompleteTask taskManager
        runApp updatedManager
    | "4" -> 
        let updatedManager = handleUpdatePriority taskManager
        runApp updatedManager
    | "5" -> 
        let _ = handleShowTask taskManager
        runApp taskManager
    | "6" -> 
        let taskManagerAfterOverdue = taskManager.OverdueTasks()
        let _ = handleShowTasks taskManagerAfterOverdue
        runApp taskManager
    | "7" -> 
        let _ = handleShowOverdueTasks taskManager
        runApp taskManager
    | "8" -> 
        handleSaveAndExit taskManager
    | _ -> 
        printfn "Invalid choice, please try again."
        runApp taskManager

[<EntryPoint>]
let main argv =
    let filePath = "MyTasks.json"
    let taskManager = TaskManager([], 0)
    let myTasks = FileOperations.loadTasksFromFile(filePath)
    let taskManagerWithTasks = if not myTasks.IsEmpty then taskManager.SetTasks(myTasks) else taskManager

    let taskManagerAfterOverdue = taskManagerWithTasks.OverdueTasks()

    let _ = runApp taskManagerAfterOverdue filePath
    0
