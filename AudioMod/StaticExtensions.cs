using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioMod
{
    public static class StaticExtensions
    {
        /// <summary>
        /// Scales a mesh to be a maximum size in any axis
        /// </summary>
        /// <param name="mesh">The mesh to scale</param>
        /// <param name="max">The max size of this mesh in any axi</param>
        /// <returns>The mesh after scaling</returns>
        public static Mesh ScaleToMaxSize(this Mesh mesh, float max)
        {
            var baseVertices = mesh.vertices;
            var vertices = new Vector3[baseVertices.Length];
            var neededXScale = max / mesh.bounds.size.x;
            var neededYScale = max / mesh.bounds.size.y;
            var neededZScale = max / mesh.bounds.size.z;
            var neededScale = Math.Min(neededXScale, Math.Min(neededYScale, neededZScale));
            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i] = new Vector3(baseVertices[i].x * neededScale, baseVertices[i].y * neededScale, baseVertices[i].z * neededScale);
            }

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
