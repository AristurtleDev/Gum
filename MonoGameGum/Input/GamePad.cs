﻿using Microsoft.Xna.Framework.Input;
using MonoGameGum.Forms.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameGum.Input;

#region DPadDirection Enum
public enum DPadDirection
{
    Up,
    Down,
    Left,
    Right
}
#endregion

public class GamePad
{
    #region Fields/Properties

    GamePadState mGamePadState = new GamePadState();
    GamePadState mLastGamePadState = new GamePadState();

    public bool IsConnected => mGamePadState.IsConnected;
    public bool WasDisconnectedThisFrame
    {
        get
        {
            return mLastGamePadState.IsConnected && !mGamePadState.IsConnected;
        }
    }

    AnalogStick mLeftStick;
    AnalogStick mRightStick;

    // The curren time, as of the last time Update was called
    double currentTime;

    /// <summary>
    /// Returns a reference to the left analog stick. This value will always be non-null even if the gamepad doesn't have a physical analog stick.
    /// </summary>
    public AnalogStick LeftStick => mLeftStick;


    /// <summary>
    /// Returns a reference to the right analog stick. This value will always be non-null even if the gamepad doesn't have a physical analog stick.
    /// </summary>
    public AnalogStick RightStick => mRightStick;


    double[] mLastButtonPush = new double[26];
    double[] mLastRepeatRate = new double[26];

    const float AnalogOnThreshold = .5f;

    /// <summary>
    /// The left trigger values as reported directly by the gamepad, not flipped for Gamecube
    /// </summary>
    AnalogButton mLeftTrigger;

    /// <summary>
    /// The right trigger values as reported directly by the gamepad, not flipped for Gamecube
    /// </summary>
    AnalogButton mRightTrigger;

    #endregion

    public GamePad()
    {
        mLeftStick = new AnalogStick();
        mRightStick = new AnalogStick();

        mLeftTrigger = new AnalogButton();
        mRightTrigger = new AnalogButton();
    }

    public bool ButtonDown(Buttons button)
    {
        //if (mButtonsIgnoredForThisFrame[(int)button] || InputManager.CurrentFrameInputSuspended)
        //    return false;

        bool returnValue = false;

        #region Handle the buttons if there isn't a ButtonMap (this can happen even if there is a ButtonMap)


        bool areShouldersAndTriggersFlipped =
            //AreShoulderAndTriggersFlipped;
            false;


        switch (button)
        {
            case Buttons.A:
                returnValue |= mGamePadState.Buttons.A == ButtonState.Pressed;
                break;
            case Buttons.B:
                returnValue |= mGamePadState.Buttons.B == ButtonState.Pressed;
                break;
            case Buttons.X:
                returnValue |= mGamePadState.Buttons.X == ButtonState.Pressed;
                break;
            case Buttons.Y:
                returnValue |= mGamePadState.Buttons.Y == ButtonState.Pressed;
                break;
            case Buttons.LeftShoulder:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mLeftTrigger.Position >= AnalogOnThreshold;
                }
                else
                {
                    returnValue |= mGamePadState.Buttons.LeftShoulder == ButtonState.Pressed;
                }
                break;
            case Buttons.RightShoulder:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mRightTrigger.Position >= AnalogOnThreshold;
                }
                else
                {
                    returnValue |= mGamePadState.Buttons.RightShoulder == ButtonState.Pressed;
                }
                break;
            case Buttons.Back:
                returnValue |= mGamePadState.Buttons.Back == ButtonState.Pressed;
                break;
            case Buttons.Start:
                returnValue |= mGamePadState.Buttons.Start == ButtonState.Pressed;
                break;
            case Buttons.LeftStick:
                returnValue |= mGamePadState.Buttons.LeftStick == ButtonState.Pressed;
                break;
            case Buttons.RightStick:
                returnValue |= mGamePadState.Buttons.RightStick == ButtonState.Pressed;
                break;
            case Buttons.DPadUp:
                returnValue |= mGamePadState.DPad.Up == ButtonState.Pressed;
                break;
            case Buttons.DPadDown:
                returnValue |= mGamePadState.DPad.Down == ButtonState.Pressed;
                break;
            case Buttons.DPadLeft:
                returnValue |= mGamePadState.DPad.Left == ButtonState.Pressed;
                break;
            case Buttons.DPadRight:
                returnValue |= mGamePadState.DPad.Right == ButtonState.Pressed;
                break;
            case Buttons.LeftTrigger:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mGamePadState.Buttons.LeftShoulder == ButtonState.Pressed;
                }
                else
                {
                    returnValue |= mLeftTrigger.Position >= AnalogOnThreshold;
                }
                break;
            case Buttons.RightTrigger:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mGamePadState.Buttons.RightShoulder == ButtonState.Pressed;
                }
                else
                {
                    returnValue |= mRightTrigger.Position >= AnalogOnThreshold;
                }
                break;
        }

        #endregion

        return returnValue;

    }

    public bool ButtonPushed(Buttons button)
    {
        //if (InputManager.mIgnorePushesThisFrame || mButtonsIgnoredForThisFrame[(int)button] || InputManager.CurrentFrameInputSuspended || ignoredNextPushes[(int)button])
        //    return false;

        bool returnValue = false;

        bool areShouldersAndTriggersFlipped =
            false;
        //AreShoulderAndTriggersFlipped;

        switch (button)
        {
            case Buttons.A:
                returnValue |= mGamePadState.Buttons.A == ButtonState.Pressed && mLastGamePadState.Buttons.A == ButtonState.Released;
                break;
            case Buttons.B:
                returnValue |= mGamePadState.Buttons.B == ButtonState.Pressed && mLastGamePadState.Buttons.B == ButtonState.Released;
                break;
            case Buttons.X:
                returnValue |= mGamePadState.Buttons.X == ButtonState.Pressed && mLastGamePadState.Buttons.X == ButtonState.Released;
                break;
            case Buttons.Y:
                returnValue |= mGamePadState.Buttons.Y == ButtonState.Pressed && mLastGamePadState.Buttons.Y == ButtonState.Released;
                break;
            case Buttons.LeftShoulder:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mLeftTrigger.Position >= AnalogOnThreshold && mLeftTrigger.LastPosition < AnalogOnThreshold;
                }
                else
                {
                    returnValue |= mGamePadState.Buttons.LeftShoulder == ButtonState.Pressed && mLastGamePadState.Buttons.LeftShoulder == ButtonState.Released;
                }
                break;
            case Buttons.RightShoulder:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mRightTrigger.Position >= AnalogOnThreshold && mRightTrigger.LastPosition < AnalogOnThreshold;
                }
                else
                {
                    returnValue |= mGamePadState.Buttons.RightShoulder == ButtonState.Pressed && mLastGamePadState.Buttons.RightShoulder == ButtonState.Released;
                }
                break;
            case Buttons.Back:
                returnValue |= mGamePadState.Buttons.Back == ButtonState.Pressed && mLastGamePadState.Buttons.Back == ButtonState.Released;
                break;
            case Buttons.Start:
                returnValue |= mGamePadState.Buttons.Start == ButtonState.Pressed && mLastGamePadState.Buttons.Start == ButtonState.Released;
                break;
            case Buttons.LeftStick:
                returnValue |= mGamePadState.Buttons.LeftStick == ButtonState.Pressed && mLastGamePadState.Buttons.LeftStick == ButtonState.Released;
                break;
            case Buttons.RightStick:
                returnValue |= mGamePadState.Buttons.RightStick == ButtonState.Pressed && mLastGamePadState.Buttons.RightStick == ButtonState.Released;
                break;
            case Buttons.DPadUp:
                returnValue |= mGamePadState.DPad.Up == ButtonState.Pressed && mLastGamePadState.DPad.Up == ButtonState.Released;
                break;
            case Buttons.DPadDown:
                returnValue |= mGamePadState.DPad.Down == ButtonState.Pressed && mLastGamePadState.DPad.Down == ButtonState.Released;
                break;
            case Buttons.DPadLeft:
                returnValue |= mGamePadState.DPad.Left == ButtonState.Pressed && mLastGamePadState.DPad.Left == ButtonState.Released;
                break;
            case Buttons.DPadRight:
                returnValue |= mGamePadState.DPad.Right == ButtonState.Pressed && mLastGamePadState.DPad.Right == ButtonState.Released;
                break;
            case Buttons.LeftTrigger:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mGamePadState.Buttons.LeftShoulder == ButtonState.Pressed && mLastGamePadState.Buttons.LeftShoulder == ButtonState.Released;
                }
                else
                {
                    returnValue |= mLeftTrigger.Position >= AnalogOnThreshold && mLeftTrigger.LastPosition < AnalogOnThreshold;
                }
                break;
            case Buttons.RightTrigger:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mGamePadState.Buttons.RightShoulder == ButtonState.Pressed && mLastGamePadState.Buttons.RightShoulder == ButtonState.Released;
                }
                else
                {
                    returnValue |= mRightTrigger.Position >= AnalogOnThreshold && mRightTrigger.LastPosition < AnalogOnThreshold;
                }
                break;
            case Buttons.LeftThumbstickUp:
                returnValue |= LeftStick.AsDPadPushed(DPadDirection.Up);
                break;
            case Buttons.LeftThumbstickDown:
                returnValue |= LeftStick.AsDPadPushed(DPadDirection.Down);
                break;
            case Buttons.LeftThumbstickLeft:
                returnValue |= LeftStick.AsDPadPushed(DPadDirection.Left);
                break;
            case Buttons.LeftThumbstickRight:
                returnValue |= LeftStick.AsDPadPushed(DPadDirection.Right);
                break;
        }

        return returnValue;
    }

    public bool ButtonReleased(Buttons button)
    {
        //if (mButtonsIgnoredForThisFrame[(int)button] || InputManager.CurrentFrameInputSuspended)
        //    return false;

        bool returnValue = false;

        bool areShouldersAndTriggersFlipped =
            //AreShoulderAndTriggersFlipped;
            false;

        switch (button)
        {
            case Buttons.A:
                returnValue |= mGamePadState.Buttons.A == ButtonState.Released && mLastGamePadState.Buttons.A == ButtonState.Pressed;
                break;
            case Buttons.B:
                returnValue |= mGamePadState.Buttons.B == ButtonState.Released && mLastGamePadState.Buttons.B == ButtonState.Pressed;
                break;
            case Buttons.X:
                returnValue |= mGamePadState.Buttons.X == ButtonState.Released && mLastGamePadState.Buttons.X == ButtonState.Pressed;
                break;
            case Buttons.Y:
                returnValue |= mGamePadState.Buttons.Y == ButtonState.Released && mLastGamePadState.Buttons.Y == ButtonState.Pressed;
                break;
            case Buttons.LeftShoulder:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mLeftTrigger.Position < AnalogOnThreshold && mLeftTrigger.LastPosition >= AnalogOnThreshold;
                }
                else
                {
                    returnValue |= mGamePadState.Buttons.LeftShoulder == ButtonState.Released && mLastGamePadState.Buttons.LeftShoulder == ButtonState.Pressed;
                }
                break;
            case Buttons.RightShoulder:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mRightTrigger.Position < AnalogOnThreshold && mRightTrigger.LastPosition >= AnalogOnThreshold;
                }
                else
                {
                    returnValue |= mGamePadState.Buttons.RightShoulder == ButtonState.Released && mLastGamePadState.Buttons.RightShoulder == ButtonState.Pressed;
                }
                break;
            case Buttons.Back:
                returnValue |= mGamePadState.Buttons.Back == ButtonState.Released && mLastGamePadState.Buttons.Back == ButtonState.Pressed;
                break;
            case Buttons.Start:
                returnValue |= mGamePadState.Buttons.Start == ButtonState.Released && mLastGamePadState.Buttons.Start == ButtonState.Pressed;
                break;
            case Buttons.LeftStick:
                returnValue |= mGamePadState.Buttons.LeftStick == ButtonState.Released && mLastGamePadState.Buttons.LeftStick == ButtonState.Pressed;
                break;
            case Buttons.RightStick:
                returnValue |= mGamePadState.Buttons.RightStick == ButtonState.Released && mLastGamePadState.Buttons.RightStick == ButtonState.Pressed;
                break;
            case Buttons.DPadUp:
                returnValue |= mGamePadState.DPad.Up == ButtonState.Released && mLastGamePadState.DPad.Up == ButtonState.Pressed;
                break;
            case Buttons.DPadDown:
                returnValue |= mGamePadState.DPad.Down == ButtonState.Released && mLastGamePadState.DPad.Down == ButtonState.Pressed;
                break;
            case Buttons.DPadLeft:
                returnValue |= mGamePadState.DPad.Left == ButtonState.Released && mLastGamePadState.DPad.Left == ButtonState.Pressed;
                break;
            case Buttons.DPadRight:
                returnValue |= mGamePadState.DPad.Right == ButtonState.Released && mLastGamePadState.DPad.Right == ButtonState.Pressed;
                break;
            case Buttons.LeftTrigger:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mGamePadState.Buttons.LeftShoulder == ButtonState.Released && mLastGamePadState.Buttons.LeftShoulder == ButtonState.Pressed;
                }
                else
                {
                    returnValue |= mLeftTrigger.Position < AnalogOnThreshold && mLeftTrigger.LastPosition >= AnalogOnThreshold;
                }
                break;
            case Buttons.RightTrigger:
                if (areShouldersAndTriggersFlipped)
                {
                    returnValue |= mGamePadState.Buttons.RightShoulder == ButtonState.Released && mLastGamePadState.Buttons.RightShoulder == ButtonState.Pressed;
                }
                else
                {
                    returnValue |= mRightTrigger.Position < AnalogOnThreshold && mRightTrigger.LastPosition >= AnalogOnThreshold;
                }
                break;
        }

        return returnValue;

    }


    /// <summary>
    /// Returns whether the argument was pushed this frame, or whether it is continually being held down and a "repeat" press
    /// has occurred.
    /// </summary>
    /// <param name="button">The button to test, which includes DPad directions.</param>
    /// <param name="timeAfterPush">The number of seconds after initial push to wait before raising repeat rates. This value is typically larger than timeBetweenRepeating.</param>
    /// <param name="timeBetweenRepeating">The number of seconds between repeats once the timeAfterPush. This value is typically smaller than timeAfterPush.</param>
    /// <returns>Whether the button was pushed or repeated this frame.</returns>
    public bool ButtonRepeatRate(Buttons button, double timeAfterPush = .35, double timeBetweenRepeating = .12)
    {
        //if (mButtonsIgnoredForThisFrame[(int)button])
        //    return false;

        if (ButtonPushed(button))
            return true;

        // If this method is called multiple times per frame this line
        // of code guarantees that the user will get true every time until
        // the next TimeManager.Update (next frame).
        // The very first frame of FRB would have CurrentTime == 0. 
        // The repeat cannot happen on the first frame, so we check for that:
        bool repeatedThisFrame = currentTime > 0 && mLastRepeatRate[(int)button] == currentTime;

        if (repeatedThisFrame ||
            (
            ButtonDown(button) &&
            currentTime - mLastButtonPush[(int)button] > timeAfterPush &&
            currentTime - mLastRepeatRate[(int)button] > timeBetweenRepeating)
            )
        {
            mLastRepeatRate[(int)button] = currentTime;
            return true;
        }

        return false;

    }

    internal void Activity(GamePadState gamepadState, double time)
    {
        currentTime = time;
        mLastGamePadState = mGamePadState;
        mGamePadState = gamepadState;

        if (IsConnected || WasDisconnectedThisFrame)
        {
            UpdateAnalogStickAndTriggerValues(time);
            UpdateLastButtonPushedValues(time);
        }

    }

    private void UpdateAnalogStickAndTriggerValues(double time)
    {
        var leftStick = mGamePadState.ThumbSticks.Left;
        var rightStick = mGamePadState.ThumbSticks.Right;

        mLeftStick.Update(leftStick, time);
        mRightStick.Update(rightStick, time);

        //if (AreShoulderAndTriggersFlipped)
        //{
        //    mFlippedLeftTrigger.Update((int)mGamePadState.Buttons.LeftShoulder);
        //    mFlippedRightTrigger.Update((int)mGamePadState.Buttons.RightShoulder);

        //}

        // Even if using Gamecube, record these values as they are used above in button maps
        mLeftTrigger.Update(mGamePadState.Triggers.Left, time);
        mRightTrigger.Update(mGamePadState.Triggers.Right, time);

        
    }

    private void UpdateLastButtonPushedValues(double currentTime)
    {
        // Set the last pushed and clear the ignored input

        for (int i = 0; i < mLastButtonPush.Length; i++)
        {
            //mButtonsIgnoredForThisFrame[i] = false;

            if (ButtonPushed((Buttons)i))
            {
                mLastButtonPush[i] = currentTime;
            }
        }
    }

}
