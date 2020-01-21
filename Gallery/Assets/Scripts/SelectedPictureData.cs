using UnityEngine;
using UnityEngine.UI;

public class SelectedPictureData : MonoBehaviour
{
    [SerializeField]
    Text title;

    [SerializeField]
    Text coordinates;

    [SerializeField]
    Text rotation;

    void Update()
    {   if(TouchManager.selectedObject != null)
        {
            title.text = "Picture title: " + TouchManager.selectedObject.name;
            coordinates.text = "Coordinates: " + TouchManager.selectedObject.transform.position.ToString();
            rotation.text = "Rotation: " + TouchManager.selectedObject.transform.position.ToString();
        }
    }
}
