using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    public Action OnClickJoin = null;
    [SerializeField] private InputField inputField;
    [SerializeField] private Button loginButton;

    private void Start()
    {
        loginButton.onClick.AddListener(OnClickLoginButton);
    }

    private void OnClickLoginButton()
    {
        if(string.IsNullOrEmpty(inputField.text))
            return;

        PhotonNetwork.LocalPlayer.NickName = inputField.text;
        OnClickJoin?.Invoke();
        gameObject.SetActive(false);
    }
}
