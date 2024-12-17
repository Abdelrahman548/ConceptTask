namespace App.Data

type Priority =
    | High = 0
    | Normal = 1
    | Low = 2

type Status =
    | Pending = 0
    | Completed = 1
    | Overdue = 2

type TaskR(id: int, description: string, dueDate: System.DateTime, priority: Priority, status: Status) =
    let mutable taskId = id
    let mutable taskDescription = description
    let mutable taskDueDate = dueDate
    let mutable taskPriority = priority
    let mutable taskStatus = status
    let mutable isDead = false

    member this.id with get() = taskId and set(value) = taskId <- value
    member this.description with get() = taskDescription and set(value) = taskDescription <- value
    member this.dueDate with get() = taskDueDate and set(value) = taskDueDate <- value
    member this.priority with get() = taskPriority and set(value) = taskPriority <- value
    member this.status with get() = taskStatus and set(value) = taskStatus <- value
    member this.isdead with get() = isDead and set(value) = isDead <- value

    member this.Describe() = 
        $"""Task ID: {this.id}, Description: {this.description}, Due Date: {this.dueDate.ToString("yyyy-MM-dd HH:mm")}, Priority: {this.priority}, Status: {this.status}"""

    member this.Complete() = 
        if this.status <> Status.Overdue then this.status <- Status.Completed

    member this.UpdatePriority(newPriority: Priority) = 
        this.priority <- newPriority

    member this.Overdue() = 
        this.status <- Status.Overdue
