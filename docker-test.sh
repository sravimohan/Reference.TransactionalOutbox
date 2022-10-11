#!/bin/bash -x
set -e         # exit shell on failure
set -o nounset # undefined variable will cause an error

trap "docker compose kill --remove-orphans" EXIT

SCALE=$1

rm -rf ./coverage
docker compose build

# run tests
docker compose up \
    --exit-code-from test-runner \
    --abort-on-container-exit \
    --scale api=${SCALE} \
    test-runner

# docker compose up \
#     --scale api=${SCALE} \
#     proxy

# docker compose up db
