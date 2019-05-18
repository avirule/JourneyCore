﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JourneyCore.Lib.System.Event
{
    public class UpdatedProperty
    {
        public string ParentId { get; }
        public string PropertyName { get; }
        public object Value { get; }

        public UpdatedProperty(string parentId, string propertyName, object value)
        {
            ParentId = parentId;
            PropertyName = propertyName;
            Value = value;
        }
    }
}
