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
        printf "Task ID: %d\nDescription: %s\nDue Date: %A\nPriority: %A\nStatus: %A\n" this.id this.description this.dueDate this.priority this.status

    member this.Complete() = 
        if this.status = Status.Overdue then this
        else Task(this.id, this.description, this.dueDate, this.priority, Status.Completed)

    member this.UpdatePriority(newPriority: Priority) = 
        Task(this.id, this.description, this.dueDate, newPriority, this.status)

    member this.Overdue() = 
        Task(this.id, this.description, this.dueDate, this.priority, Status.Overdue)