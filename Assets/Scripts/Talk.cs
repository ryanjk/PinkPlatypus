using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * PlayerMain
 @author Taha Ghassemi
 * 
 *
 *
 */
public class Talk : MonoBehaviour
{
    private Text _text;
    private Image _container;
    public string message;
    void Start()
    {
        _text = GameObject.FindGameObjectWithTag("Messages").GetComponent<Text>();
        _container = GameObject.FindGameObjectWithTag("MessageBox").GetComponent<Image>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player"||other.tag=="FPPlayer") {
            _container.enabled = true;
            _text.enabled = true;
            _text.text = message;

        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" || other.tag == "FPPlayer") {
            _container.enabled = false;
            _text.enabled = false;

            _text.text = "";
        }
    }
    private Dictionary<string, string> _conversations;
}