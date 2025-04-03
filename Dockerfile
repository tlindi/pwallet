# Step 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project files
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application files
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Step 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Install required system libraries for SkiaSharp
RUN apt-get update && apt-get install -y --no-install-recommends \
    libfontconfig1 \
    libfreetype6 \
    libpng16-16 \
    libjpeg62-turbo \
    libglib2.0-0 \
    libx11-6 \
    libxext6 \
    libxrender1 && \
    rm -rf /var/lib/apt/lists/*

# Copy published files from the build stage
COPY --from=build /app/out .

# Expose the port your application will run on
EXPOSE 4949

# Set the entry point to start the application
ENTRYPOINT ["dotnet", "SimpLN.dll"]
