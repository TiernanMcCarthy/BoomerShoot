using BoomerShoot.Character.Components;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


/// <summary>
/// All Characters inherit and work from this class
/// Composition of extra functionality
/// </summary>
[RequireComponent(typeof(Health)), RequireComponent(typeof(MovementProvider)),RequireComponent(typeof(InputProvider)),RequireComponent(typeof(InteractionHandler))]
public class BaseCharacter : MonoBehaviour
{
    public bool ExecuteComponents { get; private set; }
    public Health health { get; private set; }
    public MovementProvider movementProvider { get; private set; }
    
    public InputProvider inputProvider { get; private set; }

    public InteractionHandler interactionHandler { get; private set; }

    public UnityEvent onDeath;

    
    private void Awake()
    {
        health=GetComponent<Health>();
        movementProvider=GetComponent<MovementProvider>();
        inputProvider=GetComponent<InputProvider>();

    }

    private void HasDied()
    {
        onDeath.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        inputProvider.PollInputs();
        movementProvider.LookAtVector(inputProvider.GetLookVector());
        movementProvider.Move(inputProvider.GetMovementVector());
        movementProvider.Jump(inputProvider.GetJumpState());
        
    }
}
