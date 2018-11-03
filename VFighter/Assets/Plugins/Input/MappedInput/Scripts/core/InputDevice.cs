using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class InputDevice : MonoBehaviour 
{	
	public static EnumValuesCache<MappedAxis> allMappedAxisTypes = new EnumValuesCache<MappedAxis> ();
	public static EnumValuesCache<MappedButton> allMappedButtonTypes = new EnumValuesCache<MappedButton> ();

	public bool autoActive = true;
	public float lastInputTime { get; private set;}

    public delegate void OnAxisEvent(MappedAxis axis);

    internal bool HasAnyInputLastFrame = false;
	
	protected float[] prevAxes = new float[System.Enum.GetValues(typeof(MappedAxis)).Length];
	protected float[] axes = new float[System.Enum.GetValues(typeof(MappedAxis)).Length];
	protected float[] axesRaw = new float[System.Enum.GetValues(typeof(MappedAxis)).Length];
    protected float[] prevAxesRaw = new float[System.Enum.GetValues(typeof(MappedAxis)).Length];
    protected AxisDirection[] prevAxisButtonsActive = new AxisDirection[System.Enum.GetValues(typeof(MappedAxis)).Length];
	protected AxisDirection[] axisButtonsActive = new AxisDirection[System.Enum.GetValues(typeof(MappedAxis)).Length];
	protected float axisButtonResetThreshold = 0.2f;
	protected float axisButtonActivateThreshold = 0.4f;
	protected float sensitivity = 3;
	protected float lastKeyPressTime;
    protected Dictionary<MappedAxis, List<float>> PastAxisVals = new Dictionary<MappedAxis, List<float>>();
    //protected Dictionary<MappedAxis, bool> AxisTappedPos = new Dictionary<MappedAxis, bool>();
    //protected Dictionary<MappedAxis, bool> AxisTappedNeg = new Dictionary<MappedAxis, bool>();
    protected bool[] AxisTappedPos = new bool[System.Enum.GetValues(typeof(MappedAxis)).Length];
    protected bool[] AxisTappedNeg = new bool[System.Enum.GetValues(typeof(MappedAxis)).Length];
    public Vector3 Center; 

	void Awake()
	{
	}

	protected virtual void Update()
	{
		HasAnyInputLastFrame = false;

		for (int i = 0; i < allMappedAxisTypes.Count; i++)
		{
			HasAnyInputLastFrame |= UpdateAxis (allMappedAxisTypes [i]); // update axis and check if there is any change
		}

		for (int i = 0; i < allMappedButtonTypes.Count; i++)
		{
			HasAnyInputLastFrame |= GetButton (allMappedButtonTypes[i]); // check if any mapped button is pressed
		}

		if (HasAnyInputLastFrame)
			lastInputTime = Time.time;
	
	}


	public abstract string GetButtonName (MappedButton button);	
	public abstract string GetAxisName (MappedAxis axis);

    public abstract Sprite GetButtonIcon(MappedButton button);
    public abstract Sprite GetAxisIcon(MappedAxis axis);

    public abstract bool GetButton (MappedButton button);
	public abstract bool GetButtonDown (MappedButton button);
	public abstract bool GetButtonUp (MappedButton button);

	protected abstract float GetAxisValueRaw (MappedAxis axis);

	// use axis as buttons (eg. analog stick for menu selection). 
	public bool GetAxisButtonDown(MappedAxis axis, AxisDirection dir)
	{
		return axisButtonsActive[(int)axis] == dir && prevAxisButtonsActive[(int)axis] != dir;
	}
	
	public bool GetAxisButton(MappedAxis axis, AxisDirection dir)
	{
		return axisButtonsActive[(int)axis] == dir;
	}
	
	public bool GetAxisButtonUp(MappedAxis axis, AxisDirection dir)
	{
		return axisButtonsActive[(int)axis] != dir && prevAxisButtonsActive[(int)axis] == dir;
	}

	public float GetAxis(MappedAxis axis)
    { 
        return axes[(int)axis];
	}
	
	public float GetAxisRaw(MappedAxis axis)
	{ 
		return axesRaw[(int)axis];
	}


	bool UpdateAxis(MappedAxis axis)
	{
		bool changed = false;
        prevAxesRaw[(int)axis] = axesRaw[(int)axis];

        axesRaw[(int)axis] = GetAxisValueRaw (axis);
		prevAxes [(int)axis] = axes [(int)axis];

        axes [(int)axis] = GetSmoothValue (axes [(int)axis],axesRaw [(int)axis],sensitivity);

        if (!Mathf.Approximately(axes[(int)axis], prevAxes[(int)axis]))
			changed = true;
		
		prevAxisButtonsActive [(int)axis] = axisButtonsActive [(int)axis];

		int dir = 0;
		if (axesRaw [(int)axis] > axisButtonActivateThreshold)
			dir ++;
		
		if (axesRaw [(int)axis] < -axisButtonActivateThreshold)
			dir --;
		
		if (Mathf.Abs(axesRaw [(int)axis]) < axisButtonResetThreshold)
			dir = 0;

		axisButtonsActive [(int)axis] = (AxisDirection)dir;


        /*
        if (prevAxesRaw[(int)axis] > -.9f && val < -.9f)
        {
            AxisTappedNeg[(int)axis] = true;
        }
        else
        {
            AxisTappedNeg[(int)axis] = false;
        }
        */
        return changed;
	}

	public Vector2 GetAxis2DCircleClamp(MappedAxis axis1, MappedAxis axis2)
	{
		return Vector2.ClampMagnitude( GetAxis2D(axis1,axis2), 1.0f );
	}

	public Vector2 GetAxis2D(MappedAxis axis1, MappedAxis axis2)
	{
		return new Vector2(GetAxis(axis1),GetAxis(axis2));
	}
	
	public Vector2 GetAxisRaw2DCircleClamp(MappedAxis axis1, MappedAxis axis2)
	{
		return Vector2.ClampMagnitude(GetAxisRaw2D(axis1,axis2), 1.0f );
	}

	public Vector2 GetAxisRaw2D(MappedAxis axis1, MappedAxis axis2)
	{
		return new Vector2(GetAxisRaw(axis1),GetAxisRaw(axis2));
	}
	
	public virtual float GetSmoothValue(float lastVal, float currentValRaw, float sensitivity)
	{
		float currentVal = currentValRaw;
		float maxDelta = Time.deltaTime * sensitivity;
		
		if( Mathf.Sign(lastVal) != Mathf.Sign(currentValRaw)) // move faster towards zero
			maxDelta *= 2;
		
		currentVal = Mathf.Clamp( lastVal + Mathf.Clamp(currentVal-lastVal,-maxDelta,maxDelta), -1, 1 );
		
		return currentVal;
	}

    public virtual bool GetIsAxisTappedPos(MappedAxis axis, float threshold = .9f)
    {
        float val = axesRaw[(int)axis];

        return prevAxesRaw[(int)axis] < threshold && val > threshold;
    }

    public virtual bool GetIsAxisTappedNeg(MappedAxis axis, float threshold = -.9f)
    {
        float val = axesRaw[(int)axis];
        return prevAxesRaw[(int)axis] > threshold && val < threshold;
    }
}

public enum AxisDirection
{
	Positive = 1,
	Centered = 0,
	Negative = -1
}