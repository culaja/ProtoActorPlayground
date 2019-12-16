namespace ProtoActorAdapter
{
    public sealed class EventStoreConfiguration
    {
        private readonly string _hostName;
        private readonly ushort _port;
        private readonly string _userName;
        private readonly string _password;

        public EventStoreConfiguration(
            string hostName,
            ushort port,
            string userName,
            string password,
            string streamsPrefix)
        {
            StreamsPrefix = streamsPrefix;
            _hostName = hostName;
            _port = port;
            _userName = userName;
            _password = password;
        }

        public string ConnectionString =>
            $"ConnectTo=tcp://{_userName}:{_password}@{_hostName}:{_port}; DefaultUserCredentials={_userName}:{_password};";

        public string StreamsPrefix { get; }
    }
}