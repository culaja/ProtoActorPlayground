using System;
using System.Net;

namespace EventStoreAdapter
{
    public static class ConnectionStringUtilities
    {
        public static DnsEndPoint ToDnsEndPoint(this Uri connectionString)
        {
            var firstSplitArray = connectionString.AbsoluteUri.Split('@', StringSplitOptions.RemoveEmptyEntries);
            if (firstSplitArray.Length != 2) throw new ArgumentException(nameof(connectionString));

            var secondSplitArray = firstSplitArray[1].Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (secondSplitArray.Length != 2) throw new ArgumentException(nameof(connectionString));

            var host = secondSplitArray[0];
            var port = int.Parse(secondSplitArray[1].TrimEnd('/')) + 1000;
            return new DnsEndPoint(host, port);
        }
    }
}