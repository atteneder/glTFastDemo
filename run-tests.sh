#!/bin/sh

set -e

PWD=$(pwd)

UNITY_2019=/Applications/Unity/Hub/Editor/2019.4.36f1
UNITY_2020=/Applications/Unity/Hub/Editor/2020.3.29f1
UNITY_2021=/Applications/Unity/Hub/Editor/2021.2.12f1

UNITY_2019_EXE="$UNITY_2019/Unity.app/Contents/MacOS/Unity"
UNITY_2020_EXE="$UNITY_2020/Unity.app/Contents/MacOS/Unity"
UNITY_2021_EXE="$UNITY_2021/Unity.app/Contents/MacOS/Unity"

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

PROJECT="/Users/aa/u/glTFastDemo/projects/glTF-demo-2019.4"
echo "2019 LTS BiRP EditMode"
time $UNITY_2019_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4 -testResults "$PWD/test-results/glTF-demo-2019.4-editor.xml" -testPlatform EditMode
#UnifiedTestRunner --suite=editor  --testproject="$PROJECT" --editor-location="$UNITY_2019"
echo "2019 LTS BiRP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2019_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4 -testResults "$PWD/test-results/glTF-demo-2019.4-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"
#UnifiedTestRunner --suite=playmode --testlist="testlist.txt" --testproject="$PROJECT" --editor-location="$UNITY_2019" --platform="$PLAYMODE_PLATFORM" --a="$PWD/test-results/glTF-demo-2019.4-runtime"

echo "2019 LTS URP EditMode"
time $UNITY_2019_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4-urp -testResults "$PWD/test-results/glTF-demo-2019.4-urp-editor.xml" -testPlatform EditMode
echo "2019 LTS URP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2019_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4-urp -testResults "$PWD/test-results/glTF-demo-2019.4-urp-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

echo "2021.2 BiRP EditMode"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2 -testResults "$PWD/test-results/glTF-demo-2021.2-editor.xml" -testPlatform EditMode
echo "2021.2 BiRP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2 -testResults "$PWD/test-results/glTF-demo-2021.2-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance"

echo "2021.2 HDRP EditMode"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-hdrp -testResults "$PWD/test-results/glTF-demo-2021.2-hdrp-editor.xml" -testPlatform EditMode
echo "2021.2 HDRP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-hdrp -testResults "$PWD/test-results/glTF-demo-2021.2-hdrp-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance"

echo "2021.2 URP EditMode"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-urp -testResults "$PWD/test-results/glTF-demo-2021.2-urp-editor.xml" -testPlatform EditMode
echo "2021.2 URP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_2021_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-urp -testResults "$PWD/test-results/glTF-demo-2021.2-urp-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance"

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
-debugCodeOptimization \
-burst-disable-compilation \
-enableCodeCoverage \
-testPlatform EditMode \
-testResults "$PWD/test-results/glTF-demo-code-coverage-editmode.xml" \
-coverageResultsPath "$PWD/test-results/CodeCoverage" \
-coverageHistoryPath "$PWD/test-results/CodeCoverage" \
-coverageOptions "generateAdditionalMetrics;assemblyFilters:+glTFast,+glTFast.*,+glTFastSchema,+glTFastFakeSchema,+glTFastEditor" \
-testCategory "!Performance" \
-runTests

echo "Code Coverage PlayMode"
time $UNITY_2021_EXE \
-projectPath ./projects/glTF-demo-2021.2 \
-batchmode \
-debugCodeOptimization \
-burst-disable-compilation \
-enableCodeCoverage \
-testPlatform PlayMode \
-testResults "$PWD/test-results/glTF-demo-code-coverage-playmode.xml" \
-coverageResultsPath "$PWD/test-results/CodeCoverage" \
-coverageHistoryPath "$PWD/test-results/CodeCoverage" \
-coverageOptions "generateAdditionalMetrics;assemblyFilters:+glTFast,+glTFast.*,+glTFastSchema,+glTFastFakeSchema,+glTFastEditor" \
-testCategory "!Performance" \
-runTests

echo "Code Coverage HTML Report"
$UNITY_2021_EXE \
-projectPath ./projects/glTF-demo-2021.2 \
-batchmode \
-debugCodeOptimization \
-burst-disable-compilation \
-enableCodeCoverage \
-testPlatform PlayMode \
-testResults "$PWD/test-results/glTF-demo-coverage.xml" \
-coverageResultsPath "$PWD/test-results/CodeCoverage" \
-coverageHistoryPath "$PWD/test-results/CodeCoverage" \
-coverageOptions "generateHtmlReport;generateHtmlReportHistory;generateBadgeReport;assemblyFilters:+glTFast,+glTFast.*,+glTFastSchema,+glTFastFakeSchema,+glTFastEditor" \
-testCategory "!Performance" \
-runTests

cp "test-results/CodeCoverage/Report/badge_linecoverage.svg" "packages/glTFast/Documentation~/Images/badge_linecoverage.svg"

#
# Create builds
#

echo "glTF-demo-minsize WebGL Build"
time $UNITY_2021_EXE -batchmode -quit -projectPath ./projects/glTF-demo-minsize -executeMethod GltfDemo.Editor.BuildScripts.BuildWebPlayer -buildPath "$PWD/builds/WebGL"

echo "glTF-demo-min-feature macOS Build"
time $UNITY_2019_EXE -batchmode -quit -projectPath ./projects/glTF-demo-min-feature -buildOSXUniversalPlayer "$PWD/builds/macOS.app"
