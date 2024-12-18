open System
open App.Data
open ListExtensions
open TaskCRUD
open TaskManagerOperations

let displayMenu () =
    printfn "========================================================"
    printfn "Task Manager"
    printfn "1. Add Task"
    printfn "2. Delete Task"
    printfn "3. Complete Task"
    printfn "4. Update Task Priority"
    printfn "5. Show Task with ID"
    printfn "6. Show Pending Tasks"
    printfn "7. Show Completed Tasks"
    printfn "8. Show Overdue Tasks"
    printfn "9. Show Tasks Sorted Ascending by [DueDate]"
    printfn "10. Show Tasks Sorted Descending by [DueDate]"
    printfn "11. Show Tasks Sorted Ascending by [Priority]"
    printfn "12. Show Tasks Sorted Descending by [Priority]"
    printfn "13. Show Notifications"
    printfn "14. Save & Exit"
    printfn "========================================================"


let numOfDays = 1
let dateFormat = "yyyy-MM-dd HH:mm"

let printTask (task: Task) =
        if task.status = Status.Overdue then
            Console.ForegroundColor <- ConsoleColor.DarkMagenta
        elif task.status = Status.Completed then
            Console.ForegroundColor <- ConsoleColor.Green
        elif (task.dueDate - DateTime.Now).TotalDays < float numOfDays then
            Console.ForegroundColor <- ConsoleColor.Red
        else
            Console.ForegroundColor <- ConsoleColor.Yellow
        printfn "\t{ %s }" (describe task)
        Console.ForegroundColor <- ConsoleColor.White


let handleAddTask (taskManager: TaskManager) =
    printf "Enter description: "
    let description = Console.ReadLine()
    printf "Enter due date (YYYY-MM-DD HH:mm): "
    let dueDate = DateTime.ParseExact(Console.ReadLine(),dateFormat, null, System.Globalization.DateTimeStyles.None )
    printf "Enter priority (High=0, Normal=1, Low=2): "
    let priority = Enum.Parse<Priority>(Console.ReadLine())
    let updatedManager = addTask taskManager description dueDate priority
    printfn "Task added successfully."
    updatedManager

let handleDeleteTask (taskManager: TaskManager) =
    printf "Enter Task ID to delete: "
    let id = int(Console.ReadLine())
    if searchTaskExists(taskManager , id) then
        let updatedManager = deleteTask taskManager  id
        printfn "Task deleted successfully."
        updatedManager
    else
        printfn "Invalid ID."
        taskManager

let handleCompleteTask (taskManager: TaskManager) =
    printf "Enter Task ID to complete: "
    let id = int(Console.ReadLine())
    if searchTaskExists(taskManager , id) then
        let updatedManager = completeTask taskManager id
        printfn "Task completed successfully."
        updatedManager
    else
        printfn "Invalid ID."
        taskManager

let handleUpdatePriority (taskManager: TaskManager) =
    printf "Enter Task ID to Update Priority: "
    let id = int(Console.ReadLine())
    if searchTaskExists(taskManager , id) then
        printf "Enter priority (High=0, Normal=1, Low=2): "
        let priority = Enum.Parse<Priority>(Console.ReadLine())
        let updatedManager = updateTaskPriority taskManager id priority
        printfn "Task priority updated successfully."
        updatedManager
    else
        printfn "Invalid ID."
        taskManager

let handleShowTask (taskManager: TaskManager) =
    printf "Enter Task ID : "
    let id = int(Console.ReadLine())
    if searchTaskExists(taskManager , id) then
        match searchTask(taskManager , id) with
        | [] -> printfn "Task not found"
        | H::T -> printfn "%s" (describe H)
    else
        printfn "Invalid ID."

let handleShowTasks (taskManager: TaskManager) =
    printfn "All Tasks:"
    let allTasks = taskManager.tasks
    let filteredTasks = MyFilter (fun t -> t.status <> Status.Overdue) allTasks
    MyIter printTask filteredTasks

let handleFilteringTasks (taskManager: TaskManager,filterFunc: Task -> bool, title: string) =
    printfn "%s" title
    let filteredTasks = filterTasks taskManager filterFunc
    MyIter printTask filteredTasks

let handleSortingAscTasks (taskManager: TaskManager, taskAttrbuite: Task -> 'a) =
    printfn "Sorted Tasks:"
    let sortedTasks = MySortAscending taskAttrbuite taskManager.tasks
    MyIter printTask sortedTasks

let handleSortingDescTasks (taskManager: TaskManager, taskAttrbuite: Task -> 'a) =
    printfn "Sorted Tasks:"
    let sortedTasks = MySortDescending taskAttrbuite taskManager.tasks
    MyIter printTask sortedTasks

let notifyTask(task: Task) = 
    if task.dueDate <= System.DateTime.Now && task.status <> Status.Completed && task.isdead = false then
        { task with isdead = true }
    else
        task

let handleShowNotifications (taskManager: TaskManager) =
    let filteredTasks = filterTasks taskManager (fun task -> task.dueDate <= System.DateTime.Now && task.status <> Status.Completed && task.isdead = false )
    if filteredTasks.IsEmpty then
        printfn ""
        taskManager
    else
        Console.Beep()
        printfn "You Have New Overdue Tasks:"
        MyIter printTask filteredTasks
        let updatedTasks = MyMap (notifyTask) taskManager.tasks
        let updatedManager = setTasks taskManager updatedTasks
        updatedManager


let handleSaveAndExit (taskManager: TaskManager) filePath =
    FileOperations.saveTasksToFile (taskManager.tasks) filePath
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
        let updatedManager01 = overdueTasks taskManager
        let updatedManager02 = handleCompleteTask updatedManager01
        runApp updatedManager02
    | "4" -> 
        let updatedManager = handleUpdatePriority taskManager
        runApp updatedManager
    | "5" -> 
        let updatedManager = overdueTasks taskManager
        handleShowTask updatedManager
        runApp updatedManager
    | "6" -> 
        let updatedManager = overdueTasks taskManager
        handleFilteringTasks(updatedManager, (fun t -> t.status = Status.Pending), "Pending Tasks")
        runApp updatedManager
    | "7" -> 
        let updatedManager = overdueTasks taskManager
        handleFilteringTasks(updatedManager, (fun t -> t.status = Status.Completed), "Completed Tasks")
        runApp updatedManager
    | "8" -> 
        let updatedManager = overdueTasks taskManager
        handleFilteringTasks(updatedManager, (fun t -> t.status = Status.Overdue), "Overdued Tasks")
        runApp updatedManager
    | "9" -> 
        let updatedManager = overdueTasks taskManager
        handleSortingAscTasks(updatedManager, (fun t -> t.dueDate))
        runApp updatedManager
    | "10" -> 
        let updatedManager = overdueTasks taskManager
        handleSortingDescTasks(updatedManager, (fun t -> t.dueDate))
        runApp updatedManager
    | "11" -> 
        let updatedManager = overdueTasks taskManager
        handleSortingAscTasks(updatedManager, (fun t -> t.priority))
        runApp updatedManager
    | "12" -> 
        let updatedManager = overdueTasks taskManager
        handleSortingDescTasks(updatedManager, (fun t -> t.priority))
        runApp updatedManager
    | "13" -> 
        let updatedManager01 = overdueTasks taskManager
        let updatedManager02 =  handleShowNotifications updatedManager01
        runApp updatedManager02
    | "14" -> 
        handleSaveAndExit taskManager
    | _ -> 
        printfn "Invalid choice, please try again."
        runApp taskManager

[<EntryPoint>]
let main argv =
    let filePath = "MyTasks.json"
    let taskManager = {
        tasks =  []
        currentId = 1000
    }
    let myTasks = FileOperations.loadTasksFromFile(filePath)
    let taskManagerWithTasks = if not myTasks.IsEmpty then
                                                setTasks taskManager myTasks 
                                            else taskManager

    let taskManagerAfterOverdue = overdueTasks taskManagerWithTasks

    let _ = runApp taskManagerAfterOverdue filePath
    0
