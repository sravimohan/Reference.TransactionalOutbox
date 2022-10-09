global using Dapper;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Reference.TransactionalOutbox.Application.Services;
global using Reference.TransactionalOutbox.Application.Usecase.CreateOrder;
global using Reference.TransactionalOutbox.Application.Usecase.HealthChecks;
global using Reference.TransactionalOutbox.Application.Usecase.PublishOrderCreated;
global using System.Data;
global using System.Data.SqlClient;
global using System.Text.Json;
global using System.Transactions;
