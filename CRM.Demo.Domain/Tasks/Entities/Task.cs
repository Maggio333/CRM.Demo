using CRM.Demo.Domain.Common;
using CRM.Demo.Domain.Tasks.ValueObjects;
using CRM.Demo.Domain.Tasks.DomainEvents;
using TaskStatus = CRM.Demo.Domain.Tasks.ValueObjects.TaskStatus;

namespace CRM.Demo.Domain.Tasks.Entities;

public class Task : Entity<Guid>
{
    // Podstawowe informacje
    public string Title { get; private set; }
    public string? Description { get; private set; }

    // Value Objects
    public TaskType Type { get; private set; }
    public TaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }

    // Terminy
    public DateTime? DueDate { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? CompletedDate { get; private set; }

    // Relacje (tylko ID - referencje do innych agregatów)
    public Guid? CustomerId { get; private set; }
    public Guid? ContactId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }

    // Metadata
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Prywatny konstruktor
    private Task() { }

    // Factory method
    public static Task Create(
        string title,
        TaskType type,
        TaskPriority priority,
        Guid createdByUserId,
        string? description = null,
        DateTime? dueDate = null,
        DateTime? startDate = null,
        Guid? customerId = null,
        Guid? contactId = null,
        Guid? assignedToUserId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title cannot be empty");

        if (dueDate.HasValue && startDate.HasValue && dueDate < startDate)
            throw new DomainException("Due date cannot be before start date");

        var task = new Task
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Type = type,
            Status = TaskStatus.ToDo,
            Priority = priority,
            DueDate = dueDate,
            StartDate = startDate,
            CustomerId = customerId,
            ContactId = contactId,
            AssignedToUserId = assignedToUserId,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };

        task.AddDomainEvent(new TaskCreatedEvent(
            task.Id,
            task.Title,
            task.Type.Value,
            task.CreatedAt,
            task.AssignedToUserId,
            task.CustomerId,
            task.ContactId
        ));

        return task;
    }

    // Metody biznesowe
    public void AssignToUser(Guid userId)
    {
        if (Status == TaskStatus.Completed)
            throw new DomainException("Cannot assign completed task");

        if (Status == TaskStatus.Cancelled)
            throw new DomainException("Cannot assign cancelled task");

        var oldUserId = AssignedToUserId;
        AssignedToUserId = userId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TaskAssignedEvent(
            Id,
            oldUserId,
            userId,
            DateTime.UtcNow
        ));
    }

    public void ChangeStatus(TaskStatus newStatus)
    {
        if (Status == newStatus)
            return;

        // Walidacja przejść statusów
        if (Status == TaskStatus.Completed && newStatus != TaskStatus.Completed)
            throw new DomainException("Cannot change status from Completed");

        if (Status == TaskStatus.Cancelled && newStatus != TaskStatus.Cancelled)
            throw new DomainException("Cannot change status from Cancelled");

        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        // Jeśli ukończone - ustaw datę ukończenia
        if (newStatus == TaskStatus.Completed)
        {
            CompletedDate = DateTime.UtcNow;
            AddDomainEvent(new TaskCompletedEvent(
                Id,
                CompletedDate.Value
            ));
        }
        else
        {
            CompletedDate = null;
        }

        AddDomainEvent(new TaskStatusChangedEvent(
            Id,
            oldStatus,
            newStatus,
            DateTime.UtcNow
        ));
    }

    public void UpdateDueDate(DateTime? newDueDate)
    {
        if (Status == TaskStatus.Completed)
            throw new DomainException("Cannot update due date for completed task");

        if (newDueDate.HasValue && StartDate.HasValue && newDueDate < StartDate)
            throw new DomainException("Due date cannot be before start date");

        DueDate = newDueDate;
        UpdatedAt = DateTime.UtcNow;

        // Sprawdź czy zadanie jest przeterminowane
        if (newDueDate.HasValue && newDueDate < DateTime.UtcNow && Status != TaskStatus.Completed)
        {
            AddDomainEvent(new TaskOverdueEvent(
                Id,
                newDueDate.Value,
                AssignedToUserId
            ));
        }
    }

    public void LinkToCustomer(Guid customerId)
    {
        CustomerId = customerId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void LinkToContact(Guid contactId)
    {
        ContactId = contactId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? description)
    {
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsOverdue =>
        DueDate.HasValue &&
        DueDate < DateTime.UtcNow &&
        Status != TaskStatus.Completed;

    // Metoda do pełnej aktualizacji
    public void Update(
        string title,
        TaskType type,
        TaskPriority priority,
        string? description,
        DateTime? startDate,
        DateTime? dueDate,
        Guid? customerId,
        Guid? contactId,
        Guid? assignedToUserId)
    {
        if (Status == TaskStatus.Completed)
            throw new DomainException("Cannot update completed task");

        if (Status == TaskStatus.Cancelled)
            throw new DomainException("Cannot update cancelled task");

        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Task title cannot be empty");

        if (dueDate.HasValue && startDate.HasValue && dueDate < startDate)
            throw new DomainException("Due date cannot be before start date");

        Title = title;
        Type = type;
        Priority = priority;
        Description = description;
        StartDate = startDate;
        DueDate = dueDate;

        if (CustomerId != customerId)
        {
            CustomerId = customerId;
        }

        if (ContactId != contactId)
        {
            ContactId = contactId;
        }

        if (AssignedToUserId != assignedToUserId)
        {
            var oldUserId = AssignedToUserId;
            AssignedToUserId = assignedToUserId;
            if (oldUserId.HasValue || assignedToUserId.HasValue)
            {
                AddDomainEvent(new TaskAssignedEvent(
                    Id,
                    oldUserId,
                    assignedToUserId ?? Guid.Empty,
                    DateTime.UtcNow
                ));
            }
        }

        UpdatedAt = DateTime.UtcNow;

        // Sprawdź czy zadanie jest przeterminowane
        if (dueDate.HasValue && dueDate < DateTime.UtcNow && Status != TaskStatus.Completed)
        {
            AddDomainEvent(new TaskOverdueEvent(
                Id,
                dueDate.Value,
                assignedToUserId
            ));
        }
    }
}
