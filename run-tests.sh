#!/bin/sh

# TODO: Bring back export tests, at least for 2020.3 and newer

set -e

PWD=$(pwd)

PLAYMODE_PLATFORM=StandaloneOSX

UNITY_EXE=/Applications/Unity/Hub/Editor/2019.4.32f1/Unity.app/Contents/MacOS/Unity

echo "2019 LTS BiRP EditMode"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4 -testResults "$PWD/test-results/glTF-demo-2019.4-editor.xml" -testPlatform EditMode
echo "2019 LTS BiRP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4 -testResults "$PWD/test-results/glTF-demo-2019.4-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

echo "2019 LTS URP EditMode"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4-urp -testResults "$PWD/test-results/glTF-demo-2019.4-urp-editor.xml" -testPlatform EditMode
echo "2019 LTS URP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4-urp -testResults "$PWD/test-results/glTF-demo-2019.4-urp-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

UNITY_EXE=/Applications/Unity/Hub/Editor/2021.2.0f1/Unity.app/Contents/MacOS/Unity

echo "2021.2 BiRP EditMode"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2 -testResults "$PWD/test-results/glTF-demo-2021.2-editor.xml" -testPlatform EditMode
echo "2021.2 BiRP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2 -testResults "$PWD/test-results/glTF-demo-2021.2-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

echo "2021.2 HDRP EditMode"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-hdrp -testResults "$PWD/test-results/glTF-demo-2021.2-hdrp-editor.xml" -testPlatform EditMode
echo "2021.2 HDRP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-hdrp -testResults "$PWD/test-results/glTF-demo-2021.2-hdrp-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"

echo "2021.2 URP EditMode"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-urp -testResults "$PWD/test-results/glTF-demo-2021.2-urp-editor.xml" -testPlatform EditMode
echo "2021.2 URP PlayMode $PLAYMODE_PLATFORM"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2021.2-urp -testResults "$PWD/test-results/glTF-demo-2021.2-urp-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"


UNITY_EXE=/Applications/Unity/Hub/Editor/2020.3.21f1/Unity.app/Contents/MacOS/Unity

echo "DOTS EditMode"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-dots -testResults "$PWD/test-results/glTF-demo-dots-editor.xml" -testPlatform EditMode
echo "DOTS PlayMode $PLAYMODE_PLATFORM"
time $UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-dots -testResults "$PWD/test-results/glTF-demo-dots-runtime.xml" -testPlatform "$PLAYMODE_PLATFORM" -testCategory "!Performance;!Export"
