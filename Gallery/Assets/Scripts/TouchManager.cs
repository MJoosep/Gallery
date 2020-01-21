#region Imports

using UnityEngine;
using UnityEngine.UI;

#endregion

public class TouchManager : MonoBehaviour
{
    #region Variables

    public static GameObject selectedObject;

    private Vector3 initialScale;
    private Vector3 initialRotation;
    private Vector3 initialCamSize;

    private float zoomFingerDistance;
    private float initialFingersRotation;
    private float initialFingersDistance;

    private bool change = false;
    private bool frameSelected = false;
    private bool movingFrame = false;

    #endregion

    private void DetectTouches()
    {
        //Check current number of fingers on screen
        if(Input.touchCount == 1)
        {
            //Check if an Image Frame is currently selected
            if (frameSelected)
            {
                switch (Input.GetTouch(0).phase)
                {
                    case TouchPhase.Began:
                        //When touch is initiated then it checks if frame was been switched or background has been clicked
                        if (IsTouching(Input.GetTouch(0).position).name == selectedObject.name)
                            movingFrame = true;

                        else if (IsTouching(Input.GetTouch(0).position).name.Contains("Picture Frame") 
                            && IsTouching(Input.GetTouch(0).position).name != selectedObject.name)
                        {
                            DeactivateAllPins();
                            selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y, selectedObject.transform.position.z + 5);
                            selectedObject = IsTouching(Input.GetTouch(0).position).gameObject;
                            selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y, selectedObject.transform.position.z - 5);
                            frameSelected = true;
                            ActivatePins(selectedObject);
                        }
                        else if (IsTouching(Input.GetTouch(0).position).name == "Border")
                        {
                            DeactivateAllPins();
                            selectedObject = null;
                            frameSelected = false;
                        }

                        break;

                    case TouchPhase.Moved:
                        //Move Image Frame when finger moves
                        if (movingFrame == true)
                            MoveFrame(selectedObject);

                        break;

                    case TouchPhase.Ended:
                        //When finger is released checks if an image has been sent to the trash
                        if (IsTouching(Input.GetTouch(0).position).name == "Bin"){
                            Destroy(selectedObject);
                            selectedObject = null;
                            frameSelected = false;
                        }

                        movingFrame = false;
                        
                        break;
                }
            }
            else
            {   //if no frame has been selected then it selects the frame that was clicked
                if (IsTouching(Input.GetTouch(0).position).name.Contains("Picture Frame"))
                {
                    selectedObject = IsTouching(Input.GetTouch(0).position).gameObject;
                    frameSelected = true;
                    ActivatePins(selectedObject);
                }
            }
        }
        else if(Input.touchCount == 2)
        {
            if(frameSelected == true)
            {   //Change variable to avoid slow raycasting every update
                if (!change)
                {
                    if (IsTouching(Input.GetTouch(0).position).name == "Pin" && IsTouching(Input.GetTouch(1).position).name == "Pin")
                    {
                        change = true;
                    }
                }

                if (change)
                {
                    ChangeScaleAndRotation(selectedObject, Input.GetTouch(0).position, Input.GetTouch(1).position);
                    MoveFrame(selectedObject);
                }
            }
            else if (Input.touchCount == 2)
            {   //Ran out of time
                //CameraZoom();
            }
        }
    }

    private void Update()
    {
        DetectTouches();
    }

    private void CameraZoom()
    {
        switch (Input.GetTouch(1).phase)
        {
            case TouchPhase.Began:
                zoomFingerDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                initialCamSize = Camera.main.transform.position;
                break;

            case TouchPhase.Moved:
                var currentDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                var scaleFactor = currentDistance / zoomFingerDistance;
                Camera.main.transform.localPosition = new Vector3(initialCamSize.x, initialCamSize.y, currentDistance - zoomFingerDistance);
                break;
        }
    }

    private void MoveFrame(GameObject frame)
    {   //Checks for current finger position
        Vector3 fingerPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 100);

        if (Input.touchCount == 2)
        {
            fingerPosition = (Input.GetTouch(0).position + Input.GetTouch(1).position) / 2;
            fingerPosition.z = 100;
        }
        //Sets frame position to corresponding coordinates in app
        frame.transform.position = Camera.main.ScreenToWorldPoint(fingerPosition);
    }

    private void ChangeScaleAndRotation(GameObject obj, Vector2 fingerPos1, Vector2 fingerPos2)
    {
        if (Input.GetTouch(1).phase == TouchPhase.Began)
        {   //Get data to compare after finger motion
            initialFingersDistance = Vector2.Distance(fingerPos1, fingerPos2);
            initialFingersRotation = Angle(fingerPos1, fingerPos2);
            initialScale = obj.transform.localScale;
            initialRotation = obj.transform.eulerAngles;
        }
        else if (Input.GetTouch(1).phase == TouchPhase.Ended)
        {
            change = false;
        }
        
        if (Input.GetTouch(1).phase == TouchPhase.Stationary || Input.GetTouch(1).phase == TouchPhase.Moved)
        {   //Use collected data to determine where the frame should now go
            var currentFingersDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            var scaleFactor = currentFingersDistance / initialFingersDistance;
            obj.transform.localScale = initialScale * scaleFactor;

            var currentFingersRotation = Angle(Input.GetTouch(0).position, Input.GetTouch(1).position);

            obj.transform.localRotation = Quaternion.Euler(obj.transform.localRotation.x, obj.transform.localRotation.y, initialRotation.z - (initialFingersRotation - currentFingersRotation));
        }
    }
    private float Angle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0)
        {
            result = 360f - result;
        }

        return result;
    }

    private Transform IsTouching(Vector2 touchPos)
    {
        Ray touchRay = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit[] hit = Physics.RaycastAll(touchRay);

        return hit[0].transform;
    }

    private void ActivatePins(GameObject frame)
    {
        GameObject image = frame.transform.GetChild(0).gameObject;

        for(int i = 0; i < image.transform.childCount; i++)
        {
            image.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void DeactivateAllPins()
    {
        GameObject[] gameObjectArray = GameObject.FindGameObjectsWithTag("Pins");

        foreach (GameObject go in gameObjectArray)
        {
            go.SetActive(false);
        }
    }
}
