// File: Application.cs
namespace ProjectName.Types
{
    public class Application
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? AllowedGrantTypes { get; set; }
        public string? APICHostname { get; set; }
        public string? CredentialsDev { get; set; }
        public string? CredentialsProd { get; set; }
        public string? Description { get; set; }
        public Guid? IdDev { get; set; }
        public Guid? IdProd { get; set; }
        public Guid? ImageId { get; set; }
        public string? Status { get; set; }
        public Guid? ClientIdDev { get; set; }
        public Guid? ClientIdProd { get; set; }
        public string? ClientSecret { get; set; }
        public string? ClientUriDev { get; set; }
        public string? ClientUriProd { get; set; }
        public string? CreatedEvent { get; set; }
        public Guid? EnvironmentId { get; set; }
        public List<Subscription>? Subscriptions { get; set; }
        public List<AppTag>? AppTags { get; set; }
        public int? Version { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Changed { get; set; }
        public Guid? CreatorId { get; set; }
        public Guid? ChangedUser { get; set; }
    }
}
