using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace TBD.Networking {
	[RequireComponent(typeof(NetworkIdentity))]
	public class NetworkTransform : MonoBehaviour {

		public int NetworkSendRate = 9;
		public TransformSyncMethod TransformSyncMethod;

		[Header("Movement:")]
		public float MovementThreshold = 0.001f;
		public float VelocityThreshold = 0.0001f;
		public float SnapThreshold = 5f;
		public float InterpolateMovementFactor = 1f;

		[Header("Rotation:")]
		public RotationAxisSyncMethod RotationAxisSyncMethod = RotationAxisSyncMethod.XYZ;
		public float InterpolateRotationFactor = 1f;
		public bool SyncAngularVelocity = false;
	}

	public enum TransformSyncMethod {
		SyncNone,
		SyncTransform,
		SyncRigidbody3D
	}

	public enum RotationAxisSyncMethod {
		None,
		XYZ
	}
}
