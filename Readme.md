# Event Store setup
To setup in-memory event store run it using following command (linux or mac):
```bash
sudo eventstore --run-projections=all --start-standard-projections=true --mem-db=true
```
... or windows:
```bash
EventStore.ClusterNode.exe --run-projections=all --start-standard-projections=true --mem-db=true
```
EventStore Web Client:
```bash
http://localhost:2113/web/index.html#/
User name: admin
Password: changeit
```

# Event Store all domain events projection
In order to create single stream for all domain events a projection
needs to be created. Use the following code to create a continuous
projection in Event Store:
```bash
fromAll().
   when({
        $any : function(s,e) {
            if (e.streamId.startsWith('Domain_'))
                linkTo('AllDomainEvents', e);
   }});
```
Note: Replace Domain with prefix of all of your streams. (e.g. if 
you have book library domain you can have `BookLibrary_` prefix for 
all domain events.