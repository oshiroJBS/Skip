using UnityEngine;
using System.Collections;

public class FileSelectorExample : MonoBehaviour {
	
	private GUIStyle style;
	private string path = "";
	private bool windowOpen;


    void Start()
	{
		style = new GUIStyle();
		style.fontSize = 40;
		
		style.normal.textColor = Color.white;
    }
	
	void OnGUI(){
		//Instructions
    }
	
	// Update is called once per frame
	void Update () {
		
		//if we don't have an open window yet, and the spacebar is down...
		
	}

	public void OpenWindow()
	{
        if (!windowOpen)
        {
            FileSelector.GetFile(GotFile, ".mp3"); //generate a new FileSelector window

            //FileSelector.windowStyle
            windowOpen = true; //record that we have a window open
        }
    }
				
	//This is called when the FileSelector window closes for any reason.
	//'Status' is an enumeration that tells us why the window closed and if 'path' is valid.
	void GotFile(FileSelector.Status status, string path){
		Debug.Log("File Status : "+status+", Path : "+path);
		this.path = path;

		this.windowOpen = false;
	}
}
