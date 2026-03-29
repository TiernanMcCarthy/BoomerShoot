using TMPro;
using UnityEngine;

public class PlayerSpeed : MonoBehaviour
{
    
    [SerializeField] private TMP_Text speedText;

    public FloatingBiped playerbiped;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        speedText.text = string.Format("PlayerSpeed: {0}", playerbiped.GetSpeed().ToString("F2"));
    }
}
