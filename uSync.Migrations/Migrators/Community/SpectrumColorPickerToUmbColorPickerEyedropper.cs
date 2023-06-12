

[SyncMigrator("Spectrum.Color.Picker")]
  public class SpectrumColorPickerToUmbColorPickerEyedropper : SyncPropertyMigratorBase
  {


    public override string GetEditorAlias(SyncMigrationDataTypeProperty dataTypeProperty, SyncMigrationContext context) => Constants.PropertyEditors.Aliases.ColorPickerEyeDropper;

    public override object? GetConfigValues(SyncMigrationDataTypeProperty dataTypeProperty, SyncMigrationContext context)
    {
      bool enableTransparency = false;
      var config = new EyeDropperColorPickerConfiguration();
      if (dataTypeProperty.PreValues == null) return config;

      foreach (var prevalue in dataTypeProperty.PreValues)
      {
        if (prevalue.Alias == "enableTransparency")
        {
          if (prevalue.Value == "1")
          {
            enableTransparency = true;
          }
        }
      }
      config.ShowAlpha = enableTransparency;
      config.ShowPalette = true;

      return config;
    }

    public override string? GetContentValue(SyncMigrationContentProperty contentProperty, SyncMigrationContext context)
    {
      if (string.IsNullOrWhiteSpace(contentProperty.Value)) return null;

      if (contentProperty.Value.ToLower().IndexOf("hsl") == 0) return HexFromHSL(contentProperty.Value);
      if (contentProperty.Value.ToLower().IndexOf("rgb") == 0) return HexOrRGBAFromRGB(contentProperty.Value);
      if (contentProperty.Value.ToLower().IndexOf("#") == 0 && contentProperty.Value.Length < 5) return contentProperty.Value.Insert(2, "0").Insert(4, "0").Insert(6, "0");
      if (contentProperty.Value.ToLower().IndexOf("#") == 0) return contentProperty.Value;
      return HexFromName(contentProperty.Value);
    }


    string HexOrRGBAFromRGB(string rgb)
    {
      var rgbPart = rgb.Substring(rgb.IndexOf('(') + 1).Trim(')');
      var rgbarr = rgbPart.Split(',');
      if (rgbarr.Length > 3)
      {
        return rgb;
      }
      return "#" + int.Parse(rgbarr[0]).ToString("X2") + int.Parse(rgbarr[1]).ToString("X2") + int.Parse(rgbarr[2]).ToString("X2");
    }

    string HexFromName(string name)
    {
      if (name.IndexOf('#') == 0) return name;
      var colour = Color.FromName(name);
      if (colour.IsKnownColor) return "#" + colour.R.ToString("X2") + colour.G.ToString("X2") + colour.B.ToString("X2");
      return "";


    }

    string HexFromHSL(string hsl)
    {
      hsl = hsl.Substring(hsl.IndexOf('(') + 1).Trim(')');
      var hslarr = hsl.Split(",");
      float hue = float.Parse(hslarr[0]);
      float saturation = float.Parse(hslarr[1].Trim('%')) / 100;
      float lightness = float.Parse(hslarr[2].Trim('%')) / 100;

      float max = lightness < 0.5f ? lightness * (1 + saturation) : (lightness + saturation) - (lightness * saturation);
      float min = 2 * lightness - max;
      hue = hue / 360f;

      return "#" + int.Parse(HueToRGB(min, max, hue + 1 / 3f)).ToString("X2") + int.Parse(HueToRGB(min, max, hue)).ToString("X2") + int.Parse(HueToRGB(min, max, hue - 1 / 3f)).ToString("X2");
    }

    string HueToRGB(float p, float q, float t)
    {
      if (t < 0) t++;
      if (t > 1) t--;
      if (t < 1 / 6f) return Math.Round((p + (q - p) * 6 * t) * 255).ToString();
      if (t < 1 / 2f) return Math.Round(q * 255).ToString();
      if (t < 2 / 3f) return Math.Round(p + (q - p) * (2 / 3f - t) * 6 * 255).ToString();
      return Math.Round(p * 255).ToString();
    }
  }
