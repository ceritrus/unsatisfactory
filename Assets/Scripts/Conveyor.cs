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
    [SerializeField] private float speed = 0.8f;
    [SerializeField] private Material material;
    [SerializeField] private List<Transform> points = new List<Transform>();

    [Header("Spawner")]
    [SerializeField] private float spawnDelay = 1.5f;
    private float timeSinceLastSpawn = 1;
    [SerializeField] private GameObject defaultItem;
    [SerializeField] private GameObject alternateItem;


    private List<GameObject> queue = new List<GameObject>();
    private void Start()
    {
        GameManager.conveyor = this;
    }

    private void Update()
    {
        if (queue.Count == 0)
        {
            float rd = Random.Range(0, 100);
            print(rd);
            if (rd < 20)
            {
                queue.Add(alternateItem);
            }
            else
            {
                queue.Add(defaultItem);
            }
            return;
        }
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn > spawnDelay)
        {
            SpawnItem();
        }
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
            offset.y + item.transform.localScale.y + .05f,
            Mathf.Clamp(hit.point.z, transform.position.z + offset.z - size.y / 2, transform.position.z + offset.z + size.y / 2)
        );
        item.transform.rotation = Quaternion.identity;
        item.conveyorPoint = 2;
        item.state = State.OnConveyor;
    }

    public void MoveItem(Item item)
    {
        Transform point = points[item.conveyorPoint];


        item.transform.localScale = Vector3.MoveTowards(item.transform.localScale, point.localScale * item.scale, speed * item.scale * 1.5f * Time.deltaTime);
        item.transform.position = Vector3.MoveTowards(item.transform.position, point.position, speed * Time.deltaTime);
        if (Vector3.Distance(item.transform.position, point.position) < .1f)
        {
            item.conveyorPoint += 1;
            if (item.conveyorPoint >= points.Count)
            {
                GameObject.Destroy(item.gameObject);
            }
        }
    }
}
