using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraController : MonoBehaviour {
    //Objects
    GameObject[] ballz;
    public Transform focus;            //Player
    static CameraController instance;

    [Header("Movement")]
    public float horizontalSmoothTime;
    public float verticalSmoothTime;
    float smoothVelocityX;
    float smoothVelocityY;
    public Vector2 forcedOffset;
    Vector3 goalPosition;
    Vector3 offset;             //Current Offset
    Vector2 lastFocusPosition;
    public bool fixToX, fixToY;

    [Header("Momentum")]
    public float maxVelocity;
    public float velocityMult;
    public bool useMomentum;

    [Header("Size")]
    public float sizeSmoothTime;
    public float skinWidth;
    public float minimumSize;
    float smoothVelocitySize;
    float newSize;
    float currentSize;
    Vector2 sizeTest;

    [Header("Rotation")]
    public float rotation;      //Goal roation
    public float currentRotation;   //Current rotation
    public float rotationSmoothTime;
    float rotationVelocity;
    

    [Header("Screen Shake")]
    public float shakeSmoothTime;
    public float maxRotation;
    public float maxShakeMagnitude = 2;
    float shakeMagnitude;
    float shakeVelocity;

    [Header("Zoom")]
    public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
    public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.
    

    //Color
    public Color standardColor;
    Stack<ColorChange> colors = new Stack<ColorChange>();

    //Camera
    Camera mainCamera;
    void Start()
    {
        instance = this;
        Application.runInBackground = true;
        SetFocus(focus);
        mainCamera = Camera.main;

        standardColor = mainCamera.backgroundColor;
        currentSize = mainCamera.orthographicSize;


        offset = focus.position - transform.position;

    }
    public void Awake()
    {
        ForceAspect();
    }

    public void ForceAspect()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 800.0f / 1280.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    public void SetFocus(Transform focus)
    {
        this.focus = focus;
        lastFocusPosition = focus.position;
    }

    public void SetObject(Vector2 point)
    {
        /*
        Vector2[] points = { Vector2.zero, point - (Vector2)focus.position };
        points = ConvertVectors(points);
        goalPosition = FindCenter(points);
        newSize = ToOrthographicSize(FindMax(goalPosition, points));
        goalPosition = focus.TransformDirection(goalPosition);
        goalPosition.z = transform.position.z;
        */
    }
    void FixedUpdate()
    {
        ManageColors();
    }

    void LateUpdate()
    {
        //Position
        goalPosition = focus.position;
        if (fixToY)
        {
            goalPosition.x = 0;
        }
        if (fixToX)
        {
            goalPosition.y = 0;
        }
        goalPosition += (Vector3)forcedOffset;
        offset =   transform.position -goalPosition;
        offset.x = Mathf.SmoothDamp(offset.x, 0, ref smoothVelocityX, horizontalSmoothTime);
        offset.y = Mathf.SmoothDamp(offset.y, 0, ref smoothVelocityY, verticalSmoothTime);
        Vector3 newPosition = goalPosition + offset;
        newPosition.z = transform.position.z;
        transform.position = newPosition;


        //Size
        newSize = minimumSize;
        currentSize = Mathf.SmoothDamp(currentSize, newSize, ref smoothVelocitySize, sizeSmoothTime);
        mainCamera.orthographicSize = currentSize;

        //boxCollider.size.x = camera.aspect * 2f * camera.orthographicSize;
        //boxCollider.size.y = 2f * camera.orthographicSize;

        //Rotation
        currentRotation = Mathf.SmoothDampAngle(currentRotation, rotation, ref rotationVelocity, rotationSmoothTime);
        Vector3 myRotation = transform.eulerAngles;
        myRotation.z = currentRotation;
        transform.eulerAngles = myRotation;

        //ManageZoom();
        ScreenShake();
    }

    public float ToOrthographicSize(Vector2 size)
    {
        this.sizeTest = new Vector2(size.x,size.y);
        float oSize = 0;
        size = new Vector2(size.x + skinWidth, size.y + skinWidth);
        size = size * 2;

        if (size.x > size.y * Camera.main.aspect)
        {
            size.y = size.x / Camera.main.aspect;
        }

        oSize = size.y/2;

        return oSize;
    }

    Vector3 FindCenter(Vector2[] points)
    {
        float mult = points.Length;
        Vector3 middle = Vector3.zero;
        float xMax=points[0].x, xMin = points[0].x, yMax = points[0].y, yMin = points[0].y;
        foreach(Vector2 point in points)
        {
            float x;
            x = point.x;
            xMax = x > xMax ?x : xMax;
            xMin = x < xMin ? x : xMin;

            float y;
            y = point.y;
            yMax = y > yMax ? y : yMax;
            yMin = y < yMin ? y : yMin;
        }

        middle = new Vector3((xMax + xMin) / 2, (yMax + yMin) / 2);
        return middle;
    }

    Vector2 FindMax(Vector3 center, Vector2[] points)
    {
        Vector2 size = Vector2.zero;
        foreach (Vector2 point in points)
        {
            size.x = Mathf.Abs(point.x - center.x)> size.x ? Mathf.Abs(point.x - center.x) : size.x;
            size.y = Mathf.Abs(point.y - center.y) > size.y ? Mathf.Abs(point.y - center.y) : size.y;
        }
        return size;
    }

    //
    //Screenshake
    //
    void ScreenShake()
    {
        shakeMagnitude = Mathf.Min(maxShakeMagnitude, shakeMagnitude);
        shakeMagnitude = Mathf.SmoothDamp(shakeMagnitude, 0,ref shakeVelocity, shakeSmoothTime);
        Vector3 position = transform.position;
        position += Random.Range(-1f, 1f) * shakeMagnitude * Vector3.up;
        position += Random.Range(-1f, 1f) * shakeMagnitude * Vector3.right;
        transform.Rotate(Vector3.forward * Random.Range(-1f, 1f) * shakeMagnitude * 10);
        transform.position = position;

        /*
        currentRotation += Random.Range(-1f, 1f) * shakeMagnitude * 10;
        currentRotation = currentRotation - rotation > maxRotation ? maxRotation + rotation : currentRotation;
        currentRotation = currentRotation - rotation < -maxRotation ? -maxRotation + rotation: currentRotation;
        transform.rotation = Quaternion.Euler(Vector3.forward * currentRotation);
        */
    }
    public void Push(Vector2 push)
    {
        smoothVelocityX += push.x;
        smoothVelocityY += push.y;
    }
    public static void Shake(float magnitude)
    {
        if (instance)
        {
            instance.shakeMagnitude += magnitude;
        }
    }
    //
    //Abilities
    //
    void ManageColors()     //colors ist ein Stack mit den structs ColorChange
    {
        if (colors.Count > 0)
        {
            if (Time.time > colors.Peek().timeStamp)
            {
                colors.Pop();
                if (colors.Count > 0)
                {
                    mainCamera.backgroundColor = colors.Peek().color;
                }
                else
                {
                    ResetColor();
                }
            }
        }

        mainCamera.backgroundColor = Color.Lerp(standardColor, mainCamera.backgroundColor, 0.7f);
    }
    struct ColorChange
    {
        public float timeStamp { get; set; }
        public Color color { get; set; }
    }
    public void ChangeColor(Color color, float interval)
    {
        mainCamera.backgroundColor = color;
        ColorChange colorChange = new ColorChange();
        colorChange.color = color;
        colorChange.timeStamp = Time.time + interval;
        colors.Push(colorChange);
    }
    void ResetColor()
    {
        mainCamera.backgroundColor = standardColor;
    }

    Vector2[] ConvertVectors(Vector2[] vectors)
    {

        for (int i = 0; i < vectors.Length; i++)
        { 
            vectors[i] = focus.InverseTransformDirection(vectors[i]);
        }
        return vectors;
    }

    void ManageZoom()
    {
        // If there are two touches on the device...
        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            
            // ... change the orthographic size based on the change in distance between the touches.
            newSize += deltaMagnitudeDiff * orthoZoomSpeed;

        }
        // Make sure the orthographic size never drops below zero.
        newSize = Mathf.Max(newSize, minimumSize);
    }
}
