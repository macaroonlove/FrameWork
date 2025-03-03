using FrameWork;
using FrameWork.GameSettings;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Temporary.Save
{
    public class SaveManager : PersistentSingleton<SaveManager>
    {
        private SaveDataTemplate _profileData;

        protected override void Initialize()
        {
            // 게임 설정 불러오기
            GameSettingsManager.RestoreSettings();

            // 데이터 지우기
            _profileData.Clear();
        }

        #region Profile Data
        public async Task<bool> Load_ProfileData()
        {
            bool isSuccess = false;

            var tcs = new TaskCompletionSource<bool>();

            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = PlayFabAuthService.PlayFabId,
                Keys = new List<string> { "ProfileData" }
            }, result =>
            {
                if (result.Data != null && result.Data.ContainsKey("ProfileData"))
                {
                    isSuccess = _profileData.Load(result.Data["ProfileData"].Value);
                }
                else
                {
                    isSuccess = true;
                }
                tcs.SetResult(isSuccess);
            }, error =>
            {
                tcs.SetResult(false);
            });

            isSuccess = await tcs.Task;

            if (isSuccess == false)
            {
                _profileData.SetDefaultValues();
                await Save_ProfileData();
            }

            return true;
        }

        public async Task<bool> Save_ProfileData()
        {
            var tcs = new TaskCompletionSource<bool>();

            string jsonData = _profileData.ToJson();

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string> { { "ProfileData", jsonData } }
            }, result =>
            {
                tcs.SetResult(true);
            }, error =>
            {
                tcs.SetResult(false);
            });

            return await tcs.Task;
        }

        public async Task<bool> Clear_ProfileData()
        {
            var tcs = new TaskCompletionSource<bool>();

            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                KeysToRemove = new List<string> { "ProfileData" }
            }, result =>
            {
                tcs.SetResult(true);
            }, error =>
            {
                tcs.SetResult(false);
            });

            return await tcs.Task;
        }

        #endregion
    }
}