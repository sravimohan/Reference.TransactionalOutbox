#!/bin/bash -x
set -e         # exit shell on failure
set -o nounset # undefined variable will cause an error

trap "docker compose kill --remove-orphans" EXIT

rm -rf ./coverage
docker compose build

# docker compose up --scale api=3 test-runner

docker compose run --rm test-runner
