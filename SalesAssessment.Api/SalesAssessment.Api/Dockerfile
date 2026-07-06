FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["SalesAssessment.Api/SalesAssessment.Api.csproj", "SalesAssessment.Api/"]
RUN dotnet restore "SalesAssessment.Api/SalesAssessment.Api.csproj"
COPY . .
WORKDIR "/src/SalesAssessment.Api"
RUN dotnet build "SalesAssessment.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SalesAssessment.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SalesAssessment.Api.dll"]
