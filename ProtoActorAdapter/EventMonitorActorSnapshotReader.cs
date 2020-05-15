using System.Threading;
using System.Threading.Tasks;
using Domain;
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

        public async Task<DomainEventPosition> ReadLastSnapshot(CancellationToken cancellationToken)
        {
            var lastSnapshottedPosition = DomainEventPosition.Start; 
            await Task.WhenAny(
                Task.Run(async () =>
                {
                    lastSnapshottedPosition = await ReadSnapshotAsync();
                }),
                cancellationToken.WaitAsync());

            return lastSnapshottedPosition;
        }

        private async Task<DomainEventPosition> ReadSnapshotAsync()
        {
            var result = await _snapshotStore.GetSnapshotAsync(_snapshotName);
            switch (result.Snapshot)
            {
                case null:
                    return DomainEventPosition.Start;
                default:
                    return (DomainEventPosition) result.Snapshot;
            }
        }
    }
}