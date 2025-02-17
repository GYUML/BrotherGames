using GameA;
using UnityEngine;

public class UserDataController : MonoBehaviour
{
    readonly string UserDataDirectory = "UserData";
    readonly string CommonDataFileName = "Common.json";

    UserData userData;
    CommonData commonData;

    private void Start()
    {
        if (!TryLoadUserData())
            Managers.UI.ShowPopup<MakeAccountPopup>().Set(OnRegister);
    }

    public bool SaveUserData()
    {
        var fileName = $"{userData.nickname}.json";
        var saveResult = Managers.File.SaveData(UserDataDirectory, fileName, userData);

        if (!saveResult)
            Managers.UI.ShowPopup<MessagePopup>().Set("Error", "Failed to save user data.");

        return saveResult;
    }

    public bool TryLoadUserData()
    {
        var commonDataPath = $"{UserDataDirectory}/{CommonDataFileName}";

        if (Managers.File.TryLoadData<CommonData>(commonDataPath, out var common))
        {
            if (common != null && !string.IsNullOrEmpty(common.lastUserName))
            {
                var userDataPath = $"{UserDataDirectory}/{common.lastUserName}.json";
                if (Managers.File.TryLoadData<UserData>(userDataPath, out var userData))
                {
                    if (userData != null)
                    {
                        this.userData = userData;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public string GetUserUID()
    {
        // TODO
        return userData.nickname;
    }

    void SaveCommonData()
    {
        Managers.File.SaveData(UserDataDirectory, CommonDataFileName, commonData);
    }

    void OnRegister(string nickname)
    {
        if (string.IsNullOrEmpty(nickname) || nickname.Length < 5)
        {
            Managers.UI.ShowPopup<MessagePopup>().Set("Warning", "Nickname must be at least 5 characters long.");
            return;
        }

        userData = new UserData()
        {
            nickname = nickname,
            gold = 500,
            volt = 30,
        };

        commonData = new CommonData()
        {
            lastUserName = nickname
        };

        if (SaveUserData())
        {
            Managers.UI.HidePopup<MakeAccountPopup>();
            SaveCommonData();
        }
    }

    public class CommonData
    {
        public string lastUserName;
    }

    public class UserData
    {
        public string nickname;
        public int gold;
        public int volt;
    }
}
