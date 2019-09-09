#!/bin/bash

docker-compose -p simple-load-test -f ./build/docker-compose.test.yml up --build --force-recreate --abort-on-container-exit
docker-compose -p simple-load-test -f ./build/docker-compose.test.yml down
