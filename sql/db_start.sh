echo "waiting for db to start..."
sleep 10

/opt/mssql-tools/bin/sqlcmd -S . -U sa -P ${SA_PASSWORD} -d master -i /usr/src/sql/db_setup.sql
echo "setup completed"
