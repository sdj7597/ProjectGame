using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    GameObject target;

    public float distanceX, distanceY, distanceZ, speed;
    Vector3 pos;

    private bool isSetting = false;

    private void FixedUpdate()
    {
        if (!isSetting)
            return;

        pos = new Vector3(target.transform.position.x + distanceX, target.transform.position.y + distanceY, target.transform.position.z + distanceZ);
        transform.position = Vector3.Lerp(transform.position, pos, speed * Time.deltaTime);
    }

    public void SetTarget(GameObject targetObj)
    {
        target = targetObj;
        isSetting = true;
    }
}