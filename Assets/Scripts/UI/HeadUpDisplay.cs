using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Script that handle in-game user interface. It will listen to events sent by MonoBehaviours and will start animate, rotate, the crosshair
 */ 
public class HeadUpDisplay : MonoBehaviour
{
    private enum InteractionState
    {
        NEUTRAL,
        IS,
        CAN,
        CANNOT,
    }

    [Header("LINKED GAMEOBJECTS")]
    [SerializeField] private GameObject m_crosshairPrefab;

    private static ImageAnimation m_crosshairAnimation;
    private static InteractionState m_interactionState;

    private void Awake()
    {
        ListenToEvents();

        var crosshair = Instantiate(m_crosshairPrefab, transform);
        if (crosshair)
        {
            crosshair.name = "[Instanced] Crosshair";
            m_crosshairAnimation = crosshair.GetComponent<ImageAnimation>();
        }
    }

    private void ListenToEvents()
    {
        GrabbableInteraction.InteractEvent.AddListener(OnInteract);
        GrabbableInteraction.CanInteractEvent.AddListener(OnCanInteract);
        GrabbableInteraction.CannotInteractEvent.AddListener(OnCannotInteract);

        DoorInteraction.InteractEvent.AddListener(OnInteract);
        DoorInteraction.CanInteractEvent.AddListener(OnCanInteract);
        DoorInteraction.CannotInteractEvent.AddListener(OnCannotInteract);
    }

    private static uint GetPriority(InteractionState p_state)
    {
        switch (p_state)
        {
            default:
            case InteractionState.NEUTRAL:  return 0;
            case InteractionState.IS:       return 1;
            case InteractionState.CAN:      return 2;
            case InteractionState.CANNOT:   return 2;
        }
    }

    private static void SetInteractionState(InteractionState p_newState)
    { 
        if (GetPriority(p_newState) > GetPriority(m_interactionState))
        {
            m_interactionState = p_newState;
        }
    }

    private static void OnCanInteract()
    {
        SetInteractionState(InteractionState.CAN);
    }

    private static void OnCannotInteract()
    {
        SetInteractionState(InteractionState.CANNOT);
    }

    private static void OnIsInteracting()
    {
        SetInteractionState(InteractionState.IS);
    }

    private static void OnInteract()
    {
        m_crosshairAnimation.Rotate(180);
    }

    private void LateUpdate()
    {
        switch (m_interactionState)
        {
            case InteractionState.NEUTRAL:
                m_crosshairAnimation.ResetColor();
                m_crosshairAnimation.ResetScale();
                m_crosshairAnimation.ResetAlpha();
                break;

            case InteractionState.IS:
                m_crosshairAnimation.SetAlpha(0.0f);
                break;

            case InteractionState.CAN:
                m_crosshairAnimation.ResetAlpha();
                m_crosshairAnimation.SetColor(Color.yellow);
                m_crosshairAnimation.SetRelativeScale(2.0f);
                break;

            case InteractionState.CANNOT:
                m_crosshairAnimation.ResetAlpha();
                m_crosshairAnimation.SetColor(Color.red);
                m_crosshairAnimation.ResetScale();
                break;
        }

        m_interactionState = InteractionState.NEUTRAL;
    }
}
