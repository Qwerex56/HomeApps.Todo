# The Todo plugin's image. The HomeApps kit comes from the local feed (HomeApps' scripts/pack.sh), passed in as an
# extra build context named "homeapps"; compose.yaml does that, or by hand:
#   docker build --build-context homeapps=$HOME/.homeapps/packages -t homeapps/todo:1.0.0 .
# The UI is not built here: its build in src/Todo.Api/wwwroot/ui is committed (npm run build in src/Todo.Web).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY --from=homeapps . /packages/
COPY .editorconfig global.json Directory.Build.props Directory.Packages.props nuget.config ./
COPY src/ src/
# nuget.config reads the feed from %HOME%/.homeapps/packages; inside the image it is /packages.
RUN sed -i 's#%HOME%/.homeapps/packages#/packages#' nuget.config \
 && dotnet publish src/Todo.Api/Todo.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
USER $APP_UID
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Todo.Api.dll"]
