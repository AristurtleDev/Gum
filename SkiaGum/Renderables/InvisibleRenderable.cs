﻿using Gum;
using RenderingLibrary;
using RenderingLibrary.Graphics;
using SkiaGum.GueDeriving;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;

namespace SkiaGum.Renderables
{
    public class InvisibleRenderable : IVisible, IRenderableIpso, ISetClipsChildren
    {
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

        public bool IsRenderTarget => false;

        public BlendState BlendState => BlendState.AlphaBlend;

        ObservableCollection<IRenderableIpso> children = new ObservableCollection<IRenderableIpso>();
        public ObservableCollection<IRenderableIpso> Children => children;

        // If a GUE uses this, it needs to support storing the values.
        public bool ClipsChildren { get; set; }

        float height;
        public float Height
        {
            get { return height; }
            set
            {
#if DEBUG
                if (float.IsPositiveInfinity(value))
                {
                    throw new ArgumentException();
                }
#endif
                height = value;
            }
        }

        public string Name { get; set; }

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

        public float Rotation { get; set; }

        public object Tag { get; set; }

        public bool Visible { get; set; }

        public float Width { get; set; }

        public bool Wrap => false;

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public bool FlipHorizontal { get; set; }

        public ColorOperation ColorOperation { get; set; } = ColorOperation.Modulate;


        IVisible IVisible.Parent { get { return Parent as IVisible; } }

        public InvisibleRenderable()
        {
            Visible = true;
        }

        public void PreRender()
        {
        }

        public void Render(ISystemManagers managers)
        {

        }

        void IRenderableIpso.SetParentDirect(IRenderableIpso parent)
        {
            mParent = parent;
        }
    }
}
