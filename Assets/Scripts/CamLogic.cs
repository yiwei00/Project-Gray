using UnityEngine;

public class CamLogic : MonoBehaviour
{
    public Transform follow;
    public float camOffset = 5f;
    public float camHeight = 10f;
    public float camAngle = 60f;
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = follow.position + new Vector3(0f, camHeight, -camOffset);
        cam.transform.rotation = Quaternion.AngleAxis(camAngle, Vector3.right);
    }
}
