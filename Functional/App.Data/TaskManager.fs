namespace App.Data

open ListExtensions

type TaskManager(tasks: Task list, currentId: int) = 
    member this.SetTasks(newTasks: Task list) = 
        let newId = newTasks |> List.maxBy (fun t -> t.id) |> fun t -> t.id
        TaskManager(newTasks, newId)

    member this.GetAllTasks() = tasks

    member this.AddTask(description: string, dueDate: System.DateTime, priority: Priority) =
        let status = if dueDate > System.DateTime.Now then Status.Pending else Status.Overdue
        let newTask = Task(currentId + 1, description, dueDate, priority, status)
        TaskManager(newTask :: tasks, currentId + 1)

    member this.DeleteTask(id: int) =
        let updatedTasks = tasks |> List.filter (fun task -> task.id <> id)
        TaskManager(updatedTasks, currentId)

    member this.CompleteTask(id: int) =
        let updatedTasks =  MyMap (fun t -> if t.id = id then t.Complete() else t) tasks
        TaskManager(updatedTasks, currentId)

    member this.OverdueTasks() =
        let updatedTasks = MyMap (fun t -> if t.dueDate <= System.DateTime.Now then t.Overdue() else t) tasks
        TaskManager(updatedTasks, currentId)

    member this.UpdateTaskPriority(id: int, newPriority: Priority) =
        let updatedTasks = MyMap (fun t -> if t.id = id then t.UpdatePriority(newPriority) else t) tasks
        TaskManager(updatedTasks, currentId)

    member this.FilterTasks(filterFunc: Task -> bool) =
        MyFilter filterFunc tasks

    member this.SortTasks(sortAttribute: Task -> 'a) =
        MySortAscending sortAttribute tasks

    member this.SortTasksDec(sortAttribute: Task -> 'a) =
        MySortDescending sortAttribute tasks

    member this.SearchTaskExists(id: int) =
        let res = this.FilterTasks(fun t -> t.id = id)
        if res.IsEmpty then false
        else true

    member this.SearchTask(id: int) =
        let res = this.FilterTasks(fun t -> t.id = id)
        res


open Newtonsoft.Json
open System.IO

module FileOperations = 
    let saveTasksToFile (tasks: Task list) (filePath: string) =
        try
            let json = JsonConvert.SerializeObject(tasks, Formatting.Indented)
            File.WriteAllText(filePath, json)
        with
        | ex -> printfn "Error saving tasks to file: %s" ex.Message

    let loadTasksFromFile (filePath: string) =
        try
            if File.Exists(filePath) then
                let json = File.ReadAllText(filePath)
                let tasks = JsonConvert.DeserializeObject<Task list>(json)
                tasks
            else
                printfn "File not found."; []
        with
        | ex -> printfn "Error loading tasks from file: %s" ex.Message; []
