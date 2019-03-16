using System;

namespace Tweezers.Api.Exceptions
{
    public sealed class ItemNotFoundException : Exception
    {
        private string Id { get; set; }

        public ItemNotFoundException(string id = null)
        {
            Id = id ?? string.Empty;
        }

        public override string Message => $"Could not find item with ID={Id}";
    }
}
