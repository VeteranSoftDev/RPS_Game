FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
# EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["src/RockPaperScissors.API.GameService/RockPaperScissors.API.GameService.csproj", "src/RockPaperScissors.API.GameService/"]
COPY ["src/RockPaperScissors.Core.Services/RockPaperScissors.Core.Services.csproj", "src/RockPaperScissors.Core.Services/"]
COPY ["src/RockPaperScissors.Data.Model/RockPaperScissors.Data.Model.csproj", "src/RockPaperScissors.Data.Model/"]
RUN dotnet restore "src/RockPaperScissors.API.GameService/RockPaperScissors.API.GameService.csproj"
COPY . .
WORKDIR "/src/src/RockPaperScissors.API.GameService"
RUN dotnet build "RockPaperScissors.API.GameService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RockPaperScissors.API.GameService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RockPaperScissors.API.GameService.dll"]