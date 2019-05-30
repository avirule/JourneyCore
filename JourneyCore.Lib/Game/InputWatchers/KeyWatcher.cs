﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFML.Window;

namespace JourneyCore.Lib.Game.InputWatchers
{
    public class KeyWatcher
    {
        private readonly List<KeyWatch> _watchedKeys;

        public bool WindowFocused { get; set; }

        public KeyWatcher()
        {
            WindowFocused = true;
            _watchedKeys = new List<KeyWatch>();
        }

        #region METHODS

        public void AddWatchedKeyAction(Keyboard.Key key, Func<Keyboard.Key, Task> keyAction)
        {
            if (!GetWatchedKeys().Contains(key))
            {
                _watchedKeys.Add(new KeyWatch(key));
            }

            GetKeyWatch(key).AddKeyAction(keyAction);
        }

        public void RemoveWatchedKeyAction(Keyboard.Key key, Func<Keyboard.Key, Task> keyAction)
        {
            if (!GetWatchedKeys().Contains(key))
            {
                throw new ArgumentException($"Keyboard.Key {key} does not exist in watched keys list.");
            }

            GetKeyWatch(key).RemoveKeyAction(keyAction);
        }

        public List<Keyboard.Key> GetWatchedKeys()
        {
            return _watchedKeys.Select(keyWatch => keyWatch.Key).ToList();
        }

        public KeyWatch GetKeyWatch(Keyboard.Key key)
        {
            return _watchedKeys.SingleOrDefault(keyWatch => keyWatch.Key.Equals(key));
        }

        public void CheckWatchedKeys()
        {
            if (!WindowFocused)
            {
                return;
            }

            foreach (Keyboard.Key watchedKey in GetWatchedKeys())
            {
                if (Keyboard.IsKeyPressed(watchedKey))
                {
                    GetKeyWatch(watchedKey).Invoke();
                }
            }
        }

        #endregion
    }
}