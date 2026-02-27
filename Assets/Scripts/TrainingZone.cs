using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingZone : MonoBehaviour
{

    [Header("Configuration")]
    public string ActionToTrain = "";

    [Header("UI locla (WorldSpace)")]
    public GameObject FloatingText;

    private bool _playerInside = false;
   
    void Start()
    {
        if(FloatingText) FloatingText.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plaeyr")) 
        { 
            _playerInside = true;
            if(FloatingText) FloatingText.SetActive(true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Plaeyr"))
        {
            _playerInside = false;
            if (FloatingText) FloatingText.SetActive(false);
        }
    }
    void Update()
    {
        if (_playerInside && Input.GetKeyDown(KeyCode.E) && !MentalGymTrainer.Instance._isTraining)
        {
            Debug.Log("Iniciando entrenamiento zona: " + ActionToTrain);
            MentalGymTrainer.Instance.StartTrainingAction(ActionToTrain);
        }
    }
}
