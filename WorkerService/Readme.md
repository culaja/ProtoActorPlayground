# Service configuration
All service configuration can be found and changed in `appsettings.json`
file or one of it's overrides (`appsettings.Development.json` or 
`appsettings.Production.json`). Note that all configuration changes on
hot service are applied after service restart.

## Application configuration
- `EventStoreConnectionString` - Event Store server instance that will be
used as a source for domain events that need to be applied. The same
Event Store server instance is used to save snapshot of the last known
applied domain event.
- `SourceStreamName` - Name of the stream in the Event Store where all 
domain events are kept.
- `Snapshot` - Snapshot of the last known applied domain event is also
kept in the Event Store. *Name* of the snapshot can be set in this
configuration as well as *PeriodMs* that dictates period of storing of
the new snapshot in the Event Store.
- `DestinationServiceUri` - target URL where all domain events will be
passed. Note that this end point needs to follow the same contract as
in [DestinationServiceSampleCall.http](DestinationServiceSampleCall.http). 
Domain event will be considered as applied if the target
service returns HTTP 200 status code. All other status codes will result
in continuous retries.

## Serilog configuration
Serilog is used as a logging library with enabled console and rolling
file sinks. The following configuration can be changed:
- Minimum level of logging. Supported levels are: `Verbose`, 
`Debug`, `Information`, `Warning`, `Error` and `Fatal`.
- Console sink configuration. (It can be removed if console logging is 
not needed)
- Rolling file configuration that includes format of created log files,
rolling interval, etc.
- Enrich configuration like thread ID or machine name.
- Properties configuration like application name. (This is useful if in
the future Seq server is used as a sink to distinguish logs between 
different applications.

# Installing and running as Windows service
In order to install the application as a Windows service run the
following command in `cmd`:
```bash
sc create EventStoreMessageDispatcher binPath="<dir_path>\EventStoreMessageDispatcher.exe"
```
After Windows service is installed it can be run using the following
command:
```bash
sc start EventStoreMessageDispatcher
```

# Running the application as console application
Regardless if the application is already installed as a Windows service
you can run a separate instance though `cmd`:
```bash
C:\dir_path>EventStoreMessageDispatcher.exe
```