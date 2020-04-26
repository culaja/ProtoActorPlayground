using System.Threading;
using System.Threading.Tasks;
using Framework;
using Proto.Persistence;

namespace ProtoActorAdapter
{
    internal sealed class EventMonitorActorSnapshotReader
    {
        private readonly ISnapshotStore _snapshotStore;
        private readonly string _snapshotName;

        public EventMonitorActorSnapshotReader(
            ISnapshotStore snapshotStore,
            string snapshotName)
        {
            _snapshotStore = snapshotStore;
            _snapshotName = snapshotName;
        }

        public async Task<long> ReadLastSnapshot(CancellationToken cancellationToken)
        {
            long lastSnapshottedPosition = -1; 
            await Task.WhenAny(
                Task.Run(async () =>
                {
                    lastSnapshottedPosition = await ReadSnapshotAsync();
                }),
                cancellationToken.WaitAsync());

            return lastSnapshottedPosition;
        }

        private async Task<long> ReadSnapshotAsync()
        {
            var result = await _snapshotStore.GetSnapshotAsync(_snapshotName);
            switch (result.Snapshot)
            {
                case null:
                    return -1;
                default:
                    return (long) result.Snapshot;
            }
        }
    }
}