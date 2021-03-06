﻿//   
// Copyright (c) Jesse Freeman. All rights reserved.  
//  
// Licensed under the Microsoft Public License (MS-PL) License. 
// See LICENSE file in the project root for full license information. 
// 
// Contributors
// --------------------------------------------------------
// This is the official list of Pixel Vision 8 contributors:
//  
// Jesse Freeman - @JesseFreeman
// Christer Kaitila - @McFunkypants
// Pedro Medeiros - @saint11
// Shawn Rakowski - @shwany

namespace PixelVisionRunner.Unity
{
    public class TextureFactory : ITextureFactory
    {
        public TextureFactory(bool flip = false)
        {
            this.flip = flip;
        }

        public ITexture2D NewTexture2D(int width, int height)
        {
            return new Texture2DAdapter(width, height, flip);
        }

        public bool flip { get; set; }
    }
}
