using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamShake : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    float startingIntesity;
    float shakeTimer;
    float shakeTimerTotal;

    public static CamShake instance;

    private void Awake()
    {
        instance = this;
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    public void ShakeCam(float intensity, float duration)
    {
        CinemachineBasicMultiChannelPerlin perlin =
            cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        perlin.ReSeed();

        perlin.m_AmplitudeGain = intensity;

        if (intensity > startingIntesity) startingIntesity = intensity;
        shakeTimer = duration;
        shakeTimerTotal = duration;
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin perlin =
                cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            perlin.m_AmplitudeGain =
                Mathf.Lerp(startingIntesity, 0f, 1 - (shakeTimer / shakeTimerTotal));
        }
        else startingIntesity = 0;
    }
}
