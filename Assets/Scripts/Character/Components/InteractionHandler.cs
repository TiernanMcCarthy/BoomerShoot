using UnityEngine;

namespace BoomerShoot.Character.Components
{
    /// <summary>
    /// Generic Interactable Handler for the Player. This Handler can and will pass weapons or equipment to the weapon handler if it finds them
    /// Otherwise, this handler will work with buttons, swapping weapons with characters e.t.c
    /// </summary>
    public class InteractionHandler : MonoBehaviour
    {

        /// <summary>
        /// This handler will scan for weapons and equipables in a sphere, interactables should also probably be scanned in a cone from the player
        /// </summary>
        [SerializeField] private float _sphericalScanRange;

        

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}