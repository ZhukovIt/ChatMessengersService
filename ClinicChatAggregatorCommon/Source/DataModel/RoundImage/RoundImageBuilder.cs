using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SiMed.Clinic.DataModel
{
    public sealed class RoundImageBuilder : AbstractImageBuilder
    {
        private Size m_Size;

        public RoundImageBuilder(Image Result, Size _Size) : base(Result)
        {
            m_Size = _Size;
        }

        public override void BuildRoundImage()
        {
            // Настройка стартовых параметров
            Image userPhoto = m_Result;
            Rectangle bounds = new Rectangle(0, 0, m_Size.Width + 2, m_Size.Height + 2);
            int roundRadius = bounds.Width / 2;
            Color shadowColor = Color.FromArgb(200, 200, 200);
            Color transparentColor = Color.Transparent;

            // Фото пользователя
            Image userImage = DrawAvatar(bounds, userPhoto, roundRadius, shadowColor, transparentColor);

            //Результирующее изображение
            Bitmap resultImage = new Bitmap(userImage.Width, userImage.Height);
            using (Graphics g = Graphics.FromImage(resultImage))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(userImage, 0, 0);
            }

            m_Result = resultImage;
        }

        public override void BuildRoundFrame()
        {
            Image userPhoto = m_Result;
            Rectangle bounds = new Rectangle(0, 0, userPhoto.Width, userPhoto.Height);

            using (Graphics graphics = Graphics.FromImage(userPhoto))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.DrawImage(userPhoto, 5, 5);

                //Рамка
                Pen pen = new Pen(Color.White, 2f);
                graphics.DrawEllipse(pen, new Rectangle(6, 6, bounds.Width - 12, bounds.Height - 12));
            }

            m_Result = userPhoto;
        }

        public override void BuildShadowInsideImage()
        {
            Image userPhoto = m_Result;
            Rectangle bounds = new Rectangle(0, 0, userPhoto.Width, userPhoto.Height);

            //Тень под фото
            Image shadowImage = new Bitmap(bounds.Width + 10, bounds.Height + 10);
            bounds = new Rectangle(0, 0, shadowImage.Width, shadowImage.Height);
            shadowImage = DrawAvatar(bounds, shadowImage, shadowImage.Width / 2, Color.Transparent, Color.FromArgb(220, 220, 220));

            using (Graphics g = Graphics.FromImage(userPhoto))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(shadowImage, 0, 0);
                g.DrawImage(userPhoto, 5, 5);
            }

            m_Result = userPhoto;
        }

        #region Низкоуровневые закрытые методы
        private Image DrawAvatar(Rectangle bounds, Image userPhoto, int roundRadius, Color shadowColor, Color transparentColor)
        {
            Bitmap image = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                // Изменяем размер под нужный
                userPhoto = new Bitmap(userPhoto, bounds.Width, bounds.Height);

                // Обрезаем фото
                graphics.DrawImage(CropRoundImage(bounds, userPhoto, bounds.Width / 2, Color.Transparent), 0, 0);
                /*using (GraphicsPath graphicsPath = new GraphicsPath())
                {
                    graphicsPath.AddEllipse(bounds);
                    using (PathGradientBrush pathGradientBrush = new PathGradientBrush(graphicsPath))
                    {
                        pathGradientBrush.CenterPoint = new PointF(bounds.Width / 2, bounds.Height / 2);
                        pathGradientBrush.CenterColor = transparentColor;
                        pathGradientBrush.SurroundColors = new Color[] { shadowColor };
                        pathGradientBrush.FocusScales = new PointF(0.8f, 0.8f); //начало перехода градиента (ширина градиента)

                        graphics.FillPath(pathGradientBrush, graphicsPath);
                    }
                }*/
            }
            return image;
        }

        private Image CropRoundImage(Rectangle bounds, Image startImage, int radius, Color backgroundColor)
        {
            radius *= 2;
            Bitmap roundedImage = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics graphics = Graphics.FromImage(roundedImage))
            {
                graphics.Clear(backgroundColor);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                using (Brush brush = new TextureBrush(startImage))
                {
                    using (GraphicsPath graphicsPath = new GraphicsPath())
                    {
                        graphicsPath.AddArc(0, 0, radius, radius, 180, 90);
                        graphicsPath.AddArc(bounds.Width - radius - 0, 0, radius, radius, 270, 90);
                        graphicsPath.AddArc(0 + bounds.Width - radius, 0 + bounds.Height - radius, radius, radius, 0, 90);
                        graphicsPath.AddArc(0, bounds.Height - radius, radius, radius, 90, 90);

                        graphics.FillPath(brush, graphicsPath);
                    }
                }

                return roundedImage;
            }
        }
        #endregion
    }
}
