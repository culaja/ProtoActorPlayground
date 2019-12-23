using Proto;

namespace ProtoActorAdapter.Logging
{
    internal static class PidExtensions
    {
        public static string Name(this PID pid) => pid != null
        ? pid.Id
        : "Unknown";
    }
}