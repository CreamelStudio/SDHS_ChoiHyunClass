using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Vector2 inputVector;
    private float2 position = new float2();
    private EntityManager em;

    public int maxHp = 100;
    public int hp;

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    public void Start()
    {
        em = World.DefaultGameObjectInjectionWorld.EntityManager;
        em.CreateEntity(typeof(PlayerPosition));
        em.SetComponentData(PlayerPosition ) ;
    }

        void Update()
        {
            transform.Translate(inputVector * moveSpeed * Time.deltaTime, Space.World);
            
            position.x = inputVector.x;
            position.y = inputVector.y;
            em.SetComponentData(em, new PlayerPosition { Value = position });
        }
}
