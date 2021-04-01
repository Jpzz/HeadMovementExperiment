/* Copyright (c) 2019 ExT (V.Sigalkin) */

using UnityEngine;

namespace extOSC.Examples
{
	public class SimpleMessageReceiver : MonoBehaviour
	{
		#region Public Vars

		public string Address = "/example/1";
		public GameObject cube;
		[Header("OSC Settings")]
		public OSCReceiver Receiver;

		#endregion

		#region Unity Methods

		protected virtual void Start()
		{
			Receiver.Bind(Address, ReceivedMessage);
		}

		#endregion

		#region Private Methods

		private void ReceivedMessage(OSCMessage message)
		{
			Debug.LogFormat("Received: {0}", message);
			if (message.ToFloat(out var value))
			{
				cube.transform.Translate(new Vector3(value,0f,0f) * Time.deltaTime);
			}
		}
		
		#endregion
	}
}