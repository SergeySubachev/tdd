﻿using FluentAssertions;
using NUnit.Framework;
using System;
using System.Drawing;

namespace TagsCloudVisualization
{
    public static class Utils
    {
        private static bool IsApproximatelyEquals(this double me, double val)
        {
            return Math.Abs(me - val) < 1E-6;
        }

        private static bool IsApproximatelyMoreThan(this double me, double val)
        {
            return me > val || me.IsApproximatelyEquals(val);
        }

        private static bool IsApproximatelyLessThan(this double me, double val)
        {
            return me < val || me.IsApproximatelyEquals(val);
        }

        private class Segment
        {
            public Point p1, p2;
            public Segment(Point p1, Point p2)
            {
                this.p1 = p1;
                this.p2 = p2;
            }
        }

        /// <summary>
        /// Пересекает ли указанный прямоугольник луч из центра координат под углом rayAngle
        /// (проверяются дальние от начала координат стороны прямоугольника)
        /// </summary>
        /// <param name="rayAngle">Угол ПО ЧАСОВОЙ стрелке, т.к. ордината идет вниз, рад</param>
        public static bool IsRayIntersectsRectangle(Rectangle rect, double rayAngle, out Point IntersectionPoint)
        {
            var vLeftTop = new Point(rect.Left, rect.Top);
            var vRightTop = new Point(rect.Right, rect.Top);
            var vRightBottom = new Point(rect.Right, rect.Bottom);
            var vLeftBottom = new Point(rect.Left, rect.Bottom);

            Segment hSeg, vSeg;
            if (rayAngle >= 1.5 * Math.PI)
            {
                hSeg = new Segment(vLeftTop, vRightTop);
                vSeg = new Segment(vRightTop, vRightBottom);
            }
            else if (rayAngle >= Math.PI)
            {
                hSeg = new Segment(vLeftTop, vRightTop);
                vSeg = new Segment(vLeftTop, vLeftBottom);
            }
            else if (rayAngle >= 0.5 * Math.PI)
            {
                hSeg = new Segment(vLeftBottom, vRightBottom);
                vSeg = new Segment(vLeftTop, vLeftBottom);
            }
            else
            {
                hSeg = new Segment(vLeftBottom, vRightBottom);
                vSeg = new Segment(vRightTop, vRightBottom);
            }

            //IsRayIntersectsHorizontal
            if (rayAngle.IsApproximatelyEquals(1.5 * Math.PI) && hSeg.p1.Y < 0)
            {
                if (hSeg.p1.X <= 0 && hSeg.p2.X >= 0)
                {
                    IntersectionPoint = new Point(0, hSeg.p1.Y);
                    return true;
                }
            }
            else if (rayAngle.IsApproximatelyEquals(0.5 * Math.PI) && hSeg.p1.Y > 0)
            {
                if (hSeg.p1.X <= 0 && hSeg.p2.X >= 0)
                {
                    IntersectionPoint = new Point(0, hSeg.p1.Y);
                    return true;
                }
            }
            else if ((rayAngle > Math.PI && hSeg.p1.Y < 0) || (rayAngle < Math.PI && hSeg.p1.Y > 0))
            {
                double x = hSeg.p1.Y / Math.Tan(rayAngle);
                if (x.IsApproximatelyMoreThan(hSeg.p1.X) && x.IsApproximatelyLessThan(hSeg.p2.X))
                {
                    IntersectionPoint = new Point((int)x, hSeg.p1.Y);
                    return true;
                }
            }

            //IsRayIntersectsVertical
            if (rayAngle.IsApproximatelyEquals(Math.PI) && vSeg.p1.X < 0)
            {
                if (vSeg.p1.Y <= 0 && vSeg.p2.Y >= 0)
                {
                    IntersectionPoint = new Point(vSeg.p1.X, 0);
                    return true;
                }
            }
            else if (rayAngle.IsApproximatelyEquals(0) && vSeg.p1.X > 0)
            {
                if (vSeg.p1.Y <= 0 && vSeg.p2.Y >= 0)
                {
                    IntersectionPoint = new Point(vSeg.p1.X, 0);
                    return true;
                }
            }
            else if ((rayAngle > Math.PI && vSeg.p1.Y < 0) || (rayAngle < Math.PI && vSeg.p1.Y > 0))
            {
                double x = vSeg.p1.Y / Math.Tan(rayAngle);
                if (x.IsApproximatelyMoreThan(vSeg.p1.X) && x.IsApproximatelyLessThan(vSeg.p2.X))
                {
                    IntersectionPoint = new Point((int)x, vSeg.p1.Y);
                    return true;
                }
            }

            IntersectionPoint = default;
            return false;
        }
    }

    [TestFixture]
    public class Utils_Should
    {
        [TestCase(50, -5, 0)]
        [TestCase(-5, 50, Math.PI / 2)]
        [TestCase(-50, -5, Math.PI)]
        [TestCase(-5, -50, 1.5 * Math.PI)]
        [TestCase(50, 50, Math.PI / 4)]
        [TestCase(-50, 50, Math.PI * 3 / 4)]
        [TestCase(-50, -50, Math.PI * 1.25)]
        [TestCase(50, -50, 1.76 * Math.PI)]
        public void IsRayIntersectsRectangle_ShouldReturnTrue(int rectX, int rectY, double rayAngle)
        {
            Utils.IsRayIntersectsRectangle(new Rectangle(rectX, rectY, 10, 10), rayAngle, out Point _).Should().BeTrue();
        }
    }
}