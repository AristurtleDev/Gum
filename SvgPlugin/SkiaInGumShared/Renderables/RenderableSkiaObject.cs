﻿using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SkiaPlugin.Renderables
{
    #region Enums

    public enum GradientType
    {
        Linear,
        Radial
    }

    #endregion

    public abstract class RenderableSkiaObject : IRenderableIpso, IVisible
    {
        #region General Fields/Properties

        protected Microsoft.Xna.Framework.Vector2 Position;

        IRenderableIpso mParent;
        public IRenderableIpso Parent
        {
            get { return mParent; }
            set
            {
                if (mParent != value)
                {
                    if (mParent != null)
                    {
                        mParent.Children.Remove(this);
                    }
                    mParent = value;
                    if (mParent != null)
                    {
                        mParent.Children.Add(this);
                    }
                }
            }
        }

        ObservableCollection<IRenderableIpso> mChildren;
        public ObservableCollection<IRenderableIpso> Children
        {
            get { return mChildren; }
        }

        public BlendState BlendState
        {
            get;
            set;
        }

        public bool ClipsChildren => false;

        public bool Wrap => false;

        protected Texture2D texture;

        public float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public float Z
        {
            get;
            set;
        }

        public float Rotation { get; set; }

        public bool FlipHorizontal { get; set; }

        float width;
        public float Width
        {
            get => width;
            set
            {
                if(value != width)
                {
                    width = value;
                    needsUpdate = true;
                }
            }
        }

        float height;
        public float Height
        {
            get => height;
            set
            {
                if(value != height)
                {
                    height = value;
                    needsUpdate = true;
                }
            }
        }

        public string Name
        {
            get;
            set;
        }

        public object Tag { get; set; }
        /// <summary>
        /// If this is false, then the DrawToSurface will handle applying the colors (like when creating a RoundedRectangle). If true,
        /// then this will multiply the rendering by the argument color (like when rendering an SVG).
        /// </summary>
        protected virtual bool ShouldApplyColorOnSpriteRender => false;
        protected bool needsUpdate = true;

        protected bool ForceUseColor
        {
            get; set;
        }

        ColorOperation IRenderableIpso.ColorOperation => ColorOperation.Modulate;

        #endregion

        #region Colors/Stroke

        bool isFilled = true;
        public bool IsFilled
        {
            get => isFilled;
            set { isFilled = value; needsUpdate = true; }
        }

        float strokeWidth = 1;
        public float StrokeWidth
        { 
            get => strokeWidth;
            set { strokeWidth = value; needsUpdate = true; }
        }

        public int Red
        {
            get => Color.R;
            set
            {
                Color.R = (byte)value;
                needsUpdate = true;
            }
        }

        public int Green
        {
            get => Color.G;
            set
            {
                Color.G = (byte)value;
                needsUpdate = true;
            }
        }

        public int Blue
        {
            get => Color.B;
            set
            {
                Color.B = (byte)value;
                needsUpdate = true;
            }
        }

        public Color Color = Color.White;

        public int Alpha
        {
            get => Color.A;
            set
            {
                Color.A = (byte)value;
                needsUpdate = true;
            }
        }

        #endregion

        #region Gradients

        bool useGradient;
        public bool UseGradient
        {
            get => useGradient;
            set
            {
                if (value != useGradient)
                {
                    useGradient = value;
                    needsUpdate = true;
                }
            }
        }

        int red1;
        public int Red1
        {
            get => red1;
            set
            {
                if (red1 != value)
                {
                    red1 = value;
                    needsUpdate = true;
                }
            }
        }

        int green1;
        public int Green1
        {
            get => green1;
            set
            {
                if (green1 != value)
                {
                    green1 = value;
                    needsUpdate = true;
                }
            }
        }

        int blue1;
        public int Blue1
        {
            get => blue1;
            set
            {
                if (blue1 != value)
                {
                    blue1 = value;
                    needsUpdate = true;
                }
            }
        }

        int red2;
        public int Red2
        {
            get => red2;
            set
            {
                if (red2 != value)
                {
                    red2 = value;
                    needsUpdate = true;
                }
            }
        }

        int green2;
        public int Green2
        {
            get => green2;
            set
            {
                if (green2 != value)
                {
                    green2 = value;
                    needsUpdate = true;
                }
            }
        }

        int blue2;
        public int Blue2
        {
            get => blue2;
            set
            {
                if (blue2 != value)
                {
                    blue2 = value;
                    needsUpdate = true;
                }
            }
        }

        protected float gradientX1;
        public float GradientX1
        {
            get => gradientX1;
            set
            {
                if (value != gradientX1)
                {
                    gradientX1 = value;
                    needsUpdate = true;
                }
            }
        }

        protected float gradientY1;
        public float GradientY1
        {
            get => gradientY1;
            set
            {
                if (value != gradientY1)
                {
                    gradientY1 = value;
                    needsUpdate = true;
                }
            }
        }

        protected float gradientX2;
        public float GradientX2
        {
            get => gradientX2;
            set
            {
                if (value != gradientX2)
                {
                    gradientX2 = value;
                    needsUpdate = true;
                }
            }
        }

        protected float gradientY2;
        public float GradientY2
        {
            get => gradientY2;
            set
            {
                if (value != gradientY2)
                {
                    gradientY2 = value;
                    needsUpdate = true;
                }
            }
        }


        PositionUnitType gradientX1Units;
        public PositionUnitType GradientX1Units
        {
            get => gradientX1Units;
            set
            {
                if(value != gradientX1Units)
                {
                    gradientX1Units = value;
                    needsUpdate = true;
                }
            }
        }

        PositionUnitType gradientX2Units;
        public PositionUnitType GradientX2Units
        {
            get => gradientX2Units;
            set
            {
                if (value != gradientX2Units)
                {
                    gradientX2Units = value;
                    needsUpdate = true;
                }
            }
        }

        PositionUnitType gradientY1Units;
        public PositionUnitType GradientY1Units
        {
            get => gradientY1Units;
            set
            {
                if (value != gradientY1Units)
                {
                    gradientY1Units = value;
                    needsUpdate = true;
                }
            }
        }

        PositionUnitType gradientY2Units;
        public PositionUnitType GradientY2Units
        {
            get => gradientY2Units;
            set
            {
                if (value != gradientY2Units)
                {
                    gradientY2Units = value;
                    needsUpdate = true;
                }
            }
        }

        float gradientInnerRadius;
        public float GradientInnerRadius
        {
            get => gradientInnerRadius;
            set
            {
                if (value != gradientInnerRadius)
                {
                    gradientInnerRadius = value;
                    needsUpdate = true;
                }
            }
        }

        float gradientOuterRadius;
        public float GradientOuterRadius
        {
            get => gradientOuterRadius;
            set
            {
                if (value != gradientOuterRadius)
                {
                    gradientOuterRadius = value;
                    needsUpdate = true;
                }
            }
        }


        GradientType gradientType;
        public GradientType GradientType
        {
            get => gradientType;
            set
            {
                if (gradientType != value)
                {
                    gradientType = value;
                    needsUpdate = true;
                }
            }
        }

        protected DimensionUnitType gradientInnerRadiusUnits;
        public DimensionUnitType GradientInnerRadiusUnits
        {
            get => gradientInnerRadiusUnits;
            set
            {
                if(value != gradientInnerRadiusUnits)
                {
                    gradientInnerRadiusUnits = value;
                    needsUpdate = true;
                }
            }
        }

        protected DimensionUnitType gradientOuterRadiusUnits;
        public DimensionUnitType GradientOuterRadiusUnits
        {
            get => gradientOuterRadiusUnits;
            set
            {
                if (value != gradientOuterRadiusUnits)
                {
                    gradientOuterRadiusUnits = value;
                    needsUpdate = true;
                }
            }
        }

        #endregion

        #region Dropshadow

        public bool HasDropshadow { get; set; }

        public float DropshadowOffsetX { get; set; }
        public float DropshadowOffsetY { get; set; }

        public float DropshadowBlurX { get; set; }
        public float DropshadowBlurY { get; set; }


        public Color DropshadowColor = Color.White;

        public int DropshadowAlpha
        {
            get => DropshadowColor.A;
            set
            {
                DropshadowColor.A = (byte)value;
                needsUpdate = true;
            }
        }

        public int DropshadowRed
        {
            get => DropshadowColor.R;
            set
            {
                DropshadowColor.R = (byte)value;
                needsUpdate = true;
            }
        }

        public int DropshadowGreen
        {
            get => DropshadowColor.G;
            set
            {
                DropshadowColor.G = (byte)value;
                needsUpdate = true;
            }
        }

        public int DropshadowBlue
        {
            get => DropshadowColor.B;
            set
            {
                DropshadowColor.B = (byte)value;
                needsUpdate = true;
            }
        }


        protected float XSizeSpillover => HasDropshadow ? DropshadowBlurX + Math.Abs(DropshadowOffsetX) : 0;
        protected float YSizeSpillover => HasDropshadow ? DropshadowBlurY + Math.Abs(DropshadowOffsetY) : 0;

        #endregion

        #region IVisible Implementation

        public bool Visible
        {
            get;
            set;
        }

        public bool AbsoluteVisible
        {
            get
            {
                if (((IVisible)this).Parent == null)
                {
                    return Visible;
                }
                else
                {
                    return Visible && ((IVisible)this).Parent.AbsoluteVisible;
                }
            }
        }

        IVisible IVisible.Parent
        {
            get
            {
                return ((IRenderableIpso)this).Parent as IVisible;
            }
        }


        #endregion

        public RenderableSkiaObject()
        {
            this.Visible = true;
            mChildren = new ObservableCollection<IRenderableIpso>();
        }

        public void Render(SpriteRenderer spriteRenderer, SystemManagers managers)
        {
            if(AbsoluteVisible)
            {
                var oldX = this.X;
                var oldY = this.Y;
                var oldWidth = this.Width;
                var oldHeight = this.Height;

                this.X -= XSizeSpillover;
                this.Y -= YSizeSpillover;
                // use fields not props, to not trigger a needsUpdate = true
                this.width += XSizeSpillover * 2;
                this.height += YSizeSpillover * 2;

                var color = ShouldApplyColorOnSpriteRender ? Color : Color.White;

                Sprite.Render(managers, spriteRenderer, this, texture, color, rotationInDegrees: Rotation);

                this.X = oldX;
                this.Y = oldY;
                this.width = oldWidth;
                this.height = oldHeight;
            }
        }

        void IRenderableIpso.SetParentDirect(IRenderableIpso parent)
        {
            mParent = parent;
        }

        public void PreRender()
        {
            if (needsUpdate && Width > 0 && Height > 0 && AbsoluteVisible)
            {
                if (texture != null)
                {
                    texture.Dispose();
                    texture = null;
                }

                var colorType = SKImageInfo.PlatformColorType;

                var widthToUse = Math.Min(2048, Width + XSizeSpillover * 2);
                var heightToUse = Math.Min(2048, Height + YSizeSpillover * 2);

                //var imageInfo = new SKImageInfo((int)widthToUse, (int)heightToUse, colorType, SKAlphaType.Unpremul);
                var imageInfo = new SKImageInfo((int)widthToUse, (int)heightToUse, colorType, SKAlphaType.Premul);
                using (var surface = SKSurface.Create(imageInfo))
                {
                    // It's possible this can fail
                    if(surface != null)
                    {
                        DrawToSurface(surface);

                        var skImage = surface.Snapshot();

                        Color? forcedColor = null;
                        if(ForceUseColor)
                        {
                            forcedColor = this.Color;
                        }

                        texture = RenderImageToTexture2D(skImage, SystemManagers.Default.Renderer.GraphicsDevice, colorType, forcedColor);
                        needsUpdate = false;
                    }
                }
            }
        }

        internal abstract void DrawToSurface(SKSurface surface);

        public static Texture2D RenderImageToTexture2D(SKImage image, GraphicsDevice graphicsDevice, SKColorType skiaColorType, Color? forcedColor = null)
        {
            var pixelMap = image.PeekPixels();
            var pointer = pixelMap.GetPixels();
            var originalPixels = new byte[image.Height * pixelMap.RowBytes];

            Marshal.Copy(pointer, originalPixels, 0, originalPixels.Length);

            var texture = new Texture2D(graphicsDevice, image.Width, image.Height);
            if (skiaColorType == SKColorType.Rgba8888)
            {
                texture.SetData(originalPixels);
            }
            else
            {
                // need a new byte[] to convert from BGRA to ARGB
                var convertedBytes = new byte[originalPixels.Length];

                var premult = false;
            
                if(premult)
                {
                    for (int i = 0; i < convertedBytes.Length; i += 4)
                    {
                        var b = originalPixels[i + 0];
                        var g = originalPixels[i + 1];
                        var r = originalPixels[i + 2];
                        var a = originalPixels[i + 3];

                        //var ratio = a / 255.0f;

                        //convertedBytes[i + 0] = (byte)(r * ratio + .5);
                        //convertedBytes[i + 1] = (byte)(g * ratio + .5);
                        //convertedBytes[i + 2] = (byte)(b * ratio + .5);
                        //convertedBytes[i + 3] = a;

                        if (forcedColor != null)
                        {
                            r = forcedColor.Value.R;
                            g = forcedColor.Value.G;
                            b = forcedColor.Value.B;
                        }

                        convertedBytes[i + 0] = r;
                        convertedBytes[i + 1] = g;
                        convertedBytes[i + 2] = b;
                        convertedBytes[i + 3] = a;
                    }
                }
                else
                {
                    for (int i = 0; i < convertedBytes.Length; i += 4)
                    {
                        var b = originalPixels[i + 0];
                        var g = originalPixels[i + 1];
                        var r = originalPixels[i + 2];
                        var a = originalPixels[i + 3];
                        var ratio = a / 255.0f;

                        if(forcedColor != null)
                        {
                            r = forcedColor.Value.R;
                            g = forcedColor.Value.G;
                            b = forcedColor.Value.B;
                        }

                        // output will always be premult so we need to unpremult
                        convertedBytes[i + 0] = (byte)(r / ratio + .5);
                        convertedBytes[i + 1] = (byte)(g / ratio + .5);
                        convertedBytes[i + 2] = (byte)(b / ratio + .5);
                        convertedBytes[i + 3] = a;
                    }
                }

                texture.SetData(convertedBytes);

            }
            return texture;
        }

        protected void SetGradientOnPaint(SKPaint paint)
        {
            var firstColor = new SKColor((byte)red1, (byte)green1, (byte)blue1);
            var secondColor = new SKColor((byte)red2, (byte)green2, (byte)blue2);

            var effectiveWidth = Width + XSizeSpillover*2;
            var effectiveHeight = Height + YSizeSpillover*2;

            var effectiveGradientX1 = gradientX1;
            switch (this.GradientX1Units)
            {
                case PositionUnitType.PixelsFromLeft:
                    effectiveGradientX1 += XSizeSpillover;
                    break;
                case PositionUnitType.PixelsFromCenterX:
                    effectiveGradientX1 += effectiveWidth / 2.0f;
                    break;
                case PositionUnitType.PixelsFromRight:
                    effectiveGradientX1 += effectiveWidth;
                    break;
                case PositionUnitType.PercentageWidth:
                    effectiveGradientX1 = effectiveWidth * gradientX1 / 100;
                    break;
            }

            var effectiveGradientX2 = gradientX1;
            switch (this.GradientX2Units)
            {
                case PositionUnitType.PixelsFromLeft:
                    effectiveGradientX2 += XSizeSpillover;
                    break;
                case PositionUnitType.PixelsFromCenterX:
                    effectiveGradientX2 += effectiveWidth / 2.0f;
                    break;
                case PositionUnitType.PixelsFromRight:
                    effectiveGradientX2 += effectiveWidth;
                    break;
                case PositionUnitType.PercentageWidth:
                    effectiveGradientX2 = effectiveWidth * gradientX2 / 100;
                    break;
            }

            var effectiveGradientY1 = gradientY1;
            switch (this.GradientY1Units)
            {
                case PositionUnitType.PixelsFromTop:
                    effectiveGradientY1 += YSizeSpillover;
                    break;
                case PositionUnitType.PixelsFromCenterY:
                    effectiveGradientY1 += effectiveHeight / 2.0f;
                    break;
                case PositionUnitType.PixelsFromBottom:
                    effectiveGradientY1 += effectiveHeight;
                    break;
                case PositionUnitType.PercentageHeight:
                    effectiveGradientY1 = effectiveHeight * gradientY1/100;
                    break;
            }

            var effectiveGradientY2 = gradientY2;
            switch (this.GradientY2Units)
            {
                case PositionUnitType.PixelsFromTop:
                    effectiveGradientY2 += YSizeSpillover;
                    break;
                case PositionUnitType.PixelsFromCenterY:
                    effectiveGradientY2 += effectiveHeight / 2.0f;
                    break;
                case PositionUnitType.PixelsFromBottom:
                    effectiveGradientY2 += effectiveHeight;
                    break;
                case PositionUnitType.PercentageHeight:
                    effectiveGradientY2 = effectiveHeight * gradientY2 / 100;
                    break;
            }


            if (gradientType == GradientType.Linear)
            {

                paint.Shader = SKShader.CreateLinearGradient(
                    new SKPoint(effectiveGradientX1, effectiveGradientY1), // left, top
                    new SKPoint(effectiveGradientX2, effectiveGradientY2), // right, bottom
                    new SKColor[] { firstColor, secondColor },
                    new float[] { 0, 1 },
                    SKShaderTileMode.Clamp);
            }
            else if (gradientType == GradientType.Radial)
            {
                var effectiveOuterRadius = gradientOuterRadius;

                switch (gradientOuterRadiusUnits)
                {
                    case Gum.DataTypes.DimensionUnitType.Percentage:
                        effectiveOuterRadius = effectiveWidth * gradientOuterRadius / 100;
                        break;
                    case Gum.DataTypes.DimensionUnitType.RelativeToContainer:
                        effectiveOuterRadius = effectiveWidth  / 2 + gradientOuterRadius;
                        break;
                }

                if (effectiveOuterRadius <= 0)
                {
                    effectiveOuterRadius = 100;
                }

                var effectiveInnerRadius = gradientInnerRadius;

                switch (gradientInnerRadiusUnits)
                {
                    case Gum.DataTypes.DimensionUnitType.Percentage:
                        effectiveInnerRadius = effectiveWidth * gradientInnerRadius / 100;
                        break;
                    case Gum.DataTypes.DimensionUnitType.RelativeToContainer:
                        effectiveInnerRadius = effectiveWidth / 2 + gradientInnerRadius;
                        break;
                }

                var innerToOuterRatio = effectiveInnerRadius / effectiveOuterRadius;

                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(effectiveGradientX1, effectiveGradientY1), // center
                    effectiveOuterRadius,
                    new SKColor[] { firstColor, secondColor },
                    new float[] { innerToOuterRatio, 1 },
                    SKShaderTileMode.Clamp);
            }
        }

        protected virtual SKPaint CreatePaint()
        {
            var skColor = new SKColor(Color.R, Color.G, Color.B, Color.A);

            var paint = new SKPaint
            {
                Color = skColor,
                Style = IsFilled ? SKPaintStyle.Fill : SKPaintStyle.Stroke,
                StrokeWidth = StrokeWidth,
                IsAntialias = true
            };

            if (UseGradient)
            {
                SetGradientOnPaint(paint);
            }

            if (HasDropshadow)
            {
                var dropshadowSkColor = new SKColor(DropshadowColor.R, DropshadowColor.G, DropshadowColor.B, DropshadowColor.A);
                paint.ImageFilter = SKImageFilter.CreateDropShadow(
                            DropshadowOffsetX,
                            // See https://stackoverflow.com/questions/60456526/how-can-i-tell-the-amount-of-space-needed-for-a-skia-dropshadow
                            DropshadowOffsetY,
                            DropshadowBlurX / 3.0f,
                            DropshadowBlurY / 3.0f,
                            dropshadowSkColor,
                            SKDropShadowImageFilterShadowMode.DrawShadowAndForeground);
            }

            return paint;
        }
    }
}
