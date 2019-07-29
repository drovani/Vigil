using System;
using Ardalis.GuardClauses;

namespace Vigil.Framework
{
    public static class GuardClauseExtensions
    {
        public static void NonUtcDateTimeKind(this IGuardClause guardClause, DateTime input, string parameterName)
        {
            Guard.Against.Default(input, parameterName);
            if (input.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException($"{parameterName} must be DateTimeKind.UTC.", parameterName);
            }
        }
    }
}
