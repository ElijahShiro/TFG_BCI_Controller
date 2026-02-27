using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Inputs")]
    public InputField NameInput;
    public Dropdown GenreDropdown;

    [Header("UI Btn")]
    public Button CreateSessionBtn;
    public Button HeadsetQueryBtn;
    public Button LoadProfileBtn;
    public Button UnloadProfileBtn;
    public Button StartBtn;


    [Header("UI Feedback")]
    public Text StatusText;


    private bool _isProfileReady = false;
    private void Start()
    {
        UpdateUIState();
    }
    void Update()
    {
        UpdateUIState();
    }

    private void UpdateUIState()
    {
        bool l_isHeadsetReady = MentalGymTrainer.Instance.IsSessionReady();
        bool l_isProfileLoaded = MentalGymTrainer.Instance.IsProfileLoaded();
        CreateSessionBtn.interactable = !l_isHeadsetReady;
        HeadsetQueryBtn.interactable = l_isHeadsetReady;
        LoadProfileBtn.interactable = l_isHeadsetReady;
        UnloadProfileBtn.interactable = l_isHeadsetReady && l_isProfileLoaded;
        StartBtn.interactable = l_isHeadsetReady && _isProfileReady;

        if (!l_isHeadsetReady)
        {
            StatusText.text = "Estado: Esperando conexión...";
            StatusText.color = Color.red;
            _isProfileReady = false;
        }
        else if (l_isHeadsetReady && !l_isProfileLoaded) 
        {
            StatusText.text = "Estado: Conectado. Carga un perfil.";
            StatusText.color = Color.yellow;
        }
        else if (_isProfileReady)
        {
            StatusText.text = "Estado: Conectado. Puedes empezar.";
            StatusText.color = Color.green;
        }
    }

    public void OnConnectClick() 
    {
        StatusText.text = "Buscando...";
        MentalGymTrainer.Instance.ConnectToHeadset();
    }
    public void OnQueryHeadset()
    {
        MentalGymTrainer.Instance.ShowQueryHeadset(); //enseña en el log de los headsets conectados.
    }
    public void OnLoadProfile() 
    {
        if (string.IsNullOrEmpty(NameInput.text))
        {
            Debug.LogError("¡Error! Debes escribir un nombre de usuario.");
            StatusText.color = Color.red;
            return;
        }

        string l_name = NameInput.text;
        string l_genre = GenreDropdown.options[GenreDropdown.value].text;

        MentalGymTrainer.Instance.LoadProfile(l_name, l_genre);

        // Marcamos que ya estamos listos para jugar
        _isProfileReady = true;

        // Feedback para el usuario
        StatusText.text = $"Perfil listo";
        StatusText.color = Color.green;

        Debug.Log("Perfil cargado. Esperando confirmación para entrar al nivel.");
    }

    public void OnUnloadProfile()
    {
        MentalGymTrainer.Instance.UnloadProfile();
    }
    public void OnStartLevel() 
    {
        MentalGymTrainer.Instance.LoadLevel();
    }
}
