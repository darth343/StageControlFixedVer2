using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float sensitivityX;
    public float sensitivityY;
    public float zoomlevel;
    public float zoomPCSensitivity;
    public float zoomAndroidSensitivity;
    public float CameraDistanceFromTerrain;
    public float MinConstrainX;
    public float MaxConstrainX;
    public float MinConstrainZ;
    public float MaxConstrainZ;
    public float zoomMaxLevel;
    public float zoomMinLevel;
    float defaultCameraDistanceFromTerrain;
    public Terrain ground;
    Vector3 lasttouchposition;
    public Text debugtext;
    Vector3 newCameraPos = new Vector3();
    void SetCameraPosition(Vector3 newPosition)
    {
        GetComponent<Camera>().transform.position = newPosition;
    }
#if UNITY_ANDROID
    bool FingerDown;
    bool Finger2Down;

    Vector2 inbetween; 
#else
    bool LeftMouseDown;
#endif

    Camera GetCamera()
    {
        return GetComponent<Camera>();
    }

    //float GetCameraLevel()
    //{
    //    if (GetComponent<Camera>().transform.position.y < ground.SampleHeight(GetComponent<Camera>().transform.position) + MinimumCameraHeight * zoomlevel)
    //    {
    //        return ground.SampleHeight(GetComponent<Camera>().transform.position) + MinimumCameraHeight * zoomlevel;
    //    }

    //    return GetComponent<Camera>().transform.position.y;
    //}

    void OnButtonDown()
    {
#if UNITY_ANDROID
        if(SceneData.sceneData.handhandler.onPlayArea(Input.GetTouch(0).position) == false || SceneData.sceneData.isHoldingCard)
        {
            return;
        }
        lasttouchposition = Input.GetTouch(0).position;
        FingerDown = true;
#else
        lasttouchposition = Input.mousePosition;
        LeftMouseDown = true;
#endif
        debugtext.text = "IN MOUSE DOWN";
    }

    void OnButtonUp()
    {
#if UNITY_ANDROID
        FingerDown = false;
#else
        LeftMouseDown = false;
#endif
        debugtext.text = "IN LET GO";
    }

    void OnDrag()
    {
        if (SceneData.sceneData.isHoldingCard)
            return;
#if UNITY_ANDROID
        float xdelta = (lasttouchposition.x - Input.GetTouch(0).position.x) * sensitivityX;
        float ydelta = (lasttouchposition.y - Input.GetTouch(0).position.y) * sensitivityY;
        //newCameraPos.Set(Mathf.Clamp(lastcameraposition.x + (lasttouchposition.x - Input.mousePosition.x) * sensitivityX, MinConstrainX, MaxConstrainX), lastcameraposition.y, Mathf.Clamp(lastcameraposition.z + (lasttouchposition.y - Input.mousePosition.y) * sensitivityY, MinConstrainZ, MaxConstrainZ));
        newCameraPos = GetCamera().transform.position;
        newCameraPos.x += GetCamera().transform.forward.x * ydelta + GetCamera().transform.right.x * xdelta;
        newCameraPos.z += GetCamera().transform.forward.z * ydelta + GetCamera().transform.right.z * xdelta;
        newCameraPos.x = Mathf.Clamp(newCameraPos.x, MinConstrainX, MaxConstrainX);
        newCameraPos.z = Mathf.Clamp(newCameraPos.z, MinConstrainZ, MaxConstrainZ);
        SetCameraPosition(newCameraPos);
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        if (ground.GetComponent<Collider>().Raycast(ray, out hit, 1000.0f))
        {
            Vector3 direction = new Vector3(-GetCamera().transform.forward.x, -GetCamera().transform.forward.y, -GetCamera().transform.forward.z).normalized;
            ground.SampleHeight(ray.GetPoint(hit.distance));
            SetCameraPosition(ray.GetPoint(hit.distance) + (direction * CameraDistanceFromTerrain));
        }
        lasttouchposition = Input.GetTouch(0).position;
#else
        float xdelta = (lasttouchposition.x - Input.mousePosition.x) * sensitivityX;
        float ydelta = (lasttouchposition.y - Input.mousePosition.y) * sensitivityY;
        //newCameraPos.Set(Mathf.Clamp(lastcameraposition.x + (lasttouchposition.x - Input.mousePosition.x) * sensitivityX, MinConstrainX, MaxConstrainX), lastcameraposition.y, Mathf.Clamp(lastcameraposition.z + (lasttouchposition.y - Input.mousePosition.y) * sensitivityY, MinConstrainZ, MaxConstrainZ));
        newCameraPos = GetCamera().transform.position;
        newCameraPos.x += GetCamera().transform.forward.x * ydelta + GetCamera().transform.right.x * xdelta;
        newCameraPos.z += GetCamera().transform.forward.z * ydelta + GetCamera().transform.right.z * xdelta;
        newCameraPos.x = Mathf.Clamp(newCameraPos.x, MinConstrainX, MaxConstrainX);
        newCameraPos.z = Mathf.Clamp(newCameraPos.z, MinConstrainZ, MaxConstrainZ);
        SetCameraPosition(newCameraPos);
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        if (ground.GetComponent<Collider>().Raycast(ray, out hit, 1000.0f))
        {
            Vector3 direction = new Vector3(-GetCamera().transform.forward.x, -GetCamera().transform.forward.y, -GetCamera().transform.forward.z).normalized;
            SetCameraPosition(ray.GetPoint(hit.distance) + (direction * CameraDistanceFromTerrain));
            float groundy = ground.SampleHeight(GetCamera().transform.position);
            if (GetCamera().transform.position.y < groundy)
            {
                GetCamera().transform.position = new Vector3(GetCamera().transform.position.x, groundy, GetCamera().transform.position.z);
            }
        }
        lasttouchposition = Input.mousePosition;
#endif
        debugtext.text = "PANNING\nCamera Pos: [" + GetCamera().transform.position.x + "," + GetCamera().transform.position.y + "," + GetCamera().transform.position.z + "]";
    }

    void zoomUpdate()
    {
        zoomlevel = Mathf.Clamp(zoomlevel - Input.GetAxis("Mouse ScrollWheel"), zoomMinLevel, zoomMaxLevel);
        CameraDistanceFromTerrain = defaultCameraDistanceFromTerrain * zoomlevel;
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        if (ground.GetComponent<Collider>().Raycast(ray, out hit, 1000.0f))
        {
            Vector3 direction = new Vector3(-GetCamera().transform.forward.x, -GetCamera().transform.forward.y, -GetCamera().transform.forward.z).normalized;
            ground.SampleHeight(ray.GetPoint(hit.distance));
            newCameraPos = ray.GetPoint(hit.distance) + (direction * CameraDistanceFromTerrain);
            newCameraPos.x = Mathf.Clamp(newCameraPos.x, MinConstrainX, MaxConstrainX);
            newCameraPos.z = Mathf.Clamp(newCameraPos.z, MinConstrainZ, MaxConstrainZ);
            SetCameraPosition(newCameraPos);
        }
        debugtext.text = "ZOOMING\nZoom Out Level: " + zoomlevel;
    }

    void Start()
    {
#if UNITY_ANDROID
        FingerDown = false;
        Finger2Down = false;
#else
        LeftMouseDown = false;
#endif
        defaultCameraDistanceFromTerrain = CameraDistanceFromTerrain;

        //SetCameraPosition(new Vector3(transform.position.x + (transform.position.x - Input.mousePosition.x) * sensitivityX, lastcameraposition.y, lastcameraposition.z + (lasttouchposition.y - Input.mousePosition.y) * sensitivityY));
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
        if (ground.GetComponent<Collider>().Raycast(ray, out hit, 1000.0f))
        {
            Vector3 direction = new Vector3(-GetCamera().transform.forward.x, -GetCamera().transform.forward.y, -GetCamera().transform.forward.z).normalized;
            SetCameraPosition(ray.GetPoint(hit.distance) + (direction * CameraDistanceFromTerrain));
            float groundy = ground.SampleHeight(GetCamera().transform.position);
            if (GetCamera().transform.position.y < groundy)
            {
                GetCamera().transform.position = new Vector3(GetCamera().transform.position.x, groundy, GetCamera().transform.position.z);
            }
        }
        //GetComponent<Camera>().transform.position = new Vector3(GetComponent<Camera>().transform.position.x, ground.SampleHeight(GetComponent<Camera>().transform.position) + CameraHeight, GetComponent<Camera>().transform.position.z);
    }

    // Update is called once per frame
	void Update ()
    {
#if UNITY_ANDROID

        if (Input.touchCount == 2)
        {
            if (!Finger2Down)
            {
                inbetween = Input.GetTouch(0).position - Input.GetTouch(1).position;
                Finger2Down = true;
            }
            else 
            {
                debugtext.text = "ZOOMING";
                Vector2 currentinBetween = Input.GetTouch(0).position - Input.GetTouch(1).position;
                debugtext.text = debugtext.text + "\n Current: " + currentinBetween.magnitude + "\n Initial: " + inbetween.magnitude;
                zoomlevel = Mathf.Clamp(zoomlevel + (inbetween.magnitude - currentinBetween.magnitude) * zoomAndroidSensitivity, 1, 10);
                CameraDistanceFromTerrain = defaultCameraDistanceFromTerrain * zoomlevel;
                RaycastHit hit;
                Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
                if (ground.GetComponent<Collider>().Raycast(ray, out hit, 1000.0f))
                {
                    Vector3 direction = new Vector3(-GetCamera().transform.forward.x, -GetCamera().transform.forward.y, -GetCamera().transform.forward.z).normalized;
                    ground.SampleHeight(ray.GetPoint(hit.distance));
                    SetCameraPosition(ray.GetPoint(hit.distance) + (direction * CameraDistanceFromTerrain));
                }
                inbetween = currentinBetween;
            }
        }
        else if(Input.touchCount == 1)
        {
            if (Finger2Down)
                return;
            if (Input.touchCount == 1 && !FingerDown)
            {
                OnButtonDown();
            }
            else if (Input.touchCount == 1 && FingerDown)
            {
                OnDrag();
            }
        }
        else if (Input.touchCount == 0)
        { 
            if(FingerDown)
            {
                OnButtonUp();
            }
            else if (Finger2Down)
            {
                Finger2Down = false;
            }
        }
#else
        if (Input.GetMouseButton(0) && !LeftMouseDown)
        {
            OnButtonDown();
        }
        else if (!Input.GetMouseButton(0) && LeftMouseDown)
        {
            OnButtonUp();
        }
        else if (Input.GetMouseButton(0) && LeftMouseDown)
        {
            OnDrag();
            debugtext.text = "PANNING\nCamera Pos: [" + GetCamera().transform.position.x + "," + GetCamera().transform.position.y + "," + GetCamera().transform.position.z + "]";
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            zoomUpdate();
        }
#endif

    }
}
