#!/bin/sh

set -e

PWD=$(pwd)

UNITY_EXE=/Applications/Unity/Hub/Editor/2019.4.32f1/Unity.app/Contents/MacOS/Unity

$UNITY_EXE -runTests -batchmode -projectPath ./projects/glTF-demo-2019.4 -testResults "$PWD/test-results/glTF-demo-2019.4.xml" -testPlatform StandaloneOSX -testCategory "!Performance;!Export"
