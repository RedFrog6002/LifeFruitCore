using Terraria.ModLoader;
using Terraria;
using ReLogic.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Localization;
using System.IO;
using System;
using System.Collections.Generic;

namespace LifeFruitCore
{
    public class LifeFruitCore : Mod
    {
        public override void Load()
        {
			blankheart = GetTexture("blankheart");

			On.Terraria.Main.DrawInterface_Resources_Life += Main_DrawInterface_Resources_Life;
		}

        public override void Unload()
		{
			On.Terraria.Main.DrawInterface_Resources_Life -= Main_DrawInterface_Resources_Life;
		}

        public static void AddFruits(Func<int, Color> func)
		{
			ColorFuncs.Add(func);
		}

		public static void AddFruits(Func<int, Texture2D> func)
		{
			TexFuncs.Add(func);
		}

		public static void AddFruits(Func<int, (Color, Texture2D)> func)
		{
			ColorTexFuncs.Add(func);
		}

		private static List<Func<int, Color>> ColorFuncs = new List<Func<int, Color>>();

		private static List<Func<int, Texture2D>> TexFuncs = new List<Func<int, Texture2D>>();

		private static List<Func<int, (Color, Texture2D)>> ColorTexFuncs = new List<Func<int, (Color, Texture2D)>>();

		private Color GetColor(int i)
        {
			if (ColorFuncs.Count == 0)
            {
				return Color.Transparent;
            }
            else
            {
				Color tmpcolor = Color.Transparent;
				foreach (Func<int, Color> func in ColorFuncs)
                {
					tmpcolor = func(i);
					if (tmpcolor != Color.Transparent)
						return tmpcolor;
                }
				return tmpcolor;
            }
		}

		private Texture2D GetTex(int i)
		{
			if (TexFuncs.Count == 0)
			{
				return null;
			}
			else
			{
				Texture2D tmptex = null;
				foreach (Func<int, Texture2D> func in TexFuncs)
				{
					tmptex = func(i);
					if (tmptex != null)
						return tmptex;
				}
				return tmptex;
			}
		}

		private Texture2D GetColorTex(int i, out Color color)
		{
			color = Color.Transparent;
			if (TexFuncs.Count == 0)
			{
				return null;
			}
			else
			{
				Texture2D tmptex = null;
				foreach (Func<int, (Color, Texture2D)> func in ColorTexFuncs)
				{
					(Color tmpcolor, Texture2D tex) = func(i);
					tmptex = tex;
					color = tmpcolor;
					if (tmptex != null)
						return tmptex;
				}
				return tmptex;
			}
		}

		private static Texture2D blankheart;
		private void Main_DrawInterface_Resources_Life(On.Terraria.Main.orig_DrawInterface_Resources_Life orig)
		{
			float UIDisplay_LifePerHeart = 20f;
			int UI_ScreenAnchorX = Main.screenWidth - 800;
			if (Main.LocalPlayer.ghost)
			{
				return;
			}
			int hearts = Main.player[Main.myPlayer].statLifeMax / 20;
			int fruit = (Main.player[Main.myPlayer].statLifeMax - 400) / 5;
			if (fruit < 0)
			{
				fruit = 0;
			}
			if (fruit > 0)
			{
				hearts = Main.player[Main.myPlayer].statLifeMax / (20 + fruit / 4);
				UIDisplay_LifePerHeart = (float)Main.player[Main.myPlayer].statLifeMax / 20f;
			}
			int heartbuffdiff = Main.player[Main.myPlayer].statLifeMax2 - Main.player[Main.myPlayer].statLifeMax;
			UIDisplay_LifePerHeart += (float)(heartbuffdiff / hearts);
			int totalhearts = (int)((float)Main.player[Main.myPlayer].statLifeMax2 / UIDisplay_LifePerHeart);
			if (totalhearts >= 10)
			{
				totalhearts = 10;
			}
			string text = string.Concat(new string[]
			{
				Lang.inter[0].Value,
				" ",
				Main.player[Main.myPlayer].statLifeMax2.ToString(),
				"/",
				Main.player[Main.myPlayer].statLifeMax2.ToString()
			});
			Vector2 vector = Main.fontMouseText.MeasureString(text);
			if (!Main.player[Main.myPlayer].ghost)
			{
				DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Lang.inter[0].Value, new Vector2((float)(500 + 13 * totalhearts) - vector.X * 0.5f + (float)UI_ScreenAnchorX, 6f), new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
				DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, Main.fontMouseText, Main.player[Main.myPlayer].statLife.ToString() + "/" + Main.player[Main.myPlayer].statLifeMax2.ToString(), new Vector2((float)(500 + 13 * totalhearts) + vector.X * 0.5f + (float)UI_ScreenAnchorX, 6f), new Color((int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor, (int)Main.mouseTextColor), 0f, new Vector2(Main.fontMouseText.MeasureString(Main.player[Main.myPlayer].statLife.ToString() + "/" + Main.player[Main.myPlayer].statLifeMax2.ToString()).X, 0f), 1f, SpriteEffects.None, 0f);
			}
			for (int i = 1; i < (int)((float)Main.player[Main.myPlayer].statLifeMax2 / UIDisplay_LifePerHeart) + 1; i++)
			{
				float scale = 1f;
				bool flag = false;
				int heartcolor;
				if ((float)Main.player[Main.myPlayer].statLife >= (float)i * UIDisplay_LifePerHeart)
				{
					heartcolor = 255;
					if ((float)Main.player[Main.myPlayer].statLife == (float)i * UIDisplay_LifePerHeart)
					{
						flag = true;
					}
				}
				else
				{
					float percentofheart = ((float)Main.player[Main.myPlayer].statLife - (float)(i - 1) * UIDisplay_LifePerHeart) / UIDisplay_LifePerHeart;
					heartcolor = (int)(30f + 225f * percentofheart);
					if (heartcolor < 30)
					{
						heartcolor = 30;
					}
					scale = percentofheart / 4f + 0.75f;
					if ((double)scale < 0.75)
					{
						scale = 0.75f;
					}
					if (percentofheart > 0f)
					{
						flag = true;
					}
				}
				if (flag)
				{
					scale += Main.cursorScale - 1f;
				}
				int xmod = 0;
				int ymod = 0;
				if (i > 10)
				{
					xmod -= 260;
					ymod += 26;
				}
				int a = (int)((double)((float)heartcolor) * 0.9);
				Texture2D tex = GetColorTex(i, out Color colormod);
				if (colormod == Color.Transparent)
                {
					tex = null;
					colormod = GetColor(i);
				}
				if (tex == null)
				{
					tex = GetTex(i);
					if (tex != null)
						colormod = Color.White;
				}
				if (!Main.player[Main.myPlayer].ghost)
				{
                    #region Custom Color Stuff
                    if (colormod != Color.Transparent)
					{
						/*
						int colorR = colormod.R < heartcolor ? colormod.R : heartcolor;
						int colorG = colormod.G < heartcolor ? colormod.G : heartcolor;
						int colorB = colormod.B < heartcolor ? colormod.B : heartcolor;
						*/
						int colorR = colormod.R * heartcolor / 255;
						int colorG = colormod.G * heartcolor / 255;
						int colorB = colormod.B * heartcolor / 255;
						if (fruit > 0)
						{
							fruit--;
							Main.spriteBatch.Draw(
								(tex != null) ? tex : blankheart,
								new Vector2(
									(float)(500 + 26 * (i - 1) + xmod + UI_ScreenAnchorX + Main.heartTexture.Width / 2),
									32f + ((float)Main.heartTexture.Height - (float)Main.heartTexture.Height * scale) / 2f + (float)ymod + (float)(Main.heartTexture.Height / 2)),
								new Rectangle?(new Rectangle(
									0,
									0,
									Main.heartTexture.Width,
									Main.heartTexture.Height)),
								new Color(
									colorR,
									colorG,
									colorB,
									a),
								0f,
								new Vector2(
									(float)(Main.heartTexture.Width / 2),
									(float)(Main.heartTexture.Height / 2)),
								scale,
								SpriteEffects.None,
								0f);
						}
						else
						{
							Main.spriteBatch.Draw(
								(tex != null) ? tex : blankheart,
								new Vector2(
									(float)(500 + 26 * (i - 1) + xmod + UI_ScreenAnchorX + Main.heartTexture.Width / 2),
									32f + ((float)Main.heartTexture.Height - (float)Main.heartTexture.Height * scale) / 2f + (float)ymod + (float)(Main.heartTexture.Height / 2)),
								new Rectangle?(new Rectangle(
									0,
									0,
									Main.heartTexture.Width,
									Main.heartTexture.Height)),
								new Color(
									colorR,
									colorG,
									colorB,
									a),
								0f,
								new Vector2(
									(float)(Main.heartTexture.Width / 2),
									(float)(Main.heartTexture.Height / 2)),
								scale,
								SpriteEffects.None,
								0f);
						}
					}
                    #endregion
                    else
                    {
						if (fruit > 0)
						{
							fruit--;
							Main.spriteBatch.Draw(
								Main.heart2Texture,
								new Vector2(
									(float)(500 + 26 * (i - 1) + xmod + UI_ScreenAnchorX + Main.heartTexture.Width / 2),
									32f + ((float)Main.heartTexture.Height - (float)Main.heartTexture.Height * scale) / 2f + (float)ymod + (float)(Main.heartTexture.Height / 2)),
								new Rectangle?(new Rectangle(
									0,
									0,
									Main.heartTexture.Width,
									Main.heartTexture.Height)),
								new Color(
									heartcolor,
									heartcolor,
									heartcolor,
									a),
								0f,
								new Vector2(
									(float)(Main.heartTexture.Width / 2),
									(float)(Main.heartTexture.Height / 2)),
								scale,
								SpriteEffects.None,
								0f);
						}
						else
						{
							Main.spriteBatch.Draw(
								Main.heartTexture,
								new Vector2(
									(float)(500 + 26 * (i - 1) + xmod + UI_ScreenAnchorX + Main.heartTexture.Width / 2),
									32f + ((float)Main.heartTexture.Height - (float)Main.heartTexture.Height * scale) / 2f + (float)ymod + (float)(Main.heartTexture.Height / 2)),
								new Rectangle?(new Rectangle(
									0,
									0,
									Main.heartTexture.Width,
									Main.heartTexture.Height)),
								new Color(
									heartcolor,
									heartcolor,
									heartcolor,
									a),
								0f,
								new Vector2(
									(float)(Main.heartTexture.Width / 2),
									(float)(Main.heartTexture.Height / 2)),
								scale,
								SpriteEffects.None,
								0f);
						}
					}
				}
			}
		}
    }
}