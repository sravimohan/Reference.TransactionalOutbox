#!/bin/bash -x
set -e         # exit shell on failure
set -o nounset # undefined variable will cause an error
IS_COVERAGE_ENABLED=${IS_COVERAGE_ENABLED:-false}

if [ "$IS_COVERAGE_ENABLED" = false ]; then
    echo 'Running tests without code coverage'
    dotnet test --no-restore --no-build --verbosity normal
    exit 0
fi

echo 'Running tests with code coverage'
dotnet sonarscanner begin \
    /k:"Reference.TransactionalOutbox" \
    /d:sonar.host.url="http://host.docker.internal:9000" \
    /d:sonar.login="sqp_fc8dfb8ff47aa1d0608c51fb6056c72fafa91bb1" \
    /d:sonar.cs.vscoveragexml.reportsPaths=/coverage/coverage.xml

dotnet build --no-incremental
dotnet-coverage collect 'dotnet test --no-restore --no-build --verbosity normal' -f xml -o '/coverage/test.xml'
echo 'waiting for coverage data to be processed...' && sleep 5

dotnet-coverage merge -o /coverage/coverage.xml -f xml -r /coverage/*.xml
dotnet sonarscanner end /d:sonar.login="sqp_fc8dfb8ff47aa1d0608c51fb6056c72fafa91bb1"
