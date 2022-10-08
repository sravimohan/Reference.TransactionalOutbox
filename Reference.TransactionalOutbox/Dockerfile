#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Reference.TransactionalOutbox/Reference.TransactionalOutbox.csproj", "Reference.TransactionalOutbox/"]
RUN dotnet restore "Reference.TransactionalOutbox/Reference.TransactionalOutbox.csproj"
COPY . .
WORKDIR "/src/Reference.TransactionalOutbox"
RUN dotnet build "Reference.TransactionalOutbox.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reference.TransactionalOutbox.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Reference.TransactionalOutbox.dll"]