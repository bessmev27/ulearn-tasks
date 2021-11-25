
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inheritance.Geometry.Visitor
{
    public abstract class Body : IVisitable
    {
        public Vector3 Position { get; }

        protected Body(Vector3 position)
        {
            Position = position;
        }

        public abstract object Accept(IVisitor bodyVisitor);
    }

    public interface IVisitable
    {
        object Accept(IVisitor bodyVisitor);
    }

    public interface IVisitor
    {
        object VisitBall(Ball b);
        object VisitRectangularCuboid(RectangularCuboid rc);
        object VisitCylinder(Cylinder c);
        object VisitCompound(CompoundBody cb);
    }

    public class Ball : Body
    {
        public double Radius { get; }

        public Ball(Vector3 position, double radius) : base(position)
        {
            Radius = radius;
        }

        public override object Accept(IVisitor bodyVisitor)
        {
            return bodyVisitor.VisitBall(this);
        }
    }

    public class RectangularCuboid : Body
    {
        public double SizeX { get; }
        public double SizeY { get; }
        public double SizeZ { get; }

        public RectangularCuboid(Vector3 position, double sizeX, double sizeY, double sizeZ) : base(position)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;
        }

        public override object Accept(IVisitor bodyVisitor)
        {
            return bodyVisitor.VisitRectangularCuboid(this);
        }
    }

    public class Cylinder : Body
    {
        public double SizeZ { get; }

        public double Radius { get; }

        public Cylinder(Vector3 position, double sizeZ, double radius) : base(position)
        {
            SizeZ = sizeZ;
            Radius = radius;
        }

        public override object Accept(IVisitor bodyVisitor)
        {
            return bodyVisitor.VisitCylinder(this);
        }
    }

    public class CompoundBody : Body
    {
        public IReadOnlyList<Body> Parts { get; }

        public CompoundBody(IReadOnlyList<Body> parts) : base(parts[0].Position)
        {
            Parts = parts;
        }

        public override object Accept(IVisitor bodyVisitor)
        {
            return bodyVisitor.VisitCompound(this);
        }
    }

    public class BoundingBoxVisitor : IVisitor
    {
        //TODO
        public object VisitBall(Ball b)
        {
            var length = b.Radius * 2;
            return new RectangularCuboid(b.Position, length, length, length);
        }

        public object VisitCompound(CompoundBody cb)
        {
            var boundingBoxes = cb.Parts.Select(part => (RectangularCuboid)part.Accept(this));
            var (minPosX, maxPosX) = boundingBoxes.GetRectangularListMaxAndMin(
                x => x.SizeX, x => x.Position.X);
            var (minPosY, maxPosY) = boundingBoxes.GetRectangularListMaxAndMin(
                x => x.SizeY, x => x.Position.Y);
            var (minPosZ, maxPosZ) = boundingBoxes.GetRectangularListMaxAndMin(
                x => x.SizeZ, x => x.Position.Z);

            var position = new Vector3((minPosX + maxPosX) / 2, (minPosY + maxPosY) / 2, (minPosZ + maxPosZ) / 2);
            return new RectangularCuboid(position, maxPosX - minPosX, maxPosY - minPosY, maxPosZ - minPosZ);
        }


        public object VisitCylinder(Cylinder c)
        {
            return new RectangularCuboid
            (
            c.Position,
            c.Radius * 2,
            c.Radius * 2,
            c.SizeZ
            );
        }

        public object VisitRectangularCuboid(RectangularCuboid rc)
        {
            return new RectangularCuboid(rc.Position, rc.SizeX, rc.SizeY, rc.SizeZ);
        }
    }

    public class BoxifyVisitor : IVisitor
    {
        //TODO
        public object VisitBall(Ball b)
        {
            return new BoundingBoxVisitor().VisitBall(b);
        }

        public object VisitCompound(CompoundBody cb)
        {
            var parts = cb.Parts.Select(p =>
            {
                if (p is CompoundBody cmpb) return (Body)(CompoundBody)VisitCompound(cmpb);
                return (RectangularCuboid)p.Accept(this);
            })
            .ToList();

            return new CompoundBody(parts);
        }

        public object VisitCylinder(Cylinder c)
        {
            return new BoundingBoxVisitor().VisitCylinder(c);
        }

        public object VisitRectangularCuboid(RectangularCuboid rc)
        {
            return new BoundingBoxVisitor().VisitRectangularCuboid(rc);
        }
    }

    public static class RectangularExtensions
    {
        public static (double, double) GetRectangularListMaxAndMin(
            this IEnumerable<RectangularCuboid> rectangularCuboids,
            Func<RectangularCuboid, double> sizeSelector,
            Func<RectangularCuboid, double> positionSelector)
        {
            var positions = rectangularCuboids.SelectMany(
                box =>
                {
                    var halfSize = sizeSelector(box) / 2;
                    var position = positionSelector(box);
                    return new[] { position + halfSize, position - halfSize };
                });

            return (positions.Min(), positions.Max());
        }
    }
}