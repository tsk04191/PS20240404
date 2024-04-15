using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class AccountManager : MonoBehaviour
{
    [Header("Script Only")]
    [SerializeField] private GameObject pgStart;
    [SerializeField] private GameObject pgSignUp;
    [SerializeField] private GameObject pgSignIn;

    private GameManager GM() { return transform.parent.GetComponent<GameManager>(); }
    private AudioManager AM() { return GM().AM(); }

    private int VerifyCode = 1000000;
    private bool ID = false, PW = false, Email = false;

    [Header("UI")]
    public TMP_InputField inpID;
    public TMP_Text txtWarmmingID;

    [Space(5.0f)]
    public TMP_InputField inpPW;
    public TMP_InputField inpPWRepeat;
    public TMP_Text txtWarmmingPW;
    public TMP_Text txtWarmmingPWDifferent;

    [Space(5.0f)]
    public TMP_InputField inpEmail;
    public TMP_InputField inpVerifyCode;

    [Space(5.0f)]
    public Button btEmailVerify;
    public TMP_Text txtWarmmingSendError;
    public TMP_Text txtWarmmingVerifyTime;
    public TMP_Text txtNoticeVerify;

    [Space(5.0f)]
    public Button btSignUp;

    [Space(10.0f)]
    public TMP_InputField inpIDSignIn;
    public TMP_InputField inpPWSignIn;
    public Button btSignIn;
    public TMP_Text txtWarmmingSignIn;

    public void OpenPgSignUp(bool b)
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

        pgSignUp.SetActive(b);

        if (!b)
        {
            inpID.text = "";
            inpPW.text = "";
            inpPWRepeat.text = "";
            inpEmail.text = "";
            inpVerifyCode.text = "";

            txtWarmmingID.gameObject.SetActive(false);
            txtWarmmingPW.gameObject.SetActive(false);
            txtWarmmingPWDifferent.gameObject.SetActive(false);
            txtWarmmingSendError.gameObject.SetActive(false);
            txtWarmmingVerifyTime.gameObject.SetActive(false);

            btEmailVerify.gameObject.SetActive(true);
            inpVerifyCode.gameObject.SetActive(true);
            VerifyCode = 1000000;
        }
    }

    public void OpenPgStart(bool b)
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

        pgStart.SetActive(b);
        GM().CheckSignIn();
    }

    public void OpenPgSignIn(bool b)
    {
        AM().PlayClick(AudioManager.ButtonClick.On);

        pgSignIn.SetActive(b);

        if (!b)
        {
            inpIDSignIn.text = "";
            inpPWSignIn.text = "";
        }
    }

    public void DelAccount()
    {
        AuthenticationService.Instance.DeleteAccountAsync();
    }

    public void CheckIDLength()
    {
        bool b = inpID.text.Length < 3;
        txtWarmmingID.gameObject.SetActive(b);
        ID = !b;
    }

    public void CheckPW()
    {
        bool a = false, b = false;

        if (inpPW.text.Length < 8 || !CheckPWChar())
        {
            a = true;
        }
        else if (inpPW.text != inpPWRepeat.text)
        {
            b = true;
        }

        txtWarmmingPW.gameObject.SetActive(a);
        txtWarmmingPWDifferent.gameObject.SetActive(b);

        PW = !a && !b;
    }

    private bool CheckPWChar()
    {
        bool u = false, l = false, n = false, s = false;

        foreach (char c in inpPW.text)
        {
            if (c >= 48 && c <= 57)
            {
                n = true;
            }
            else if (c >= 65 && c <= 90)
            {
                u = true;
            }
            else if (c >= 97 && c <= 122)
            {
                l = true;
            }
            else
            {
                s = (c != 0);
            }
            
            if (u && l && n && s)
            {
                break;
            }
        }

        return u && l && n && s;
    }

    public async void SignUp()
    {
        if (ID && PW && Email)
        {
            await AuthenticationService.Instance.AddUsernamePasswordAsync(inpID.text, inpPW.text);

            var UID = await CloudCodeService.Instance.CallEndpointAsync("sysCloudData");

            var DataUID = new Dictionary<string, object>
            { 
                { "UID", UID },
                { "Email", inpEmail.text }
            };
            await CloudSaveService.Instance.Data.Player.SaveAsync(DataUID);

            SetDefaultLocker();
            SetDefaultStatistics();

            OpenPgSignUp(false);
            OpenPgStart(true);

            GM().CheckSignIn();
        }
    }

    public async void SendEmail()
    {
        MailMessage msg = new MailMessage();
        string EmailTo = inpEmail.text;
        try
        {
            msg.To.Add(EmailTo);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch
        {
            txtWarmmingSendError.gameObject.SetActive(true);
            return;
        }

        var EmailFrom = await CloudCodeService.Instance.CallEndpointAsync("sysSMTP_ID");
        var EmailPW = await CloudCodeService.Instance.CallEndpointAsync("sysSMTP_PW");

        VerifyCode = UnityEngine.Random.Range(100000, 1000000);

        msg.From = new MailAddress(EmailFrom.ToString(), "PS20240404@non-reply.com");

        msg.Subject = "PS20240404 계정 생성 본인인증";

        msg.Body = VerifyCode.ToString();

        msg.IsBodyHtml = true;
        msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

        msg.SubjectEncoding = Encoding.UTF8;
        msg.BodyEncoding = Encoding.UTF8;

        SmtpClient smtp = new SmtpClient();
        smtp.Host = "smtp.gmail.com";
        smtp.Port = 587;

        smtp.Timeout = 10000;
        smtp.EnableSsl = true;
        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtp.Credentials = new System.Net.NetworkCredential(EmailFrom.ToString(), EmailPW.ToString());
        try
        {
            inpEmail.readOnly = true;

            smtp.Send(msg);
            smtp.Dispose();

            txtWarmmingSendError.gameObject.SetActive(false);
            btEmailVerify.gameObject.SetActive(false);

            await TimerVerify();

            AM().PlayClick(AudioManager.ButtonClick.On);
        }
        catch
        {
            AM().PlayClick(AudioManager.ButtonClick.Fail);
        }
    }

    public void ChangeEmail()
    {
        txtWarmmingSendError.gameObject.SetActive(false);
    }

    private async UniTask TimerVerify()
    {
        float lefttime = 100;

        txtWarmmingVerifyTime.gameObject.SetActive(true);

        while (lefttime > 0)
        {
            txtWarmmingVerifyTime.text = (int)(lefttime / 60.0f) + ":" + (int)(lefttime % 60.0f);
            lefttime--;

            await UniTask.Delay(1000);
        }

        VerifyCode = 1000000;

        txtWarmmingVerifyTime.gameObject.SetActive(false);
        btEmailVerify.gameObject.SetActive(true);
        inpEmail.readOnly = false;
    }

    public void CheckVerify()
    {
        txtWarmmingVerifyTime.gameObject.SetActive(false);
        inpVerifyCode.gameObject.SetActive(false);
        btEmailVerify.gameObject.SetActive(false);

        txtNoticeVerify.gameObject.SetActive(true);
        Email = true;

        AM().PlayClick(AudioManager.ButtonClick.On);
    }

    public async void SignIn()
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(inpIDSignIn.text, inpPWSignIn.text);
            await AuthenticationService.Instance.GetPlayerNameAsync();

            txtWarmmingSignIn.gameObject.SetActive(false);
            OpenPgSignIn(false);
            OpenPgStart(true);

            GM().CheckSignIn();

            AM().PlayClick(AudioManager.ButtonClick.On);
        }
        catch
        {
            txtWarmmingSignIn.gameObject.SetActive(true);

            AM().PlayClick(AudioManager.ButtonClick.Fail);
        }
    }

    public void SignOut()
    {
        AuthenticationService.Instance.SignOut(this);

        AM().PlayClick(AudioManager.ButtonClick.On);
        GM().CheckSignIn();
    }

    public async void SetDefaultLocker()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        Dictionary<string, object> locker = new Dictionary<string, object>()
        {
            { "money", 100000 },
            { "point", 1000 },
            { "cash", 1 }
        };

        data.Add("Locker", locker);

        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }

    public async void SetDefaultStatistics()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        Dictionary<string, object> statistics = new Dictionary<string, object>();
        Dictionary<string, Dictionary<string, object>> stats = new Dictionary<string, Dictionary<string, object>>();

        stats.Add("durability", new Dictionary<string, object>());
        stats["durability"].Add("max", 100);
        stats["durability"].Add("now", 100);

        stats.Add("will", new Dictionary<string, object>());
        stats["will"].Add("max", 100);
        stats["will"].Add("now", 100);

        statistics.Add("durability", stats["durability"]);
        statistics.Add("will", stats["will"]);

        data.Add("Statistics", statistics);

        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
    }
}
