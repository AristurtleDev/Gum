﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using RenderingLibrary.Graphics;
using Microsoft.Xna.Framework.Content;
using ToolsUtilities;
using Color = System.Drawing.Color;
using Matrix = System.Numerics.Matrix4x4;

namespace RenderingLibrary.Content
{
    public class LoaderManager
    {
        #region Fields

        bool mCacheTextures = false;

        static LoaderManager mSelf;
        
        Dictionary<string, IDisposable> mCachedDisposables = new Dictionary<string, IDisposable>();

        ContentManager mContentManager;

        

        #endregion

        #region Properties

        public IContentLoader ContentLoader
        {
            get;
            set;
        }

        public bool CacheTextures
        {
            get { return mCacheTextures; }
            set
            {
                mCacheTextures = value;

                if (!mCacheTextures)
                {
                    foreach (KeyValuePair<string, IDisposable> kvp in mCachedDisposables)
                    {
                        kvp.Value.Dispose();
                    }

                    mCachedDisposables.Clear();

                }
            }
        }

        public Texture2D InvalidTexture => Sprite.InvalidTexture;
        public static LoaderManager Self
        {
            get
            {
                if (mSelf == null)
                {
                    mSelf = new LoaderManager();
                }
                return mSelf;
            }
        }

        [Obsolete("Use Text.DefaultFont instead")]
        public SpriteFont DefaultFont => Text.DefaultFont;

        [Obsolete("Use Text.DefaultBitmapFont instead")]
        public BitmapFont DefaultBitmapFont => Text.DefaultBitmapFont;

        public IEnumerable<string> ValidTextureExtensions
        {
            get
            {
                yield return "png";
                yield return "jpg";
                yield return "tga";
                yield return "gif";
                yield return "svg";
                yield return "bmp";
            }
        }

        #endregion

        #region Methods

        public void AddDisposable(string name, IDisposable disposable)
        {
            mCachedDisposables.Add(name, disposable);
        }

        public void Dispose(string name)
        {
            if(mCachedDisposables.ContainsKey(name))
            {
                mCachedDisposables[name].Dispose();
                mCachedDisposables.Remove(name);
            }
        }

        public IDisposable GetDisposable(string name)
        {
            if (mCachedDisposables.ContainsKey(name))
            {
                return mCachedDisposables[name];
            }
            else
            {
                return null;
            }
        }

        public void Initialize(string invalidTextureLocation, string defaultFontLocation, IServiceProvider serviceProvider, SystemManagers managers)
        {
            if (mContentManager == null)
            {
                CreateInvalidTextureGraphic(invalidTextureLocation, managers);

                mContentManager = new ContentManager(serviceProvider, "ContentProject");

                if(defaultFontLocation == null)
                {
                    defaultFontLocation = "hudFont";
                }

                if (defaultFontLocation.EndsWith(".fnt"))
                {
                    Text.DefaultBitmapFont = new BitmapFont(defaultFontLocation, managers);
                }
                else
                {
                    Text.DefaultFont = mContentManager.Load<SpriteFont>(defaultFontLocation);
                }
            }
        }

        private void CreateInvalidTextureGraphic(string invalidTextureLocation, SystemManagers managers)
        {
            if (!string.IsNullOrEmpty(invalidTextureLocation) &&
                FileManager.FileExists(invalidTextureLocation))
            {

                Sprite.InvalidTexture = LoadContent<Texture2D>(invalidTextureLocation);
            }
            else
            {
                ImageData imageData = new ImageData(16, 16, managers);
                imageData.Fill(Microsoft.Xna.Framework.Color.White);
                for (int i = 0; i < 16; i++)
                {
                    imageData.SetPixel(i, i, Microsoft.Xna.Framework.Color.Red);
                    imageData.SetPixel(15 - i, i, Microsoft.Xna.Framework.Color.Red);

                }
                Sprite.InvalidTexture = imageData.ToTexture2D(false);
            }
        }

        public Texture2D LoadOrInvalid(string fileName, SystemManagers managers, out string errorMessage)
        {
            Texture2D toReturn;
            errorMessage = null;
            try
            {
                toReturn = LoadContent<Texture2D>(fileName);
            }
            catch(Exception e)
            {
                errorMessage = e.ToString();
                toReturn = InvalidTexture;
            }

            return toReturn;
        }

        
        public T TryLoadContent<T>( string contentName)
        {

#if DEBUG
            if (this.ContentLoader == null)
            {
                throw new Exception("The content loader is null - you must set it prior to calling LoadContent.");
            }
#endif
            return ContentLoader.TryLoadContent<T>(contentName);
        }

        public T LoadContent<T>(string contentName)
        {
#if DEBUG
            if(this.ContentLoader == null)
            {
                throw new Exception("The content loader is null - you must set it prior to calling LoadContent.");
            }
#endif

            return ContentLoader.LoadContent<T>(contentName);
        }

        public SpriteFont LoadSpriteFont(string fileName)
        {
            return mContentManager.Load<SpriteFont>(fileName);

        }

        /// <summary>
        /// Loads a Texture2D from a file name.  Supports
        /// .tga, png, jpg, and .gif.
        /// </summary>
        /// <param name="fileName">The name of the file (full file name) to load from.</param>
        /// <param name="managers">The SystemManagers to pull the GraphicsDevice for.  A valid
        /// GraphicsDevice is needed to load Texture2D's.  If "null" is passed, then the singleton
        /// Renderer will be used.  </param>
        /// <returns></returns>
        // TODO: Need to remove this to ContentLoader, but that would
        // require moving the cached textures there too
        [Obsolete("Use LoadContent instead for general loading, LoadTexture2D for Texture2D specific implementation")]
        internal Texture2D Load(string fileName, SystemManagers managers)
        {
            string fileNameStandardized = FileManager.Standardize(fileName, false, false);

            if (FileManager.IsRelative(fileNameStandardized) && FileManager.IsUrl(fileName) == false)
            {
                fileNameStandardized = FileManager.RelativeDirectory + fileNameStandardized;

                fileNameStandardized = FileManager.RemoveDotDotSlash(fileNameStandardized);
            }


            Texture2D toReturn = null;
            lock (mCachedDisposables)
            {
                if (CacheTextures)
                {

                    if (mCachedDisposables.ContainsKey(fileNameStandardized))
                    {
                        return (Texture2D)mCachedDisposables[fileNameStandardized];
                    }
                }

                if(FileManager.IsUrl(fileName))
                {
                    toReturn = LoadTextureFromUrl(fileName, managers);
                }
                else
                {
                    toReturn = LoadTextureFromFile(fileName, managers);
                }
                if (CacheTextures)
                {
                    mCachedDisposables.Add(fileNameStandardized, toReturn);
                }
            }
            return toReturn;
        }

        private Texture2D LoadTextureFromUrl(string fileName, SystemManagers managers)
        {

            Renderer renderer = null;
            if (managers == null)
            {
                renderer = Renderer.Self;
            }
            else
            {
                renderer = managers.Renderer;
            }
            string fileNameStandardized = FileManager.Standardize(fileName, false, false);

            Texture2D texture = null;
            using (var stream = GetUrlStream(fileName))
            {

                texture = Texture2D.FromStream(renderer.GraphicsDevice,
                    stream);

                texture.Name = fileNameStandardized;
            }
            return texture;
        }

        private static Stream GetUrlStream(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }

        /// <summary>
        /// Performs a no-caching load of the texture. This will always go to disk to access a file and 
        /// will always return a unique Texture2D. This should not be used in most cases, as caching is preferred
        /// </summary>
        /// <param name="fileName">The filename to load</param>
        /// <param name="managers">The optional SystemManagers to use when loading the file to obtain a GraphicsDevice</param>
        /// <returns>The loaded Texture2D</returns>
        public Texture2D LoadTextureFromFile(string fileName, SystemManagers managers = null)
        {
            string fileNameStandardized = FileManager.Standardize(fileName, false, false);

            if (FileManager.IsRelative(fileNameStandardized))
            {
                fileNameStandardized = FileManager.RelativeDirectory + fileNameStandardized;

                fileNameStandardized = FileManager.RemoveDotDotSlash(fileNameStandardized);
            }

            Texture2D toReturn;
            string extension = FileManager.GetExtension(fileName);
            Renderer renderer = null;
            if (managers == null)
            {
                renderer = Renderer.Self;
            }
            else
            {
                renderer = managers.Renderer;
            }
            if (extension == "tga")
            {
#if RENDERING_LIB_SUPPORTS_TGA
                    if (renderer.GraphicsDevice == null)
                    {
                        throw new Exception("The renderer is null - did you forget to call Initialize?");
                    }

                    Paloma.TargaImage tgaImage = new Paloma.TargaImage(fileName);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        tgaImage.Image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        stream.Seek(0, SeekOrigin.Begin); //must do this, or error is thrown in next line
                        toReturn = Texture2D.FromStream(renderer.GraphicsDevice, stream);
                        toReturn.Name = fileName;
                    }
#else
                throw new NotImplementedException();
#endif
            }

#if HAS_SYSTEM_DRAWING_IMAGE

            else if (extension == "bmp")
            {
                var image = System.Drawing.Image.FromFile(fileNameStandardized);
                var bitmap = new System.Drawing.Bitmap(image);
                
                var texture = new Texture2D(renderer.GraphicsDevice, bitmap.Width, bitmap.Height, false, SurfaceFormat.Color);
                var pixels = new Microsoft.Xna.Framework.Color[bitmap.Width * bitmap.Height];
                var index = 0;
                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        var r = color.R;
                        var g = color.G;
                        var b = color.B;
                        var a = color.A;
                        pixels[index] = new Microsoft.Xna.Framework.Color(r, g, b, a);
                        index++;
                    }
                }
                
                texture.SetData(pixels);
                texture.Name = fileNameStandardized;

                toReturn = texture;
            }
#endif
            else
            {
                using (var stream = FileManager.GetStreamForFile(fileNameStandardized))
                {
                    Texture2D texture = null;

                    texture = Texture2D.FromStream(renderer.GraphicsDevice,
                        stream);

                    texture.Name = fileNameStandardized;

                    toReturn = texture;

                }
            }

            return toReturn;
        }

#endregion
    }
}
