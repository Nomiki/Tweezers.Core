using System;

namespace Tweezers.DBConnector
{
    public sealed class FindOptions<T>
    {
        private const int DefaultSkip = 0;
        private const int DefaultTake = 10;

        public int Skip { get; set; } = DefaultSkip;

        public int Take { get; set; } = DefaultTake;

        public Func<T, bool> Predicate { get; set; }

        public static FindOptions<T> Default(int skip = DefaultSkip, int take = DefaultTake) => 
            new FindOptions<T>() { Skip = skip, Take = take, Predicate = obj => true };
    }
}