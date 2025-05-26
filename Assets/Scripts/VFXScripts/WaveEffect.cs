using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class WaveEffect : MonoBehaviour
{
    [SerializeField] private GameObject waveEffect;
    [SerializeField] private string waveEffectName = "_STime";
    [SerializeField] private float waveEffectDuration = 0.25f;
    [SerializeField] private float waveEffectPositionZ = -11;
    async Awaitable AddEffect(Vector3 Position)
    {
        if (waveEffect == null || waveEffectDuration == default || waveEffectName == "")
        {
            return;
        }
        GameObject cloned = Instantiate(waveEffect);
        cloned.transform.position = Position;
        Material material = cloned.GetComponent<SpriteRenderer>().material;
        float stimer = 0;
        float multiply = 1 / waveEffectDuration;
        while (stimer < waveEffectDuration)
        {
            float dt = Time.deltaTime;
            stimer += dt;
            material.SetFloat(waveEffectName, stimer*multiply);
            await Awaitable.WaitForSecondsAsync(dt);
        }
        Destroy(cloned);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 POSITION = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            POSITION.z = waveEffectPositionZ;
            _ = AddEffect(POSITION);
        }
    }
}
