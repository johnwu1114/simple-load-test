#!/bin/bash

docker stop simple-load-test && docker rm simple-load-test
docker run --name simple-load-test -d -p 27017:27017 mongo
dotnet test
docker stop simple-load-test && docker rm simple-load-test
