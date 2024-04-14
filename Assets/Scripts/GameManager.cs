using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int SeedNum {  get; set; }

    public ResourceManager RM() { return transform.Find("sysPlay").Find("sysResource").GetComponent<ResourceManager>(); }
    public AudioManager AM() { return transform.Find("sysAudio").GetComponent<AudioManager>(); }
    private StartManager SM() { return transform.Find("sysStart").GetComponent<StartManager>(); }

    public List<Dictionary<string, object>> Statistics = new List<Dictionary<string, object>>();


    [Space(5.0f)]
    public AudioMixer adoMixer;

    [Header("Player Info")]
    public string PlayerUID;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();

        SeedNum = Random.Range(2, 10001);

        CheckSettingLocal();
    }

    private void Start()
    {
        AM().PlayBGM();

        CheckSignIn();

        SM().SetPgSetting();
    }

    private async void OnApplicationQuit()
    {
        var data = new Dictionary<string, object>
        {
            { "LastAccessTime", Time.captureDeltaTime },
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);

        PlayerPrefs.Save();
    }

    public async void CheckSignIn()
    {
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            SM().SetSignOut(false);
            SM().SetSignIn(true);

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

                RM().LoadLocker();
            }
            catch
            {
            }

            CloudGetUID();
        }
        else
        {
            SM().SetSignIn(false);
            SM().SetSignOut(true);

            SM().txtUID.text = "";
        }
    }

    public void CheckSettingLocal()
    {
        if (!PlayerPrefs.HasKey("AudioMaster"))
        {
            PlayerPrefs.SetFloat("AudioMaster", 10);
            PlayerPrefs.SetFloat("AudioBGM", 10);
            PlayerPrefs.SetFloat("AudioSFX", 10);
            PlayerPrefs.SetFloat("AudioUI", 10);
        }

        if (!PlayerPrefs.HasKey("PlayerLocale"))
        {
            PlayerPrefs.SetInt("PlayerLocale", LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale));
        }
        else
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("PlayerLocale")];
        }
    }

    private async void CloudGetUID()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "UID" });

        if (playerData.TryGetValue("UID", out var keyName))
        {
            PlayerUID = keyName.Value.GetAs<string>().ToString();
            SM().txtUID.text = "UID : " + PlayerUID;
        }
    }

    public void GameQuit()
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void ConFullScreen(Image img)
    {
        bool full = Screen.fullScreen;

        if (full)
        {
            AM().PlayClick(AudioManager.ButtonClick.Off);
        }
        else
        {
            AM().PlayClick(AudioManager.ButtonClick.On);
        }

        Screen.fullScreen = !full;
        img.gameObject.SetActive(!full);
    }

    public void ConScreenSize(bool b)
    {
        int screenlevel = Screen.width / 320;
        int w = Screen.width, h = Screen.height;

        if (b && screenlevel < 6)
        {
            AM().PlayClick(AudioManager.ButtonClick.On);

            w = (screenlevel + 1) * 320;
            h = w * 9 / 16;

            Screen.SetResolution(w, h, Screen.fullScreen);
        }
        else if (!b && screenlevel > 2)
        {
            AM().PlayClick(AudioManager.ButtonClick.On);

            w = (screenlevel - 1) * 320;
            h = w * 9 / 16;

            Screen.SetResolution(w, h, Screen.fullScreen);
        }
        else
        {
            AM().PlayClick(AudioManager.ButtonClick.Fail);
        }

        SM().txtScreenSize.text = w + " x " + h;
    }

    public void LoadAudio(Slider sld, TMP_Text txt, string key)
    {
        float vol = PlayerPrefs.GetFloat("Audio" + key);
        sld.value = vol;
        txt.text = vol.ToString();

        if (vol != 0)
        {
            adoMixer.SetFloat(key, (vol - 10.0f) * 4.0f);
        }
        else
        {
            adoMixer.SetFloat(key, -100.0f);
        }
    }

    public void SetAudio(Slider sld, TMP_Text txt, string key)
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

        if (sld.value != 0)
        {
            adoMixer.SetFloat(key, (sld.value - 10.0f) * 4.0f);
        }
        else
        {
            adoMixer.SetFloat(key, -100.0f);
        }

        txt.text = sld.value.ToString();

        PlayerPrefs.SetFloat("Audio" + key, sld.value);
    }

    public void ConAudio(Slider sld, TMP_Text txt, string key, bool b)
    {
        if (b && sld.value < sld.maxValue)
        {
            AM().PlayClick(AudioManager.ButtonClick.On);

            sld.value++;
        }
        else if (!b && sld.value > sld.minValue)
        {
            AM().PlayClick(AudioManager.ButtonClick.On);

            sld.value--;
        }
        else
        {
            AM().PlayClick(AudioManager.ButtonClick.Fail);
        }

        SetAudio(sld, txt, key);
    }

    public void ConLang(bool b)
    {
        int select = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        int count = LocalizationSettings.AvailableLocales.Locales.Count;

        if (b && select < count - 1)
        {
            AM().PlayClick(AudioManager.ButtonClick.On);

            LocalizationSettings.SelectedLocale =  LocalizationSettings.AvailableLocales.Locales[select + 1];
        }
        else if (!b && select > 0)
        {
            AM().PlayClick(AudioManager.ButtonClick.On);

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[select - 1];
        }
        else
        {
            AM().PlayClick(AudioManager.ButtonClick.Fail);
        }

        PlayerPrefs.SetInt("PlayerLocale", LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale));
    }

    public void DeleteCach()
    {
        PlayerPrefs.DeleteAll();
        AuthenticationService.Instance.SignOut(this);

        GameQuit();
    }

    public void GameUpdatePlayerName(TMP_InputField inp)
    {
        if (inp.text.Length > 2)
        {
            AM().PlayClick(AudioManager.ButtonClick.On);
            AuthenticationService.Instance.UpdatePlayerNameAsync(inp.text);
        }
        else
        {
            AM().PlayClick(AudioManager.ButtonClick.Fail);
        }
    }
}
