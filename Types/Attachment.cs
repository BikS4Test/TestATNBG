// File: Attachment.cs
namespace ProjectName.Types
{
    public class Attachment
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string File { get; set; }
        public DateTime Timestamp { get; set; }
        public int? Version { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? ChangedUser { get; set; }
    }
}
