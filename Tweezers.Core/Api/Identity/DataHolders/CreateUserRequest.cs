namespace Tweezers.Api.Identity.DataHolders
{
    public sealed class CreateUserRequest : LoginRequest
    {
        public string RoleId { get; set; }
    }
}
