# Ubuntu Docker file
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-bionic AS build-env
WORKDIR /app

# switch to root
USER 0
RUN mkdir obj

# Copy csproj and restore as distinct layers
COPY . ./

RUN dotnet restore MicroServiceTemplate.sln

# run unit tets - build will fail if tests fail
#RUN dotnet test Tests

# build
RUN dotnet publish MicroServiceTemplate.csproj -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic AS runtime
EXPOSE 8080
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "MicroServiceTemplate.dll"]