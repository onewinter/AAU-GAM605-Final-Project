// source: https://ilhamhe.medium.com/sprite-flash-in-unity-b4b466f875d1

using System.Collections;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    public Color flashColor;
    public float flashDuration;

    private Material mat;

    private IEnumerator flashCoroutine;

    private void Awake()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    private void Start()
    {
        mat.SetColor("_FlashColor", flashColor);
    }

    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = DoFlash();
        StartCoroutine(flashCoroutine);
    }

    public void StopFlash()
    {
        if(flashCoroutine != null) StopCoroutine(flashCoroutine);
        SetFlashAmount(0);
    }

    private IEnumerator DoFlash()
    {
        float lerpTime = 0;

        while (lerpTime < flashDuration)
        {
            lerpTime += Time.deltaTime;
            var perc = lerpTime / flashDuration;

            SetFlashAmount(1f - perc);
            yield return null;
        }

        SetFlashAmount(0);
    }

    private void SetFlashAmount(float flashAmount)
    {
        mat.SetFloat("_FlashAmount", flashAmount);
    }
}