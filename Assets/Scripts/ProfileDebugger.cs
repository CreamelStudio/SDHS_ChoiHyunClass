using TMPro;
using UnityEngine;
using Unity.Entities;

public class ProfileDebugger : MonoBehaviour
{
    public TMP_Text profileText;

    private float entityCountTimer = 0f;
    private int cachedEntityCount = 0;

    private void Update()
    {
        // FPS는 매 프레임 계산
        float fps = 1f / Time.unscaledDeltaTime;

        // Entity Count는 0.5초마다만 갱신
        entityCountTimer -= Time.unscaledDeltaTime;
        if (entityCountTimer <= 0f)
        {
            entityCountTimer = 0.5f;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world != null && world.IsCreated)
            {
                cachedEntityCount = world.EntityManager.UniversalQuery.CalculateEntityCount();
            }
            else
            {
                cachedEntityCount = 0;
            }
        }

        profileText.text =
            $"FPS : {fps:F1}\n" +
            $"Entity Count : {cachedEntityCount}";
    }
}