FROM mcr.microsoft.com/dotnet/core/runtime:3.1

RUN apt-get update
RUN apt-get -qq -y install libasound2-dev

COPY bin/Debug/netcoreapp3.1/publish/ App/

WORKDIR /App

ENTRYPOINT ["dotnet", "/App/NtFreX.Audio.Console.dll"]