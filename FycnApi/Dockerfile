FROM microsoft/aspnetcore:2.0
ARG source
WORKDIR /app
EXPOSE 8088
COPY ${source:-obj/Docker/publish} .
ENTRYPOINT ["dotnet", "FycnApi.dll"]
