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
    match task.status with
    | Status.Overdue -> Console.ForegroundColor <- ConsoleColor.DarkMagenta
    | Status.Completed -> Console.ForegroundColor <- ConsoleColor.Green
    | _ when (task.dueDate - DateTime.Now).TotalDays < float numOfDays -> Console.ForegroundColor <- ConsoleColor.Red
    | _ -> Console.ForegroundColor <- ConsoleColor.Yellow

    printfn "\t{ %s }" (describe task)
    Console.ForegroundColor <- ConsoleColor.White



let handleAddTask (taskManager: TaskManager) =
    printf "Enter description: "
    let description = Console.ReadLine()

    let dateFormat = "yyyy-MM-dd HH:mm"

    printf "Enter due date (YYYY-MM-DD HH:mm): "
    let inputDate = Console.ReadLine()
    match System.DateTime.TryParseExact(inputDate, dateFormat, null, System.Globalization.DateTimeStyles.None) with
    | (true, dueDate) ->

        printf "Enter priority (High=0, Normal=1, Low=2): "
        match System.Int32.TryParse(Console.ReadLine()) with
        | (true, priorityInt) when priorityInt >= 0 && priorityInt <= 2 ->
            let priority = enum<Priority>(priorityInt)
            let updatedManager = addTask taskManager description dueDate priority
            printfn "Task added successfully."
            updatedManager
        | _ ->

            printfn "Invalid priority. Please enter a valid priority (0=High, 1=Normal, 2=Low)."
            taskManager
    | (false, _) ->

        printfn "Invalid date format. Please use the format YYYY-MM-DD HH:mm."
        taskManager


let handleDeleteTask (taskManager: TaskManager) =
    printf "Enter Task ID to delete: "
    let input = Console.ReadLine()


    match System.Int32.TryParse(input) with
    | (true, id) -> 

        match searchTaskExists(taskManager, id) with
        | true ->
            let updatedManager = deleteTask taskManager id
            printfn "Task deleted successfully."
            updatedManager
        | false ->
            printfn "Invalid ID."
            taskManager
    | (false, _) -> 

        printfn "Invalid input. Please enter a valid integer."
        taskManager

let handleCompleteTask (taskManager: TaskManager) =
    printf "Enter Task ID to complete: "
    let input = Console.ReadLine()

    match System.Int32.TryParse(input) with
    | (true, id) -> 
        match searchTaskExists(taskManager, id) with
        | true ->
            let updatedManager = completeTask taskManager id
            printfn "Task completed successfully."
            updatedManager
        | false ->
            printfn "Invalid ID."
            taskManager
    | (false, _) -> 
        printfn "Invalid input. Please enter a valid integer."
        taskManager


let handleUpdatePriority (taskManager: TaskManager) =
    printf "Enter Task ID to Update Priority: "
    let inputId = Console.ReadLine()

    // Try to parse the task ID as an integer
    match System.Int32.TryParse(inputId) with
    | (true, id) -> 
        if searchTaskExists(taskManager, id) then
            printf "Enter priority (High=0, Normal=1, Low=2): "
            let inputPriority = Console.ReadLine()

            match System.Enum.TryParse<Priority>(inputPriority) with
            | (true, priority) when System.Enum.IsDefined(typeof<Priority>, priority) ->

                let updatedManager = updateTaskPriority taskManager id priority
                printfn "Task priority updated successfully."
                updatedManager
            | _ -> 

                printfn "Invalid priority. Please enter a valid priority (0=High, 1=Normal, 2=Low)."
                taskManager
        else
            // If the task doesn't exist
            printfn "Invalid Task ID."
            taskManager
    | (false, _) -> 
        // If the task ID parsing fails
        printfn "Invalid Task ID. Please enter a valid integer."
        taskManager

let handleShowTask (taskManager: TaskManager) =
    printf "Enter Task ID: "
    let input = Console.ReadLine()

    match System.Int32.TryParse(input) with
    | (true, id) ->

        match searchTaskExists(taskManager, id) with
        | true ->
            match searchTask(taskManager, id) with
            | [] -> printfn "Task not found"
            | H :: _ -> printfn "%s" (describe H) 
        | false -> printfn "Invalid ID."
    | (false, _) -> 

        printfn "Invalid input. Please enter a valid integer."

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

let notifyTask (task: Task) =
    match task.dueDate <= System.DateTime.Now, task.status <> Status.Completed, task.isdead with
    | true, true, false -> { task with isdead = true }
    | _ -> task

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

    let taskManagerWithTasks =
        match myTasks.IsEmpty with
        | false -> setTasks taskManager myTasks
        | true  -> taskManager

    let taskManagerAfterOverdue = overdueTasks taskManagerWithTasks

    let _ = runApp taskManagerAfterOverdue filePath
    0

