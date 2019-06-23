﻿using System;
using System.Collections.Generic;

namespace JourneyCore.Lib.System.Event
{
    public class InputActionList
    {
        private bool _SinglePress { get; }
        private List<Action> Actions { get; }
        private Func<bool> EnabledCheck { get; }
        private bool HasReleased { get; set; }

        public InputActionList(Func<bool> enabledCheck, bool singlePress)
        {
            _SinglePress = singlePress;
            EnabledCheck = enabledCheck ?? (() => true);
            Actions = new List<Action>();
            HasReleased = true;
        }

        public void AddInputAction(Action inputAction)
        {
            Actions.Add(inputAction);
        }

        public bool ActivatePress(bool pressed)
        {
            if (!CheckActivationRequirements(pressed))
            {
                return false;
            }

            IterateActions();

            return true;
        }

        private bool CheckActivationRequirements(bool pressed)
        {
            if (EnabledCheck == null || !EnabledCheck())
            {
                return false;
            }

            bool invokePress = pressed && HasReleased;

            if (_SinglePress)
            {
                HasReleased = !pressed;
            }

            return invokePress;
        }

        private void IterateActions()
        {
            Actions.ForEach(action => action());
        }
    }
}