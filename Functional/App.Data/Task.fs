namespace App.Data

type Priority =
    | High = 0
    | Normal = 1
    | Low = 2

type Status =
    | Pending = 0
    | Completed = 1
    | Overdue = 2

type Task(id: int, description: string, dueDate: System.DateTime, priority: Priority, status: Status) =
    member this.id = id
    member this.description = description
    member this.dueDate = dueDate
    member this.priority = priority
    member this.status = status

    member this.Describe() = 
        $"""Task ID: {this.id}, Description: {this.description}, Due Date: {this.dueDate.ToString("yyyy-MM-dd")}, Priority: {this.priority}, Status: {this.status}"""

    member this.Complete() = 
        if this.status = Status.Overdue then this
        else Task(this.id, this.description, this.dueDate, this.priority, Status.Completed)

    member this.UpdatePriority(newPriority: Priority) = 
        Task(this.id, this.description, this.dueDate, newPriority, this.status)

    member this.Overdue() = 
        Task(this.id, this.description, this.dueDate, this.priority, Status.Overdue)

module ListExtensions = 
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
