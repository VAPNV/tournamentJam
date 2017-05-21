using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextTyper : MonoBehaviour {
	
	public float letterPause = 0.1f;
	
	public string message;
	public Text textComp;

	public bool WRITING = false;

	//not pretty but works
	void Start()
	{
		this.WriteText (this.transform.GetComponent<Text>().text);

	}

	// Use this for initialization
	public void WriteText() {

		StartCoroutine(Write());

	}
	public void WriteText(Text MessageInject) {

		this.message = MessageInject.text;
		StartCoroutine(Write());
		
	}

	public void WriteText(string MessageInject) {
		
		this.message = MessageInject;
		StartCoroutine(Write());
		
	}

	IEnumerator Write()
	{ 
		if (WRITING == true) {
		}
		else 
		{
			WRITING = true;
			textComp.text = "";
			foreach (char letter in message.ToCharArray()) {

				textComp.text += letter;
				yield return new WaitForFixedUpdate();
				//yield return new WaitForSeconds (letterPause);
			}
			WRITING = false;
		}
	}
	
//	IEnumerator TypeText () {
//		foreach (char letter in message.text.ToCharArray()) {
//			textComp.text += letter;
//			yield return 0;
//			yield return new WaitForSeconds (letterPause);
//		}
//	}

}