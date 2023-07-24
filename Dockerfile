# Stage 1: Build the .NET application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the .csproj and .sln files to the container's working directory
COPY . .

# Restore the NuGet packages from the solution file
RUN dotnet restore "ApacBreachersRanked/ApacBreachersRanked/ApacBreachersRanked.csproj"

# Copy the rest of the application code to the container's working directory
COPY . .

# Build the application
RUN dotnet build "ApacBreachersRanked/ApacBreachersRanked/ApacBreachersRanked.csproj" -c Release -o /app/build

# Stage 2: Create a smaller runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime

# Set the working directory inside the container
WORKDIR /app

# Copy the build output from the build stage
COPY --from=build /app/build .

# Command to start the application
ENTRYPOINT ["dotnet", "ApacBreachersRanked.dll"]