FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build

ADD "Tweezers.Core" "/opt/tweezers.core/"

WORKDIR "/opt/tweezers.core"

RUN ["dotnet", "build", "-o", "/opt/tweezers.core/docker/publish", "-c", "Release"]
RUN ["dotnet", "publish", "-o", "/opt/tweezers.core/docker/publish", "-c", "Release", "-r", "linux-x64"]



FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

COPY --from=build "/opt/tweezers.core/docker/publish" "/opt/tweezers.core"

RUN ["ls", "/opt/tweezers.core"]

WORKDIR "/opt/tweezers.core"

ENTRYPOINT ["dotnet", "Tweezers.dll"]