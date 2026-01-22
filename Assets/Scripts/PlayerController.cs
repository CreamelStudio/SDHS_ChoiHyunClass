using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    EntityManager em;
    Entity inputEntity;
    float2 inputMove;

    void Awake()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;

        var query = em.CreateEntityQuery(typeof(PlayerInput));
        if (query.IsEmpty)
        {
            inputEntity = em.CreateEntity(typeof(PlayerInput));
            em.SetComponentData(inputEntity, new PlayerInput { Move = float2.zero });
        }
        else
        {
            inputEntity = query.GetSingletonEntity();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        var v = context.ReadValue<Vector2>();
        inputMove = new float2(v.x, v.y);
    }

    void Update()
    {
        if (em.Exists(inputEntity))
        {
            em.SetComponentData(inputEntity, new PlayerInput { Move = inputMove });
        }
    }
}