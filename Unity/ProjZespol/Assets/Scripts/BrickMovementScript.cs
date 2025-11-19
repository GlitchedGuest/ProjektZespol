using UnityEngine;

public class BrickMovementScript : MonoBehaviour
{
    private float speed = 0f; //prêdkoœæ bloków
    public int overlap = 0; //status overlapowania 0-nie dotkne³o 1-dotkne³o lewej strony 2-dotkne³o prawej strony 3-wysze³o poza granice
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private SpriteRenderer sr;

    void Start()
    {
        sr = transform.Find("Render").GetComponent<SpriteRenderer>();
        float random = UnityEngine.Random.Range(0.00f, 100.00f);
        if (random > 50.00f)
            speed = 2f;
        else
        {
            sr.color = new Color(1f, 0.4f, 0.7f);
            sr.flipX = true;
            speed = -2f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.left * Time.deltaTime * speed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LeftZone") 
            overlap = 1;
        else if( collision.gameObject.name == "RightZone")
            overlap = 2;

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "LeftZone" || collision.gameObject.name == "RightZone")
            overlap = 3;
    }
}
