using System.Collections.Generic;
using GPUInstancedGrass.Common;
using UnityEngine;
using UnityEngine.Rendering;

namespace GPUInstancedGrass {
	public class InstancedGrassDrawer : MonoBehaviour {
	
		private static readonly int PositionsShaderProperty = Shader.PropertyToID("PositionsBuffer");

		[SerializeField]
		private Mesh _instanceMesh;
		[SerializeField]
		private Material _instanceMaterial;

		private Bounds _grassBounds;

		private ComputeBuffer _positionBuffer;
		private List<Vector3> _positions;
		private int _positionsCount;
		public TerrainData[] terrainData;
		public Transform TargetObject;
		public float DrawDistance;
		private List<Vector3> _acceptedPositions = new List<Vector3>();
		private Vector3 _previousPlayerPosition;
		public List<int> GrassIndexes = new List<int>();
		private List<TreeInstance[]> _savedTerrainData = new List<TreeInstance[]>();
		public float OffsetYAxis;
		public float GrassCenterOffset;
		public int GrassCounter;
		/*private GridCell _bottomLeftCameraCell;
	private GridCell _topRightCameraCell;*/

		public void Start()
		{
			_grassBounds = new Bounds(Vector3.zero, new Vector3(4000, 4000));
			_positions = new List<Vector3>();
			List<TreeInstance> _otherInstances = new List<TreeInstance>();
			foreach (TerrainData terrData in terrainData)
			{
				_savedTerrainData.Add(terrData.treeInstances);
				foreach (var elem in terrData.treeInstances)
				{
					if (!GrassIndexes.Contains(elem.prototypeIndex))
					{
						_otherInstances.Add(elem);
					}
					var treeInstancePos = elem.position;
					var localPos = new Vector3(treeInstancePos.x * terrData.size.x, treeInstancePos.y * terrData.size.y, treeInstancePos.z * terrData.size.z);

					var worldPos = Terrain.activeTerrain.transform.TransformPoint(localPos);
					_acceptedPositions.Add(worldPos);
				}

				terrData.treeInstances = _otherInstances.ToArray();
				_otherInstances.Clear();
			}
			
		}


		public void UpdatePositions() {
			_positions.Clear();
			foreach (var elem in _acceptedPositions)
			{
				if (Vector3.Distance(TargetObject.transform.position, elem) <= DrawDistance)
				{
					for (int i = -GrassCounter/2;i<(GrassCounter/2)+1;i++)
					{
						for (int j = -GrassCounter/2;j<(GrassCounter/2)+1;j++)
						{
							Vector3 pos = elem + new Vector3(GrassCenterOffset * i, 0, GrassCenterOffset * j);
							if (Vector3.Distance(elem, pos) <= GrassCenterOffset*(GrassCounter/2))
							{
								_positions.Add(pos - new Vector3(0, OffsetYAxis, 0) + new Vector3(Random.Range(-0.1f, 0.1f), 0, Random.Range(-0.1f, 0.1f)));
							}
						}
					}
				}
			}
			_positionsCount = _positions.Count;
			_positionBuffer?.Release();
			if (_positionsCount == 0) return;
			_positionBuffer = new ComputeBuffer(_positionsCount, 12);
			_positionBuffer.SetData(_positions);
			_instanceMaterial.SetBuffer(PositionsShaderProperty, _positionBuffer);
		}

		private void Update() {
			if (_previousPlayerPosition != TargetObject.position)
			{
				UpdatePositions();
				_previousPlayerPosition = TargetObject.position;
			}
			if (_positionsCount == 0) return;
			Graphics.DrawMeshInstancedProcedural(_instanceMesh, 0, _instanceMaterial,
				_grassBounds, _positionsCount,
				null, ShadowCastingMode.Off, false);
		}

		private void OnDestroy() {
			_positionBuffer?.Release();
			_positionBuffer = null;
			for (int i = 0;i < _savedTerrainData.Count;i++)
			{
				terrainData[i].treeInstances = _savedTerrainData[i];
			}
		}
	}
}