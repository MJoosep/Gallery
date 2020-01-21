#region Imports

using UnityEngine;

#endregion

public class UIManager : MonoBehaviour
{
    #region Variables

    public GameObject Frame;
    public GameObject ImagesList;

    private Object[] images;

    private int frameCounter = 1;
    private int pictureHeight = -225;

    #endregion

    private void Start()
    {
        LoadImages();
    }

    private void Update()
    {
        DetectTouchInput();
    }

    public void DetectTouchInput()
    {
        if(Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                Ray touchRay = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit[] hit = Physics.RaycastAll(touchRay);

                if (hit[0].transform.name.Contains("ListItem-"))
                {
                    string name = hit[0].transform.name.Replace("ListItem-", "");
                    name = name.Replace("(Clone)", "");
                    AddImage(name);
                }
            }
        }
    }

    public void DisableBoolAnimator(Animator anim)
    {
        anim.SetBool("IsDisplayed", false);
    }

    public void ToggleBoolAnimator(Animator anim)
    {
        if (anim.GetBool("IsDisplayed") == true)
            anim.SetBool("IsDisplayed", false);
        else
            anim.SetBool("IsDisplayed", true);
    }

    public void LoadImages()
    {
        images = Resources.LoadAll("Images", typeof(Sprite));

        foreach (var image in images)
        {
            AddImageToList(image as Sprite);
        }
    }

    public void AddImageToList(Sprite image)
    {
        GameObject newImage = new GameObject("ListItem-"+image.name);
        SpriteRenderer imageRenderer = newImage.AddComponent<SpriteRenderer>();
        BoxCollider collider = newImage.AddComponent<BoxCollider>();

        collider.size = new Vector3(7, 7, 0);
        newImage.transform.localScale = new Vector3(2f, 2f, 2f);
        imageRenderer.sortingOrder = 0;

        imageRenderer.sprite = image;
        
        var imageObject = Instantiate(newImage);
        imageObject.transform.SetParent(ImagesList.transform);

        GameObject deleteCopy = GameObject.Find("/ListItem-"+image.name);
        if (deleteCopy != null)
            Destroy(deleteCopy);

        imageObject.transform.localPosition = new Vector3(0, pictureHeight, 0);

        pictureHeight += 200;
    }

    public void AddImage(string pictureID)
    {
        foreach(var image in images)
        {
            if(image.name == pictureID)
            {
                GameObject newImage = Instantiate(Frame, new Vector3(0, 0, 100), Quaternion.identity);

                newImage.GetComponentInChildren<SpriteRenderer>().sprite = image as Sprite;
                newImage.name = "Picture Frame " + frameCounter;

                frameCounter++;
            }
        }
    }
}
