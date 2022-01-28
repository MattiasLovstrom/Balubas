FROM dtr-acc.sebank.se/operationslinux/dotnet-core-sdk:3.1.100-alpine3.10 AS build-env
WORKDIR /app

COPY . ./
run echo ml
run ls /
run ls /app
run echo ml
ENV no_proxy=*.sebank.se

RUN dotnet restore Balubas.sln --configfile NuGet.config
RUN dotnet publish Balubas -c Release -o /
run ls /
EXPOSE 1050
ENTRYPOINT ["dotnet" , "/Balubas.dll",  "server"]