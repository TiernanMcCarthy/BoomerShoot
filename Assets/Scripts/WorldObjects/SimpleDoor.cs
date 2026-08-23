using NUnit.Framework;
using UnityEngine;

/// <summary>
/// This script acts as a door and lerps a move target from its start and end position
/// </summary>
public class SimpleDoor : MonoBehaviour
{

    [SerializeField] private Transform _closePosition;
    [SerializeField] private Transform _openPosition ;


    [SerializeField] private float _doorSpeed;


    [SerializeField] private Transform _doorVisual;

    [SerializeField]
    private bool _isOpen = false;

    private bool _isMoving = false;


    private Vector3 _lerpTarget=Vector3.zero;

    /// <summary>
    /// Lerps to a target until a target has been reached
    /// </summary>
    private void ManageDoorMovement()
    {
        if (!_isMoving)
        {
            return;
        }

        _doorVisual.position = Vector3.Lerp(_doorVisual.position, _lerpTarget, _doorSpeed * Time.deltaTime);

        if(Vector3.Distance(_doorVisual.position, _lerpTarget) < 0.04f)
        {
            _doorVisual.position = _lerpTarget;
            _isMoving = false;
        }

    }


    private void Update()
    {
        ManageDoorMovement();


    }

    public void ToggleDirection()
    {
        _isOpen = !_isOpen;

        if( _isOpen )
        {
            _lerpTarget=_openPosition.position;
        }
        else
        {
             _lerpTarget= _closePosition.position;
        }

        _isMoving = true;
    }

}
