using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace MonoGame.Extended.Testing
{
    static class WebResourceManager
    {
        public delegate void TextureReadyDelegate(Texture2D texture);

        private static bool _running = true;

        private static Queue<TextureRequest> _textureRequests;
        private static AutoResetEvent _textureRequestReset;
        private static Thread _textureRequestHandler;

        static WebResourceManager()
        {
            _textureRequests = new Queue<TextureRequest>();
            _textureRequestReset = new AutoResetEvent(false);
            _textureRequestHandler = new Thread(TextureRequestHandler);
            _textureRequestHandler.Start();
        }

        private static void TextureRequestHandler()
        {
            while (_running)
            {
                if (_textureRequestReset.WaitOne())
                {
                    while(_textureRequests.Count > 0)
                    {
                        var textureRequest = _textureRequests.Dequeue();
                        try
                        {
                            var headRequest = WebRequest.CreateHttp(textureRequest.Url);
                            headRequest.Method = "HEAD";
                            using (var response = headRequest.GetResponse())
                            {
                                switch (response.ContentType.ToLower())
                                {
                                    case "image/jpg":
                                    case "image/jpeg":
                                    case "image/png":
                                    case "image/bmp":
                                    case "image/gif":
                                        break;

                                    default:
                                        continue;
                                }
                            }

                            var request = WebRequest.CreateHttp(textureRequest.Url);
                            using (var response = request.GetResponse())
                            using (var stream = response.GetResponseStream())
                            {
                                var tex = Texture2D.FromStream(textureRequest.GraphicsDevice, stream);
                                if (tex == null) // add a better error handler
                                    throw new Exception("Could not load image: " + textureRequest.Url);

                                textureRequest.OnTexture.Invoke(tex);

                                //  hmm, we need a cache for textures
                                //var fs = new System.IO.FileStream(System.IO.Path.GetFileName(textureRequest.Url), System.IO.FileMode.Create))
                                //    stream.CopyTo(fs);
                            }
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine(exc);
                        }
                    }
                }
            }
        }

        public static void RequestTexture(string url, GraphicsDevice device, TextureReadyDelegate onTexture)
        {
            _textureRequests.Enqueue(new TextureRequest(url, device, onTexture));
            _textureRequestReset.Set();
        }

        struct TextureRequest
        {
            public readonly string Url;
            public readonly GraphicsDevice GraphicsDevice;
            public readonly TextureReadyDelegate OnTexture;

            public TextureRequest(string url, GraphicsDevice device, TextureReadyDelegate onTexture)
            {
                Url = url;
                GraphicsDevice = device;
                OnTexture = onTexture;
            }
        }
    }
}
