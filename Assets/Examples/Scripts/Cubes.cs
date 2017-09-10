using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cubes : MonoBehaviour
{
		// Cube structure
		private struct Cube
		{
				public Vector3 position;
				public Vector3 velocity;
				public Vector3 scale;
				public Vector4 rotation;
		}

		#region variables

		/// <summary>
		/// Compute shader updating the cubes's position
		/// </summary>
		[SerializeField] ComputeShader computeShader;

		/// <summary>
		/// Material drawing cubes on screen
		/// </summary>
		[SerializeField] Material material;

		/// <summary>
		/// Number of cubes
		/// </summary>
		public int cubeCount = 100;

		/// <summary>
		/// Attractor's position is used by the compute shader to influence cubes
		/// </summary>
		public Transform attractor;

		/// <summary>
		/// Buffer holding the cubes data and shared by the compute shader and material 
		/// </summary>
		private ComputeBuffer cubeBuffer;

		/// <summary>
		/// Size of the structure used for the buffer
		/// </summary>
		private const int structSize = 52;


		/// <summary>
		/// Number of cubes for one group
		/// </summary>
		private const int GROUP_SIZE = 256;

		/// <summary>
		/// Number of groups needed to process all cubes
		/// </summary>
		private int groupCount;

		/// <summary>
		/// Id to select the function inside the compute shader
		/// </summary>
		private int kernel;

		#endregion


		#region DrawMeshInstancedIndirect variables

		/// <summary>
		/// Buffer for the unity Graphics.DrawMeshInstancedIndirect function
		/// </summary>
		private ComputeBuffer argsBuffer;

		/// <summary>
		/// Mesh to draw with unity Graphics.DrawMeshInstancedIndirect function
		/// </summary>
		[SerializeField] Mesh mesh;

		/// <summary>
		/// The bounding volume surrounding the instances you intend to draw
		/// </summary>
		private Bounds bounds = new Bounds(Vector3.zero, new Vector3(100.0f, 100.0f, 100.0f));

		#endregion


		void Start()
		{
				// calculate the number of groups needed to handle all cubes
				if (cubeCount <= 0)
						cubeCount = 1;
				groupCount = Mathf.CeilToInt((float)cubeCount / GROUP_SIZE);

				// create and fill the array of cubes
				Cube[] cubeArray = new Cube[cubeCount];

				for (int i = 0; i < cubeCount; ++i)
				{
						cubeArray[i].position = 5 * Random.insideUnitSphere;

						Quaternion q = Random.rotation;
						Vector4 Randomrotation = new Vector4(q.x, q.y, q.z, q.w);

						cubeArray[i].rotation = Randomrotation;
						cubeArray[i].velocity = Vector3.zero;
						cubeArray[i].scale = Random.insideUnitSphere;//new Vector3(Random.Range(0.5f,1.0f), Random.Range(0.5f, 1.0f), Random.Range(0.5f, 1.0f));
				}

				// create the ComputeBuffer holding Cubes and set its data
				cubeBuffer = new ComputeBuffer(cubeCount, structSize);
				cubeBuffer.SetData(cubeArray);

				//select the kernel we want to use inside the compute shader
				kernel = computeShader.FindKernel("CSMain");

				// bind the ComputeBuffer to the shader and the compute shader
				computeShader.SetBuffer(kernel, "cubeBuffer", cubeBuffer);
				material.SetBuffer("cubeBuffer", cubeBuffer);

				// check if mesh isn't null and return its first index
				uint numIndices = (mesh != null) ? (uint)mesh.GetIndexCount(0) : 0;

				// init buffer for the DrawMeshInstancedIndirect function
				uint[] args = new uint[5] { numIndices, (uint)cubeCount, 0, 0, 0 };
				argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
				argsBuffer.SetData(args);
		}

		void Update()
		{
				// send data to the compute shader
				computeShader.SetFloat("deltaTime", Time.deltaTime);
				computeShader.SetVector("attractorPosition", attractor.position);

				// update the compute shader
				computeShader.Dispatch(kernel, groupCount, 1, 1);
		}

		/// <summary>
		/// Executed after Update()
		/// </summary>
		void OnRenderObject()
		{
				//draw cubes on screen
				Graphics.DrawMeshInstancedIndirect(mesh, 0, material, bounds, argsBuffer);
		}

		/// <summary>
		/// Release Buffers
		/// </summary>
		void OnDisable()
		{
				if (cubeBuffer != null)
						cubeBuffer.Release();

				if (argsBuffer != null)
						argsBuffer.Release();
		}
}
