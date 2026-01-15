#!/bin/bash
set -e

echo "Running migrations..."
dotnet ef database update

echo "Starting app..."
exec dotnet BlogApp.dll
