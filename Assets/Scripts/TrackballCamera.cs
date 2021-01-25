// Copyright 2020-2021 Andreas Atteneder
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using UnityEngine;

public class TrackballCamera : MonoBehaviour
{
 
	public float yawSensitivity = 1;
	public float pitchSensitivity = 1;
    public float scrollSensitivity = 1;
    
    public Transform target;
    public float distance = 2f;
    float yaw = 25;
    float pitch = -30;


    private Vector3? lastMousePosition;
    // Use this for initialization
    void Start ()
    {
    }
 
    // Update is called once per frame
    void LateUpdate ()
    {
        var mousePosn = Input.mousePosition;
 
        var mouseBtn = Input.GetMouseButton (0);

        var scroll = Input.GetAxis("Mouse ScrollWheel");
        distance = Mathf.Clamp(distance - scroll*scrollSensitivity,.1f,50);
        if (mouseBtn) {
            var pos = Input.mousePosition;
            if(lastMousePosition.HasValue) {
                yaw = (yaw + (pos.x-lastMousePosition.Value.x)*yawSensitivity) % 360;
                pitch = Mathf.Clamp(pitch+(pos.y-lastMousePosition.Value.y)*pitchSensitivity,-80,80);
            }
            lastMousePosition = pos;
        } else {
            lastMousePosition = null;
        }
        transform.rotation = Quaternion.AngleAxis(yaw,Vector3.up) * Quaternion.AngleAxis(pitch,Vector3.left);
        var dir = transform.forward;
        transform.position = (target==null? Vector3.zero:target.position) - dir*distance;
    }
}