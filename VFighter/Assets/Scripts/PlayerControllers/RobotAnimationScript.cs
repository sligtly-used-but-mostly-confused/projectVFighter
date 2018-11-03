using System.Collections;
using UnityEngine;

public class RobotAnimationScript : PlayerController
{
    Animator m_animator;
    private bool isFlipped = false;

    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (InputDevice == null)
        {
            return;
        }

        //Move(this.InputDevice.GetAxis2DCircleClamp(MappedAxis.Horizontal, MappedAxis.Vertical));

        if (InputDevice.GetButtonDown(MappedButton.Ready) && !GameManager.Instance.CurrentlyInGame)
        {
            ToggleReady();
        }

        if (InputDevice is KeyboardMouseInputDevice)
        {
            Keyboard();
        }
        else
        {
            Gamepad();
        }
    }

    private void Keyboard()
    {
        float mouseX = InputDevice.GetAxisRaw(MappedAxis.AimX);
        float mouseY = InputDevice.GetAxisRaw(MappedAxis.AimY);

        var mousePos = Camera.main.ScreenToWorldPoint(new Vector2(mouseX, mouseY));
        var aimVector = Vector2.zero;

        if (AttachedObject == null)
        {
            aimVector = mousePos - transform.position;
        }
        else
        {
            aimVector = mousePos - AttachedObject.transform.position;
        }

        var normalizedDir = aimVector.normalized;
        m_animator.SetFloat("Horizontal", normalizedDir.x);
        m_animator.SetFloat("Vertical", normalizedDir.y);



        if (InputDevice.GetButtonDown(MappedButton.ChangeGrav) && IsReady)
        {
            //m_animator.SetBool("IsFloating", true);
            if(isFlipped){
                m_animator.SetFloat("Vertical", -normalizedDir.y);
                isFlipped = true;
    
            }
            else{
                m_animator.SetFloat("Vertical", normalizedDir.y);
                isFlipped = false;
            }
        //TODO: IsFloating should be set to true when character is in the air

        //TODO: IsRunning should be set to true when character is character is moving in the X direction.

        //TODO: Change Horizontal Float to negative normalizedDir.x when facing Left and positive when facing right.
    }

    private Vector2 _lastAimDir;

    private void Gamepad()
    {
        float rightSitckX = InputDevice.GetAxisRaw(MappedAxis.AimX);
        float rightSitckY = InputDevice.GetAxisRaw(MappedAxis.AimY);

        Vector2 aimDir = new Vector2(rightSitckX, rightSitckY);
        if (aimDir == Vector2.zero)
        {
            aimDir = _lastAimDir;
        }
        else
        {
            _lastAimDir = aimDir;
        }

        var normalizedDir = aimDir.normalized;
        m_animator.SetFloat("Horizontal", normalizedDir.x);
        m_animator.SetFloat("Vertical", normalizedDir.y);

        if (InputDevice.GetIsAxisTapped(MappedAxis.ChangeGrav) && InputDevice.GetAxis(MappedAxis.ChangeGrav) > 0 && IsReady)
        {
            //m_animator.SetBool("IsFloating", true);
            if (isFlipped)
            {
                m_animator.SetFloat("Vertical", -normalizedDir.y);
                isFlipped = true;

            }
            else
            {
                m_animator.SetFloat("Vertical", normalizedDir.y);
                isFlipped = false;
            }
        }
    }
}