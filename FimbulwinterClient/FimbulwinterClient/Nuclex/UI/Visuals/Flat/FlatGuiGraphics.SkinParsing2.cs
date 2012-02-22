#region CPL License
/*
Nuclex Framework
Copyright (C) 2002-2010 Nuclex Development Labs

This library is free software; you can redistribute it and/or
modify it under the terms of the IBM Common Public License as
published by the IBM Corporation; either version 1.0 of the
License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
IBM Common Public License for more details.

You should have received a copy of the IBM Common Public
License along with this library
*/
#endregion

#if USE_XMLDOCUMENT

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using Nuclex.Support;

namespace Nuclex.UserInterface.Visuals.Flat {

  partial class FlatGuiGraphics {

    #region class RegionListBuilder

    /// <summary>Builds a region list from the regions in an frame XML node</summary>
    private class RegionListBuilder {

      /// <summary>Initializes a new frame region list builder</summary>
      /// <param name="frameNode">Node of the frame whose regions will be processed</param>
      private RegionListBuilder(XmlNode frameNode) {
        this.regionNodes = frameNode.SelectNodes("region");
      }

      /// <summary>
      ///   Builds a region list from the regions specified in the provided frame XML node
      /// </summary>
      /// <param name="frameNode">
      ///   XML node for the frame whose regions wille be processed
      /// </param>
      /// <param name="bitmaps">
      ///   Bitmap lookup table used to associate a region's bitmap id to the real bitmap
      /// </param>
      /// <returns>
      ///   A list of the regions that have been extracted from the frame XML node
      /// </returns>
      public static Frame.Region[] Build(
        XmlNode frameNode, IDictionary<string, Texture2D> bitmaps
      ) {
        RegionListBuilder builder = new RegionListBuilder(frameNode);
        builder.retrieveBorderSizes();
        return builder.createAndPlaceRegions(bitmaps);
      }

      /// <summary>Retrieves the sizes of the border regions in a frame</summary>
      private void retrieveBorderSizes() {
        for (int regionIndex = 0; regionIndex < this.regionNodes.Count; ++regionIndex) {

          // Left and right border width determination
          string hplacement = this.regionNodes[regionIndex].Attributes["hplacement"].Value;
          string w = this.regionNodes[regionIndex].Attributes["w"].Value;
          if (hplacement == "left") {
            this.leftBorderWidth = Math.Max(this.leftBorderWidth, int.Parse(w));
          } else if (hplacement == "right") {
            this.rightBorderWidth = Math.Max(this.rightBorderWidth, int.Parse(w));
          }

          // Top and bottom border width determination
          string vplacement = this.regionNodes[regionIndex].Attributes["vplacement"].Value;
          string h = this.regionNodes[regionIndex].Attributes["h"].Value;
          if (vplacement == "top") {
            this.topBorderWidth = Math.Max(this.topBorderWidth, int.Parse(h));
          } else if (vplacement == "bottom") {
            this.bottomBorderWidth = Math.Max(this.bottomBorderWidth, int.Parse(h));
          }

        }
      }

      /// <summary>
      ///   Creates and places the regions needed to be drawn to render the frame
      /// </summary>
      /// <param name="bitmaps">
      ///   Bitmap lookup table to associate a region's bitmap id to the real bitmap
      /// </param>
      /// <returns>The regions created for the frame</returns>
      private Frame.Region[] createAndPlaceRegions(IDictionary<string, Texture2D> bitmaps) {
        Frame.Region[] regions = new Frame.Region[this.regionNodes.Count];

        // Fill all regions making up the current frame
        for (int regionIndex = 0; regionIndex < this.regionNodes.Count; ++regionIndex) {

          // Obtain all attributes of the region node
          XmlAttribute idAttribute = this.regionNodes[regionIndex].Attributes["id"];
          string id = (idAttribute == null) ? null : idAttribute.Value;
          string source = this.regionNodes[regionIndex].Attributes["source"].Value;
          string hplacement = this.regionNodes[regionIndex].Attributes["hplacement"].Value;
          string vplacement = this.regionNodes[regionIndex].Attributes["vplacement"].Value;
          string x = this.regionNodes[regionIndex].Attributes["x"].Value;
          string y = this.regionNodes[regionIndex].Attributes["y"].Value;
          string w = this.regionNodes[regionIndex].Attributes["w"].Value;
          string h = this.regionNodes[regionIndex].Attributes["h"].Value;

          // Assign the trivial attributes
          regions[regionIndex].Id = id;
          regions[regionIndex].Texture = bitmaps[source];
          regions[regionIndex].SourceRegion.X = int.Parse(x);
          regions[regionIndex].SourceRegion.Y = int.Parse(y);
          regions[regionIndex].SourceRegion.Width = int.Parse(w);
          regions[regionIndex].SourceRegion.Height = int.Parse(h);

          // Process each region's placement and set up the unified coordinates
          calculateRegionPlacement(
            getHorizontalPlacementIndex(hplacement),
            int.Parse(w),
            this.leftBorderWidth,
            this.rightBorderWidth,
            ref regions[regionIndex].DestinationRegion.Location.X,
            ref regions[regionIndex].DestinationRegion.Size.X
          );
          calculateRegionPlacement(
            getVerticalPlacementIndex(vplacement),
            int.Parse(h),
            this.topBorderWidth,
            this.bottomBorderWidth,
            ref regions[regionIndex].DestinationRegion.Location.Y,
            ref regions[regionIndex].DestinationRegion.Size.Y
          );

        }

        return regions;
      }

      /// <summary>
      ///   Calculates the unified coordinates a region needs to be placed at
      /// </summary>
      /// <param name="placementIndex">
      ///   Placement index indicating where in a frame the region will be located
      /// </param>
      /// <param name="width">Width of the region in pixels</param>
      /// <param name="lowBorderWidth">
      ///   Width of the border on the lower end of the coordinate range
      /// </param>
      /// <param name="highBorderWidth">
      ///   Width of the border on the higher end of the coordinate range
      /// </param>
      /// <param name="location">
      ///   Receives the target location of the region in unified coordinates
      /// </param>
      /// <param name="size">
      ///   Receives the size of the region in unified coordinates
      /// </param>
      private void calculateRegionPlacement(
        int placementIndex, int width,
        int lowBorderWidth, int highBorderWidth,
        ref UniScalar location, ref UniScalar size
      ) {
        switch (placementIndex) {
          case (-1): { // left or top
            int gapWidth = lowBorderWidth - width;

            location.Fraction = 0.0f;
            location.Offset = (float)gapWidth;
            size.Fraction = 0.0f;
            size.Offset = (float)width;
            break;
          }
          case (+1): { // right or bottom
            location.Fraction = 1.0f;
            location.Offset = -(float)highBorderWidth;
            size.Fraction = 0.0f;
            size.Offset = (float)width;
            break;
          }
          case (0): { // stretch
            location.Fraction = 0.0f;
            location.Offset = (float)lowBorderWidth;
            size.Fraction = 1.0f;
            size.Offset = -(float)(highBorderWidth + lowBorderWidth);
            break;
          }
        }
      }

      /// <summary>Converts a horizontal placement string into a placement index</summary>
      /// <param name="placement">String containing the horizontal placement</param>
      /// <returns>A placement index that is equivalent to the provided string</returns>
      private int getHorizontalPlacementIndex(string placement) {
        switch (placement) {
          case "left": { return -1; }
          case "right": { return +1; }
          case "stretch":
          default: { return 0; }
        }
      }

      /// <summary>Converts a vertical placement string into a placement index</summary>
      /// <param name="placement">String containing the vertical placement</param>
      /// <returns>A placement index that is equivalent to the provided string</returns>
      private int getVerticalPlacementIndex(string placement) {
        switch (placement) {
          case "top": { return -1; }
          case "bottom": { return +1; }
          case "stretch":
          default: { return 0; }
        }
      }

      /// <summary>List of the XML nodes for the regions in the current frame</summary>
      private XmlNodeList regionNodes;
      /// <summary>Width of the frame's left border regions</summary>
      private int leftBorderWidth;
      /// <summary>Width of the frame's top border regions</summary>
      private int topBorderWidth;
      /// <summary>Width of the frame's right border regions</summary>
      private int rightBorderWidth;
      /// <summary>Width of the frame's bottom border regions</summary>
      private int bottomBorderWidth;

    }

    #endregion // class RegionListBuilder

    #region class TextListBuilder

    /// <summary>Builds a text list from the regions in an frame XML node</summary>
    private class TextListBuilder {

      /// <summary>
      ///   Builds a text list from the text placements specified in the provided node
      /// </summary>
      /// <param name="frameNode">
      ///   XML node for the frame whose text placements wille be processed
      /// </param>
      /// <param name="fonts">
      ///   Font lookup table used to associate a text's font id to the real font
      /// </param>
      /// <returns>
      ///   A list of the texts that have been extracted from the frame XML node
      /// </returns>
      public static Frame.Text[] Build(
        XmlNode frameNode, IDictionary<string, SpriteFont> fonts
      ) {
        XmlNodeList textNodes = frameNode.SelectNodes("text");

        Frame.Text[] texts = new Frame.Text[textNodes.Count];

        for (int index = 0; index < textNodes.Count; ++index) {
          string font = textNodes[index].Attributes["font"].Value;
          string horizontalPlacement = textNodes[index].Attributes["hplacement"].Value;
          string verticalPlacement = textNodes[index].Attributes["vplacement"].Value;

          XmlAttribute xOffsetAttribute = textNodes[index].Attributes["xoffset"];
          int xOffset = (xOffsetAttribute == null) ? 0 : int.Parse(xOffsetAttribute.Value);

          XmlAttribute yOffsetAttribute = textNodes[index].Attributes["yoffset"];
          int yOffset = (yOffsetAttribute == null) ? 0 : int.Parse(yOffsetAttribute.Value);

          XmlAttribute colorAttribute = textNodes[index].Attributes["color"];
          Color color;
          if (colorAttribute == null) {
            color = Color.White;
          } else {
            color = colorFromString(colorAttribute.Value);
          }

          texts[index].Font = fonts[font];
          texts[index].HorizontalPlacement = horizontalPlacementFromString(
            horizontalPlacement
          );
          texts[index].VerticalPlacement = verticalPlacementFromString(
            verticalPlacement
          );
          texts[index].Offset = new Point(xOffset, yOffset);
          texts[index].Color = color;
        }

        return texts;
      }

      /// <summary>Converts a string into a horizontal placement enumeration value</summary>
      /// <param name="placement">Placement string that will be converted</param>
      /// <returns>The horizontal placement enumeration value matching the string</returns>
      private static Frame.HorizontalTextAlignment horizontalPlacementFromString(
        string placement
      ) {
        switch (placement) {
          case "left": { return Frame.HorizontalTextAlignment.Left; }
          case "right": { return Frame.HorizontalTextAlignment.Right; }
          case "center":
          default: { return Frame.HorizontalTextAlignment.Center; }
        }
      }

      /// <summary>Converts a string into a vertical placement enumeration value</summary>
      /// <param name="placement">Placement string that will be converted</param>
      /// <returns>The vertical placement enumeration value matching the string</returns>
      private static Frame.VerticalTextAlignment verticalPlacementFromString(
        string placement
      ) {
        switch (placement) {
          case "top": { return Frame.VerticalTextAlignment.Top; }
          case "bottom": { return Frame.VerticalTextAlignment.Bottom; }
          case "center":
          default: { return Frame.VerticalTextAlignment.Center; }
        }
      }

    }

    #endregion // class TextListBuilder

    /// <summary>Loads a skin from the specified path</summary>
    /// <param name="skinStream">Stream containing the skin description</param>
    private void loadSkin(Stream skinStream) {

      // Load the schema
      XmlSchema schema;
      using (Stream schemaStream = getResourceStream("Resources.skin.xsd")) {
        schema = XmlHelper.LoadSchema(schemaStream);
      }

      // Load the XML document and validate it against the schema
      XmlDocument skinDocument = XmlHelper.LoadDocument(schema, skinStream);

      // The XML document is validated, we don't have to worry about the structure
      // of the thing anymore, only about the values it provides us with ;)
      // Load everything contained in the skin and set up our data structures
      // so we can efficiently render everything
      loadResources(skinDocument);
      loadFrames(skinDocument);

    }

    /// <summary>Loads the resources contained in a skin document</summary>
    /// <param name="skinDocument">
    ///   XML document containing a skin description whose resources will be loaded
    /// </param>
    private void loadResources(XmlDocument skinDocument) {

      // Load the fonts specified in the skin
      XmlNodeList fonts = skinDocument.SelectNodes("/skin/resources/font");
      for (int index = 0; index < fonts.Count; ++index) {
        string fontName = fonts[index].Attributes["name"].Value;
        string contentPath = fonts[index].Attributes["contentPath"].Value;

        SpriteFont spriteFont = this.contentManager.Load<SpriteFont>(contentPath);
        this.fonts.Add(fontName, spriteFont);
      }

      // Load the bitmaps specified in the skin
      XmlNodeList bitmaps = skinDocument.SelectNodes("/skin/resources/bitmap");
      for (int index = 0; index < bitmaps.Count; ++index) {
        string bitmapName = bitmaps[index].Attributes["name"].Value;
        string contentPath = bitmaps[index].Attributes["contentPath"].Value;

        Texture2D bitmap = this.contentManager.Load<Texture2D>(contentPath);
        this.bitmaps.Add(bitmapName, bitmap);
      }

    }

    /// <summary>Loads the frames contained in a skin document</summary>
    /// <param name="skinDocument">
    ///   XML document containing a skin description whose frames will be loaded
    /// </param>
    private void loadFrames(XmlDocument skinDocument) {

      // Extract all frames from the skin
      XmlNodeList frames = skinDocument.SelectNodes("/skin/frames/frame");
      for (int frameIndex = 0; frameIndex < frames.Count; ++frameIndex) {

        // Extract the frame's attributes
        string name = frames[frameIndex].Attributes["name"].Value;

        Frame.Region[] regions = RegionListBuilder.Build(frames[frameIndex], this.bitmaps);
        Frame.Text[] texts = TextListBuilder.Build(frames[frameIndex], this.fonts);
        this.frames.Add(name, new Frame(regions, texts));

      }
    }

    /// <summary>Returns a stream for a resource embedded in this assembly</summary>
    /// <param name="resourceName">Name of the resource for which to get a stream</param>
    /// <returns>A stream for the specified embedded resource</returns>
    private static Stream getResourceStream(string resourceName) {
      Assembly self = Assembly.GetCallingAssembly();
      string[] resources = self.GetManifestResourceNames();
      return self.GetManifestResourceStream(typeof(GuiManager), resourceName);
    }

    /// <summary>Converts a string in the style "#rrggbb" into a Color value</summary>
    /// <param name="color">String containing a hexadecimal color value</param>
    /// <returns>The equivalent color as a Color value</returns>
    private static Color colorFromString(string color) {
      string trimmedColor = color.Trim();

      int startIndex = 0;
      if (trimmedColor[0] == '#') {
        ++startIndex;
      }

      bool isValidColor =
        ((trimmedColor.Length - startIndex) == 6) ||
        ((trimmedColor.Length - startIndex) == 8);

      if (!isValidColor) {
        throw new ArgumentException("Invalid color format '" + color + "'", "color");
      }

      int r = Convert.ToInt32(trimmedColor.Substring(startIndex + 0, 2), 16);
      int g = Convert.ToInt32(trimmedColor.Substring(startIndex + 2, 2), 16);
      int b = Convert.ToInt32(trimmedColor.Substring(startIndex + 4, 2), 16);
      int a;
      if ((trimmedColor.Length - startIndex) == 8) {
        a = Convert.ToInt32(trimmedColor.Substring(startIndex + 6, 2), 16);
      } else {
        a = 255;
      }

      // No need to worry about overflows: two hexadecimal digits can
      // by definition not grow larger than 255 ;-)        
      return new Color((byte)r, (byte)g, (byte)b, (byte)a);
    }

  }

} // namespace Nuclex.UserInterface.Visuals.Flat

#endif // USE_XMLDOCUMENT