using System;
using UnityEngine;

public class FruitCollision : MonoBehaviour
{
    [NonSerialized] public bool isMearged = false;
    [NonSerialized] public GameObject PareFruit;
    [NonSerialized] public Vector3 point;

    private FruitCollision colInfo;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != gameObject.name) return;

        colInfo = collision.gameObject.GetComponent<FruitCollision>();

        if (isMearged == false && colInfo.isMearged == false)
        {
            PareFruit = collision.gameObject;
            isMearged = true;
            collision.gameObject.GetComponent<FruitCollision>().isMearged = true;

            point = collision.contacts[0].point;
            point.z = 9.9f;
            Main.CQueGo.Enqueue(gameObject);
        }
    }

    private CircleCollider2D collide2d;
    private void Awake()
    {
        collide2d = transform.GetComponent<CircleCollider2D>();
    }

    private float y;
    private void FixedUpdate()
    {
        // NOTE: Check Fruit in Bucket Hight with 2 for loop
        if (Main.Instance.isRunGame && !transform.gameObject.Equals(Main.Instance.cfruit.fruit.gameObject))
        {
            y = transform.position.y + collide2d.radius;
            if (y > 4)
            {
                Main.Instance.isBlinking = true;
            }
            else
            {
                Main.Instance.isBlinking = false;
                Main.Instance.setDLTransparent();
            }
            if (y > 6.8)
            {
                Main.Instance.isRunGame = false;
            }
        }
    }
}