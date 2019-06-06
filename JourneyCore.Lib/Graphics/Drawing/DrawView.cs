﻿using System;
using System.Collections.Generic;
using System.Linq;
using JourneyCore.Lib.Game.Object;
using JourneyCore.Lib.System;
using SFML.Graphics;
using SFML.System;

namespace JourneyCore.Lib.Graphics.Drawing
{
    public class DrawView : IAnchorable
    {
        public const float DefaultPlayerViewRotation = 180f;

        // todo define this non-arbitrarily
        public const float DefaultZoomIncrement = 01.05f;

        private float _ZoomFactor = 1.0f;

        public string Name { get; }
        public int Layer { get; }
        public View View { get; }

        private SortedList<int, List<DrawItem>> DrawQueue { get; }
        private Vector2f DefaultSize { get; }

        public float ZoomFactor
        {
            get => _ZoomFactor;
            set
            {
                _ZoomFactor = value.LimitToRange(1.0f, 10f);

                View.Size = DefaultSize * _ZoomFactor;
            }
        }

        public DrawView(string name, int layer, View view)
        {
            Name = name;
            Layer = layer;
            View = view;

            DrawQueue = new SortedList<int, List<DrawItem>>();
            DefaultSize = View.Size;
        }

        public Vector2f Position
        {
            get => View.Center;
            set => View.Center = value;
        }

        public float Rotation
        {
            get => View.Rotation;
            set => View.Rotation = value + DefaultPlayerViewRotation % 360;
        }

        public void AddDrawItem(int layer, DrawItem drawItem)
        {
            if (!DrawQueue.Keys.Contains(layer))
            {
                DrawQueue.Add(layer, new List<DrawItem>());
            }

            DrawQueue[layer].Add(drawItem);
        }

        public void Draw(RenderWindow window, float frameTime)
        {
            DateTime absoluteNow = DateTime.Now;

            if (DrawQueue.Count <= 0)
            {
                return;
            }

            foreach ((int key, List<DrawItem> drawItemsPrelim) in DrawQueue)
            {
                drawItemsPrelim.RemoveAll(drawItem =>
                    drawItem == null ||
                    drawItem.MaxLifetime.Ticks != DateTime.MinValue.Ticks &&
                    drawItem.MaxLifetime.Ticks < absoluteNow.Ticks);

                foreach ((RenderStates renderStates, List<DrawItem> drawItems) in drawItemsPrelim.GroupBy(item =>
                        item.SubjectRenderStates, item => item,
                    (states, items) => new KeyValuePair<RenderStates, List<DrawItem>>(states, items.ToList())))
                {
                    VertexArray vArray = new VertexArray(PrimitiveType.Quads);

                    foreach (DrawItem drawItem in drawItems)
                    {
                        drawItem.PreDraw?.Invoke(frameTime);

                        if (!drawItem.DrawSubject.Batchable)
                        {
                            window.Draw(drawItem.DrawSubject, renderStates);

                            continue;
                        }

                        uint startIndex = vArray.VertexCount;
                        vArray.Resize(vArray.VertexCount + 4);

                        Vertex[] vertices = drawItem.DrawSubject.GetVertices();

                        vArray[startIndex + 0] = vertices[0];
                        vArray[startIndex + 1] = vertices[1];
                        vArray[startIndex + 2] = vertices[2];
                        vArray[startIndex + 3] = vertices[3];
                    }

                    if (vArray.VertexCount == 0)
                    {
                        continue;
                    }

                    window.Draw(vArray, renderStates);
                }
            }
        }
    }
}