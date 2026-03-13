using UnityEngine;
using TMPro;

public class Counter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Text displaying counter")]
    public TMP_Text counterText;

    private int count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        count = 0;
        counterText.text = count.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            Increment();
        }
    }

    public void Increment()
    {
        count++;
        counterText.text = count.ToString();
    }
}
