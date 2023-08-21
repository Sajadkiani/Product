FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY "Product.sln" "Product.sln"
COPY ["Product.Api/Product.Api.csproj", "Product.Api/Product.Api.csproj"]
COPY ["Product.Domain/Product.Domain.csproj", "Product.Domain/Product.Domain.csproj"]
COPY ["Product.Infrastructure/Product.Infrastructure.csproj", "Product.Infrastructure/Product.Infrastructure.csproj"]
COPY ["BuildingBlocks/EventBus/EventBus.csproj", "BuildingBlocks/EventBus/EventBus.csproj"]
COPY ["BuildingBlocks/IntegrationEventLogEF/IntegrationEventLogEF.csproj", "BuildingBlocks/IntegrationEventLogEF/IntegrationEventLogEF.csproj"]
RUN dotnet restore Product.Api

WORKDIR /app
COPY . .

RUN dotnet build "Product.Api/Product.Api.csproj" -c Release -o /build

FROM build AS publish
RUN dotnet publish "Product.Api/Product.Api.csproj" -c Release -o /publish

FROM base AS final
WORKDIR /app/publish
COPY --from=publish /publish .
ENTRYPOINT ["dotnet", "Product.Api.dll"]

