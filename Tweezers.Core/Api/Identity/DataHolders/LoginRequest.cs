namespace Tweezers.Api.Identity.DataHolders
{
    public sealed class LoginRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }
    }
}
