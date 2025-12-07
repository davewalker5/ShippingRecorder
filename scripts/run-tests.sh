#!/usr/bin/env bash

PROJECT_FOLDER=$( cd "$( dirname "$0" )/.." && pwd )

# Run the tests
dotnet test \
    --settings "$PROJECT_FOLDER/ShippingRecorder.Tests/mstest.runsettings" \
    -p:UseSharedCompilation=false \
    -nr:false 
