﻿using System;
using System.Linq;
using System.Xml.Serialization;
using SFML.Graphics;

namespace JourneyCore.Lib.Graphics.Rendering.Environment.Tiling
{
    public struct Tile
    {
        [XmlAttribute("id")] public short Id { get; set; }
        [XmlAttribute("type")] public string Group { get; set; }
        [XmlAttribute("probability")] public float Probability { get; set; }

        [XmlArray("properties")]
        [XmlArrayItem("property")]
        public CustomProperty[] Properties { get; set; }

        [XmlIgnore] public short SizeX { get; set; }
        [XmlIgnore] public short SizeY { get; set; }
        [XmlIgnore] public IntRect TextureRect { get; set; }
        [XmlIgnore] public QuadCoords TextureCoords { get; set; }
        [XmlIgnore] public bool IsRandomizable { get; set; }
        [XmlIgnore] public bool IsRandomlyRotatable { get; set; }

        public void ApplyProperties()
        {
            CustomProperty isRandomizable = GetProperty("IsRandomizable");
            CustomProperty isRandomlyRotatable = GetProperty("IsRandomlyRotatable");

            if (isRandomizable != null)
                IsRandomizable = (bool) Convert.ChangeType(isRandomizable.Value, typeof(bool));

            if (isRandomlyRotatable != null)
                IsRandomlyRotatable = (bool) Convert.ChangeType(isRandomlyRotatable.Value, typeof(bool));
        }

        public CustomProperty GetProperty(string propertyName)
        {
            return !Properties.Any(property => property.Name.Equals(propertyName))
                ? null
                : Properties.Single(property => property.Name.Equals(propertyName));
        }
    }
}