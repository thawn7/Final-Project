using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ProblemType : MonoBehaviour
{
    public Text DisplayText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void addmsg()
    {
        DisplayText.text = "Addition";
    }

    public void subtractmsg()
    {
        DisplayText.text = "Subtraction";
    }

    public void multiplymsg()
    {
        DisplayText.text = "Multiplication";
    }

    public void dividemsg()
    {
        DisplayText.text = "Division";
    }
}
