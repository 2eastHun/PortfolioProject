using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputField : MonoBehaviour
{
    public TMP_InputField inputField; // InputField를 에디터에서 할당
    string inputValue;
    void Start()
    {
        
    }

    public string GetInputFieldValue()
    {
        inputValue = inputField.text;
        return inputValue;
    }

    public void LoginButton()
    {
        //DBManager.instance.Login("https://localhost:7275/PlayerData/InsertPlayer", "playerName", inputValue);

        NetworkManager.instance.Init();

        C_Login loginPacket = new C_Login();
        loginPacket.PlayerName = GetInputFieldValue();
        NetworkManager.instance.Send(loginPacket);

        //SceneManager.LoadScene("Menu");

        //string inputValue = GetInputFieldValue();
        //Debug.Log("InputField 값: " + inputValue);
        //DBManager.instance.Login("https://localhost:7275/PlayerData/InsertPlayer", inputValue);
    }
}