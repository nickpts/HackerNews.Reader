FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY HackerNews.Reader/HackerNews.Reader.csproj HackerNews.Reader/
RUN dotnet restore HackerNews.Reader/HackerNews.Reader.csproj
COPY . .
WORKDIR /src/HackerNews.Reader
RUN dotnet build HackerNews.Reader.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish HackerNews.Reader.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "HackerNews.Reader.dll"]
