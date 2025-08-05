namespace Entities.Dtos
{
    public record AccountDtoForUpdate : AccountDto
    {
        public HashSet<string?> RolesList { get; set; } = new HashSet<string?>();
    }
}
