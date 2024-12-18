open App.Data
open System
open System.Threading
open System.Threading.Tasks

module Notifier =
    let OverDueTasksNotifier (taskManager: TaskManager, cancellationToken: CancellationToken) =
        async {
            while not cancellationToken.IsCancellationRequested do
                let overdueTasks = taskManager.WillBeNotifiedTasks()
                
                if overdueTasks.Count > 0 then
                    System.Console.Beep()
                    Console.ForegroundColor <- ConsoleColor.Red
                    printfn "\nYou have overdue tasks!"
                    for task in overdueTasks do
                        task.Overdue()
                        task.isdead <- true
                        printfn "\tOverdue Task: %s (Due: %s)" task.description (task.dueDate.ToString("yyyy-MM-dd HH:mm"))
                    Console.ForegroundColor <- ConsoleColor.White
                do! Async.Sleep(30000) // 30 Second
        }


[<EntryPoint>]
let main argv =
    let filePath = "MyTasks.json"
    let numOfDays = 1
    let taskManager = TaskManager()
    let myTasks = FileOperations.loadTasksFromFile(filePath)
    if myTasks.Count <> 0 then
        taskManager.SetTasks(myTasks)

    let cancellationTokenSource = new CancellationTokenSource()
    let cancellationToken = cancellationTokenSource.Token

    // Start the overdue task notifier
    Async.Start (Notifier.OverDueTasksNotifier(taskManager, cancellationToken))

    let printTask (task: TaskR) = 
        if(task.status = Status.Overdue) then
            Console.ForegroundColor <- ConsoleColor.DarkMagenta
        elif(task.status = Status.Completed) then
            Console.ForegroundColor <- ConsoleColor.Green
        // elif((task.dueDate - DateTime.Now).TotalDays < numOfDays) then
        //     Console.ForegroundColor <- ConsoleColor.Red
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
        printfn "5. Show Task with ID"
        printfn "6. Show Pending Tasks"
        printfn "7. Show Completed Tasks"
        printfn "8. Show Overdue Tasks"
        printfn "9. Show Tasks Sorted Ascending by [DueDate]"
        printfn "10. Show Tasks Sorted Descending by [DueDate]"
        printfn "11. Show Tasks Sorted Ascending by [Priority]"
        printfn "12. Show Tasks Sorted Descending by [Priority]"
        printfn "13. Save & Exit"
        printfn "========================================================"

    let mutable running = true

    let isTaskExist id =
        taskManager.searchTask(id)

    let dateFormat = "yyyy-MM-dd HH:mm"
    while running do
        displayMenu()
        printf "Enter your choice: "
        let input = Console.ReadLine()

        if (input = "1") then
            let mutable result = false
            let mutable description = ""
            let mutable dueDate = DateTime.Now
            let mutable priority = Priority.Normal
            while(result <> true) do
                printf "Enter description: "
                description <- Console.ReadLine()
                if (description <> "") then
                    result <- true
            
            result <- false
            while (result <> true) do
                printf "Enter due date (YYYY-MM-DD HH:mm): "
                result <- DateTime.TryParseExact(Console.ReadLine(),dateFormat, null, System.Globalization.DateTimeStyles.None, &dueDate )

            result <- false
            while (result <> true) do
                 printf "Enter priority (High=0, Normal=1, Low=2): "
                 result <- Enum.TryParse<Priority>(Console.ReadLine(),&priority) && Enum.IsDefined priority

           
            
            taskManager.AddTask(description, dueDate, priority)
            printfn "Task added successfully."
            printfn ""
        elif(input ="2") then
            let mutable result = false
            let mutable id = 0
            while (result <> true) do
                printf "Enter Task ID to Update Priority: "
                result <- Int32.TryParse(Console.ReadLine(),&id)
            if isTaskExist(id) then
                taskManager.DeleteTask(id)
                printfn "Task deleted successfully."
            else
                printfn "Invalid ID."
            printfn ""
        elif(input ="3") then
            let mutable result = false
            let mutable id = 0
            while (result <> true) do
                printf "Enter Task ID to Update Priority: "
                result <- Int32.TryParse(Console.ReadLine(),&id)
            if isTaskExist(id) then
                taskManager.CompleteTask(id)
                printfn "Task completed successfully."
            else
                printfn "Invalid ID."
            printfn ""
        elif(input ="4") then
            let mutable result = false
            let mutable priority = Priority.Normal
            let mutable id  = 0
            while (result <> true) do
                printf "Enter Task ID to Update Priority: "
                result <- Int32.TryParse(Console.ReadLine(),&id)
            if isTaskExist(id) then
                result <- false
                while (result <> true) do
                    printf "Enter priority (High=0, Normal=1, Low=2): "
                    result <- Enum.TryParse<Priority>(Console.ReadLine(),&priority) && Enum.IsDefined priority
                taskManager.UpdateTaskPriority(id, priority)
                printfn "Priority updated successfully."
            else
                printfn "Invalid ID."
            printfn ""
        elif(input ="5") then
            let mutable result = false
            let mutable id = 0
            while (result <> true) do
                printf "Enter Task ID to Update Priority: "
                result <- Int32.TryParse(Console.ReadLine(),&id)
            if isTaskExist(id) then
                let task = taskManager.getTask(id)
                printTask(task)
            else
                printfn "Invalid ID."
            printfn ""
        elif(input ="6") then
            printfn "Pending Tasks:"
            let pendingTasks = taskManager.PendingTasks()
            for task in pendingTasks do
                printTask(task)
            printfn ""
        elif(input ="7") then
            printfn "Completed Tasks:"
            let completedTasks = taskManager.CompletedTasks()
            for task in completedTasks do
                printTask(task)
            printfn ""
        elif(input ="8") then
            printfn "Overdue Tasks:"
            let overdueTasks = taskManager.OverduedTasks()
            for task in overdueTasks do
                printTask(task)
            printfn ""
        elif(input ="9") then
            printfn "Sorted Tasks:"
            let sortedTasks = taskManager.sortTasksAscByDueDate()
            for task in sortedTasks do
                printTask(task)
            printfn ""
        elif(input ="10") then
            printfn "Sorted Tasks:"
            let sortedTasks = taskManager.sortTasksDescByDueDate()
            for task in sortedTasks do
                printTask(task)
            printfn ""
        elif(input = "11") then
            printfn "Sorted Tasks:"
            let sortedTasks = taskManager.sortTasksAscByPriority()
            for task in sortedTasks do
                printTask(task)
            printfn ""
        elif(input ="12") then
            printfn "Sorted Tasks:"
            let sortedTasks = taskManager.sortTasksDescByPriority()
            for task in sortedTasks do
                printTask(task)
            printfn ""
        elif(input ="13") then
            FileOperations.saveTasksToFile (taskManager.GetAllTasks()) (filePath)
            printfn "Exiting... Goodbye!"
            running <- false
        else
            printfn "Invalid choice, please try again."
    cancellationTokenSource.Cancel()
    0
