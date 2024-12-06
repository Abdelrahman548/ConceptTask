namespace App.Data

type TaskManager() = 
    let mutable currentId = 1000
    let mutable tasks = []

    member this.SetTasks(newTasks: Task List) = 
        currentId <- newTasks |> List.maxBy (fun t -> t.id) |> fun t -> t.id
        tasks <- newTasks

    member this.GetAllTasks() =
        tasks

    member this.AddTask(description: string, dueDate: System.DateTime, priority : Priority) =
        currentId <- currentId + 1
        tasks <- Task(currentId, description, dueDate, priority, Status.Pending) :: tasks
    
    member this.DeleteTask(id: int) =
        let updatedTasks = ResizeArray()
        for task in tasks do
            if task.id <> id then
                updatedTasks.Add(task)
        tasks <- List.ofSeq updatedTasks

    member this.CompleteTask(id: int) =
        let updatedTasks = ResizeArray()
        for task in tasks do
            if task.id = id then
                task.Complete()
                updatedTasks.Add(task)
            else
                updatedTasks.Add(task)
        tasks <- List.ofSeq updatedTasks

    member this.OverdueTasks() =
        let updatedTasks = ResizeArray()
        for task in tasks do
            if task.dueDate <= System.DateTime.Now then
                task.Overdue()
                updatedTasks.Add(task)
            else
                updatedTasks.Add(task)
        tasks <- List.ofSeq updatedTasks

    member this.UpdateTaskPriority(id: int, newPriority: Priority) =
        let updatedTasks = ResizeArray()
        for task in tasks do
            if task.id = id then
                task.UpdatePriority(newPriority)
                updatedTasks.Add(task)
            else
                updatedTasks.Add(task)
        tasks <- List.ofSeq updatedTasks

    member this.filterTasks (filterFunc: Task -> bool) =
        let filteredTasks = ResizeArray()
        for task in tasks do
            if filterFunc(task) then
                filteredTasks.Add(task)
        List.ofSeq filteredTasks

    member this.sortTasks (sortFunc: Task -> 'a)=
        tasks |> List.sortBy sortFunc

    member this.sortTasksDec (sortFunc: Task -> 'a)=
        tasks |> List.sortByDescending sortFunc

    member this.searchTask (id: int) =
        let mutable isFound = false
        for task in tasks do
            if task.id = id then
                isFound <- true
        isFound

    member this.getTask(id: int) =
        let mutable t = Unchecked.defaultof<Task>
        for task in tasks do
            if task.id = id then
                t <- task
        t

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
