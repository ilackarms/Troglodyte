using UnityEngine;
using System.Collections;

public class CustomSendMessage<T> : MonoBehaviour where T : MonoBehaviour
{
	public static void SendMessageUpwards(Transform transform, string functionName, object value, SendMessageOptions options){
		T targetScript = transform.GetComponentInParent<T>();
		if(targetScript!=null){
			GameObject obj = ((MonoBehaviour) targetScript).gameObject;
			obj.SendMessage(functionName, value, options);
		}

	}
	
	public static void SendMessageUpwards(Transform transform, string functionName, SendMessageOptions options){
		T targetScript = transform.GetComponentInParent<T>();
		if(targetScript!=null){
			GameObject obj = ((MonoBehaviour) targetScript).gameObject;
			obj.SendMessage(functionName, options);
		}
	}
}

