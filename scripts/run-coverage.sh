#!/usr/bin/env bash

# Per the following article:
#
# https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-code-coverage?tabs=linux
#
# Requires installation of the report generator:
#
# dotnet tool install -g dotnet-reportgenerator-globaltool

PROJECT_FOLDER=$( cd "$( dirname "$0" )/.." && pwd )
TEST_RESULTS_FOLDER_NAME="TestResults/"
TEST_RESULTS_FOLDER_PATH="$PROJECT_FOLDER/ShippingRecorder.Tests/$TEST_RESULTS_FOLDER_NAME"
REPORT_FOLDER="$TEST_RESULTS_FOLDER_PATH/report"

echo ""
echo "Project folder           : $PROJECT_FOLDER"
echo "Test results folder name : $TEST_RESULTS_FOLDER_NAME"
echo "Test results folder path : $TEST_RESULTS_FOLDER_PATH"
echo "Cobertura report folder  : $REPORT_FOLDER"
echo ""

dotnet test \
    "$PROJECT_FOLDER/ShippingRecorder.sln" \
    --settings "$PROJECT_FOLDER/ShippingRecorder.Tests/mstest.runsettings" \
    -p:UseSharedCompilation=false \
    -nr:false \
    /p:CollectCoverage=true \
    /p:CoverletOutput=$TEST_RESULTS_FOLDER_NAME \
    /p:CoverletOutputFormat=cobertura \
  /p:ExcludeByFile="**/System.Text.RegularExpressions.Generator/**/*.cs"

reportgenerator -reports:"$TEST_RESULTS_FOLDER_PATH/coverage.cobertura.xml" -targetdir:"$REPORT_FOLDER" -reporttypes:Html
open "$REPORT_FOLDER/index.html"