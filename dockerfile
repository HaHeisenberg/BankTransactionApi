FROM mcr.microsoft.com/dotnet/nightly/sdk:7.0 AS build
WORKDIR /source
COPY ./BankTransactionApi .
RUN dotnet restore "./BankTransactionApi.csproj" --disable-parallel
RUN dotnet publish "./BankTransactionApi.csproj" -c release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/nightly/sdk:7.0
WORKDIR /app
COPY --from=build /app ./

EXPOSE 4000
#EXPOSE 4001

ENTRYPOINT ["dotnet", "BankTransactionApi.dll"]