#!/bin/bash

set -e
run_cmd="dotnet run --server.urls http://*:80 --project Conduit"

until dotnet ef database update --project Conduit; do
>&2 echo "SQL Server is starting up"
sleep 1
done

>&2 echo "SQL Server is up - executing command"
exec $run_cmd
