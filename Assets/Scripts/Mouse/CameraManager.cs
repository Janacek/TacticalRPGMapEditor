using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : MonoBehaviour
{
    public float ScrollSensitivity = 5.0f;
    public Vector2 SlideSensitivity = new Vector2(1.0f, 1.0f);

    void Start()
    {
        m_oldMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButton(1))
        {
            Vector3 offset = m_oldMousePosition - Input.mousePosition;

            Camera.main.transform.position += new Vector3(offset.x * SlideSensitivity.x, 0, offset.y * SlideSensitivity.y);
        }

        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.1f)
        {
            Camera.main.transform.position += new Vector3(0, Input.mouseScrollDelta.y, 0) * ScrollSensitivity;
        }

        m_oldMousePosition = Input.mousePosition;
    }

    Vector3 m_oldMousePosition;
}
