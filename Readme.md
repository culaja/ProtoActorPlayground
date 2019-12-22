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