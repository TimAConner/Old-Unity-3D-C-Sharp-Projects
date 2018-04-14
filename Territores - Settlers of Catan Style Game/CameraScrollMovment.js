#pragma strict
var CamSpeed = 0.05;
var GUIsize = 25;
var minFov: float = 15f;
var maxFov: float = 90f;
var sensitivity: float = 10f;


function Update() {
    var recdown = Rect(0, 0, Screen.width, GUIsize);
    var recup = Rect(0, Screen.height - GUIsize, Screen.width, GUIsize);
    var recleft = Rect(0, 0, GUIsize, Screen.height);
    var recright = Rect(Screen.width - GUIsize, 0, GUIsize, Screen.height);


    var fov: float = Camera.main.fieldOfView;
    fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
    fov = Mathf.Clamp(fov, minFov, maxFov);
    Camera.main.fieldOfView = fov;



    if (recdown.Contains(Input.mousePosition))
        transform.Translate(0, 0, -CamSpeed, Space.World);

    if (recup.Contains(Input.mousePosition))
        transform.Translate(0, 0, CamSpeed, Space.World);

    if (recleft.Contains(Input.mousePosition))
        transform.Translate(-CamSpeed, 0, 0, Space.World);

    if (recright.Contains(Input.mousePosition))
        transform.Translate(CamSpeed, 0, 0, Space.World);
}