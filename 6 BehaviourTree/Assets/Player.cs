using UnityEngine;

public class Player : MonoBehaviour {

    private void Start () {
		GetComponent<Renderer>().material.color = new Color(1.0f, 0.7f, 0);
	}

    private void Update ()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 10;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        transform.localPosition = mousePos;
    }
}
