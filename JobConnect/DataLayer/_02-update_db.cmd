dotnet tool install --global dotnet-ef --version 5.0.10
dotnet tool update --global dotnet-ef --version 5.0.10
dotnet build
dotnet ef --startup-project ../Web/ database update
pause