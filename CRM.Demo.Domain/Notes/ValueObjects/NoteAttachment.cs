using CRM.Demo.Domain.Common;

namespace CRM.Demo.Domain.Notes.ValueObjects;

public class NoteAttachment : ValueObject
{
    public Guid Id { get; }
    public string FileName { get; }
    public string ContentType { get; }
    public long FileSize { get; }
    public string FilePath { get; }
    public DateTime UploadedAt { get; }
    
    private NoteAttachment(
        Guid id,
        string fileName,
        string contentType,
        long fileSize,
        string filePath,
        DateTime uploadedAt)
    {
        Id = id;
        FileName = fileName;
        ContentType = contentType;
        FileSize = fileSize;
        FilePath = filePath;
        UploadedAt = uploadedAt;
    }
    
    public static NoteAttachment Create(
        Guid id,
        string fileName,
        string contentType,
        long fileSize,
        string filePath)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("File name cannot be empty");
        
        if (fileSize <= 0)
            throw new DomainException("File size must be greater than 0");
        
        if (fileSize > 10 * 1024 * 1024)  // 10 MB limit
            throw new DomainException("File size exceeds maximum limit (10 MB)");
        
        return new NoteAttachment(
            id,
            fileName,
            contentType,
            fileSize,
            filePath,
            DateTime.UtcNow
        );
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
    
    public override string ToString() => FileName;
}
