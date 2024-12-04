open App.Data

let taskManager = TaskManager()

// taskManager.AddTask("Complete the report", System.DateTime(2024, 11, 27), Priority.Normal)
// taskManager.AddTask("Prepare presentation", System.DateTime(2024, 12, 5), Priority.High)
// taskManager.AddTask("Studying .NET", System.DateTime(2024, 12, 13), Priority.Low)

// ////Check Task's Due Date and Mark it Overdue
// taskManager.OverdueTasks()

// printfn "All Tasks"
// let tasks = taskManager.GetAllTasks()
// tasks |> List.iter (fun task -> task.Describe())

// printfn "-------------------------------------------"

// let id = 1001
// taskManager.DeleteTask(id)

// printfn "All Tasks after Deleting Task #%d" id
// let tasksAfterDeletion = taskManager.GetAllTasks()
// tasksAfterDeletion |> List.iter (fun task -> task.Describe())

// printfn "-------------------------------------------"

/////////////////////////////////////////////////////////////////////////////////

// let id2 = 1002

// printfn "All Tasks after Updating Task #%d Priority" id2
// taskManager.UpdateTaskPriority(id2, Priority.Low)
// let tasksAfterPriorityUpdate = taskManager.GetAllTasks()
// tasksAfterPriorityUpdate |> List.iter (fun task -> task.Describe())
// printfn "-------------------------------------------"

/////////////////////////////////////////////////////////////////////////////////

// let id3 = 1001
// printfn "All Tasks after Completing Task #%d" id3
// taskManager.CompleteTask(id3)
// let tasksAfterPriorityUpdate = taskManager.GetAllTasks()
// tasksAfterPriorityUpdate |> List.iter (fun task -> task.Describe())
// printfn "-------------------------------------------"

/////////////////////////////////////////////////////////////////////////////////
let filePath = "MyTasks.json"

//// Saving Tasks
// FileOperations.saveTasksToFile (taskManager.GetAllTasks()) (filePath)

//// Loading Tasks
let myTasks = FileOperations.loadTasksFromFile(filePath)
taskManager.SetTasks(myTasks)

// taskManager.AddTask("Studying SQL Server", System.DateTime(2024, 12, 15), Priority.Normal)

printfn "All Tasks"
printfn "-------------------------------------------"
let tasks = taskManager.GetAllTasks()
tasks |> List.iter (fun task -> task.Describe())
printfn "-------------------------------------------"
