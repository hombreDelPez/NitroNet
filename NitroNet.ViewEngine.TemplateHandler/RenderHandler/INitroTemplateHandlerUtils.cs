﻿using System;
using System.Collections.Generic;
using Veil;

namespace NitroNet.ViewEngine.TemplateHandler.RenderHandler
{
    public interface INitroTemplateHandlerUtils
    {
        SubModel FindSubModel(RenderingParameter component, RenderingParameter skin,
            RenderingParameter dataVariation, object model, RenderingContext context);
        bool TryRenderPartial(object model, object subModel, string componentValue, string skinValue,
            RenderingContext renderingContext, IDictionary<string, string> parameters,
            Action<string, object, RenderingContext> renderPartial);
        void LogErrorIfSubModelFoundAndNull(bool modelFound, object subModel, string propertyName, object model);
        string CleanName(string text);
        bool GetPropertyValueFromObjectHierarchically(object model, string propertyName, out object modelValue);
    }
}
