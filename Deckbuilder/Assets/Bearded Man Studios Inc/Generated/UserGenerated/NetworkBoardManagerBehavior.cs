using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"bool\", \"int\", \"string\"][\"bool\", \"int\", \"string\", \"bool\"][\"bool\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"hostSide\", \"spotNum\", \"cardName\"][\"hostSide\", \"spotNum\", \"cardName\", \"hostCasted\"][\"hostEnded\"]]")]
	public abstract partial class NetworkBoardManagerBehavior : NetworkBehavior
	{
		public const byte RPC_SUMMON_UNIT = 0 + 5;
		public const byte RPC_CAST_SPELL = 1 + 5;
		public const byte RPC_END_TURN = 2 + 5;
		
		public NetworkBoardManagerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (NetworkBoardManagerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("SummonUnit", SummonUnit, typeof(bool), typeof(int), typeof(string));
			networkObject.RegisterRpc("CastSpell", CastSpell, typeof(bool), typeof(int), typeof(string), typeof(bool));
			networkObject.RegisterRpc("EndTurn", EndTurn, typeof(bool));

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId))
					ProcessOthers(gameObject.transform, obj.NetworkId + 1);
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new NetworkBoardManagerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new NetworkBoardManagerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// bool hostSide
		/// int spotNum
		/// string cardName
		/// </summary>
		public abstract void SummonUnit(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// bool hostSide
		/// int spotNum
		/// string cardName
		/// bool hostCasted
		/// </summary>
		public abstract void CastSpell(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// bool hostEnded
		/// </summary>
		public abstract void EndTurn(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}