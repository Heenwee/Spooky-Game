using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{
    public IEnumerator Shake(float duration, float magnitude)
    {
        transform.localPosition = new Vector3(0, 0, -10);
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            if (Time.timeScale != 0)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = new Vector3(x, originalPos.y + y, originalPos.z);
                elapsed += Time.deltaTime;
            }
            else elapsed = duration;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
