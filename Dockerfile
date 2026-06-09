FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/FCG.Payments.Domain/FCG.Payments.Domain.csproj", "src/FCG.Payments.Domain/"]
COPY ["src/FCG.Payments.Application/FCG.Payments.Application.csproj", "src/FCG.Payments.Application/"]
COPY ["src/FCG.Payments.Infrastructure/FCG.Payments.Infrastructure.csproj", "src/FCG.Payments.Infrastructure/"]
COPY ["src/FCG.Payments.Worker/FCG.Payments.Worker.csproj", "src/FCG.Payments.Worker/"]

RUN dotnet restore "src/FCG.Payments.Worker/FCG.Payments.Worker.csproj"

COPY src/ .
WORKDIR /src/FCG.Payments.Worker
RUN dotnet publish "FCG.Payments.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FCG.Payments.Worker.dll"]
