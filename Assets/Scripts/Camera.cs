using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public static Camera instance;

    public Transform target;
    public Transform campiv;
    public Transform Cameraman;
    public float sens;

    public bool canfeild;
    float f;
    float speed;

    public void Awake()
    {
        Cameraman.transform.SetParent(null);

        instance = this;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetFeildofview(float _f , float _speed)
    {
        canfeild = true;
        f = _f;
        speed = _speed;
    }

    public void SetcamPiv(Transform _trans)
    {
        if (!campiv.parent != _trans.gameObject)
            campiv.transform.SetParent(_trans);
    }

    float xRotation;
    public void Update()
    {
        float mousex = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        float mousey = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;

        Cameraman.transform.position = target.transform.position;

        xRotation -= mousey;
        xRotation = Mathf.Clamp(xRotation, -80f, 50f);

        campiv.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        Cameraman.transform.Rotate(Vector3.up * mousex);

        if (canfeild)
        {
            GetComponent<UnityEngine.Camera>().fieldOfView =
            Mathf.Lerp(GetComponent<UnityEngine.Camera>().fieldOfView,  f, speed * Time.deltaTime);

            if (GetComponent<UnityEngine.Camera>().fieldOfView ==  f)
            {
                canfeild = false;
            }
        }

    }
}
