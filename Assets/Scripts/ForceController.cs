using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceController : MonoBehaviour
{
    public float xRotation;//垂直后坐力
    public float xRotationSpeed;//垂直后坐力增加速度
    public float xRecoverySpeed;//垂直后坐力恢复速度
    private float currentXRotation;//当前垂直后坐力
    private float targetXRotation;//目标垂直后坐力


    private void Update()
    {
        //枪口回正
        targetXRotation = Mathf.Lerp(targetXRotation, 0, xRecoverySpeed * Time.deltaTime);

        //应用后坐力
        currentXRotation = Mathf.Lerp(currentXRotation, targetXRotation, xRotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(currentXRotation,transform.localEulerAngles.y, 0);
    }

    //开火时调用，增加后坐力
    public void Fire()
    {
       targetXRotation += xRotation;
    }
}
