using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    [Header("Scripts Only")]
    [SerializeField] private Canvas cavStart;
    [SerializeField] private Canvas cavPlay;

    [Space(5.0f)]
    [SerializeField] private GameObject pgFirst;
    [SerializeField] private GameObject pgMain;

    [Space(5.0f)]
    [SerializeField] private GameObject pgLocker;

    [Header("UI")]
    public TMP_Text txtMoney;
    public TMP_Text txtPoint;
    public TMP_Text txtCash;

    private GameManager GM() { return transform.parent.parent.GetComponent<GameManager>(); }
    public ResourceManager RM() { return GM().RM(); }
    private AudioManager AM() { return GM().AM(); }

    [Header("UI")]
    public TMP_Text txtPlayerName;

    public void MoveFromStartToPlay()
    {
        cavStart.gameObject.SetActive(false);
        cavPlay.gameObject.SetActive(true);

        if (AuthenticationService.Instance.PlayerName != null)
        {
            pgFirst.gameObject.SetActive(false);
            OpenPgMain(true);

            txtPlayerName.text = AuthenticationService.Instance.PlayerName;
        }
    }

    public void OpenPgFirst(bool b)
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

        pgFirst.gameObject.SetActive(b);
    }

    public void OpenPgMain(bool b)
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

        pgMain.gameObject.SetActive(b);
    }

    public void OpenPgLocker(bool b)
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

        pgLocker.gameObject.SetActive(b);

        if (b)
        {
            txtMoney.text = RM().GetItemStack("money").ToString();
            txtPoint.text = RM().GetItemStack("point").ToString();
            txtCash.text = RM().GetItemStack("cash").ToString();
        }

        RM().AddItemStack("testitem", 1, 1, 10);
        RM().AddItemStack("testitem", 1, 0, 5);
        Debug.Log(RM().GetItemStack("testitem"));
        Debug.Log(RM().GetItemStack("testitem", 1, 1));
        Debug.Log(RM().GetItemStack("testitem", 1, 0));
    }
}
