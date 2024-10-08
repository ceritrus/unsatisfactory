using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum State
    {
        Idle,
        Carried,
        Used,
        OnConveyor,
        OnDesk,
    };

    public enum Type
    {
        DropZone,
        Pickupable,
        Usable,
        None,
    }

    [SerializeField] private string itemName;
    [SerializeField] private string itemDescription;
    [SerializeField] public Type type = Type.None;
    [SerializeField] public State state = State.OnConveyor;

    private GameObject highlight;
    private GameObject outerHighlight;
    private Animation anim;
    public int conveyorPoint = 0;
    [SerializeField] public Vector3 scale = new Vector3(1, 1, 1);
    [SerializeField] public Vector3 conveyorOffset = new Vector3(0, 0, 0);

    private Transform shadow;

    private void Start()
    {
        if (gameObject.GetComponent<Animation>() == null)
        {
            anim = gameObject.AddComponent<Animation>();
        }
        else
        {
            anim = gameObject.GetComponent<Animation>();
        }

        shadow = transform.GetChild(0);

        if (type != Type.DropZone && type != Type.None)
        {
            highlight = new GameObject("Highlight");
            highlight.transform.SetParent(transform);
            highlight.transform.localPosition = Vector3.zero;
            highlight.transform.localRotation = Quaternion.identity;
            highlight.transform.localScale = Vector3.one;
            highlight.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
            Material material = new Material(Shader.Find("Shader Graphs/Highlight"));
            material.SetColor("_Color", Color.white);
            material.SetFloat("_Size", 1.1f);
            highlight.AddComponent<MeshRenderer>().material = material;
            highlight.SetActive(false);

            outerHighlight = new GameObject("OuterHighlight");
            outerHighlight.transform.SetParent(transform);
            outerHighlight.transform.localPosition = Vector3.zero;
            outerHighlight.transform.localRotation = Quaternion.identity;
            outerHighlight.transform.localScale = Vector3.one;
            outerHighlight.AddComponent<MeshFilter>().mesh = GetComponent<MeshFilter>().mesh;
            material = new Material(Shader.Find("Shader Graphs/Highlight"));
            material.SetColor("_Color", Color.black);
            material.SetFloat("_Size", 1.2f);
            outerHighlight.AddComponent<MeshRenderer>().material = material;
            outerHighlight.SetActive(false);


        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.Idle:
                shadow.gameObject.SetActive(false);
                gameObject.GetComponent<Collider>().enabled = true;
                break;
            case State.Carried:
                shadow.gameObject.SetActive(false);
                gameObject.GetComponent<Collider>().enabled = false;
                break;
            case State.Used:
                shadow.gameObject.SetActive(false);
                gameObject.GetComponent<Collider>().enabled = true;
                break;
            case State.OnConveyor:
                GameManager.conveyor.MoveItem(this);
                gameObject.GetComponent<Collider>().enabled = true;
                shadow.gameObject.SetActive(true);
                break;
            case State.OnDesk:
                gameObject.GetComponent<Collider>().enabled = true;
                shadow.gameObject.SetActive(true);
                break;
        }
    }

    public void Highlight(bool value)
    {
        highlight.SetActive(value);
        outerHighlight.SetActive(value);
    }

    public void Shake()
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;
        clip.SetCurve("", typeof(Transform), "localPosition.x", AnimationCurve.EaseInOut(0, -.05f, .2f, .05f));

        clip.SetCurve("", typeof(Transform), "localPosition.y", AnimationCurve.EaseInOut(0, -.05f, .2f, .05f));

        clip.SetCurve("", typeof(Transform), "localPosition.z", AnimationCurve.EaseInOut(0, -.05f, .2f, .05f));

        // clip.SetCurve("", typeof(Transform), "localRotation.x", AnimationCurve.EaseInOut(0, 0, .2f, .2f));

        clip.SetCurve("", typeof(Transform), "localRotation.y", AnimationCurve.EaseInOut(0, -40f, .2f, 40f));

        // clip.SetCurve("", typeof(Transform), "localRotation.z", AnimationCurve.EaseInOut(0, 0, .2f, .2f));

        anim.AddClip(clip, "Shake");
        anim.Play("Shake");

        Debug.Log("Shake");
    }

}
