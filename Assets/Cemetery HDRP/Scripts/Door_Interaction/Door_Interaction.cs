using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Interaction : MonoBehaviour
{
    [Tooltip ("The state of the door")]
    public bool open;
    [Tooltip("If it is true, the door won't open")]
    public bool locked;
    [Tooltip ("The maximum distance in which you can interact with the door")]
    public float maxDistance = 2;
    [Tooltip("The angle in which the door rotates")]
    public float rotateAngle = 90;
    [Tooltip("The speed in which the door moves")]
    public float speed = 1;
    public enum axis { X = 0, Y = 1, Z = 2 }
    [Tooltip("The axis of rotation of the door. Defualt is Y")]
    public axis rotateAxis = axis.Y;
    [Tooltip("The key you have to press in order to open/close the door (use the 'Naming convention' of Unity).")]
    public string keyToPress = "e";

    private Quaternion originRotation;
    private Quaternion openRotation;
    private AudioSource sound;
    private float r;
    private bool opening;

    private Ray ray;
    private RaycastHit hit;

    void Start()
    {
        originRotation = transform.rotation;
        openRotation = originRotation;
        if (rotateAxis == axis.Y) {
            openRotation = Quaternion.Euler (new Vector3 (originRotation.eulerAngles.x, originRotation.eulerAngles.y + rotateAngle, originRotation.eulerAngles.z));
        } else if (rotateAxis == axis.X) {
            openRotation = Quaternion.Euler(new Vector3(originRotation.eulerAngles.x + rotateAngle, originRotation.eulerAngles.y, originRotation.eulerAngles.z));
        } else if (rotateAxis == axis.Z) {
            openRotation = Quaternion.Euler(new Vector3(originRotation.eulerAngles.x, originRotation.eulerAngles.y, originRotation.eulerAngles.z + rotateAngle));
        }
        r = 0;
        sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        r += Time.deltaTime * speed;
        if (Physics.Raycast(ray, out hit, maxDistance) && hit.transform == this.transform) {
            if (Input.GetKeyDown(keyToPress) && opening == false && locked == false) {
                open = !open;
                r = 0;
                opening = true;
                if (sound.clip != null) {
                    sound.Play();
                }
            }
        }
        if (open == false) {
            if (Quaternion.Angle(originRotation, transform.rotation) > 0.1F) {
                transform.rotation = Quaternion.Slerp(transform.rotation, originRotation, r);
            } else {
                transform.rotation = originRotation;
                r = 0;
                opening = false;
            }
        } else {
            if (Quaternion.Angle(openRotation, transform.rotation) > 0.1F) {
                transform.rotation = Quaternion.Slerp(transform.rotation, openRotation, r);
            } else {
                transform.rotation = openRotation;
                r = 0;
                opening = false;
            }
        }
    }
}
