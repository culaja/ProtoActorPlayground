using System;
using EventStore.ClientAPI.SystemData;

namespace EventStoreAdapter
{
    public static class UserCredentialsUtilities
    {
        public static UserCredentials ExtractUserCredentials(this Uri connectionString)
        {
            var firstSplitArray = connectionString.ToString().Split('@', StringSplitOptions.RemoveEmptyEntries);
            if (firstSplitArray.Length != 2) throw new ArgumentException(nameof(connectionString));

            var secondSplitArray = firstSplitArray[0]
                .Replace("tcp://", "")
                .Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (secondSplitArray.Length != 2) throw new ArgumentException(nameof(connectionString));
            
            return new UserCredentials(secondSplitArray[0], secondSplitArray[1]);
        }
    }
}