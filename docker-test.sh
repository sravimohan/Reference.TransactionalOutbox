#!/bin/bash -x
set -e         # exit shell on failure
set -o nounset # undefined variable will cause an error

trap "docker compose kill --remove-orphans" EXIT

rm -rf ./coverage
docker compose build

docker compose up proxy --scale api=3

# docker compose up \
#     --exit-code-from test-runner \
#     --abort-on-container-exit \
#     --scale api=1
