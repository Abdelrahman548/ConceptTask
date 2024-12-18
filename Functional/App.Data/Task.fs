namespace App.Data

type Priority =
    | High = 0
    | Normal = 1
    | Low = 2

type Status =
    | Pending = 0
    | Completed = 1
    | Overdue = 2

type Task = {
    id: int
    description: string
    dueDate: System.DateTime
    priority: Priority
    status: Status
    isdead: bool
}

module TaskCRUD = 
    let completeTask task =
        if task.status = Status.Overdue then task
        else { task with status = Status.Completed }

    let updatePriority task newPriority =
        { task with priority = newPriority }

    let markOverdue task =
        { task with status = Status.Overdue }
    
    let describe task =
        $"""Task ID: {task.id}, Description: {task.description}, Due Date: {task.dueDate.ToString("yyyy-MM-dd HH:mm")}, Priority: {task.priority}, Status: {task.status}"""

module ListExtensions = 
    // myMaxID for Task list
    let rec MyMaxID(tasks: Task List,maxId: int) =
        match tasks with
        | [] -> maxId
        | task :: tail -> 
            let newMaxId= if task.id > maxId then task.id else maxId
            MyMaxID (tail, newMaxId)
    
    // myMap for Task list
    let MyMap (func: Task -> 'U) (lst: Task list) : 'U list =
        let rec map lst =
            match lst with
            | [] -> []
            | head :: tail -> func head :: map tail
        map lst

    // myFilter for Task list
    let MyFilter (func: Task -> bool) (lst: Task list) : Task list =
        let rec filter lst =
            match lst with
            | [] -> []
            | head :: tail ->
                if func head then
                    head :: filter tail
                else
                    filter tail
        filter lst

    let MyIter (func: Task -> unit) (lst: Task list) : unit =
        let rec iter lst =
            match lst with
            | [] -> ()
            | head :: tail -> 
                func head
                iter tail
        iter lst


    // mySort for Task list with dynamic comparison
    let MySort (compare: Task -> Task -> int) (lst: Task list) : Task list =
        let rec insert x sortedList =
            match sortedList with
            | [] -> [x]
            | head :: tail ->
                if compare x head <= 0 then
                    x :: sortedList
                else
                    head :: insert x tail

        let rec sort lst =
            match lst with
            | [] -> []
            | head :: tail -> insert head (sort tail)

        sort lst

    // Ascending Sort for Task list with dynamic attribute comparison
    let MySortAscending (attributeSelector: Task -> 'T) (lst: Task list) : Task list when 'T : comparison =
        MySort (fun x y -> compare (attributeSelector x) (attributeSelector y)) lst

    // Descending Sort for Task list with dynamic attribute comparison
    let MySortDescending (attributeSelector: Task -> 'T) (lst: Task list) : Task list when 'T : comparison =
        MySort (fun x y -> compare (attributeSelector y) (attributeSelector x)) lst
