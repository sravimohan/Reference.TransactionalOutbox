dotnet-coverage collect --session-id test-api --background --server-mode --output-format xml
dotnet-coverage connect --background test-api dotnet Reference.TransactionalOutbox.Api.dll

# wait for application to start in background
sleep 5

while :; do
    dotnet-coverage snapshot test-api -o '/coverage/api.xml'
    sleep 5
done

dotnet-coverage shutdown test-api
