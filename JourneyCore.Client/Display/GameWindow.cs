﻿using System;
using System.Collections.Generic;
using System.Linq;
using JourneyCore.Lib.Graphics.Drawing;
using JourneyCore.Lib.System.Time;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace JourneyCore.Client.Display
{
    public class GameWindow
    {
        public const float WidescreenRatio = 9f / 16f;
        public const float LetterboxRatio = 3f / 4f;

        public GameWindow(string windowTitle, VideoMode videoMode, int targetFps, Vector2f contentScale,
            float positionScale)
        {
            ContentScale = contentScale;
            PositionScale = ContentScale * positionScale;
            TargetFps = targetFps;

            Window = new RenderWindow(videoMode, windowTitle);
            Window.Closed += OnClose;
            Window.GainedFocus += OnGainedFocus;
            Window.LostFocus += OnLostFocus;
            Window.MouseWheelScrolled += OnMouseWheelScrolled;
            Window.SetFramerateLimit((uint)TargetFps);
            Window.SetActive(false);

            DrawViews = new SortedList<int, DrawView>();
            DeltaClock = new Delta();
        }

        public RenderWindow SetActive(bool activeState)
        {
            Window.SetActive(activeState);

            return Window;
        }

        public Vector2i GetRelativeMousePosition()
        {
            return Mouse.GetPosition(Window);
        }


        #region VARIABLES

        private RenderWindow Window { get; }
        private static Delta DeltaClock { get; set; }
        private SortedList<int, DrawView> DrawViews { get; }
        private static int _TargetFps;

        public Vector2u Size => Window.Size;
        public bool IsInMenu { get; private set; }
        public bool IsActive => Window.IsOpen;
        public Vector2f ContentScale { get; set; }
        public Vector2f PositionScale { get; set; }

        public int TargetFps
        {
            get => _TargetFps;
            set
            {
                // fps changed stuff

                _TargetFps = value;
                IndividualFrameTime = 1f / _TargetFps;
            }
        }

        public float ElapsedTime { get; private set; }
        public float IndividualFrameTime { get; private set; }

        #endregion


        #region RENDERING

        public void UpdateWindow()
        {
            ElapsedTime = DeltaClock.GetDelta();

            Window.DispatchEvents();
            Window.Clear();

            foreach ((int key, DrawView drawView) in DrawViews)
            {
                SetWindowView(drawView.Name, drawView.View);

                drawView.Draw(Window, ElapsedTime);
            }

            Window.Display();
        }

        public void DrawItem(string viewName, int layer, DrawItem drawItem)
        {
            DrawView drawView = DrawViews.SingleOrDefault(view => view.Value.Name.Equals(viewName)).Value;

            drawView?.AddDrawItem(layer, drawItem);
        }

        #endregion


        #region EVENTS

        public event EventHandler Closed;
        public event EventHandler GainedFocus;
        public event EventHandler LostFocus;
        public event EventHandler<MouseWheelScrollEventArgs> MouseWheelScrolled;

        private void OnClose(object sender, EventArgs args)
        {
            Closed?.Invoke(sender, args);

            RenderWindow window = (RenderWindow)sender;
            window.Close();
        }

        public void OnGainedFocus(object sender, EventArgs args)
        {
            GainedFocus?.Invoke(sender, args);
        }

        public void OnLostFocus(object sender, EventArgs args)
        {
            LostFocus?.Invoke(sender, args);
        }

        public void OnMouseWheelScrolled(object sender, MouseWheelScrollEventArgs args)
        {
            MouseWheelScrolled?.Invoke(sender, args);
        }

        #endregion


        #region VIEW

        public DrawView CreateDrawView(string viewName, int drawLayer, View defaultView)
        {
            return CreateDrawView(new DrawView(viewName, drawLayer, defaultView));
        }

        public DrawView CreateDrawView(DrawView drawView)
        {
            if (DrawViews.Any(dView => dView.Value.Name.Equals(drawView.Name)))
            {
                return null;
            }

            DrawViews.Add(drawView.Layer, drawView);

            return drawView;
        }

        public View SetWindowView(string name, View view)
        {
            Window.SetView(view);

            return GetDrawView(name)?.View;
        }

        public DrawView GetDrawView(string name)
        {
            return DrawViews.SingleOrDefault(view => view.Value.Name.Equals(name)).Value;
        }

        public DrawView SetViewport(string name, FloatRect viewport)
        {
            DrawView drawView = GetDrawView(name);

            drawView.View.Viewport = viewport;

            SetWindowView(name, drawView.View);

            return drawView;
        }

        public DrawView MoveView(string name, Vector2f position)
        {
            DrawView drawView = GetDrawView(name);

            drawView.Position = position;

            SetWindowView(name, drawView.View);

            return drawView;
        }

        public DrawView RotateView(string name, float rotation)
        {
            DrawView drawView = GetDrawView(name);

            drawView.Rotation = rotation;

            SetWindowView(name, drawView.View);

            return drawView;
        }

        #endregion
    }
}