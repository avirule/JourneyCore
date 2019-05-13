﻿using System.Xml.Serialization;

namespace JourneyCoreLib.Rendering.Environment.Tiling
{
    public class TileSetImage
    {
        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("width")]
        public int Width { get; set; }

        [XmlAttribute("height")]
        public int Height { get; set; }
    }
}
