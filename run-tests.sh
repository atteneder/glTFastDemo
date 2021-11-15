#!/bin/sh

# TODO: Bring back export tests, at least for 2020.3 and newer

set -e

PWD=$(pwd)

UNITY_2019_EXE=/Applications/Unity/Hub/Editor/2019.4.32f1/Unity.app/Contents/MacOS/Unity
UNITY_2020_EXE=/Applications/Unity/Hub/Editor/2020.3.22f1/Unity.app/Contents/MacOS/Unity
UNITY_2021_EXE=/Applications/Unity/Hub/Editor/2021.2.2f1/Unity.app/Contents/MacOS/Unity

if [ ! -f "$UNITY_2019_EXE" ]; then
    echo "Unity not found at $UNITY_2019_EXE"
    exit -1
fi

if [ ! -f "$UNITY_2020_EXE" ]; then
    echo "Unity not found at $UNITY_2020_EXE"
    exit -1
fi

if [ ! -f "$UNITY_2021_EXE" ]; then
    echo "Unity not found at $UNITY_2021_EXE"
    exit -1
fi


#
# Run Unit Tests
#

PLAYMODE_PLATFORM=StandaloneOSX

echo "2019 LTS BiRP EditMode"
time $UNITY_2019_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4 -testResults "$PWD/test-results/glTF-demo-2019.4-editor.xml" -testPlatform EditMode
echo "2019 LTS BiRP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2019_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4 -testResults "$PWD/test-results/glTF-demo-2019.4-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

echo "2019 LTS URP EditMode"
time $UNITY_2019_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4-urp -testResults "$PWD/test-results/glTF-demo-2019.4-urp-editor.xml" -testPlatform EditMode
echo "2019 LTS URP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2019_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4-urp -testResults "$PWD/test-results/glTF-demo-2019.4-urp-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

echo "2021.2 BiRP EditMode"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2 -testResults "$PWD/test-results/glTF-demo-2021.2-editor.xml" -testPlatform EditMode
echo "2021.2 BiRP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2 -testResults "$PWD/test-results/glTF-demo-2021.2-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

echo "2021.2 HDRP EditMode"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-hdrp -testResults "$PWD/test-results/glTF-demo-2021.2-hdrp-editor.xml" -testPlatform EditMode
echo "2021.2 HDRP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-hdrp -testResults "$PWD/test-results/glTF-demo-2021.2-hdrp-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

echo "2021.2 URP EditMode"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-urp -testResults "$PWD/test-results/glTF-demo-2021.2-urp-editor.xml" -testPlatform EditMode
echo "2021.2 URP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-urp -testResults "$PWD/test-results/glTF-demo-2021.2-urp-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

echo "DOTS EditMode"
time $UNITY_2020_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-dots -testResults "$PWD/test-results/glTF-demo-dots-editor.xml" -testPlatform EditMode
echo "DOTS PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2020_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-dots -testResults "$PWD/test-results/glTF-demo-dots-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

#
# Code Coverage
#

echo "Code Coverage EditMode"
time $UNITY_2021_EXE \
-projectPath ./projects/glTF-demo-2021.2 \
-batchmode \
-testPlatform EditMode \
-runTests \
-testResults "$PWD/test-results/glTF-demo-code-coverage-editmode.xml" \
-debugCodeOptimization \
-burst-disable-compilation \
-enableCodeCoverage \
-coverageResultsPath "$PWD/test-results/CodeCoverage" \
-coverageHistoryPath "$PWD/test-results/CodeCoverage" \
-coverageOptions "generateAdditionalMetrics;assemblyFilters:+glTFast,+glTFast.*,+glTFastSchema,+glTFastFakeSchema,+glTFastEditor" \
-testCategory "!Performance"

echo "Code Coverage PlayMode"
time $UNITY_2021_EXE \
-projectPath ./projects/glTF-demo-2021.2 \
-batchmode \
-testPlatform PlayMode \
-runTests \
-testResults "$PWD/test-results/glTF-demo-code-coverage-playmode.xml" \
-debugCodeOptimization \
-burst-disable-compilation \
-enableCodeCoverage \
-coverageResultsPath "$PWD/test-results/CodeCoverage" \
-coverageHistoryPath "$PWD/test-results/CodeCoverage" \
-coverageOptions "generateAdditionalMetrics;assemblyFilters:+glTFast,+glTFast.*,+glTFastSchema,+glTFastFakeSchema,+glTFastEditor" \
-testCategory "!Performance"

echo "Code Coverage HTML Report"
$UNITY_2021_EXE \
-projectPath ./projects/glTF-demo-2021.2 \
-batchmode \
-testPlatform PlayMode \
-runTests \
-testResults "$PWD/test-results/glTF-demo-coverage.xml" \
-debugCodeOptimization \
-burst-disable-compilation \
-enableCodeCoverage \
-coverageResultsPath "$PWD/test-results/CodeCoverage" \
-coverageHistoryPath "$PWD/test-results/CodeCoverage" \
-coverageOptions "generateHtmlReport;generateHtmlReportHistory;generateBadgeReport;assemblyFilters:+glTFast,+glTFast.*,+glTFastSchema,+glTFastFakeSchema,+glTFastEditor" \
-testCategory "!Performance"

#
# Create builds
#

echo "glTF-demo-minsize WebGL Build"
time $UNITY_2021_EXE -batchmode -quit -projectPath ./projects/glTF-demo-minsize -executeMethod GltfDemo.Editor.BuildScripts.BuildWebPlayer -buildPath "$PWD/builds/WebGL"

echo "glTF-demo-min-feature macOS Build"
time $UNITY_2019_EXE -batchmode -quit -projectPath ./projects/glTF-demo-min-feature -buildOSXUniversalPlayer "$PWD/builds/macOS.app"
