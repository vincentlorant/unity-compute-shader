/*
Greatly Inspired by antoinefournier/XParticle project
https://github.com/antoinefournier/XParticle
*/

using UnityEngine;

public class Particles : MonoBehaviour
{
		/// <summary>
		/// Cube structure 
		/// </summary>
		private struct Particle
		{
				public Vector3 position;
				public Vector3 velocity;
				public Vector3 scale;
		}

		#region variables

		/// <summary>
		/// Compute shader updating the particle's position
		/// </summary>
		[SerializeField] ComputeShader computeShader;

		/// <summary>
		/// Material drawing particles on screen
		/// </summary>
		[SerializeField] Material material;

		/// <summary>
		/// Number of particles
		/// </summary>
		public int particleCount = 100;

		/// <summary>
		/// Attractor's position is used by the compute shader to influence particles
		/// </summary>
		public Transform attractor;


		/// <summary>
		/// Buffer holding the particles data and shared by the compute shader and material 
		/// </summary>
		private ComputeBuffer particleBuffer;

		/// <summary>
		/// Size of the structure used for the buffer
		/// </summary>
		private const int structSize = 24;
		
		/// <summary>
		/// Number of particles for one group
		/// </summary>
		private const int GROUP_SIZE = 256;

		/// <summary>
		/// Number of groups needed to process all particles
		/// </summary>
		private int groupCount;

		/// <summary>
		/// Id to select the function inside the compute shader
		/// </summary>
		private int kernel;

		#endregion


		void Start()
		{
				// calculate the number of groups needed to handle all particles
				if (particleCount <= 0)
					particleCount = 1;
				groupCount = Mathf.CeilToInt((float)particleCount / GROUP_SIZE);
		
			// create and fill the array of particles
			Particle[] particleArray = new Particle[particleCount];

			for (int i = 0; i < particleCount; ++i)
			{
				particleArray[i].position =  Random.insideUnitSphere;
				particleArray[i].velocity = Vector3.zero;
				particleArray[i].scale = Random.insideUnitSphere;
			}

			// create the ComputeBuffer holding Particles and set its data
			particleBuffer = new ComputeBuffer(particleCount, structSize);
			particleBuffer.SetData(particleArray);
		
			//select the kernel we want to use inside the compute shader
			kernel = computeShader.FindKernel("CSMain");
			
			// bind the ComputeBuffer to the shader and the compute shader
			computeShader.SetBuffer(kernel, "particleBuffer", particleBuffer);
			material.SetBuffer("particleBuffer", particleBuffer);
		}

		void Update()
		{
				//send data to the compute shader
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
				//draw particles on screen
				material.SetPass(0);
				Graphics.DrawProcedural(MeshTopology.Points, 1, particleCount);
		}

		/// <summary>
		/// Release Buffer
		/// </summary>
		void OnDisable()
		{
				if (particleBuffer != null)
						particleBuffer.Release();
		}
}
