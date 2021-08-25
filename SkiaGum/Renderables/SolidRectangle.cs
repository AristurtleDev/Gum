﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkiaGum.Renderables
{
    class SolidRectangle : RenderableBase
    {
       
        public override void DrawBound(SKRect boundingRect, SKCanvas canvas)
        {
            var radius = System.Math.Min(boundingRect.Width, boundingRect.Height) / 2.0f;
            using(var paint = GetPaint(boundingRect))
            {
                canvas.DrawRect(boundingRect, paint);
            }
        }
    }
}
