using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button m_HostButton;   
    [SerializeField] private Button m_ClientButton;
    [SerializeField] private Button m_ServerButton;
    [SerializeField] private Text m_StatusText;
    [SerializeField] private Button m_NewPlaceButton;

    [SerializeField] private GameObject m_StartMenu;
    [SerializeField] private GameObject m_GameView;

    private void Start()
    {
        m_HostButton.onClick.AddListener(HostButtonHandler);
        m_ClientButton.onClick.AddListener(ClientButtonHandler);
        m_ServerButton.onClick.AddListener(ServerButtonHandler);
        //m_NewPlaceButton.onClick.AddListener(NewPlaceButtonHandler);
    }

    private void HostButtonHandler()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void ClientButtonHandler()
    {
        NetworkManager.Singleton.StartClient();
    }
    private void ServerButtonHandler()
    {
        NetworkManager.Singleton.StartServer();
    }
   

    private void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        string status = "Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        status += "\n Mode: " + mode;
        m_StatusText.text = status;
    }

    private void UIView()
    {
        
        bool enable = !NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer;
        m_StartMenu.SetActive(enable);
        m_GameView.SetActive(enable == false);
    }

    private void FixedUpdate()
    {
        UIView();
        StatusLabels();
    }
}
