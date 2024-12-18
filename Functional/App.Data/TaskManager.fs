namespace App.Data

open ListExtensions

type TaskManager = {
    tasks: Task list
    currentId: int
}

module TaskManagerOperations =
    open TaskCRUD
    let setTasks manager newTasks =
        let newId = MyMaxID (newTasks, manager.currentId)
        { manager with tasks = newTasks; currentId = newId }

    let addTask manager description dueDate priority =
        let status = if dueDate > System.DateTime.Now then Status.Pending else Status.Overdue
        let newTask = { id = manager.currentId + 1; description = description; dueDate = dueDate; priority = priority; status = status ; isdead = false}
        { manager with tasks = newTask :: manager.tasks; currentId = manager.currentId + 1 }

    let deleteTask manager id =
        let updatedTasks = MyFilter (fun task -> task.id <> id) manager.tasks
        { manager with tasks = updatedTasks }

    let completeTask manager id =
        let updatedTasks = MyMap (fun t -> if t.id = id then completeTask t else t) manager.tasks
        { manager with tasks = updatedTasks }

    let overdueTasks manager =
        let updatedTasks = MyMap (fun t -> if t.dueDate <= System.DateTime.Now && t.status <> Status.Completed then markOverdue t else t) manager.tasks
        { manager with tasks = updatedTasks }

    let updateTaskPriority manager id newPriority =
        let updatedTasks = MyMap (fun t -> if t.id = id then updatePriority t newPriority else t) manager.tasks
        { manager with tasks = updatedTasks }

    let filterTasks manager filterFunc =
        MyFilter filterFunc manager.tasks

    let searchTaskExists(manager: TaskManager,id: int) =
        let res = MyFilter (fun t -> t.id = id)manager.tasks
        if res.IsEmpty then false
        else true
    let searchTask(manager: TaskManager,id: int) =
        let res = MyFilter (fun t -> t.id = id)manager.tasks
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
                let emptyListJson = JsonConvert.SerializeObject([])
                File.WriteAllText(filePath, emptyListJson)
                []
        with
        | ex -> printfn "Error loading tasks from file: %s" ex.Message; []
