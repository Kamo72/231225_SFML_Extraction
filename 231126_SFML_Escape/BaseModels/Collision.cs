using SFML.Graphics;
using SFML.System;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _231109_SFML_Test
{
    public interface ICollision : Drawable
    {
        bool IsCollision(ICollision other);
    }

    public class Box : RectangleShape, ICollision
    {
        public Box(Vector2f position, Vector2f size)
        {
            Position = position;
            Size = size;
        }

        public bool IsCollision(ICollision other)
        {
            if (other is Box box)
            {
                return Collision.CheckCollision(this, box);
            }
            else if (other is Circle circle)
            {
                return Collision.CheckCollision(this, circle);
            }
            else if (other is Line line)
            {
                return Collision.CheckCollision(this, line);
            }
            else if (other is Point point)
            {
                return Collision.CheckCollision(this, point);
            }

            return false;
        }
    }
    public class Circle : CircleShape, ICollision
    {
        public Circle(Vector2f position, float radius)
        {
            Position = position;
            Radius = radius;
        }

        public bool IsCollision(ICollision other)
        {
            if (other is Box box)
            {
                return Collision.CheckCollision(this, box);
            }
            else if (other is Circle circle)
            {
                return Collision.CheckCollision(this, circle);
            }
            else if (other is Line line)
            {
                return Collision.CheckCollision(this, line);
            }
            else if (other is Point point)
            {
                return Collision.CheckCollision(this, point);
            }

            return false;
        }
    }
    public class Line : Drawable, ICollision
    {
        public Line(Vector2f positionFrom, Vector2f positionTo)
        {
            this.positionFrom = positionFrom;
            this.positionTo = positionTo;
        }

        public Vector2f positionFrom;
        public Vector2f positionTo;

        public void Draw(RenderTarget target, RenderStates states)
        {
            ConvexShape convexShape = new ConvexShape(2);
            convexShape.SetPoint(0, positionFrom);
            convexShape.SetPoint(1, positionTo);
            convexShape.Draw(target, states);
            convexShape.Dispose();
        }

        public bool IsCollision(ICollision other)
        {
            if (other is Box box)
            {
                return Collision.CheckCollision(this, box);
            }
            else if (other is Circle circle)
            {
                return Collision.CheckCollision(this, circle);
            }
            else if (other is Line line)
            {
                return Collision.CheckCollision(this, line);
            }
            else if (other is Point point)
            {
                return Collision.CheckCollision(this, point);
            }

            return false;
        }
    }
    public class Point : Drawable, ICollision
    {
        public Point(float x, float y) 
        {
            position = new Vector2f(x, y);
        }
        public Vector2f position;

        public void Draw(RenderTarget target, RenderStates states)
        {
            CircleShape circleShape = new CircleShape(1f);
            circleShape.Position = position;
            circleShape.Draw(target, states);
            circleShape.Dispose();
        }

        public bool IsCollision(ICollision other)
        {
            if (other is Box box)
            {
                return Collision.CheckCollision(this, box);
            }
            else if (other is Circle circle)
            {
                return Collision.CheckCollision(this, circle);
            }
            else if (other is Line line)
            {
                return Collision.CheckCollision(this, line);
            }
            else if (other is Point point)
            {
                return Collision.CheckCollision(this, point);
            }

            return false;
        }
    }

    static class Collision
    {
        public static bool CheckCollision(Line line1, Line line2)
        {
            // Calculate the directional vectors of the two lines
            Vector2f dir1 = line1.positionFrom - line1.positionTo;
            Vector2f dir2 = line2.positionFrom - line2.positionTo;

            // Perform cross product calculations
            float cross1 = dir1.X * (line2.positionFrom.Y - line1.positionFrom.Y) - dir1.Y * (line2.positionFrom.X - line1.positionFrom.X);
            float cross2 = dir1.X * (line2.positionTo.Y - line1.positionFrom.Y) - dir1.Y * (line2.positionTo.X - line1.positionFrom.X);
            float cross3 = dir2.X * (line1.positionFrom.Y - line2.positionFrom.Y) - dir2.Y * (line1.positionFrom.X - line2.positionFrom.X);
            float cross4 = dir2.X * (line1.positionTo.Y - line2.positionFrom.Y) - dir2.Y * (line1.positionTo.X - line2.positionFrom.X);

            // Check for intersection between the two lines
            if ((cross1 * cross2 < 0) && (cross3 * cross4 < 0))
            {
                return true; // Collision detected
            }

            return false; // No collision
        }

        public static bool CheckCollision(Line line, Box rectangle)
        {
            FloatRect rectBounds = rectangle.GetGlobalBounds();

            // Check if the line intersects with any of the four sides of the rectangle
            if (line.positionFrom.X > rectBounds.Left && line.positionFrom.X < rectBounds.Left + rectBounds.Width &&
                line.positionFrom.Y > rectBounds.Top && line.positionFrom.Y < rectBounds.Top + rectBounds.Height)
                return true;

            if (line.positionTo.X > rectBounds.Left && line.positionTo.X < rectBounds.Left + rectBounds.Width &&
                line.positionTo.Y > rectBounds.Top && line.positionTo.Y < rectBounds.Top + rectBounds.Height)
                return true;

            return false;
        }

        public static bool CheckCollision(Line line, Circle circle)
        {
            Vector2f p1 = line.positionFrom;
            Vector2f p2 = line.positionTo;

            Vector2f center = circle.Position;
            float radius = circle.Radius;

            // Calculate the direction vector of the line segment
            Vector2f lineDir = p2 - p1;

            // Calculate the vector from one positionTopoint of the line segment to the circle center
            Vector2f circleToLine = center - p1;

            // Calculate the length of the line segment
            float lineLength = (float)Math.Sqrt(lineDir.X * lineDir.X + lineDir.Y * lineDir.Y);

            // Calculate the dot product of the line direction vector and the vector to the circle center
            float dotProduct = (circleToLine.X * lineDir.X + circleToLine.Y * lineDir.Y) / (lineLength * lineLength);

            // Calculate the closest point on the line segment to the circle center
            Vector2f closestPoint;
            if (dotProduct < 0.0f)
                closestPoint = p1;
            else if (dotProduct > 1.0f)
                closestPoint = p2;
            else
                closestPoint = p1 + dotProduct * lineDir;

            // Check if the distance between the closest point and the circle center is less than the circle radius
            Vector2f closestPointToCenter = center - closestPoint;
            float distance = (float)Math.Sqrt(closestPointToCenter.X * closestPointToCenter.X + closestPointToCenter.Y * closestPointToCenter.Y);
            return distance < radius;
        }

        public static bool CheckCollision(Line line, Point point)
        {
            // Check if the point is on the line segment
            float lineLengthSquared = (line.positionTo.X - line.positionFrom.X) * (line.positionTo.X - line.positionFrom.X) +
                (line.positionTo.Y - line.positionFrom.Y) * (line.positionTo.Y - line.positionFrom.Y);
            float dot1 = (point.position.X - line.positionFrom.X) * (line.positionTo.X - line.positionFrom.X) +
                (point.position.Y - line.positionFrom.Y) * (line.positionTo.Y - line.positionFrom.Y);
            float dot2 = (point.position.X - line.positionTo.X) * (line.positionFrom.X - line.positionTo.X) +
                (point.position.Y - line.positionTo.Y) * (line.positionFrom.Y - line.positionTo.Y);

            return dot1 >= 0 && dot2 >= 0 && dot1 <= lineLengthSquared && dot2 <= lineLengthSquared;
        }

        public static bool CheckCollision(Box rectangle, Line line)
        {
            return Collision.CheckCollision(line, rectangle);
        }

        public static bool CheckCollision(Box rectangle, Point point)
        {
            // Get the global bounds of the rectangle
            FloatRect rectBounds = rectangle.GetGlobalBounds();

            // Check if the point is inside the rectangle or on its edges
            if (rectBounds.Contains(point.position.X, point.position.Y))
                return true; // Collision detected

            return false; // No collision
        }

        public static bool CheckCollision(Box rectangle1, Box rectangle2)
        {
            FloatRect rectBounds1 = rectangle1.GetGlobalBounds();
            FloatRect rectBounds2 = rectangle2.GetGlobalBounds();

            return rectBounds1.Intersects(rectBounds2);
        }

        public static bool CheckCollision(Box rectangle, Circle circle)
        {
            Transform transform = new Transform();
            transform.Translate(rectangle.Position);
            transform.Rotate(rectangle.Rotation);

            FloatRect rectBounds = rectangle.GetLocalBounds();
            Vector2f rectCenter = new Vector2f(rectBounds.Width / 2.0f, rectBounds.Height / 2.0f);
            rectCenter = transform.TransformPoint(rectCenter);

            Vector2f circleCenter = circle.Position;
            circleCenter = transform.GetInverse().TransformPoint(circleCenter);

            float circleRadius = circle.Radius;

            Vector2f closestPoint;
            closestPoint.X = Math.Max(rectBounds.Left, Math.Min(circleCenter.X, rectBounds.Left + rectBounds.Width));
            closestPoint.Y = Math.Max(rectBounds.Top, Math.Min(circleCenter.Y, rectBounds.Top + rectBounds.Height));

            Vector2f distanceVec = circleCenter - closestPoint;
            float distanceSquared = distanceVec.X * distanceVec.X + distanceVec.Y * distanceVec.Y;

            return distanceSquared < (circleRadius * circleRadius);
        }

        public static bool CheckCollision(Circle circle, Line line)
        {
            return Collision.CheckCollision(line, circle);
        }

        public static bool CheckCollision(Circle circle, Point point)
        {
            Vector2f circleCenter = circle.Position;
            float circleRadius = circle.Radius;

            // Calculate the distance between the circle center and the point
            float distanceSquared = (circleCenter.X - point.position.X) * (circleCenter.X - point.position.X) +
                (circleCenter.Y - point.position.Y) * (circleCenter.Y - point.position.Y);

            // Check if the distance is less than the circle radius
            return distanceSquared <= (circleRadius * circleRadius);
        }

        public static bool CheckCollision(Circle circle, Box rectangle)
        {
            return Collision.CheckCollision(rectangle, circle);
        }

        public static bool CheckCollision(Circle circle1, Circle circle2)
        {
            Vector2f center1 = circle1.Position;
            Vector2f center2 = circle2.Position;
            float radius1 = circle1.Radius;
            float radius2 = circle2.Radius;

            // Calculate the distance between the centers of the circles
            float distanceSquared = (center1.X - center2.X) * (center1.X - center2.X) +
                (center1.Y - center2.Y) * (center1.Y - center2.Y);

            // Check if the distance is less than the sum of the radii
            return distanceSquared <= ((radius1 + radius2) * (radius1 + radius2));
        }

        public static bool CheckCollision(Point point, Line line)
        {
            return Collision.CheckCollision(line, point);
        }

        public static bool CheckCollision(Point point, Box rectangle)
        {
            return Collision.CheckCollision(rectangle, point);
        }

        public static bool CheckCollision(Point point, Circle circle)
        {
            return Collision.CheckCollision(circle, point);
        }

        public static bool CheckCollision(Point point1, Point point2)
        { 
            if(point1.position.X == point2.position.Y && point1.position.Y == point2.position.Y)
                return true;
            return false;
        }
    }
}
