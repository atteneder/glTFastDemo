# glTFast Demo

Unity project that demonstrates how to use the [glTFast package](https://github.com/atteneder/glTFast).

## WebGL Build Optimization

To make sure the build size remains low, certify the following:

- Activate "Strip Engine Code" in *Player Settings*
- Don't install any not strictly required packages (built-in or otherwise)
  - Physics is a good example (adds 2-3 MB wasm)

### For Release Builds

#### Disable Testing

Remove the following block from `Packages/manifest.json`

```json
  "testables": [
    "com.atteneder.gltfast"
  ],
```

Then remove the package *Test Framework* (com.unity.test-framework) and anything that depends on it.

> To remove compiler errors, restarting the Editor may be required

#### Run Brotli Compression

The build is configured to have no compression for faster iterations. 

## License

Copyright (c) 2020 Andreas Atteneder, All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use files in this repository except in compliance with the License.
You may obtain a copy of the License at

   <http://www.apache.org/licenses/LICENSE-2.0>

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
