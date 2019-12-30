using System;

namespace ProtoActorAdapter
{
    public sealed class SnapshotConfiguration
    {
        private readonly string _connectionString;

        public SnapshotConfiguration(
            string connectionString,
            string snapshotName,
            TimeSpan snapshotTimeSpan)
        {
            _connectionString = connectionString;
            SnapshotName = snapshotName;
            SnapshotTimeSpan = snapshotTimeSpan;
        }

        internal string ConnectionString { get; }
        
        public string SnapshotName { get; }
        public TimeSpan SnapshotTimeSpan { get; }
    }
}