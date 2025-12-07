#!/usr/bin/env bash

if [[ $# -ne 1 ]]; then
    echo Usage: $(basename "$0") /path/to/database
    exit 1
fi

PROJECT_FOLDER=$( cd "$( dirname "$0" )/.." && pwd )
DATA_FOLDER="$PROJECT_FOLDER/data"

echo "Project folder     : $PROJECT_FOLDER"
echo "Data folder        : $DATA_FOLDER"
echo "Database file path : $1"

# If the database file exists, delete it
if [ -f "$1" ]; then
    rm -f "$1"
fi

# Use the management tool to create the database with the latest migrations
# then import the reference data held in the "data" folder
cd "$PROJECT_FOLDER/ShippingRecorder.Manager"
dotnet run -- --update
dotnet run -- --import-operators "$DATA_FOLDER/operators.csv"
dotnet run -- --import-countries "$DATA_FOLDER/countries.csv"
dotnet run -- --import-ports "$DATA_FOLDER/ports.csv"
