using System;
using System.Linq;
using System.Threading;
using Domain;
using EventStore.ClientAPI.Exceptions;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Ports;

namespace EventStoreAdapter
{
    public sealed class SourceProjectionCreator
    {
        private readonly ProjectionsManager _projectionsManager;
        private readonly UserCredentials _extractUserCredentials;
        private readonly IInternalLogger _internalLogger;

        private string ProjectionBodyFor(StreamPrefix streamPrefix) =>
            "fromAll().when({ $any : function(s,e) { if (e.streamId.startsWith('STREAM_PREFIX')) linkTo('STREAM_NAME', e); }});"
                .Replace("STREAM_PREFIX", streamPrefix)
                .Replace("STREAM_NAME", streamPrefix.ToStreamName());

        private SourceProjectionCreator(
            ProjectionsManager projectionsManager,
            UserCredentials extractUserCredentials,
            IInternalLogger internalLogger)
        {
            _projectionsManager = projectionsManager;
            _extractUserCredentials = extractUserCredentials;
            _internalLogger = internalLogger;
        }
        
        public static SourceProjectionCreator NewFor(Uri connectionString, IInternalLogger internalLogger) => new SourceProjectionCreator(
            new ProjectionsManager(
                new NoEventStoreLogger(),
                connectionString.ToDnsEndPoint(),
                TimeSpan.FromSeconds(30)),
            connectionString.ExtractUserCredentials(),
            internalLogger);

        public void Create(StreamPrefix streamPrefix, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var isCreated = CreateFor(streamPrefix);
                    _internalLogger.Information(isCreated
                        ? $"Successfully created projection for stream prefix '{streamPrefix}'"
                        : $"Projection for stream prefix '{streamPrefix}' is already created.");
                    break;
                }
                catch (Exception ex)
                {
                    _internalLogger.Error($"Failed to create a projection for stream prefix '{streamPrefix}'", ex);
                }
            }
        }
        
        private bool CreateFor(StreamPrefix streamPrefix)
        {
            try
            {
                _projectionsManager.CreateContinuousAsync(
                    $"{streamPrefix}Projection",
                    ProjectionBodyFor(streamPrefix),
                    true,
                    _extractUserCredentials).Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerExceptions.Any(inner => inner is ProjectionCommandConflictException))
                {
                    return false;
                }

                throw;
            }
            
            return true;
        }
    }
}