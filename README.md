# Reference.TransactionalOutbox

Reference implemetnation of the transactional outbox pattern.

## Tech Stack
- Dotnet 6
- AWS SNS/SQS 
- Sql Server

## Implementation Notes
- Keep dependencies minimal
- Code organized in Usecase pattern
- Sample does not have business domain to require unit tests. But should be added it if domain logic is introduced.
- Integration tests focus on the database and messaging infrastructure.
- Component tests focus on the ablity of the service being able to run multiple instances in parellel for scalability.

## Testing
- Tests are run using docker compose
- Docker compose includes
    - Sql Server
    - LocalStack (SQS/SNS)
    - Api (publishing the messages)
    - ngnix proxy ( for testing the api in scaled mode )
    
## Run Tests
    sh docker-test.sh <number of instances>

    example,
    sh docker-test.sh 3



