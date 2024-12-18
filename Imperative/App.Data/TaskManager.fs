namespace App.Data

type TaskManager() = 
    let mutable currentId = 1000
    let mutable tasks = ResizeArray<TaskR>()
    member this.SetTasks(newTasks: ResizeArray<TaskR>) = 
        let mutable mxID = currentId;
        for task in newTasks do
            if task.id > mxID then
                mxID <- task.id
        currentId <- mxID
        tasks <- newTasks


    member this.GetAllTasks() =
        tasks

    member this.AddTask(description: string, dueDate: System.DateTime, priority : Priority) =
        currentId <- currentId + 1
        tasks.Add(TaskR(currentId, description, dueDate, priority, Status.Pending))
    
    member this.DeleteTask(id: int) =
        let updatedTasks = ResizeArray()
        for task in tasks do
            if task.id <> id then
                updatedTasks.Add(task)
        tasks <- updatedTasks

    member this.CompleteTask(id: int) =
        for task in tasks do
            if task.id = id then
                task.Complete()

    member this.OverdueTasks() =
        for task in tasks do
            if task.dueDate <= System.DateTime.Now then
                task.Overdue()

    member this.UpdateTaskPriority(id: int, newPriority: Priority) =
        for task in tasks do
            if task.id = id then
                task.UpdatePriority(newPriority)

    // Filtering Functions
    member this.PendingTasks () =
        let filteredTasks = ResizeArray()
        for task in tasks do
            if task.status = Status.Pending then
                filteredTasks.Add(task)
        filteredTasks
    
    member this.OverduedTasks () =
        let filteredTasks = ResizeArray()
        for task in tasks do
            if task.status = Status.Overdue then
                filteredTasks.Add(task)
        filteredTasks
    
    member this.CompletedTasks () =
        let filteredTasks = ResizeArray()
        for task in tasks do
            if task.status = Status.Completed then
                filteredTasks.Add(task)
        filteredTasks
    
    member this.WillBeNotifiedTasks () =
        let filteredTasks = ResizeArray()
        for task in tasks do
            if task.dueDate <= System.DateTime.Now && task.status <> Status.Completed && task.isdead = false then
                filteredTasks.Add(task)
        filteredTasks

    // Sorting Functions
    member this.sortTasksAscByDueDate() =
        let mutable swapped = true
        let mutable n = tasks.Count
        let sortedTasks = ResizeArray(tasks)
        
        while swapped do
            swapped <- false
            for i in 0 .. n - 2 do
                let compareResult = compare sortedTasks.[i].dueDate sortedTasks.[i + 1].dueDate
                
                if compareResult > 0 then
                    let temp = sortedTasks.[i]
                    sortedTasks.[i] <- sortedTasks.[i + 1]
                    sortedTasks.[i + 1] <- temp
                    swapped <- true
            n <- n - 1
        sortedTasks
    
    member this.sortTasksDescByDueDate() =
        let mutable swapped = true
        let mutable n = tasks.Count
        let sortedTasks = ResizeArray(tasks)
        
        while swapped do
            swapped <- false
            for i in 0 .. n - 2 do
                let compareResult = compare sortedTasks.[i].dueDate sortedTasks.[i + 1].dueDate
                
                if compareResult < 0 then
                    let temp = sortedTasks.[i]
                    sortedTasks.[i] <- sortedTasks.[i + 1]
                    sortedTasks.[i + 1] <- temp
                    swapped <- true
            n <- n - 1
        sortedTasks
    member this.sortTasksAscByStatus() =
        let mutable swapped = true
        let mutable n = tasks.Count
        let sortedTasks = ResizeArray(tasks)
        
        while swapped do
            swapped <- false
            for i in 0 .. n - 2 do
                let compareResult = compare sortedTasks.[i].status sortedTasks.[i + 1].status
                
                if compareResult > 0 then
                    let temp = sortedTasks.[i]
                    sortedTasks.[i] <- sortedTasks.[i + 1]
                    sortedTasks.[i + 1] <- temp
                    swapped <- true
            n <- n - 1
        sortedTasks
    
    member this.sortTasksDescByStatus() =
        let mutable swapped = true
        let mutable n = tasks.Count
        let sortedTasks = ResizeArray(tasks)
        
        while swapped do
            swapped <- false
            for i in 0 .. n - 2 do
                let compareResult = compare sortedTasks.[i].status sortedTasks.[i + 1].status
                
                if compareResult < 0 then
                    let temp = sortedTasks.[i]
                    sortedTasks.[i] <- sortedTasks.[i + 1]
                    sortedTasks.[i + 1] <- temp
                    swapped <- true
            n <- n - 1
        sortedTasks
    
    
    member this.sortTasksAscByPriority() =
        let mutable swapped = true
        let mutable n = tasks.Count
        let sortedTasks = ResizeArray(tasks)
        
        while swapped do
            swapped <- false
            for i in 0 .. n - 2 do
                let compareResult = compare sortedTasks.[i].priority sortedTasks.[i + 1].priority
                
                if compareResult > 0 then
                    let temp = sortedTasks.[i]
                    sortedTasks.[i] <- sortedTasks.[i + 1]
                    sortedTasks.[i + 1] <- temp
                    swapped <- true
            n <- n - 1
        sortedTasks
    
    member this.sortTasksDescByPriority() =
        let mutable swapped = true
        let mutable n = tasks.Count
        let sortedTasks = ResizeArray(tasks)
        
        while swapped do
            swapped <- false
            for i in 0 .. n - 2 do
                let compareResult = compare sortedTasks.[i].priority sortedTasks.[i + 1].priority
                
                if compareResult < 0 then
                    let temp = sortedTasks.[i]
                    sortedTasks.[i] <- sortedTasks.[i + 1]
                    sortedTasks.[i + 1] <- temp
                    swapped <- true
            n <- n - 1
        sortedTasks
    

    member this.searchTask (id: int) =
        let mutable isFound = false
        for task in tasks do
            if task.id = id then
                isFound <- true
        isFound

    member this.getTask(id: int) =
        let mutable t = Unchecked.defaultof<TaskR>
        for task in tasks do
            if task.id = id then
                t <- task
        t

open Newtonsoft.Json
open System.IO

module FileOperations = 
    let saveTasksToFile (tasks: ResizeArray<TaskR>) (filePath: string) =
        try
            let json = JsonConvert.SerializeObject(tasks, Formatting.Indented)
            File.WriteAllText(filePath, json)
        with
        | ex -> printfn "Error saving tasks to file: %s" ex.Message

    let loadTasksFromFile (filePath: string) =
        try
            if File.Exists(filePath) then
                let json = File.ReadAllText(filePath)
                JsonConvert.DeserializeObject<ResizeArray<TaskR>>(json)
            else
                printfn "File not found."
                ResizeArray<TaskR>()
        with ex ->
            printfn "Error loading tasks from file: %s" ex.Message
            ResizeArray<TaskR>()

