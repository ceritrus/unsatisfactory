using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conveyor : Item
{
    [Header("Drop Zone")]
    [SerializeField] private Vector2 size = new Vector2(1, 1);
    [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
    [SerializeField] private bool drawGizmos = true;

    [Header("Conveyor")]
    [SerializeField] public float speed = 0.8f;
    [SerializeField] public float matSpeed = 0.7f;

    [HideInInspector]
    public float currentSpeed;
    [HideInInspector]
    public float matCurrentSpeed;
    [HideInInspector]
    public float targetSpeed;
    [HideInInspector]
    public float matTargetSpeed;
    [SerializeField] private Material material;
    private float progress = 0f;
    [SerializeField] private List<Transform> points = new List<Transform>();

    [Header("Spawner")]
    [SerializeField] private float spawnDelay = 1.5f;
    private float timeSinceLastSpawn = 1;
    [SerializeField] private GameObject[] spawnables;


    private List<GameObject> queue = new List<GameObject>();
    private void Start()
    {
        GameManager.conveyor = this;

        timeSinceLastSpawn = spawnDelay - 0.5f;
        currentSpeed = speed;
        targetSpeed = speed;
        matCurrentSpeed = matSpeed;
        matTargetSpeed = matSpeed;
    }

    private void Update()
    {
        progress += matCurrentSpeed * Time.deltaTime;

        if (queue.Count == 0)
        {
            int rd = UnityEngine.Random.Range(0, spawnables.Length);
            queue.Add(spawnables[rd]);
        }
        timeSinceLastSpawn += Time.deltaTime * currentSpeed;
        if (timeSinceLastSpawn > spawnDelay)
        {
            SpawnItem();
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, 0.1f * speed * 3f * Time.deltaTime);
        matCurrentSpeed = Mathf.MoveTowards(matCurrentSpeed, matTargetSpeed, 0.1f * matSpeed * 3f * Time.deltaTime);
        Debug.Log("Progress: " + progress + "\tModulo: " + progress % 1f);
        material.mainTextureOffset = new Vector2(0, progress % 1f);
        // Debug.Log("Current Speed: " + currentSpeed + "\tTarget Speed: " + targetSpeed + "\tMaterial Speed: " + material.GetFloat("_Speed") + "\tSpeed: " + speed);
    }

    private void SpawnItem()
    {
        GameObject item = Instantiate(queue[0], points[0].position, Quaternion.identity);
        item.GetComponent<Item>().state = State.OnConveyor;
        item.transform.localScale = points[0].localScale;
        item.transform.rotation = points[0].rotation;
        item.transform.SetParent(transform);
        queue.RemoveAt(0);
        timeSinceLastSpawn = 0;
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + offset, new Vector3(size.x, 0.1f, size.y));
            Gizmos.color = Color.green;
            int i = 0;
            foreach (Transform point in points)
            {
                if (i != 0)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(points[i - 1].position, point.position);
                }
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(point.position, .3f);
                i++;
            }
        }
    }

    public void AddChild(Item item, RaycastHit hit)
    {
        float scale = item.transform.localScale.x;
        item.transform.SetParent(transform);
        item.transform.localScale = Vector3.one * scale;
        item.transform.position = new Vector3(
            Mathf.Clamp(hit.point.x, transform.position.x + offset.x - size.x / 2, transform.position.z + offset.x + size.x / 2),
            offset.y + .4f + item.conveyorOffset.y,
            Mathf.Clamp(hit.point.z, transform.position.z + offset.z - size.y / 2, transform.position.z + offset.z + size.y / 2)
        );
        item.transform.rotation = Quaternion.identity;
        item.conveyorPoint = 2;
        item.state = State.OnConveyor;
    }

    public void MoveItem(Item item)
    {
        Transform point = points[item.conveyorPoint];

        item.transform.localScale = Vector3.MoveTowards(item.transform.localScale, Vector3.Scale(point.localScale, item.scale), currentSpeed * item.scale.magnitude * 1.5f * Time.deltaTime);

        item.transform.position = Vector3.MoveTowards(item.transform.position, point.position + item.conveyorOffset, currentSpeed * Time.deltaTime);
        if (Vector3.Distance(item.transform.position, point.position + item.conveyorOffset) < .1f)
        {
            item.conveyorPoint += 1;
            if (item.conveyorPoint >= points.Count)
            {
                GameObject.Destroy(item.gameObject);
            }
        }
    }
}
