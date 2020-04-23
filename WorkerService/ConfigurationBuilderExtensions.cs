using Microsoft.Extensions.Configuration;

namespace WorkerService
{
    internal static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddJsonFileFrom(this IConfigurationBuilder builder, string[] args)
        {
            if (args.Length > 0)
            {
                builder.AddJsonFile(args[0], true, true);
            }

            return builder;
        }
    }
}