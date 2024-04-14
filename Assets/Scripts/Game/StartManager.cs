using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [Header("Script Only")]
    [SerializeField] private GameObject pgSetting;
    [SerializeField] private GameObject pgScreen;
    [SerializeField] private GameObject pgSound;
    [SerializeField] private GameObject pgLang;
    [SerializeField] private GameObject pgData;

    private GameManager GM() { return transform.parent.GetComponent<GameManager>(); }
    private AudioManager AM() { return GM().AM(); }

    [Header("UI")]
    public Button btStart;
    public Button btSignOut;

    [Space(5.0f)]
    public TMP_Text txtUID;
    public TMP_Text txtVersion;

    [Space(5.0f)]
    public Button btSignInIDPW;
    public Button btSignInGuest;
    public Button btSignUp;

    [Space(10.0f)]
    public Image imgScreen;
    public Image imgFullScreen;

    [Space(5.0f)]
    public TMP_Text txtScreenSize;

    [Space(10.0f)]
    public Image imgSound;

    [Space(5.0f)]
    public Slider sldMaster;
    public TMP_Text txtMasterSize;
    public Button btMasterMinus;
    public Button btMasterPlus;

    [Space(5.0f)]
    public Slider sldBGM;
    public TMP_Text txtBGMSize;
    public Button btBGMMinus;
    public Button btBGMPlus;

    [Space(5.0f)]
    public Slider sldSFX;
    public TMP_Text txtSFXSize;
    public Button btSFXMinus;
    public Button btSFXPlus;

    [Space(5.0f)]
    public Slider sldUI;
    public TMP_Text txtUISize;
    public Button btUIMinus;
    public Button btUIPlus;

    [Space(10.0f)]
    public Image imgLang;

    [Space(10.0f)]
    public Image imgData;

    public void SetSignIn(bool b)
    {
        btStart.gameObject.SetActive(b);
        btSignOut.gameObject.SetActive(b);
    }

    public void SetSignOut(bool b)
    {
        btSignInIDPW.gameObject.SetActive(b);
        btSignInGuest.gameObject.SetActive(b);
        btSignUp.gameObject.SetActive(b);
    }

    public void OpenPgSetting(bool b)
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

        pgSetting.SetActive(b);
        OpenPgSettingMenu(0);
    }

    public void OpenPgSettingMenu(int num)
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

        imgScreen.gameObject.SetActive(num == 0);
        imgSound.gameObject.SetActive(num == 1);
        imgLang.gameObject.SetActive(num == 2);
        imgData.gameObject.SetActive(num == 3);

        pgScreen.gameObject.SetActive(num == 0);
        pgSound.gameObject.SetActive(num == 1);
        pgLang.gameObject.SetActive(num == 2);
        pgData.gameObject.SetActive(num == 3);
    }

    public void SetPgSetting()
    {
        SetPgScreen();
        SetPgSound();

        OpenPgSetting(false);
    }

    public void SetPgScreen()
    {
        imgFullScreen.gameObject.SetActive(Screen.fullScreen);
        txtScreenSize.text = Screen.width.ToString() + " x " + Screen.height.ToString();
    }

    public void SetPgSound()
    {
        GM().LoadAudio(sldMaster, txtMasterSize, "Master");
        GM().LoadAudio(sldBGM, txtBGMSize, "BGM");
        GM().LoadAudio(sldSFX, txtSFXSize, "SFX");
        GM().LoadAudio(sldUI, txtUISize, "UI");

        sldMaster.onValueChanged.AddListener(delegate { GM().SetAudio(sldMaster, txtMasterSize, "Master"); });
        sldBGM.onValueChanged.AddListener(delegate { GM().SetAudio(sldBGM, txtBGMSize, "BGM"); });
        sldSFX.onValueChanged.AddListener(delegate { GM().SetAudio(sldSFX, txtSFXSize, "SFX"); });
        sldUI.onValueChanged.AddListener(delegate { GM().SetAudio(sldUI, txtUISize, "UI"); });

        btMasterMinus.onClick.AddListener(delegate { GM().ConAudio(sldMaster, txtMasterSize, "Master", false); });
        btMasterPlus.onClick.AddListener(delegate { GM().ConAudio(sldMaster, txtMasterSize, "Master", true); });
        btBGMMinus.onClick.AddListener(delegate { GM().ConAudio(sldBGM, txtBGMSize, "BGM", false); });
        btBGMPlus.onClick.AddListener(delegate { GM().ConAudio(sldBGM, txtBGMSize, "BGM", true); });
        btSFXMinus.onClick.AddListener(delegate { GM().ConAudio(sldSFX, txtSFXSize, "SFX", false); });
        btSFXPlus.onClick.AddListener(delegate { GM().ConAudio(sldSFX, txtSFXSize, "SFX", true); });
        btUIMinus.onClick.AddListener(delegate { GM().ConAudio(sldUI, txtUISize, "UI", false); });
        btUIPlus.onClick.AddListener(delegate { GM().ConAudio(sldUI, txtUISize, "UI", true); });
    }
}
