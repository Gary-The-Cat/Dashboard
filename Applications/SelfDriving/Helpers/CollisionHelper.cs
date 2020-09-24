using SelfDriving.Shared;
using SFML.Graphics;
using SFML.System;
using Shared.CollisionData;
using Shared.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelfDriving.Helpers
{
    public class CollisionHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="raycasts"></param>
        /// <param name="map"></param>
        /// <returns>A list of normalised collision distances, and the collidion points themselves.</returns>
        public static (float[], Vector2f?[]) GetRaycastCollisions(List<LineSegment> raycasts, List<LineSegment> map)
        {
            var colliding = new HashSet<LineSegment>();
            var collisionPoints = new Vector2f?[raycasts.Count];
            var collisionDistances = new float[raycasts.Count()];
            for (int i = 0; i < raycasts.Count; i++)
            {
                collisionDistances[i] = 1;
            }

            for (int i = 0; i < raycasts.Count; i++)
            {
                var ray = raycasts[i];
                var p2 = ray.Start;
                var p3 = ray.End;
                foreach (var line in map)
                {
                    var p0 = line.Start;
                    var p1 = line.End;
                    var collisionPoint = CollisionManager.CheckCollision(p0, p1, p2, p3);
                    if (collisionPoint != null)
                    {
                        colliding.Add(ray);
                        var rayLength = Math.Pow(p2.X - p3.X, 2) + Math.Pow(p2.Y - p3.Y, 2);
                        var hitLength = Math.Pow(p2.X - collisionPoint.Value.X, 2) + Math.Pow(p2.Y - collisionPoint.Value.Y, 2);
                        var hitDistance = (float)(hitLength / rayLength);
                        if (hitDistance < collisionDistances[i])
                        {
                            collisionDistances[i] = hitDistance;
                            collisionPoints[i] = collisionPoint;
                        }
                    }
                }
            }

            return (collisionDistances, collisionPoints);
        }
    }
}
