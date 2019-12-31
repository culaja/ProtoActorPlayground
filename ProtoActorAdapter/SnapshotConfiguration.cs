using System;

namespace ProtoActorAdapter
{
    public sealed class SnapshotConfiguration
    {
        private readonly Uri _connectionString;
        
        public string SnapshotName { get; }
        public TimeSpan SnapshotTimeSpan { get; }
        
        public SnapshotConfiguration(
            Uri connectionString,
            string snapshotName,
            TimeSpan snapshotTimeSpan)
        {
            _connectionString = connectionString;
            SnapshotName = snapshotName;
            SnapshotTimeSpan = snapshotTimeSpan;
        }

        public string FormattedConnectionString => $"ConnectTo={_connectionString}";
    }
}